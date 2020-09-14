using OfficeEntry.Domain.Entities;
using System.Collections.Generic;

namespace OfficeEntry.WebApp.Store.MyAccessRequestsUseCase
{
    public class MyAccessRequestsState
    {
        public bool IsLoading { get; }

        public IReadOnlyList<AccessRequest> AccessRequests { get; }

        public MyAccessRequestsState(bool isLoading, IReadOnlyList<AccessRequest> accessRequests)
        {
            IsLoading = isLoading;

            AccessRequests = accessRequests;
        }
    }
}
