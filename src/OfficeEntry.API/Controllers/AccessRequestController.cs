using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeEntry.Domain.Contracts;

namespace OfficeEntry.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccessRequestController : ControllerBase
    {
        private readonly IAccessRequestService _accessRequestService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccessRequestController(IAccessRequestService accessRequestService, IHttpContextAccessor httpContextAccessor)
        {
            _accessRequestService = accessRequestService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPatch]
        [Route("{accessRequestId}/approve")]
        public Task Approve(Guid accessRequestId)
        {
            return _accessRequestService.ApproveAccessRequestAsync(accessRequestId);
        }

        [HttpPatch]
        [Route("{accessRequestId}/cancel")]
        public Task Cancel(Guid accessRequestId)
        {
            return _accessRequestService.CancelAccessRequestAsync(accessRequestId);
        }

        [HttpPatch]
        [Route("{accessRequestId}/decline")]
        public Task Decline(Guid accessRequestId)
        {
            return _accessRequestService.DeclineAccessRequestAsync(accessRequestId);
        }
    }
}