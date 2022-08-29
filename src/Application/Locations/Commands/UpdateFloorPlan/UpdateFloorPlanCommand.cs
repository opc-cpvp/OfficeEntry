using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;

namespace OfficeEntry.Application.Locations.Commands.UpdateFloorPlan;

public class UpdateFloorPlanCommand : IRequest
{
    public FloorPlan FloorPlan { get; set; }
}

public class UpdateAccessRequestCommandHandler : IRequestHandler<UpdateFloorPlanCommand>
{
    private readonly IFloorPlanService _floorPlanService;

    public UpdateAccessRequestCommandHandler(IFloorPlanService floorPlanService)
    {
        _floorPlanService = floorPlanService;
    }

    public async Task<Unit> Handle(UpdateFloorPlanCommand request, CancellationToken cancellationToken)
    {
        await _floorPlanService.UpdateFloorPlan(request.FloorPlan);

        return Unit.Value;
    }
}
