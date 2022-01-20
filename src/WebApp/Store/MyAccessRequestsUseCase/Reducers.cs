using Fluxor;
using OfficeEntry.Domain.Entities;

namespace OfficeEntry.WebApp.Store.MyAccessRequestsUseCase
{
    public class Reducers
    {
        [ReducerMethod]
        public static MyAccessRequestsState ReduceGetMyAccessRequestsAction(MyAccessRequestsState state, GetMyAccessRequestsAction action) =>
            new MyAccessRequestsState(isLoading: true, accessRequests: Array.Empty<AccessRequest>());

        [ReducerMethod]
        public static MyAccessRequestsState ReduceGetMyAccessRequestsResultAction(MyAccessRequestsState state, GetMyAccessRequestsResultAction action) =>
            new MyAccessRequestsState(isLoading: false, accessRequests: action.AccessRequests);
    }
}
