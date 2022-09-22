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

    [HttpGet("{floorPlanId}/available")]
    public async Task<IEnumerable<Workspace>> Get(Guid floorPlanId, [FromQuery] DateTime date, [FromQuery] int start, [FromQuery] int end)
    {
        var startTime = date.AddHours(start);
        var endTime = date.AddHours(end);

        var query = new GetFloorPlanAvailableWorkspacesQuery
        {
            FloorPlanId = floorPlanId,
            StartTime = startTime,
            EndTime = endTime
        };

        var result = await Mediator.Send(query);

        return result
            .OrderBy(x => x.Name)
            .ToArray();
    }
}
