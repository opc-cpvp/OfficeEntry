using Microsoft.AspNetCore.Mvc;
using OfficeEntry.Application.AccessRequests.Queries.GetAccessRequests;
using OfficeEntry.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OfficeEntry.WebUI.Server.Controllers
{
    public class ReviewAccessRequestsController : ApiController
    {
        [HttpGet]
        public async Task<IEnumerable<AccessRequest>> Get()
        {
            return await Mediator.Send(new GetManagerAccessRequestsQuery());
        }
    }
}
