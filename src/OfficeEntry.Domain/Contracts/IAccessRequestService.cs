using OfficeEntry.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OfficeEntry.Domain.Contracts
{
    public interface IAccessRequestService
    {
        public Task ApproveAccessRequestAsync(Guid accessRequestId);
        public Task CancelAccessRequestAsync(Guid accessRequestId);
        public Task DeclineAccessRequestAsync(Guid accessRequestId);
        public Task<IEnumerable<AccessRequest>> GetAccessRequestsAsync(string username);
        public Task<IEnumerable<AccessRequest>> GetPendingAccessRequestsAsync(string username);
        public Task SubmitAccessRequestAsync(AccessRequest accessRequest);
    }
}
