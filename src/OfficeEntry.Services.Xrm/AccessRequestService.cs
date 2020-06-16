using OfficeEntry.Domain.Contracts;
using OfficeEntry.Domain.Entities;
using OfficeEntry.Services.Xrm.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OfficeEntry.Services.Xrm
{
    public class AccessRequestService : XrmService, IAccessRequestService
    {
        public AccessRequestService(string odataUrl) :
            base(odataUrl)
        {
        }

        public async Task ApproveAccessRequestAsync(Guid accessRequestId)
        {
            var client = GetODataClient();
            await client.For<gc_accessrequest>()
                .Key(accessRequestId)
                .Set( new { gc_approvalstatus = ApprovalStatus.Approved })
                .UpdateEntryAsync();
        }

        public async Task CancelAccessRequestAsync(Guid accessRequestId)
        {
            var client = GetODataClient();
            await client.For<gc_accessrequest>()
                .Key(accessRequestId)
                .Set(new { gc_approvalstatus = ApprovalStatus.Cancelled })
                .UpdateEntryAsync();
        }

        public async Task DeclineAccessRequestAsync(Guid accessRequestId)
        {
            var client = GetODataClient();
            await client.For<gc_accessrequest>()
                .Key(accessRequestId)
                .Set(new { gc_approvalstatus = ApprovalStatus.Declined })
                .UpdateEntryAsync();
        }

        public async Task<IEnumerable<AccessRequest>> GetAccessRequestsAsync(string username)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<AccessRequest>> GetPendingAccessRequestsAsync(string username)
        {
            throw new NotImplementedException();
        }

        public async Task SubmitAccessRequestAsync(AccessRequest accessRequest)
        {
            throw new NotImplementedException();
        }
    }
}