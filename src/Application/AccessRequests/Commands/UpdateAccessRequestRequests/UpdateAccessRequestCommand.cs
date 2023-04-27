using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;
using OfficeEntry.Domain.Enums;

namespace OfficeEntry.Application.AccessRequests.Commands.UpdateAccessRequestRequests;

public record UpdateAccessRequestCommand : IRequest
{
    public string BaseUrl { get; init; }
    public AccessRequest AccessRequest { get; init; }
}

public class UpdateAccessRequestCommandHandler : IRequestHandler<UpdateAccessRequestCommand>
{
    private readonly IAccessRequestService _accessRequestService;
    private readonly ILocationService _locationService;
    private readonly INotificationService _notificationService;

    public UpdateAccessRequestCommandHandler(
        IAccessRequestService accessRequestService,
        ILocationService locationService,
        INotificationService notificationService)
    {
        _accessRequestService = accessRequestService;
        _locationService = locationService;
        _notificationService = notificationService;
    }

    public async Task<Unit> Handle(UpdateAccessRequestCommand request, CancellationToken cancellationToken)
    {
        await _accessRequestService.UpdateAccessRequest(request.AccessRequest);
        await _notificationService.NotifyAccessRequestEmployee(new AccessRequestNotification
        {
            BaseUrl = request.BaseUrl,
            AccessRequest = request.AccessRequest
        });

        // Check if the request is cancelled
        var isCancelled = request.AccessRequest.Status.Key == (int)AccessRequest.ApprovalStatus.Cancelled;
        if (!isCancelled)
        {
            return Unit.Value;
        }

        // Check if a floor plan is associated with the request
        var hasFloorPlan = request.AccessRequest.FloorPlan.Id != Guid.Empty;
        if (!hasFloorPlan)
        {
            return Unit.Value;
        }

        var requestDate = request.AccessRequest.StartTime;
        var floorPlan = request.AccessRequest.FloorPlan;

        var initialFloorPlanCapacity = await _locationService.GetCapacityByFloorPlanAsync(floorPlan.Id, DateOnly.FromDateTime(requestDate));
        await ApprovePendingAccessRequests(request, initialFloorPlanCapacity);
        await NotifyEmergencyPersonnelOfMaximumCapacity(request);

        return Unit.Value;
    }

    private async Task ApprovePendingAccessRequests(UpdateAccessRequestCommand request, FloorPlanCapacity floorPlanCapacity)
    {
        // Approve pending access requests to fill the remaining spots
        if (floorPlanCapacity.HasCapacity)
        {
            var accessRequests = await _accessRequestService.GetApprovedOrPendingAccessRequestsByFloorPlan(request.AccessRequest.FloorPlan.Id, DateOnly.FromDateTime(request.AccessRequest.StartTime));
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

    private async Task NotifyEmergencyPersonnelOfMaximumCapacity(UpdateAccessRequestCommand request)
    {
        var currentFloorPlanCapacity = await _locationService.GetCapacityByFloorPlanAsync(request.AccessRequest.FloorPlan.Id, DateOnly.FromDateTime(request.AccessRequest.StartTime));

        var notifyFirstAidAttendantsOfMaxCapacity = currentFloorPlanCapacity.NeedsFirstAidAttendant;
        var notifyFloorEmergencyOfficersOfMaxCapacity = currentFloorPlanCapacity.NeedsFloorEmergencyOfficer;

        if (notifyFirstAidAttendantsOfMaxCapacity || notifyFloorEmergencyOfficersOfMaxCapacity)
        {
            // Send notifications if we reached capacity
            var capacity = currentFloorPlanCapacity.MaxCapacity;
            var notification = new CapacityNotification(CapacityNotification.NotificationType.Maximum)
            {
                Capacity = capacity,
                Date = request.AccessRequest.StartTime,
                Building = request.AccessRequest.Building,
                Floor = request.AccessRequest.Floor,
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
