using Microsoft.AspNetCore.Mvc;
using OfficeEntry.Application.Locations.Queries.GetAvailableWorkspaces;
using OfficeEntry.Domain.Entities;

namespace OfficeEntry.WebApp.Controllers;

public class LocationsController : ApiController
{
    [HttpGet("{locale}/floorplans/{floorPlanId}/workspaces/available")]
    public async Task<IEnumerable<Workspace>> GetAvailableWorkspaces(string locale, Guid floorPlanId, [FromQuery] DateTime date, [FromQuery] int start, [FromQuery] int end)
    {
        var startTime = date.AddHours(start);
        var endTime = date.AddHours(end);

        var result = await Mediator.Send(new GetAvailableWorkspacesQuery
        {
            FloorPlanId = floorPlanId,
            StartTime = startTime,
            EndTime = endTime
        });

        return result
            .OrderBy(x => x.Name)
            .ToArray();
    }
}
