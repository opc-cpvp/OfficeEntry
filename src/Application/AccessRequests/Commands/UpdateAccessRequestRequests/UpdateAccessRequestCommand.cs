using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;

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

        var date = request.AccessRequest.StartTime;
        var floorPlan = request.AccessRequest.FloorPlan;

        var floorPlanCapacity = await _locationService.GetCapacityByFloorPlanAsync(floorPlan.Id, DateOnly.FromDateTime(date));

        // Check if the floor plan has any remaining capacity
        if (floorPlanCapacity.HasCapacity)
        {
            var accessRequests = await _accessRequestService.GetApprovedOrPendingAccessRequestsByFloorPlan(floorPlan.Id, DateOnly.FromDateTime(date));
            var pendingAccessRequests = accessRequests
                .Where(a => a.Status.Key == (int)AccessRequest.ApprovalStatus.Pending)
                .GroupBy(a => a.Employee.Id)
                .ToDictionary(g => g.Key, g => g.ToList());

            var remainingCapacity = floorPlanCapacity.RemainingCapacity;
            var remainingAccessRequests = pendingAccessRequests
                .Take(Math.Min(remainingCapacity, pendingAccessRequests.Count))
                .SelectMany(x => x.Value);

            var updateAccessRequestTasks = remainingAccessRequests.SelectMany(x =>
            {
                x.Status.Key = (int)AccessRequest.ApprovalStatus.Approved;
                return new[]
                {
                    _accessRequestService.UpdateAccessRequest(x),
                    _notificationService.NotifyAccessRequestEmployee(new AccessRequestNotification
                    {
                        BaseUrl = request.BaseUrl,
                        AccessRequest = x
                    })
                };
            });

            await Task.WhenAll(updateAccessRequestTasks);

            floorPlanCapacity = await _locationService.GetCapacityByFloorPlanAsync(floorPlan.Id, DateOnly.FromDateTime(date));
            if (floorPlanCapacity.HasCapacity)
            {
                return Unit.Value;
            }
        }

        var notifyFirstAidAttendants = floorPlanCapacity.CurrentCapacity >= floorPlanCapacity.MaxFirstAidAttendantCapacity;
        var notifyFloorEmergencyOfficers = floorPlanCapacity.CurrentCapacity >= floorPlanCapacity.MaxFloorEmergencyOfficerCapacity;

        var capacity = floorPlanCapacity.MaxCapacity;
        var notification = new CapacityNotification
        {
            Capacity = capacity,
            Date = request.AccessRequest.StartTime,
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

        return Unit.Value;
    }
}
