using Microsoft.AspNetCore.Mvc;
using OfficeEntry.Application.Locations.Queries.GetAvailableWorkspaces;
using OfficeEntry.Domain.Entities;
using OfficeEntry.Domain.Services;
using OfficeEntry.Domain.ViewModels;

namespace OfficeEntry.WebApp.Controllers;

public class LocationsController : ApiController
{
    [HttpGet("{locale}/floorplans/{floorPlanId}/workspaces/available")]
    public async Task<IEnumerable<AvailableWorkspaceViewModel>> GetAvailableWorkspaces(string locale, Guid floorPlanId, [FromQuery] DateTime date, [FromQuery] int start, [FromQuery] int end)
    {
        var startTime = date.AddHours(start);
        var endTime = date.AddHours(end);

        var result = await Mediator.Send(new GetAvailableWorkspacesQuery
        {
            Locale = locale,
            FloorPlanId = floorPlanId,
            StartTime = startTime,
            EndTime = endTime
        });

        return result
            .OrderBy(x => x.Name)
            .ToArray();
    }
}
