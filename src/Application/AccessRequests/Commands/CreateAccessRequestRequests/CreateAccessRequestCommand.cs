﻿using MediatR;
using OfficeEntry.Application.AccessRequests.Queries.GetSpotsAvailablePerHour;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;
using System.Linq;
using System.Security.Principal;

namespace OfficeEntry.Application.AccessRequests.Commands.CreateAccessRequestRequests
{
    public class CreateAccessRequestCommand : IRequest
    {
        public AccessRequest AccessRequest { get; set; }
    }

    public class CreateAccessRequestCommandHandler : IRequestHandler<CreateAccessRequestCommand>
    {
        private readonly IAccessRequestService _accessRequestService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserService _userService;
        private readonly IBuildingRoleService _buildingRoleService;
        private readonly IFloorPlanService _floorPlanService;
        private readonly INotificationService _notificationService;

        private IMediator _mediator;

        public CreateAccessRequestCommandHandler(
            IAccessRequestService accessRequestService,
            ICurrentUserService currentUserService,
            IUserService userService,
            IBuildingRoleService buildingRoleService,
            IFloorPlanService floorPlanService,
            INotificationService notificationService,
            IMediator mediator
        ) {
            _accessRequestService = accessRequestService;
            _currentUserService = currentUserService;
            _userService = userService;
            _buildingRoleService = buildingRoleService;
            _floorPlanService = floorPlanService;
            _notificationService = notificationService;

            _mediator = mediator;
        }

        public async Task<Unit> Handle(CreateAccessRequestCommand request, CancellationToken cancellationToken)
        {
            var username = _currentUserService.UserId;
            var (_, contact) = await _userService.GetContactByUsername(username);

            if (contact.UserSettings?.HealthSafety is null || contact.UserSettings?.PrivacyStatement is null)
            {
                throw new Exception("Can't create an access request without accepting Privacy Act statement and Health and Safety measures");
            }

            var floorPlan = request.AccessRequest.FloorPlan;
            var floorId = floorPlan.Floor.Id;
            var date = request.AccessRequest.StartTime;
            var requestContactCount = (request.AccessRequest.Visitors?.Count ?? 0) + 1;

            var currentCapacities = await _mediator.Send(new GetSpotsAvailablePerHourQuery { FloorId = floorId, SelectedDay = date }, cancellationToken);

            var hasAvailableCapacity = currentCapacities
                .Where(x => x.Hour >= request.AccessRequest.StartTime.Hour && x.Hour < request.AccessRequest.EndTime.Hour)
                .All(x => x.Capacity - x.SpotsReserved - requestContactCount >= 0);

            if (!hasAvailableCapacity)
            {
                throw new Exception("Your request exceeds the floor capacity");
            }

            if (request.AccessRequest.Employee is null)
            {
                request.AccessRequest.Employee = contact;
            }
            else
            {
                request.AccessRequest.Delegate = contact;
            }

            var accessRequests = await _accessRequestService.GetApprovedOrPendingAccessRequestsByFloorPlan(floorPlan.Id, DateOnly.FromDateTime(date));
            var (_, buildingRoles) = await _buildingRoleService.GetBuildingRolesFor(request.AccessRequest.Employee.Id);
            buildingRoles = buildingRoles.Where(r => r.Floor?.Id == floorId);
            var floorPlanCapacity = await _floorPlanService.GetFloorPlanCapacityAsync(floorPlan.Id, DateOnly.FromDateTime(date));

            var isEmployeeFirstAidAttendant = buildingRoles.Any(b => b.Role.Key == (int)BuildingRole.BuildingRoles.FirstAidAttendant);
            var isEmployeeFloorEmergencyOfficer = buildingRoles.Any(b => b.Role.Key == (int)BuildingRole.BuildingRoles.FloorEmergencyOfficer);
            var employeeHasApprovedAccessRequest = accessRequests
                .Where(a => a.Employee.Id == request.AccessRequest.Employee.Id)
                .Any(a => a.Status.Key == (int)AccessRequest.ApprovalStatus.Approved);

            // The ordering of these checks is important
            if (employeeHasApprovedAccessRequest ||
                isEmployeeFirstAidAttendant ||
                isEmployeeFloorEmergencyOfficer ||
                floorPlanCapacity.HasCapacity)
            {
                request.AccessRequest.Status.Key = (int)AccessRequest.ApprovalStatus.Approved;
            }

            await _accessRequestService.CreateAccessRequest(request.AccessRequest);

            // Update floor plan capacity
            floorPlanCapacity = await _floorPlanService.GetFloorPlanCapacityAsync(floorPlan.Id, DateOnly.FromDateTime(date));

            // Approve pending access requests to fill the remaining spots
            if ((isEmployeeFirstAidAttendant || isEmployeeFloorEmergencyOfficer) && floorPlanCapacity.HasCapacity)
            {
                var pendingAccessRequests = accessRequests
                    .Where(a => a.Status.Key == (int)AccessRequest.ApprovalStatus.Pending)
                    .GroupBy(a => a.Employee.Id)
                    .ToDictionary(g => g.Key, g => g.ToList());

                var remainingCapacity = floorPlanCapacity.RemainingCapacity;
                var remainingAccessRequests = pendingAccessRequests
                    .Take(Math.Min(remainingCapacity, pendingAccessRequests.Count))
                    .SelectMany(x => x.Value);

                foreach (var remainingAccessRequest in remainingAccessRequests)
                {
                    remainingAccessRequest.Status.Key = (int)AccessRequest.ApprovalStatus.Approved;
                    await _accessRequestService.UpdateAccessRequest(remainingAccessRequest);
                }

                // Update floor plan capacity
                floorPlanCapacity = await _floorPlanService.GetFloorPlanCapacityAsync(floorPlan.Id, DateOnly.FromDateTime(date));
            }

            // Send notifications if we reached capacity
            if (!floorPlanCapacity.HasCapacity)
            {
                var notifyFirstAidAttendants = floorPlanCapacity.TotalCapacity == floorPlanCapacity.MaxFirstAidAttendantCapacity;
                var notifyFloorEmergencyOfficers = floorPlanCapacity.TotalCapacity == floorPlanCapacity.MaxFloorEmergencyOfficerCapacity;

                var capacity = Math.Min(floorPlanCapacity.MaxFirstAidAttendantCapacity, floorPlanCapacity.MaxFloorEmergencyOfficerCapacity);
                var notification = new Notification
                {
                    Capacity = capacity,
                    Building = request.AccessRequest.Building,
                    Floor = request.AccessRequest.Floor
                };

                if (notifyFirstAidAttendants)
                {
                    await _notificationService.NotifyFirstAidAttendants(notification);
                }

                if (notifyFloorEmergencyOfficers)
                {
                    await _notificationService.NotifyFloorEmergencyOfficers(notification);
                }
            }

            return Unit.Value;
        }
    }
}
