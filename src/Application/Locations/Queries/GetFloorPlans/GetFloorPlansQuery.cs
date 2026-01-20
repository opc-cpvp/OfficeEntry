using MagicOnion;
using MagicOnion.Server;
using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;
using OfficeEntry.Domain.Services;
using System.Collections.Immutable;

namespace OfficeEntry.Application.Locations.Queries.GetFloorPlans;

public class GetFloorPlansHandler :
    ServiceBase<IGetFloorPlansQueryService>,
    IGetFloorPlansQueryService,
    IRequestHandler<GetFloorPlansQuery, ImmutableArray<FloorPlan>>
{
    private readonly ILocationService _locationService;

    public GetFloorPlansHandler(ILocationService locationService)
    {
        _locationService = locationService;
    }

    public async Task<ImmutableArray<FloorPlan>> Handle(GetFloorPlansQuery request, CancellationToken cancellationToken)
    {
        return await _locationService.GetFloorPlansAsync(request.Keyword);
    }

    public async UnaryResult<FloorPlan[]> HandleAsync(GetFloorPlansQuery request)
    {
        return [.. await Handle(request, CancellationToken.None)];
    }
}
