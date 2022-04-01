using Fluxor;
using MediatR;
using OfficeEntry.Application.AccessRequests.Queries.GetAccessRequests;
using OfficeEntry.Domain.Entities;

namespace OfficeEntry.WebApp.Store.MyAccessRequestsUseCase;

public record MyAccessRequestsState
{
    public bool IsLoading { get; }

    public IReadOnlyList<AccessRequest> AccessRequests { get; }

    public MyAccessRequestsState(bool isLoading, IReadOnlyList<AccessRequest> accessRequests)
    {
        IsLoading = isLoading;

        AccessRequests = accessRequests;
    }
}

public class Feature : Feature<MyAccessRequestsState>
{
    public override string GetName() => "My Access Requests";

    protected override MyAccessRequestsState GetInitialState() =>
        new MyAccessRequestsState(isLoading: true, accessRequests: Array.Empty<AccessRequest>());
}

public record struct GetMyAccessRequestsAction
{
}

public record GetMyAccessRequestsResultAction
{
    public IReadOnlyList<AccessRequest> AccessRequests { get; }

    public GetMyAccessRequestsResultAction(IReadOnlyList<AccessRequest> accessRequests)
    {
        AccessRequests = accessRequests;
    }
}

public class Reducers
{
    [ReducerMethod]
    public static MyAccessRequestsState ReduceGetMyAccessRequestsAction(MyAccessRequestsState state, GetMyAccessRequestsAction action) =>
        new MyAccessRequestsState(isLoading: true, accessRequests: Array.Empty<AccessRequest>());

    [ReducerMethod]
    public static MyAccessRequestsState ReduceGetMyAccessRequestsResultAction(MyAccessRequestsState state, GetMyAccessRequestsResultAction action) =>
        new MyAccessRequestsState(isLoading: false, accessRequests: action.AccessRequests);
}

public class Effects
{
    private readonly IMediator _mediator;

    public Effects(IMediator mediator)
    {
        _mediator = mediator;
    }

    [EffectMethod]
    public async Task HandleFetchDataAction(GetMyAccessRequestsAction action, IDispatcher dispatcher)
    {
        var accessRequests = (await _mediator.Send(new GetAccessRequestsQuery())).ToArray();
        dispatcher.Dispatch(new GetMyAccessRequestsResultAction(accessRequests));
    }
}
