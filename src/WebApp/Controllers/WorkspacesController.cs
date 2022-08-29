using Microsoft.AspNetCore.Mvc;
using OfficeEntry.Application.Locations.Queries.GetFloorPlan;
using OfficeEntry.Domain.Entities;

namespace OfficeEntry.WebApp.Controllers;

public class WorkspacesController : ApiController
{
    [HttpGet("{floorPlanId}")]
    public async Task<IEnumerable<Workspace>> Get(Guid floorPlanId)
    {
        var result = await Mediator.Send(new GetFloorPlanQuery { FloorPlanId = floorPlanId });

        return result.Workspaces
            .OrderBy(x => x.Name)
            .ToArray();
    }
}
