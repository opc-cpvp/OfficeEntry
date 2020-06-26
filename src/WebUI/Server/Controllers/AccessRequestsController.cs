using Microsoft.AspNetCore.Mvc;
using OfficeEntry.Application.AccessRequest.Commands.CreateAccessRequestRequests;
using OfficeEntry.Application.AccessRequest.Queries.GetAccessRequest;
using OfficeEntry.Application.AccessRequest.Queries.GetAccessRequests;
using OfficeEntry.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OfficeEntry.WebUI.Server.Controllers
{
    public class AccessRequestsController : ApiController
    {
        [HttpGet]
        public async Task<IEnumerable<AccessRequest>> Get()
        {
            return await Mediator.Send(new GetAccessRequestsQuery());
        }

        [HttpGet("{id}")]
        public async Task<AccessRequest> Get(Guid id)
        {
            return await Mediator.Send(new GetAccessRequestQuery { AccessRequestId = id });
        }

        [HttpPost]
        public async Task Post(AccessRequest accessRequest)
        {
            await Mediator.Send(new CreateAccessRequestForCurrentUserCommand { AccessRequest = accessRequest });
        }
    }
}
