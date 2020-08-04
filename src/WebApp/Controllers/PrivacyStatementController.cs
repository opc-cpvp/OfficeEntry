using Microsoft.AspNetCore.Mvc;
using OfficeEntry.Application.TermsAndConditions.Commands.UpdatePrivacyStatementRequests;
using OfficeEntry.Application.TermsAndConditions.Queries.GetPrivacyStatementRequests;
using System.Threading.Tasks;

namespace OfficeEntry.WebApp.Controllers
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