using Fluxor;
using MediatR;
using OfficeEntry.Application.AccessRequests.Queries.GetAccessRequests;
using OfficeEntry.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

    public class Feature : Feature<MyAccessRequestsState>
    {
        public override string GetName() => "My Access Requests";

        protected override MyAccessRequestsState GetInitialState() =>
            new MyAccessRequestsState(isLoading: true, accessRequests: Array.Empty<AccessRequest>());
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
            var accessRequests = (await _mediator.Send(new GetAccessRequestsQuery())).ToArray();
            dispatcher.Dispatch(new FetchDataResultAction(accessRequests));
        }
    }

    public class Reducers
    {
        [ReducerMethod]
        public static MyAccessRequestsState ReduceFetchDataAction(MyAccessRequestsState state, FetchDataAction action) =>
            new MyAccessRequestsState(isLoading: true, accessRequests: Array.Empty<AccessRequest>());

        [ReducerMethod]
        public static MyAccessRequestsState ReduceFetchDataResultAction(MyAccessRequestsState state, FetchDataResultAction action) =>
            new MyAccessRequestsState(isLoading: false, accessRequests: action.AccessRequests);
    }
}
