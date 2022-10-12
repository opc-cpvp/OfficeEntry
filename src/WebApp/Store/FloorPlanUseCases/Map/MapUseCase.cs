using Fluxor;
using MediatR;
using OfficeEntry.Application.AccessRequests.Queries.GetAccessRequestPerFloorPlan;
using OfficeEntry.Application.Locations.Queries.GetFloorPlan;
using OfficeEntry.Domain.Entities;
using System.Collections.Immutable;

namespace OfficeEntry.WebApp.Store.FloorPlanUseCases.Map;

[FeatureState]
public record MapState(bool IsLoading, ImmutableArray<AccessRequest> AccessRequests)
{
    public static readonly MapState Empty = new();

    private MapState() :
        this(
            IsLoading: false,
            AccessRequests: ImmutableArray.Create<AccessRequest>())
    { }
}

public record GetMapAction(Guid FloorPlanId, DateOnly Date);

public record GetMapResultAction(FloorPlan Dto, ImmutableArray<AccessRequest> AccessRequests);

public class Reducers
{
    [ReducerMethod]
    public static MapState ReduceGetFloorPlanAction(MapState state, GetMapAction action) =>
        state with { IsLoading = true };

    [ReducerMethod]
    public static MapState ReduceGetFloorPlanResultAction(MapState state, GetMapResultAction action) =>
        state with { IsLoading = false };
}

public class Effects
{
    private readonly IMediator _mediator;

    public Effects(IMediator mediator)
    {
        _mediator = mediator;
    }

    [EffectMethod]
    public async Task HandleFetchDataAction(GetMapAction action, IDispatcher dispatcher)
    {
        var floorPlan = await _mediator.Send(new GetFloorPlanQuery { FloorPlanId = action.FloorPlanId });

        var accessRequests = await _mediator.Send(new GetAccessRequestPerFloorPlanQuery { FloorPlanId = action.FloorPlanId, Date = action.Date });

        dispatcher.Dispatch(new GetMapResultAction(floorPlan, accessRequests));
    }
}