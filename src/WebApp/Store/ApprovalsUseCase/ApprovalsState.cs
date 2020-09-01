using OfficeEntry.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace OfficeEntry.WebApp.Store.ApprovalsUseCase
{
    public class ApprovalsState
    {
        public bool IsLoading { get; }

        public IReadOnlyList<AccessRequest> AccessRequests { get; }

        public int PendingApprovals { get; }

        public ApprovalsState(bool isLoading, IReadOnlyList<AccessRequest> accessRequests)
        {
            IsLoading = isLoading;

            AccessRequests = accessRequests;

            PendingApprovals = accessRequests
                .Count(x => x.Status.Key == (int)AccessRequest.ApprovalStatus.Pending);
        }
    }
}
