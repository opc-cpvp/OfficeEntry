using Microsoft.AspNetCore.Mvc;
using OfficeEntry.Application.AccessRequests.Queries.GetSpotsAvailablePerHour;

namespace OfficeEntry.WebApp.Controllers;

public class SpotsAvailablePerHourController : ApiController
{
    [HttpGet]
    public async Task<IEnumerable<CurrentCapacity>> Get(Guid floorId, DateTime date)
    {
        var t = await Mediator.Send(new GetSpotsAvailablePerHourQuery { FloorId = floorId, SelectedDay = date });

        return t;
    }
}
