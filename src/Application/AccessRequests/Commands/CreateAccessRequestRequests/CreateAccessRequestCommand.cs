using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;
using OfficeEntry.Domain.Enums;
using System.Collections.Immutable;

namespace OfficeEntry.Application.AccessRequests.Commands.CreateAccessRequestRequests
{
    public record CreateAccessRequestCommand : IRequest
    {
        public string BaseUrl { get; init; }
        public AccessRequest AccessRequest { get; init; }
    }

    public class CreateAccessRequestCommandHandler : IRequestHandler<CreateAccessRequestCommand>
    {
        private readonly IAccessRequestService _accessRequestService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserService _userService;
        private readonly ILocationService _locationService;
        private readonly INotificationService _notificationService;

        public CreateAccessRequestCommandHandler(
            IAccessRequestService accessRequestService,
            ICurrentUserService currentUserService,
            IUserService userService,
            ILocationService locationService,
            INotificationService notificationService
        )
        {
            _accessRequestService = accessRequestService;
            _currentUserService = currentUserService;
            _userService = userService;
            _locationService = locationService;
            _notificationService = notificationService;
        }

        public async Task<Unit> Handle(CreateAccessRequestCommand request, CancellationToken cancellationToken)
        {
            var username = _currentUserService.UserId;
            var (_, currentContact) = await _userService.GetContactByUsername(username);

            if (currentContact.UserSettings?.HealthSafety is null || currentContact.UserSettings?.PrivacyStatement is null)
            {
                throw new Exception("Can't create an access request without accepting Privacy Act statement and Health and Safety measures");
            }

            var floorPlan = request.AccessRequest.FloorPlan;
            var requestDate = request.AccessRequest.StartTime;

            if (request.AccessRequest.Employee is null)
            {
                request.AccessRequest.Employee = currentContact;
            }
            else
            {
                var (_, employee) = await _userService.GetContact(request.AccessRequest.Employee.Id);
                request.AccessRequest.Employee = employee;
                request.AccessRequest.Delegate = currentContact;
            }

            var accessRequestsTask = _accessRequestService.GetApprovedOrPendingAccessRequestsByFloorPlan(floorPlan.Id, DateOnly.FromDateTime(requestDate));
            var floorPlanCapacityTask = _locationService.GetCapacityByFloorPlanAsync(floorPlan.Id, DateOnly.FromDateTime(requestDate));
            var firstAidAttendantsTask = _locationService.GetFirstAidAttendantsAsync(request.AccessRequest.Building.Id);
            var floorEmergencyOfficersTask = _locationService.GetFloorEmergencyOfficersAsync(request.AccessRequest.Building.Id);

            await Task.WhenAll(accessRequestsTask, floorPlanCapacityTask, firstAidAttendantsTask, floorEmergencyOfficersTask);

            var accessRequests = accessRequestsTask.Result;
            var initialFloorPlanCapacity = floorPlanCapacityTask.Result;
            var firstAidAttendants = firstAidAttendantsTask.Result;
            var floorEmergencyOfficers = floorEmergencyOfficersTask.Result;

            var isEmployeeFirstAidAttendant = firstAidAttendants.Any(x => x.Id == request.AccessRequest.Employee.Id);
            var isEmployeeFloorEmergencyOfficer = floorEmergencyOfficers.Any(x => x.Id == request.AccessRequest.Employee.Id);
            var employeeHasApprovedAccessRequest = accessRequests
                .Where(a => a.Employee.Id == request.AccessRequest.Employee.Id)
                .Any(a => a.Status.Key == (int)AccessRequest.ApprovalStatus.Approved);

            request.AccessRequest.FirstAidAttendant = isEmployeeFirstAidAttendant;
            request.AccessRequest.FloorEmergencyOfficer = isEmployeeFloorEmergencyOfficer;

            // The ordering of these checks is important
            // 1. Check if the employee already has an approved access request
            // 2. Check if the floor requires an additional first aid attendant and that the employee is one
            // 3. Check if the floor requires an additional floor emergency officer and that the employee is one
            // 4. Check if the floor has capacity
            if (employeeHasApprovedAccessRequest ||
                initialFloorPlanCapacity.NeedsFirstAidAttendant && isEmployeeFirstAidAttendant ||
                initialFloorPlanCapacity.NeedsFloorEmergencyOfficer && isEmployeeFloorEmergencyOfficer ||
                initialFloorPlanCapacity.HasCapacity)
            {
                request.AccessRequest.Status.Key = (int)AccessRequest.ApprovalStatus.Approved;
            }

            var (result, accessRequest) = await _accessRequestService.CreateAccessRequest(request.AccessRequest);

            if (!result.Succeeded)
            {
                return Unit.Value;
            }

            // Update access request properties
            request.AccessRequest.Id = accessRequest.Id;
            request.AccessRequest.CreatedOn = accessRequest.CreatedOn;

            if (request.AccessRequest.Workspace is not null)
            {
                request.AccessRequest.Workspace = await _locationService.GetWorkspaceAsync(request.AccessRequest.Workspace.Id);
            }

            await _notificationService.NotifyAccessRequestEmployee(new AccessRequestNotification
            {
                BaseUrl = request.BaseUrl,
                AccessRequest = request.AccessRequest
            });

            var floorPlanCapacityAfterRequest = await _locationService.GetCapacityByFloorPlanAsync(floorPlan.Id, DateOnly.FromDateTime(requestDate));
            await ApprovePendingAccessRequests(request, floorPlanCapacityAfterRequest, accessRequests);
            await NotifyEmergencyPersonnelOfCapacity(request, initialFloorPlanCapacity);

            return Unit.Value;
        }

