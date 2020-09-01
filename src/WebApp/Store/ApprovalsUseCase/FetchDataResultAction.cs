using OfficeEntry.Domain.Entities;
using System.Collections.Generic;

namespace OfficeEntry.WebApp.Store.ApprovalsUseCase
{
    public class FetchDataResultAction
    {
        public IReadOnlyList<AccessRequest> AccessRequests { get; }

        public FetchDataResultAction(IReadOnlyList<AccessRequest> accessRequests)
        {
            AccessRequests = accessRequests;
        }
    }
}
