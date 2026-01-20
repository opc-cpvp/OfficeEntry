using Fluxor;
using MediatR;
using OfficeEntry.Application.Locations.Commands.UpdateFloorPlan;
using OfficeEntry.Application.Locations.Queries.GetFloorPlan;
using OfficeEntry.Domain.Entities;
using OfficeEntry.Domain.Services;

namespace OfficeEntry.WebApp.Store.FloorPlanUseCases.Edit;

[FeatureState]
public record FloorPlanState(bool IsLoading, bool IsSaving)
{
    public static readonly FloorPlanState Empty = new();

    private FloorPlanState()
        : this(IsLoading: false, IsSaving: false)
    {
    }
}

public record GetFloorPlanAction(Guid FloorPlanId);

public record GetFloorPlanResultAction(FloorPlan Dto);

public record UpdateFloorPlanAction(FloorPlan Dto);

public record UpdateFloorPlanResultAction;

public class Reducers
{
    [ReducerMethod]
    public static FloorPlanState ReduceGetFloorPlanAction(FloorPlanState state, GetFloorPlanAction action) =>
        state with { IsLoading = true };

    [ReducerMethod]
    public static FloorPlanState ReduceGetFloorPlanResultAction(FloorPlanState state, GetFloorPlanResultAction action) =>
        state with { IsLoading = false };

    [ReducerMethod(typeof(UpdateFloorPlanAction))]
    public static FloorPlanState ReduceUpdateWorkspaceAction(FloorPlanState state) =>
        state with { IsSaving = true };

    [ReducerMethod(typeof(UpdateFloorPlanResultAction))]
    public static FloorPlanState ReduceUpdateFloorPlanResultAction(FloorPlanState state) =>
        state with { IsSaving = false };
}

public class Effects
{
    private readonly IMediator _mediator;

    public Effects(IMediator mediator)
    {
        _mediator = mediator;
    }

    [EffectMethod]
    public async Task HandleFetchDataAction(GetFloorPlanAction action, IDispatcher dispatcher)
    {
        var floorPlan = await _mediator.Send(new GetFloorPlanQuery {  FloorPlanId = action.FloorPlanId });
        dispatcher.Dispatch(new GetFloorPlanResultAction(floorPlan));
    }

    [EffectMethod]
    public async Task HandleUpdateFloorPlanActionAsync(UpdateFloorPlanAction action, IDispatcher dispatcher)
    {
        await _mediator.Send(new UpdateFloorPlanCommand { FloorPlan = action.Dto });
        dispatcher.Dispatch(new UpdateFloorPlanResultAction());
    }
}
