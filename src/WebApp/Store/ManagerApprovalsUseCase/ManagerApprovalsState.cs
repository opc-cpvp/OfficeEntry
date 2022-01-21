using OfficeEntry.Domain.Entities;

namespace OfficeEntry.WebApp.Store.ManagerApprovalsUseCase;

public class ManagerApprovalsState
{
    public bool IsLoading { get; }

    public IReadOnlyList<AccessRequest> AccessRequests { get; }

    public int PendingApprovals { get; }

    public ManagerApprovalsState(bool isLoading, IReadOnlyList<AccessRequest> accessRequests)
    {
        IsLoading = isLoading;

        AccessRequests = accessRequests;

        PendingApprovals = accessRequests
            .Count(x => x.Status.Key == (int)AccessRequest.ApprovalStatus.Pending);
    }
}
