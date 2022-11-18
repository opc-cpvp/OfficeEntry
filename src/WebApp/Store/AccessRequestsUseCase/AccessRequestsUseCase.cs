using Fluxor;
using MediatR;
using OfficeEntry.Application.AccessRequests.Queries.GetAccessRequests;
using OfficeEntry.Domain.Entities;

namespace OfficeEntry.WebApp.Store.AccessRequestsUseCase;

public record AccessRequestsState
{
    public bool IsLoading { get; }

    public IReadOnlyList<AccessRequest> AccessRequests { get; }

    public AccessRequestsState(bool isLoading, IReadOnlyList<AccessRequest> accessRequests)
    {
        IsLoading = isLoading;

        AccessRequests = accessRequests;
    }
}

public class Feature : Feature<AccessRequestsState>
{
    public override string GetName() => "My Access Requests";

    protected override AccessRequestsState GetInitialState() =>
        new AccessRequestsState(isLoading: true, accessRequests: Array.Empty<AccessRequest>());
}

public record struct GetAccessRequestsAction
{
}

public record GetAccessRequestsResultAction
{
    public IReadOnlyList<AccessRequest> AccessRequests { get; }

    public GetAccessRequestsResultAction(IReadOnlyList<AccessRequest> accessRequests)
    {
        AccessRequests = accessRequests;
    }
}

public class Reducers
{
    [ReducerMethod]
    public static AccessRequestsState ReduceGetMyAccessRequestsAction(AccessRequestsState state, GetAccessRequestsAction action) =>
        new AccessRequestsState(isLoading: true, accessRequests: Array.Empty<AccessRequest>());

    [ReducerMethod]
    public static AccessRequestsState ReduceGetMyAccessRequestsResultAction(AccessRequestsState state, GetAccessRequestsResultAction action) =>
        new AccessRequestsState(isLoading: false, accessRequests: action.AccessRequests);
}

public class Effects
{
    private readonly IMediator _mediator;

    public Effects(IMediator mediator)
    {
        _mediator = mediator;
    }

    [EffectMethod]
    public async Task HandleFetchDataAction(GetAccessRequestsAction action, IDispatcher dispatcher)
    {
        var accessRequests = (await _mediator.Send(GetAccessRequestsQuery.Instance)).ToArray();
        dispatcher.Dispatch(new GetAccessRequestsResultAction(accessRequests));
    }
}
