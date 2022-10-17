using Fluxor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using OfficeEntry.WebApp.Store.DelegateAccessRequestsUseCase;

namespace OfficeEntry.WebApp.Pages;

[Authorize]
public partial class RequestsForOthers
{
    [Inject] public IState<DelegateAccessRequestsState> DelegateAccessRequestsState { get; set; }
    [Inject] public IDispatcher Dispatcher { get; set; }
    [Inject] public IStringLocalizer<App> Localizer { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (!DelegateAccessRequestsState.Value.AccessRequests.Any())
        {
            Dispatcher.Dispatch(new GetDelegateAccessRequestsAction());
        }
    }

    private void Refresh()
    {
        Dispatcher.Dispatch(new GetDelegateAccessRequestsAction());
    }
}
