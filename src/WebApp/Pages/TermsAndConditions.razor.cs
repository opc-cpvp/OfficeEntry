using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;
using Blazored.LocalStorage;

namespace OfficeEntry.WebApp.Pages
{
    [Authorize]
    public partial class TermsAndConditions : ComponentBase
    {
        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] public IMediator Mediator { get; set; }
        [Inject] public IStringLocalizer<App> Localizer { get; set; }
        [Inject] public ILocalStorageService LocalStorage { get; set; }

        private bool? isHealthAndSafetyMeasuresAccepted = null;
        private bool? isPrivacyActStatementAccepted = null;

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
