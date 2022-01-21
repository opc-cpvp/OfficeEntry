using OfficeEntry.Domain.Entities;

namespace OfficeEntry.WebApp.Store.MyAccessRequestsUseCase;

public class GetMyAccessRequestsResultAction
{
    public IReadOnlyList<AccessRequest> AccessRequests { get; }

    public GetMyAccessRequestsResultAction(IReadOnlyList<AccessRequest> accessRequests)
    {
        AccessRequests = accessRequests;
    }
}
