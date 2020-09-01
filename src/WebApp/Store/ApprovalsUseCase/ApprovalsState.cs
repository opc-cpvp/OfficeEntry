using Fluxor;
using MediatR;
using OfficeEntry.Application.AccessRequests.Queries.GetAccessRequests;
using OfficeEntry.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

    public class Feature : Feature<ApprovalsState>
    {
        public override string GetName() => "Approvals";

        protected override ApprovalsState GetInitialState() =>
            new ApprovalsState(isLoading: true, accessRequests: Array.Empty<AccessRequest>());
    }

    public class Reducers
    {
        [ReducerMethod]
        public static ApprovalsState ReduceFetchDataAction(ApprovalsState state, FetchDataAction action) =>
            new ApprovalsState(isLoading: true, accessRequests: Array.Empty<AccessRequest>());

        [ReducerMethod]
        public static ApprovalsState ReduceFetchDataResultAction(ApprovalsState state, FetchDataResultAction action) =>
            new ApprovalsState(isLoading: false, accessRequests: action.AccessRequests);
    }

    public class FetchDataAction
    {
    }

    public class FetchDataResultAction
    {
        public IReadOnlyList<AccessRequest> AccessRequests { get; }

        public FetchDataResultAction(IReadOnlyList<AccessRequest> accessRequests)
        {
            AccessRequests = accessRequests;
        }
    }

    public class Effects
    {
        private readonly IMediator _mediator;

        public Effects(IMediator mediator)
        {
            _mediator = mediator;
        }

        [EffectMethod]
        public async Task HandleFetchDataAction(FetchDataAction action, IDispatcher dispatcher)
        {
            var accessRequests = (await _mediator.Send(new GetManagerAccessRequestsQuery())).ToArray();
            dispatcher.Dispatch(new FetchDataResultAction(accessRequests));
        }
    }
}
