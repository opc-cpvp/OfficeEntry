using OfficeEntry.Domain.Entities;
using System;
using System.Collections.Generic;

namespace OfficeEntry.Domain.Contracts
{
    public interface IAccessRequestService
    {
        public void ApproveAccessRequest(Guid accessRequestId);
        public void CancelAccessRequest(Guid accessRequestId);
        public void DenyAccessRequest(Guid accessRequestId);
        public List<AccessRequest> GetAccessRequests();
        public List<AccessRequest> GetPendingAccessRequests();
        public void SubmitAccessRequest(AccessRequest accessRequest);
    }
}
