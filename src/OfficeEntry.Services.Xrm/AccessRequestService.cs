using Microsoft.Extensions.Configuration;
using OfficeEntry.Domain.Contracts;
using OfficeEntry.Domain.Entities;
using System;
using System.Collections.Generic;

namespace OfficeEntry.Services.Xrm
{
    public class AccessRequestService : XrmService, IAccessRequestService
    {
        public AccessRequestService(IConfiguration configuration) :
            base(configuration)
        {
        }

        public void ApproveAccessRequest(Guid accessRequestId)
        {
            throw new NotImplementedException();
        }

        public void CancelAccessRequest(Guid accessRequestId)
        {
            throw new NotImplementedException();
        }

        public void DenyAccessRequest(Guid accessRequestId)
        {
            throw new NotImplementedException();
        }

        public List<AccessRequest> GetAccessRequests()
        {
            throw new NotImplementedException();
        }

        public List<AccessRequest> GetPendingAccessRequests()
        {
            throw new NotImplementedException();
        }

        public void SubmitAccessRequest(AccessRequest accessRequest)
        {
            throw new NotImplementedException();
        }
    }
}
