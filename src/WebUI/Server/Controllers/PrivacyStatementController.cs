using OfficeEntry.Application.Locations.Queries.GetFloors;
using OfficeEntry.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using OfficeEntry.Application.TermsAndConditions.Queries;
using OfficeEntry.Application.TermsAndConditions.Queries.GetPrivacyStatementRequests;
using OfficeEntry.Application.TermsAndConditions.Command.UpdatePrivacyStatementRequests;

namespace OfficeEntry.WebUI.Server.Controllers
{
    public class PrivacyStatementController : ApiController
    {
        [HttpGet]
        public async Task<bool> Get()
        {
            return await Mediator.Send(new GetPrivacyStatementForCurrentUserQuery());
        }

        [HttpPut]
        public async Task Put(bool isPrivateActStatementAccepted)
        {
            await Mediator.Send(new UpdatePrivacyActStatementForCurrentUserCommand { IsPrivacyActStatementAccepted = isPrivateActStatementAccepted });
        }
    }
}
