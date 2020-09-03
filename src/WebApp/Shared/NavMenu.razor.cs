using Blazored.LocalStorage;
using Fluxor;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using OfficeEntry.WebApp.Store.ManagerApprovalsUseCase;
using OfficeEntry.WebApp.Store.TermsAndConditionsUseCase;
using System.Threading.Tasks;

namespace OfficeEntry.WebApp.Shared
{
    public partial class NavMenu
    {
        [Inject]
        IStringLocalizer<App> Localizer { get; set; }


        [Inject]
        public IMediator Mediator { get; set; }

        [Inject]
        private IState<ManagerApprovalsState> ApprovalsState { get; set; }

        [Inject]
        private IState<TermsAndConditionsState> TermsAndConditionsState { get; set; }

        [Inject]
        private IDispatcher Dispatcher { get; set; }

        private bool collapseNavMenu = true;

        private string NavMenuCssClass => collapseNavMenu ? "collapse" : null;


        protected override void OnInitialized()
        {
            base.OnInitialized();
            Dispatcher.Dispatch(new GetManagerApprovalsAction());
            Dispatcher.Dispatch(new GetTermsAndConditionsAction());
        }

        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
                return;

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
