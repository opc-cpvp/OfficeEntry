using OfficeEntry.Domain.Entities;
using System.Collections.Generic;

namespace OfficeEntry.WebApp.Store.ManagerApprovalsUseCase
{
    public class GetManagerApprovalsResultAction
    {
        public IReadOnlyList<AccessRequest> AccessRequests { get; }

        public GetManagerApprovalsResultAction(IReadOnlyList<AccessRequest> accessRequests)
        {
            AccessRequests = accessRequests;
        }
    }
}
