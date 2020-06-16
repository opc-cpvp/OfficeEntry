using OfficeEntry.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OfficeEntry.Domain.Contracts
{
    public interface IAccessRequestService
    {
        public void ApproveAccessRequest(Guid accessRequestId);

        public void CancelAccessRequest(Guid accessRequestId);

        public void DenyAccessRequest(Guid accessRequestId);

        public Task<IEnumerable<AccessRequest>> GetAccessRequestsAsync();

        public Task<IEnumerable<AccessRequest>> GetPendingAccessRequestsAsync();

        public void SubmitAccessRequest(AccessRequest accessRequest);
    }
}