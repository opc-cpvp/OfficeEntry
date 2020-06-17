using OfficeEntry.Application.AccessRequest.Queries.GetAccessRequests;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace OfficeEntry.WebUI.Server.Controllers
{
    public class AccessRequestsController : ApiController
    {
        [HttpGet]
        public async Task<IEnumerable<AccessRequestDto>> Get(CancellationToken cancellationToken)
        {
            //return await Mediator.Send(new GetAccessRequestsQuery());

            var handler = new GetAccessRequestsQueryHandler();
            return await handler.Handle(new GetAccessRequestsQuery(), cancellationToken);
        }

        //[HttpPatch]
        //[Route("{accessRequestId}/approve")]
        //public Task Approve(Guid accessRequestId)
        //{
        //    return _accessRequestService.ApproveAccessRequestAsync(accessRequestId);
        //}

        //[HttpPatch]
        //[Route("{accessRequestId}/cancel")]
        //public Task Cancel(Guid accessRequestId)
        //{
        //    return _accessRequestService.CancelAccessRequestAsync(accessRequestId);
        //}

        //[HttpPatch]
        //[Route("{accessRequestId}/decline")]
        //public Task Decline(Guid accessRequestId)
        //{
        //    return _accessRequestService.DeclineAccessRequestAsync(accessRequestId);
        //}
    }
}
