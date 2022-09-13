using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;

namespace OfficeEntry.Application.Locations.Queries.GetFloorPlan;

public class GetFloorPlanQuery : IRequest<FloorPlan>
{
    public Guid FloorPlanId { get; init; }
}

public class GetFloorsQueryHandler : IRequestHandler<GetFloorPlanQuery, FloorPlan>
{
    private readonly IFloorPlanService _floorPlanService;

    public GetFloorsQueryHandler(IFloorPlanService floorPlanService)
    {
        _floorPlanService = floorPlanService;
    }

    public Task<FloorPlan> Handle(GetFloorPlanQuery request, CancellationToken cancellationToken)
    {
        return _floorPlanService.GetFloorPlanByIdAsync(request.FloorPlanId);
    }
}
