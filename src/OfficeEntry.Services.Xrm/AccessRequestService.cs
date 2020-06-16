using Microsoft.Extensions.Configuration;
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

        public void CancelAccessRequest(Guid accessRequestId)
        {
            throw new NotImplementedException();
        }

        public void DenyAccessRequest(Guid accessRequestId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<AccessRequest>> GetAccessRequestsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<AccessRequest>> GetPendingAccessRequestsAsync()
        {
            throw new NotImplementedException();
        }

        public void SubmitAccessRequest(AccessRequest accessRequest)
        {
            throw new NotImplementedException();
        }
    }
}
