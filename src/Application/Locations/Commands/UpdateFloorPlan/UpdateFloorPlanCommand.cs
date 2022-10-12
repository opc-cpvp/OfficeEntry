using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;

namespace OfficeEntry.Application.Locations.Commands.UpdateFloorPlan;

public record UpdateFloorPlanCommand : IRequest
{
    public FloorPlan FloorPlan { get; init; }
}

public class UpdateAccessRequestCommandHandler : IRequestHandler<UpdateFloorPlanCommand>
{
    private readonly ILocationService _locationService;

    public UpdateAccessRequestCommandHandler(ILocationService locationService)
    {
        _locationService = locationService;
    }

    public async Task<Unit> Handle(UpdateFloorPlanCommand request, CancellationToken cancellationToken)
    {
        await _locationService.UpdateFloorPlan(request.FloorPlan);

        return Unit.Value;
    }
}
