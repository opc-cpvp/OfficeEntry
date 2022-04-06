using Fluxor;
using MediatR;
using OfficeEntry.Application.AccessRequests.Queries.GetAccessRequests;
using OfficeEntry.Domain.Entities;

namespace OfficeEntry.WebApp.Store.ManagerApprovalsUseCase;

public record ManagerApprovalsState
{
    public bool IsLoading { get; }

    public IReadOnlyList<AccessRequest> AccessRequests { get; }

    public int PendingApprovals { get; }

    public ManagerApprovalsState(bool isLoading, IReadOnlyList<AccessRequest> accessRequests)
    {
        IsLoading = isLoading;

        AccessRequests = accessRequests;

        PendingApprovals = accessRequests
            .Count(x => x.Status.Key == (int)AccessRequest.ApprovalStatus.Pending);
    }
}

public class Feature : Feature<ManagerApprovalsState>
{
    public override string GetName() => "Manager Approvals";

    protected override ManagerApprovalsState GetInitialState() =>
        new ManagerApprovalsState(isLoading: true, accessRequests: Array.Empty<AccessRequest>());
}

public record struct GetManagerApprovalsAction
{
}

public record GetManagerApprovalsResultAction
{
    public IReadOnlyList<AccessRequest> AccessRequests { get; }

    public GetManagerApprovalsResultAction(IReadOnlyList<AccessRequest> accessRequests)
    {
        AccessRequests = accessRequests;
    }
}

public class Reducers
{
    [ReducerMethod]
    public static ManagerApprovalsState ReduceGetManagerApprovalsAction(ManagerApprovalsState state, GetManagerApprovalsAction action) =>
        new ManagerApprovalsState(isLoading: true, accessRequests: Array.Empty<AccessRequest>());

    [ReducerMethod]
    public static ManagerApprovalsState ReduceGetManagerApprovalsResultAction(ManagerApprovalsState state, GetManagerApprovalsResultAction action) =>
        new ManagerApprovalsState(isLoading: false, accessRequests: action.AccessRequests);
}

public class Effects
{
    private readonly IMediator _mediator;

    public Effects(IMediator mediator)
    {
        _mediator = mediator;
    }

    [EffectMethod]
    public async Task HandleFetchDataAction(GetManagerApprovalsAction action, IDispatcher dispatcher)
    {
        var accessRequests = (await _mediator.Send(new GetManagerAccessRequestsQuery())).ToArray();
        dispatcher.Dispatch(new GetManagerApprovalsResultAction(accessRequests));
    }
}
