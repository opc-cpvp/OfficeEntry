using MagicOnion;
using MagicOnion.Server;
using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;
using OfficeEntry.Domain.Services;

namespace OfficeEntry.Application.Locations.Queries.GetFloorPlan;

public class GetFloorsQueryHandler :
    ServiceBase<IGetFloorPlanQueryService>,
    IGetFloorPlanQueryService,
    IRequestHandler<GetFloorPlanQuery, FloorPlan>
{
    private readonly ILocationService _locationService;

    public GetFloorsQueryHandler(ILocationService locationService)
    {
        _locationService = locationService;
    }

    public async Task<FloorPlan> Handle(GetFloorPlanQuery request, CancellationToken cancellationToken)
    {
        return await _locationService.GetFloorPlanAsync(request.FloorPlanId);
    }

    public async UnaryResult<FloorPlan> HandleAsync(GetFloorPlanQuery request)
    {
        return await _locationService.GetFloorPlanAsync(request.FloorPlanId);
    }
}
