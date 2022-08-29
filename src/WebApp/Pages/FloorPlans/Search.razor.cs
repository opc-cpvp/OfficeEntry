using Fluxor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using OfficeEntry.WebApp.Store.FloorPlanUseCases.Search;

namespace OfficeEntry.WebApp.Pages.FloorPlans;

[Authorize(Policy = "EditUser")]
public partial class Search
{
	[Inject] private IDispatcher Dispatcher { get; set; } = null!;
	[Inject] private IState<SearchFloorPlansState> State { get; set; } = null!;

	protected override void OnInitialized()
	{
		base.OnInitialized();

		if (!State.Value.FloorPlans.Any())
		{
			Refresh();
		}
	}

	protected void Refresh()
	{
		Dispatcher.Dispatch(new SearchFloorPlansAction(string.Empty));
	}
}
