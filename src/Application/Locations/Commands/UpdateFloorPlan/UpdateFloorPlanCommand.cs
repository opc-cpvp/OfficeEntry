using MagicOnion;
using MagicOnion.Server;
using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Services;

namespace OfficeEntry.Application.Locations.Commands.UpdateFloorPlan;

public class UpdateAccessRequestCommandHandler :
    ServiceBase<IUpdateFloorPlanCommandService>,
    IUpdateFloorPlanCommandService,
    IRequestHandler<UpdateFloorPlanCommand>
{
    private readonly ILocationService _locationService;

    public UpdateAccessRequestCommandHandler(ILocationService locationService)
    {
        _locationService = locationService;
    }

    public async Task Handle(UpdateFloorPlanCommand request, CancellationToken cancellationToken)
    {
        await _locationService.UpdateFloorPlan(request.FloorPlan);
    }

    public async UnaryResult HandleAsync(UpdateFloorPlanCommand request)
    {
        await Handle(request, CancellationToken.None);
    }
}
