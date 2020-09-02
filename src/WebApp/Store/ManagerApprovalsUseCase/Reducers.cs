using Fluxor;
using OfficeEntry.Domain.Entities;
using System;

namespace OfficeEntry.WebApp.Store.ManagerApprovalsUseCase
{
    public class Reducers
    {
        [ReducerMethod]
        public static ManagerApprovalsState ReduceGetManagerApprovalsAction(ManagerApprovalsState state, GetManagerApprovalsAction action) =>
            new ManagerApprovalsState(isLoading: true, accessRequests: Array.Empty<AccessRequest>());

        [ReducerMethod]
        public static ManagerApprovalsState ReduceGetManagerApprovalsResultAction(ManagerApprovalsState state, GetManagerApprovalsResultAction action) =>
            new ManagerApprovalsState(isLoading: false, accessRequests: action.AccessRequests);
    }
}