        private async Task ApprovePendingAccessRequests(CreateAccessRequestCommand request, FloorPlanCapacity floorPlanCapacity, ImmutableArray<AccessRequest> accessRequests)
        {
            // Approve pending access requests to fill the remaining spots
            if (floorPlanCapacity.HasCapacity)
            {
                var pendingAccessRequests = accessRequests
                    .Where(a => a.Status.Key == (int)AccessRequest.ApprovalStatus.Pending)
                    .GroupBy(a => a.Employee.Id)
                    .ToDictionary(g => g.Key, g => g.ToList());

                var remainingCapacity = floorPlanCapacity.RemainingCapacity;
                var remainingAccessRequests = pendingAccessRequests
                    .Take(Math.Min(remainingCapacity, pendingAccessRequests.Count))
                    .SelectMany(x => x.Value);


                var updateAccessRequestTasks = remainingAccessRequests.Select(async x =>
                {
                    x.Status.Key = (int)AccessRequest.ApprovalStatus.Approved;
                    await _accessRequestService.UpdateAccessRequest(x);
                    await _notificationService.NotifyAccessRequestEmployee(new AccessRequestNotification
                    {
                        AccessRequest = x,
                        BaseUrl = request.BaseUrl
                    });
                });

                await Task.WhenAll(updateAccessRequestTasks);
            }
        }

        private async Task NotifyEmergencyPersonnelOfCapacity(CreateAccessRequestCommand request, FloorPlanCapacity initialFloorPlanCapacity)
        {
            var currentFloorPlanCapacity = await _locationService.GetCapacityByFloorPlanAsync(request.AccessRequest.FloorPlan.Id, DateOnly.FromDateTime(request.AccessRequest.StartTime));

            var notifyFirstAidAttendantsOfAvailableCapacity = initialFloorPlanCapacity.NeedsFirstAidAttendant && !currentFloorPlanCapacity.NeedsFirstAidAttendant;
            var notifyFloorEmergencyOfficersOfAvailableCapacity = initialFloorPlanCapacity.NeedsFloorEmergencyOfficer && !currentFloorPlanCapacity.NeedsFloorEmergencyOfficer;
            var notifyFirstAidAttendantsOfMaxCapacity = currentFloorPlanCapacity.NeedsFirstAidAttendant;
            var notifyFloorEmergencyOfficersOfMaxCapacity = currentFloorPlanCapacity.NeedsFloorEmergencyOfficer;

            // Send notifications if capacity is available following the request
            var capacityNofication = new CapacityNotification(CapacityNotification.NotificationType.Available)
            {
                Date = request.AccessRequest.StartTime,
                Building = request.AccessRequest.Building,
                Floor = request.AccessRequest.Floor,
            };

            if (notifyFirstAidAttendantsOfAvailableCapacity)
            {
                await _notificationService.NotifyOfAvailableCapacity(capacityAvailableNotification, EmployeeRoleType.FirstAidAttendant);
            }

            if (notifyFloorEmergencyOfficersOfAvailableCapacity)
            {
                await _notificationService.NotifyOfAvailableCapacity(capacityAvailableNotification, EmployeeRoleType.FloorEmergencyOfficer);
            }

            // Send notifications if we reached capacity
            var capacity = currentFloorPlanCapacity.MaxCapacity;
            capacityNotification = new CapacityNotification(CapacityNotification.NotificationType.Maximum)
            {
                Capacity = capacity,
                Date = request.AccessRequest.StartTime,
                Building = request.AccessRequest.Building,
                Floor = request.AccessRequest.Floor
            };

            if (notifyFirstAidAttendantsOfMaxCapacity)
            {
                await _notificationService.NotifyOfMaximumCapacityReached(notification, EmployeeRoleType.FirstAidAttendant);
            }

            if (notifyFloorEmergencyOfficersOfMaxCapacity)
            {
                await _notificationService.NotifyOfMaximumCapacityReached(notification, EmployeeRoleType.FloorEmergencyOfficer);
            }
        }
    }
}
