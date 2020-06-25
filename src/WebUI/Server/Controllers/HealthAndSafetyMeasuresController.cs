using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeEntry.Application.TermsAndConditions.Commands.UpdateHealthAndSafetyStatementRequests;
using OfficeEntry.Application.TermsAndConditions.Queries.GetHealthAndSafetyMeasuresRequests;

namespace OfficeEntry.WebUI.Server.Controllers
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
