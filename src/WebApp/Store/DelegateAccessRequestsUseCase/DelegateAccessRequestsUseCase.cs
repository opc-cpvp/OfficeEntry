using Fluxor;
using MediatR;
using OfficeEntry.Application.AccessRequests.Queries.GetDelegateAccessRequests;
using OfficeEntry.Domain.Entities;

namespace OfficeEntry.WebApp.Store.DelegateAccessRequestsUseCase;

public record DelegateAccessRequestsState
{
    public bool IsLoading { get; }

    public IReadOnlyList<AccessRequest> AccessRequests { get; }

    public DelegateAccessRequestsState(bool isLoading, IReadOnlyList<AccessRequest> accessRequests)
    {
        IsLoading = isLoading;
        AccessRequests = accessRequests;
    }
}

public class Feature : Feature<DelegateAccessRequestsState>
{
    public override string GetName() => "Delegate Access Requests";

    protected override DelegateAccessRequestsState GetInitialState() =>
        new DelegateAccessRequestsState(isLoading: true, accessRequests: Array.Empty<AccessRequest>());
}

public record struct GetDelegateAccessRequestsAction
{
}

public record GetDelegateAccessRequestsResultAction
{
    public IReadOnlyList<AccessRequest> AccessRequests { get; }

    public GetDelegateAccessRequestsResultAction(IReadOnlyList<AccessRequest> accessRequests)
    {
        AccessRequests = accessRequests;
    }
}

public class Reducers
{
    [ReducerMethod]
    public static DelegateAccessRequestsState ReduceGetDelegateAccessRequestsAction(DelegateAccessRequestsState state, GetDelegateAccessRequestsAction action) =>
        new DelegateAccessRequestsState(isLoading: true, accessRequests: Array.Empty<AccessRequest>());

    [ReducerMethod]
    public static DelegateAccessRequestsState ReduceGetDelegateAccessRequestsResultAction(DelegateAccessRequestsState state, GetDelegateAccessRequestsResultAction action) =>
        new DelegateAccessRequestsState(isLoading: false, accessRequests: action.AccessRequests);
}

public class Effects
{
    private readonly IMediator _mediator;

    public Effects(IMediator mediator)
    {
        _mediator = mediator;
    }

    [EffectMethod]
    public async Task HandleFetchDataAction(GetDelegateAccessRequestsAction action, IDispatcher dispatcher)
    {
        var accessRequests = (await _mediator.Send(new GetDelegateAccessRequestsQuery())).ToArray();
        dispatcher.Dispatch(new GetDelegateAccessRequestsResultAction(accessRequests));
    }
}
