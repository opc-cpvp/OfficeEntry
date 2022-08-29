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
    private readonly IFloorPlanService _floorPlanService;

    public GetFloorPlansHandler(IFloorPlanService floorPlanService)
    {
        _floorPlanService = floorPlanService;
    }

    public Task<ImmutableArray<FloorPlan>> Handle(GetFloorPlansQuery request, CancellationToken cancellationToken)
    {
        return _floorPlanService.GetFloorPlansAsync(request.Keyword);
    }
}
