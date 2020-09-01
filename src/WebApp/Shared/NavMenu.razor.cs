﻿using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;

namespace OfficeEntry.WebApp.Shared
{
    public partial class NavMenu : ComponentBase
    {
        [Inject]
        IStringLocalizer<App> Localizer { get; set; }
        [Inject]
        ILocalStorageService LocalStorage { get; set; }

        private bool collapseNavMenu = true;

        private string NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        private bool isPrivacyActStatementAccepted;
        private bool isHealthAndSafetyMeasuresAccepted;
        private bool areAllTermsAndConditionsAccepted => isHealthAndSafetyMeasuresAccepted && isPrivacyActStatementAccepted;

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
