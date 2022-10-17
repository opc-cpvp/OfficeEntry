using Fluxor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using OfficeEntry.WebApp.Store.AccessRequestsUseCase;
using OfficeEntry.WebApp.Store.TermsAndConditionsUseCase;

namespace OfficeEntry.WebApp.Pages;

[Authorize]
public partial class MyRequests
{
    [Inject] public IState<AccessRequestsState> AccessRequestsState { get; set; }
    [Inject] public IState<TermsAndConditionsState> TermsAndConditionsState { get; set; }
    [Inject] public IDispatcher Dispatcher { get; set; }
    [Inject] public IStringLocalizer<App> Localizer { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (!AccessRequestsState.Value.AccessRequests.Any())
        {
            Dispatcher.Dispatch(new GetAccessRequestsAction());
        }
    }

    private void Refresh()
    {
        Dispatcher.Dispatch(new GetAccessRequestsAction());
    }
}
