using MediatR;
using OfficeEntry.Application.AccessRequests.Commands.UpdateAccessRequestRequests;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;

namespace OfficeEntry.Application.AccessRequests.Commands.CreateAccessRequestRequests
{
    public record CreateAccessRequestCommand : IRequest
    {
        public AccessRequest AccessRequest { get; init; }
    }

    public class CreateAccessRequestCommandHandler : IRequestHandler<CreateAccessRequestCommand>
    {
        private readonly IAccessRequestService _accessRequestService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserService _userService;
        private readonly ILocationService _locationService;
        private readonly INotificationService _notificationService;

        private IMediator _mediator;

        public CreateAccessRequestCommandHandler(
            IAccessRequestService accessRequestService,
            ICurrentUserService currentUserService,
            IUserService userService,
            ILocationService locationService,
            INotificationService notificationService,
            IMediator mediator
        ) {
            _accessRequestService = accessRequestService;
            _currentUserService = currentUserService;
            _userService = userService;
            _locationService = locationService;
            _notificationService = notificationService;

            _mediator = mediator;
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
            var date = request.AccessRequest.StartTime;

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

            var accessRequestsTask = _accessRequestService.GetApprovedOrPendingAccessRequestsByFloorPlan(floorPlan.Id, DateOnly.FromDateTime(date));
            var floorPlanCapacityTask = _locationService.GetCapacityByFloorPlanAsync(floorPlan.Id, DateOnly.FromDateTime(date));
            var firstAidAttendantsTask = _locationService.GetFirstAidAttendantsAsync(request.AccessRequest.Building.Id);
            var floorEmergencyOfficersTask = _locationService.GetFloorEmergencyOfficersAsync(request.AccessRequest.Building.Id);

            await Task.WhenAll(accessRequestsTask, floorPlanCapacityTask, firstAidAttendantsTask, floorEmergencyOfficersTask);

            var accessRequests = accessRequestsTask.Result;
            var floorPlanCapacity = floorPlanCapacityTask.Result;
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
            if (employeeHasApprovedAccessRequest ||
                isEmployeeFirstAidAttendant ||
                isEmployeeFloorEmergencyOfficer ||
                floorPlanCapacity.HasCapacity)
            {
                request.AccessRequest.Status.Key = (int)AccessRequest.ApprovalStatus.Approved;
            }

            var (_, accessRequest) = await _accessRequestService.CreateAccessRequest(request.AccessRequest);

            // Update access request properties
            request.AccessRequest.Id = accessRequest.Id;
            request.AccessRequest.CreatedOn = accessRequest.CreatedOn;

            if (request.AccessRequest.Workspace is not null)
            {
                request.AccessRequest.Workspace = await _locationService.GetWorkspaceAsync(request.AccessRequest.Workspace.Id);
            }

            _ = _notificationService.NotifyAccessRequestEmployee(new AccessRequestNotification { AccessRequest = request.AccessRequest });

            // Update floor plan capacity
            floorPlanCapacity = await _locationService.GetCapacityByFloorPlanAsync(floorPlan.Id, DateOnly.FromDateTime(date));

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

                var updateAccessRequestTasks = remainingAccessRequests.Select(x =>
                {
                    x.Status.Key = (int)AccessRequest.ApprovalStatus.Approved;
                    return _mediator.Send(new UpdateAccessRequestCommand { AccessRequest = x }, cancellationToken);
                });

                await Task.WhenAll(updateAccessRequestTasks);

                // Update floor plan capacity
                floorPlanCapacity = await _locationService.GetCapacityByFloorPlanAsync(floorPlan.Id, DateOnly.FromDateTime(date));
            }

            if (floorPlanCapacity.HasCapacity)
            {
                return Unit.Value;
            }

            // Send notifications if we reached capacity
            var notifyFirstAidAttendants = floorPlanCapacity.TotalCapacity == floorPlanCapacity.MaxFirstAidAttendantCapacity;
            var notifyFloorEmergencyOfficers = floorPlanCapacity.TotalCapacity == floorPlanCapacity.MaxFloorEmergencyOfficerCapacity;

            var capacity = Math.Min(floorPlanCapacity.MaxFirstAidAttendantCapacity, floorPlanCapacity.MaxFloorEmergencyOfficerCapacity);
            var notification = new CapacityNotification
            {
                Capacity = capacity,
                Date = request.AccessRequest.StartTime,
                Building = request.AccessRequest.Building,
                Floor = request.AccessRequest.Floor
            };

            if (notifyFirstAidAttendants)
            {
                _ = _notificationService.NotifyFirstAidAttendants(notification);
            }

            if (notifyFloorEmergencyOfficers)
            {
                _ = _notificationService.NotifyFloorEmergencyOfficers(notification);
            }

            return Unit.Value;
        }
    }
}
