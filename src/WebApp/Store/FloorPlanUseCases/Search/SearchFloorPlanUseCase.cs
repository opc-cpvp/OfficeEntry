using Fluxor;
using MediatR;
using OfficeEntry.Application.Locations.Queries.GetFloorPlans;
using OfficeEntry.Domain.Entities;
using OfficeEntry.Domain.Services;
using System.Collections.Immutable;

namespace OfficeEntry.WebApp.Store.FloorPlanUseCases.Search;

[FeatureState]
public record SearchFloorPlansState(bool IsLoading, ImmutableArray<FloorPlan> FloorPlans)
{
	public static readonly SearchFloorPlansState Empty = new();

	private SearchFloorPlansState() :
		this(
			IsLoading: false,
			FloorPlans: ImmutableArray.Create<FloorPlan>())
	{
	}
}

public record SearchFloorPlansAction(string Keyword);

public record SearchFloorPlansActionResult(IEnumerable<FloorPlan> FloorPlans);

public static class SearchFloorPlansReducers
{
	/// <summary>
	/// Set `IsLoading` to true and clear the `FloorPlans` state when performing a search
	/// </summary>
	/// <param name="state"></param>
	/// <returns></returns>
	[ReducerMethod(typeof(SearchFloorPlansAction))]
	public static SearchFloorPlansState ReduceSearchFloorPlansAction(SearchFloorPlansState state) =>
		state with
		{
			IsLoading = true,
			FloorPlans = SearchFloorPlansState.Empty.FloorPlans
		};

	/// <summary>
	/// Set `IsLoading` to false and set the `FloorPlans` to the state returned from the server
	/// </summary>
	/// <param name="state"></param>
	/// <param name="action"></param>
	/// <returns></returns>
	[ReducerMethod]
	public static SearchFloorPlansState ReduceSearchFloorPlansActionResult(SearchFloorPlansState state, SearchFloorPlansActionResult action) =>
		state with
		{
			IsLoading = false,
			FloorPlans = action.FloorPlans.ToImmutableArray()
		};
}

public class Effects
{
	private readonly IMediator _mediator;

	public Effects(IMediator mediator)
	{
		_mediator = mediator;
	}

	[EffectMethod]
	public async Task HandleSearchFloorPlansAction(SearchFloorPlansAction action, IDispatcher dispatcher)
	{
		var floorPlans = await _mediator.Send(new GetFloorPlansQuery { Keyword = action.Keyword });
		dispatcher.Dispatch(new SearchFloorPlansActionResult(floorPlans));
	}
}