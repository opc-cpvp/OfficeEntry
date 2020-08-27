using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using OfficeEntry.Application.TermsAndConditions.Queries.GetHealthAndSafetyMeasuresRequests;
using OfficeEntry.Application.TermsAndConditions.Queries.GetPrivacyStatementRequests;
using System;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.Extensions.Localization;
using System.Globalization;

namespace OfficeEntry.WebApp.Pages
{
    [Authorize]
    public partial class Index : ComponentBase
    {
        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] public IMediator Mediator { get; set; }
        [Inject] public ILocalStorageService LocalStorage { get; set; }
        [Inject] public IStringLocalizer<App> Localizer { get; set; } 

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
                return;

            var currentPageUrl = NavigationManager.Uri.Replace(NavigationManager.BaseUri, "");

            if (currentPageUrl == "entree-splash")
                return;

            var currentCulture = await LocalStorage.GetItemAsync<string>("BlazorCulture");

            if (currentCulture is null)
                return;

            var culture = currentCulture;
            var localizedLandingPage = await GetLocalizedLandingPage(culture);
            var uri = new Uri(new Uri(NavigationManager.BaseUri), localizedLandingPage)
                .GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);
            var query = $"?culture={Uri.EscapeDataString(culture)}&" +
                $"redirectUri={Uri.EscapeDataString(uri)}";

            NavigationManager.NavigateTo("/Culture/SetCulture" + query, forceLoad: true);

            await base.OnAfterRenderAsync(firstRender);
        }

        public async Task SetLanguageToFrench()
        {
            await LocalStorage.SetItemAsync("BlazorCulture", "fr-CA");

            NavigationManager.NavigateTo("/", forceLoad: true);
        }

        public async Task SetLanguageToEnglish()
        {
            await LocalStorage.SetItemAsync("BlazorCulture", "en-CA");

            NavigationManager.NavigateTo("/", forceLoad: true);
        }

        private async Task<bool> HasAcceptedTermsAndConditions()
        {
            var isPrivacyActStatementAccepted = await Mediator.Send(new GetPrivacyStatementForCurrentUserQuery());
            await LocalStorage.SetItemAsync("isPrivacyActStatementAccepted", isPrivacyActStatementAccepted);

            var isHealthAndSafetyMeasuresAccepted = await Mediator.Send(new GetHealthAndSafetyMeasuresForCurrentUserQuery());
            await LocalStorage.SetItemAsync("isHealthAndSafetyMeasuresAccepted", isHealthAndSafetyMeasuresAccepted);

            return (isHealthAndSafetyMeasuresAccepted && isPrivacyActStatementAccepted);
        }

        private async Task<string> GetLocalizedLandingPage(string culture)
        {
            var hasAcceptedTermsAndConditions = await HasAcceptedTermsAndConditions();

            CultureInfo.CurrentUICulture = new CultureInfo(culture);

            if (!hasAcceptedTermsAndConditions)
            {               
                return Localizer.GetString("terms-and-conditions");
            }
            return Localizer.GetString("access-requests");
        }
    }
}
