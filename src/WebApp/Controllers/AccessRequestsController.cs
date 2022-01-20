using Microsoft.AspNetCore.Mvc;
using OfficeEntry.Application.AccessRequests.Commands.CreateAccessRequestRequests;
using OfficeEntry.Application.AccessRequests.Commands.UpdateAccessRequestRequests;
using OfficeEntry.Application.AccessRequests.Queries.GetAccessRequest;
using OfficeEntry.Application.AccessRequests.Queries.GetAccessRequests;
using OfficeEntry.Domain.Entities;
using System.Globalization;

namespace OfficeEntry.WebApp.Controllers
{
    public class AccessRequestsController : ApiController
    {
        [HttpGet]
        public async Task<IEnumerable<AccessRequest>> Get()
        {
            return await Mediator.Send(new GetAccessRequestsQuery());
        }

        [HttpGet("{id}")]
        public async Task<AccessRequestViewModel> Get(Guid id)
        {
            var locale = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            return await Mediator.Send(new GetAccessRequestQuery { AccessRequestId = id, Locale = locale });
        }

        [HttpPost("create")]
        public async Task Create(AccessRequest accessRequest)
        {
            await Mediator.Send(new CreateAccessRequestForCurrentUserCommand { AccessRequest = accessRequest });
        }

        [HttpPost("update")]
        public async Task Update(AccessRequest accessRequest)
        {
            await Mediator.Send(new UpdateAccessRequestCommand { AccessRequest = accessRequest });
        }
    }
}