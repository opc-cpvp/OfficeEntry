using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;
using System.Security.Principal;

namespace OfficeEntry.Application.AccessRequests.Commands.UpdateAccessRequestRequests;

public class UpdateAccessRequestCommand : IRequest
{
    public AccessRequest AccessRequest { get; set; }
}

public class UpdateAccessRequestCommandHandler : IRequestHandler<UpdateAccessRequestCommand>
{
    private readonly IAccessRequestService _accessRequestService;
    private readonly IBuildingRoleService _buildingRoleService;
    private readonly IFloorPlanService _floorPlanService;
    private readonly IUserService _userService;
    private readonly INotificationService _notificationService;

    public UpdateAccessRequestCommandHandler(
        IAccessRequestService accessRequestService,
        IBuildingRoleService buildingRoleService,
        IFloorPlanService floorPlanService,
        IUserService userService,
        INotificationService notificationService)
    {
        _accessRequestService = accessRequestService;
        _buildingRoleService = buildingRoleService;
        _floorPlanService = floorPlanService;
        _userService = userService;
        _notificationService = notificationService;
    }

    public async Task<Unit> Handle(UpdateAccessRequestCommand request, CancellationToken cancellationToken)
    {
        await _accessRequestService.UpdateAccessRequest(request.AccessRequest);

        // Check if the request is cancelled
        var isCancelled = request.AccessRequest.Status.Key == (int)AccessRequest.ApprovalStatus.Cancelled;
        if (!isCancelled)
        {
            return Unit.Value;
        }

        var date = request.AccessRequest.StartTime;
        var floorPlan = request.AccessRequest.FloorPlan;
        var floorId = floorPlan.Floor.Id;

        var (_, buildingRoles) = await _buildingRoleService.GetBuildingRolesFor(request.AccessRequest.Employee.Id);
        buildingRoles = buildingRoles.Where(r => r.Floor?.Id == floorId);

        var isEmployeeFirstAidAttendant = buildingRoles.Any(b => b.Role.Key == (int)BuildingRole.BuildingRoles.FirstAidAttendant);
        var isEmployeeFloorEmergencyOfficer = buildingRoles.Any(b => b.Role.Key == (int)BuildingRole.BuildingRoles.FloorEmergencyOfficer);

        // Check if the employee is either a First Aid Attendant or Floor Emergency Officer
        if (!isEmployeeFirstAidAttendant || !isEmployeeFloorEmergencyOfficer)
        {
            return Unit.Value;
        }

        var floorPlanCapacity = await _floorPlanService.GetFloorPlanCapacityAsync(floorPlan.Id, DateOnly.FromDateTime(date));

        // Check if the floor plan has any remaining capacity
        if (floorPlanCapacity.HasCapacity)
        {
            return Unit.Value;
        }

        var notifyFirstAidAttendants = floorPlanCapacity.CurrentCapacity >= floorPlanCapacity.MaxFirstAidAttendantCapacity;
        var notifyFloorEmergencyOfficers = floorPlanCapacity.CurrentCapacity >= floorPlanCapacity.MaxFloorEmergencyOfficerCapacity;

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

        return Unit.Value;
    }
}
