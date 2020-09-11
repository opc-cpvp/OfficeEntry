using Microsoft.AspNetCore.Mvc;
using OfficeEntry.Application.User.Commands.UpdateHealthAndSafetyStatementRequests;
using System.Threading.Tasks;

namespace OfficeEntry.WebApp.Controllers
{
    public class HealthAndSafetyMeasuresController : ApiController
    {
        [HttpPut]
        public async Task Put(bool isHealthAndSafetyMeasuresAccepted)
        {
            await Mediator.Send(new UpdateHealthAndSafetyMeasuresStatementForCurrentUserCommand { IsHealthAndSafetyMeasuresAccepted = isHealthAndSafetyMeasuresAccepted });
        }
    }
}