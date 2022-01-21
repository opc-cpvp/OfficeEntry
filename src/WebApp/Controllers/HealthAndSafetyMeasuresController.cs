using Microsoft.AspNetCore.Mvc;
using OfficeEntry.Application.User.Commands.UpdateHealthAndSafetyStatementRequests;

namespace OfficeEntry.WebApp.Controllers;

public class HealthAndSafetyMeasuresController : ApiController
{
    [HttpPut]
    public async Task Put(bool isHealthAndSafetyMeasuresAccepted)
    {
        await Mediator.Send(new UpdateHealthAndSafetyMeasuresStatementForCurrentUserCommand { IsHealthAndSafetyMeasuresAccepted = isHealthAndSafetyMeasuresAccepted });
    }
}
