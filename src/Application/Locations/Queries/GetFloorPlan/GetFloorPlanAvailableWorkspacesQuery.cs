using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;

namespace OfficeEntry.Application.Locations.Queries.GetFloorPlan;

public class GetFloorPlanAvailableWorkspacesQuery : IRequest<IEnumerable<Workspace>>
{
    public Guid FloorPlanId { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
}

public class GetFloorPlanAvailableWorkspacesQueryHandler : IRequestHandler<GetFloorPlanAvailableWorkspacesQuery, IEnumerable<Workspace>>
{
    private readonly IAccessRequestService _accessRequestService;
    private readonly ILocationService _locationService;

    public GetFloorPlanAvailableWorkspacesQueryHandler(IAccessRequestService accessRequestService, ILocationService locationService)
    {
        _accessRequestService = accessRequestService;
        _locationService = locationService;
    }

    public async Task<IEnumerable<Workspace>> Handle(GetFloorPlanAvailableWorkspacesQuery request, CancellationToken cancellationToken)
    {
        var date = DateOnly.FromDateTime(request.StartTime);
        var accessRequests = await _accessRequestService.GetApprovedOrPendingAccessRequestsByFloorPlan(request.FloorPlanId, date);
        var reservedWorkspaces = accessRequests.Where(ar =>
                ar.StartTime < request.EndTime &&
                ar.EndTime > request.StartTime
            )
            .Select(a => a.Workspace.Id)
            .Distinct();

        var floorPlan = await _locationService.GetFloorPlanAsync(request.FloorPlanId);
        var availableWorkspaces = floorPlan.Workspaces
            .Where(w => !reservedWorkspaces.Contains(w.Id));

        return availableWorkspaces;
    }
}
