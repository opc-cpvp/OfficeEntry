using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OfficeEntry.Domain.Contracts;

namespace OfficeEntry.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccessRequestController : ControllerBase
    {
        private readonly IAccessRequestService _accessRequestService;

        public AccessRequestController(IAccessRequestService accessRequestService)
        {
            _accessRequestService = accessRequestService;
        }

        [HttpPatch]
        [Route("{accessRequestId}/approve")]
        public Task Approve(Guid accessRequestId)
        {
             return _accessRequestService.ApproveAccessRequestAsync(accessRequestId);
        }
    }
}