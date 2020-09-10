using Blazored.LocalStorage;
using Fluxor;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using OfficeEntry.WebApp.Store.ManagerApprovalsUseCase;
using OfficeEntry.WebApp.Store.MyTermsAndConditionsUseCase;
using System.Threading.Tasks;

namespace OfficeEntry.WebApp.Shared
{
    public partial class NavMenu
    {
        [Inject] public IStringLocalizer<App> Localizer { get; set; }
        [Inject] public IState<ManagerApprovalsState> ApprovalsState { get; set; }
        [Inject] public IState<MyTermsAndConditionsState> MyTermsAndConditionsState { get; set; }
        [Inject] public IDispatcher Dispatcher { get; set; }

        private bool collapseNavMenu = true;

        private string NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Dispatcher.Dispatch(new GetManagerApprovalsAction());
            Dispatcher.Dispatch(new GetMyTermsAndConditions());
        }

        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }
    }
}
