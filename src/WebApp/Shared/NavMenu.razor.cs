using Blazored.LocalStorage;
using Fluxor;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using OfficeEntry.WebApp.Store.ManagerApprovalsUseCase;
using System.Threading.Tasks;

namespace OfficeEntry.WebApp.Shared
{
    public partial class NavMenu
    {
        [Inject]
        IStringLocalizer<App> Localizer { get; set; }

        [Inject]
        ILocalStorageService LocalStorage { get; set; }

        [Inject]
        public IMediator Mediator { get; set; }

        [Inject]
        private IState<ManagerApprovalsState> ApprovalsState { get; set; }

        [Inject]
        private IDispatcher Dispatcher { get; set; }

        private bool collapseNavMenu = true;

        private string NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        private bool isPrivacyActStatementAccepted;
        private bool isHealthAndSafetyMeasuresAccepted;

        private bool areAllTermsAndConditionsAccepted => isHealthAndSafetyMeasuresAccepted && isPrivacyActStatementAccepted;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Dispatcher.Dispatch(new GetManagerApprovalsAction());
        }

        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
                return;

            isHealthAndSafetyMeasuresAccepted = await LocalStorage.GetItemAsync<bool>("isHealthAndSafetyMeasuresAccepted");
            isPrivacyActStatementAccepted = await LocalStorage.GetItemAsync<bool>("isPrivacyActStatementAccepted");

            StateHasChanged();

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
