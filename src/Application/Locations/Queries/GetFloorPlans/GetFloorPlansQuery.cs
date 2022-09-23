using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;
using System.Collections.Immutable;

namespace OfficeEntry.Application.Locations.Queries.GetFloorPlans;

public class GetFloorPlansQuery : IRequest<ImmutableArray<FloorPlan>>
{
    public string Keyword { get; init; }
}

public class GetFloorPlansHandler : IRequestHandler<GetFloorPlansQuery, ImmutableArray<FloorPlan>>
{
    private readonly ILocationService _locationService;

    public GetFloorPlansHandler(ILocationService locationService)
    {
        _locationService = locationService;
    }

    public Task<ImmutableArray<FloorPlan>> Handle(GetFloorPlansQuery request, CancellationToken cancellationToken)
    {
        return _locationService.GetFloorPlansAsync(request.Keyword);
    }
}
