using OfficeEntry.Application.AccessRequest.Queries.GetAccessRequests;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;
using OfficeEntry.Domain.Entities;
using OfficeEntry.Application.AccessRequest.Commands.CreateAccessRequestRequests;

namespace OfficeEntry.WebUI.Server.Controllers
{
    public class AccessRequestsController : ApiController
    {
        [HttpGet]
        public async Task<IEnumerable<AccessRequest>> Get()
        {
            return await Mediator.Send(new GetAccessRequestsQuery());
        }

        [HttpPost]
        public async Task Post(AccessRequest accessRequest)
        {
            await Mediator.Send(new CreateAccessRequestForCurrentUserCommand { AccessRequest = accessRequest });
        }
    }
}
