using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;

namespace OfficeEntry.Application.AccessRequests.Commands.UpdateAccessRequestRequests;

public record UpdateAccessRequestCommand : IRequest
{
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
        await _notificationService.NotifyAccessRequestEmployee(new AccessRequestNotification { AccessRequest = request.AccessRequest });

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

        var firstAidAttendants = await _locationService.GetFirstAidAttendantsAsync(request.AccessRequest.Building.Id);
        var floorEmergencyOfficers = await _locationService.GetFloorEmergencyOfficersAsync(request.AccessRequest.Building.Id);

        var isEmployeeFirstAidAttendant = firstAidAttendants.Any(x => x.Id == request.AccessRequest.Employee.Id);
        var isEmployeeFloorEmergencyOfficer = floorEmergencyOfficers.Any(x => x.Id == request.AccessRequest.Employee.Id);

        // Check if the employee is either a First Aid Attendant or Floor Emergency Officer
        if (!isEmployeeFirstAidAttendant || !isEmployeeFloorEmergencyOfficer)
        {
            return Unit.Value;
        }

        var floorPlanCapacity = await _locationService.GetCapacityByFloorPlanAsync(floorPlan.Id, DateOnly.FromDateTime(date));

        // Check if the floor plan has any remaining capacity
        if (floorPlanCapacity.HasCapacity)
        {
            return Unit.Value;
        }

        var notifyFirstAidAttendants = floorPlanCapacity.CurrentCapacity >= floorPlanCapacity.MaxFirstAidAttendantCapacity;
        var notifyFloorEmergencyOfficers = floorPlanCapacity.CurrentCapacity >= floorPlanCapacity.MaxFloorEmergencyOfficerCapacity;

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
            await _notificationService.NotifyFirstAidAttendants(notification);
        }

        if (notifyFloorEmergencyOfficers)
        {
            await _notificationService.NotifyFloorEmergencyOfficers(notification);
        }

        return Unit.Value;
    }
}
