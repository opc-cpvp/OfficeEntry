using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Application.Common.Models;
using OfficeEntry.Domain.Entities;
using OfficeEntry.Domain.Enums;

namespace OfficeEntry.Application.Locations.Queries.GetAvailableWorkspaces;

public record GetAvailableWorkspacesQuery : IRequest<IEnumerable<AvailableWorkspaceViewModel>>
{
    public string Locale { get; init; }
    public Guid FloorPlanId { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
}

public class GetAvailableWorkspacesQueryHandler : IRequestHandler<GetAvailableWorkspacesQuery, IEnumerable<AvailableWorkspaceViewModel>>
{
    private readonly IAccessRequestService _accessRequestService;
    private readonly ILocationService _locationService;

    public GetAvailableWorkspacesQueryHandler(IAccessRequestService accessRequestService, ILocationService locationService)
    {
        _accessRequestService = accessRequestService;
        _locationService = locationService;
    }

    public async Task<IEnumerable<AvailableWorkspaceViewModel>> Handle(GetAvailableWorkspacesQuery request, CancellationToken cancellationToken)
    {
        var date = DateOnly.FromDateTime(request.StartTime);
        var accessRequests = await _accessRequestService.GetApprovedOrPendingAccessRequestsByFloorPlan(request.FloorPlanId, date);
        var reservedWorkspaces = accessRequests.Where(ar =>
                ar.Workspace is not null &&
                ar.StartTime < request.EndTime &&
                ar.EndTime > request.StartTime
            )
            .Select(a => a.Workspace.Id)
            .Distinct();

        var floorPlanWorkspaces = await _locationService.GetWorkspacesAsync(request.FloorPlanId);
        var availableWorkspaces = floorPlanWorkspaces
            .Where(w => !reservedWorkspaces.Contains(w.Id))
            .Select(x =>
            {
                var description = (request.Locale == Locale.French) ? x.FrenchDescription : x.EnglishDescription;
                var hasDescription = !string.IsNullOrWhiteSpace(description);

                return new AvailableWorkspaceViewModel
                {
                    Id = x.Id,
                    Name = hasDescription ? $"{x.Name} - {description}" : x.Name
                };
            });

        return availableWorkspaces;
    }
}
