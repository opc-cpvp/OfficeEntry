using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;

namespace OfficeEntry.Application.Locations.Queries.GetFloorPlan;

public record GetFloorPlanQuery : IRequest<FloorPlan>
{
    public Guid FloorPlanId { get; init; }
}

public class GetFloorsQueryHandler : IRequestHandler<GetFloorPlanQuery, FloorPlan>
{
    private readonly ILocationService _locationService;

    public GetFloorsQueryHandler(ILocationService locationService)
    {
        _locationService = locationService;
    }

    public Task<FloorPlan> Handle(GetFloorPlanQuery request, CancellationToken cancellationToken)
    {
        return _locationService.GetFloorPlanAsync(request.FloorPlanId);
    }
}
