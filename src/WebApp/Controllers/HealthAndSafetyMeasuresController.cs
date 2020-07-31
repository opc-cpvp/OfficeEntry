using Microsoft.AspNetCore.Mvc;
using OfficeEntry.Application.TermsAndConditions.Commands.UpdateHealthAndSafetyStatementRequests;
using OfficeEntry.Application.TermsAndConditions.Queries.GetHealthAndSafetyMeasuresRequests;
using System.Threading.Tasks;

namespace OfficeEntry.WebApp.Controllers
{
    public class HealthAndSafetyMeasuresController : ApiController
    {
        [HttpGet]
        public async Task<bool> Get()
        {
            return await Mediator.Send(new GetHealthAndSafetyMeasuresForCurrentUserQuery());
        }

        [HttpPut]
        public async Task Put(bool isHealthAndSafetyMeasuresAccepted)
        {
            await Mediator.Send(new UpdateHealthAndSafetyMeasuresStatementForCurrentUserCommand { IsHealthAndSafetyMeasuresAccepted = isHealthAndSafetyMeasuresAccepted });
        }
    }
}