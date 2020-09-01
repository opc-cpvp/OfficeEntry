using Fluxor;
using OfficeEntry.Domain.Entities;
using System;

namespace OfficeEntry.WebApp.Store.ApprovalsUseCase
{
    public class Reducers
    {
        [ReducerMethod]
        public static ApprovalsState ReduceFetchDataAction(ApprovalsState state, FetchDataAction action) =>
            new ApprovalsState(isLoading: true, accessRequests: Array.Empty<AccessRequest>());

        [ReducerMethod]
        public static ApprovalsState ReduceFetchDataResultAction(ApprovalsState state, FetchDataResultAction action) =>
            new ApprovalsState(isLoading: false, accessRequests: action.AccessRequests);
    }
}
