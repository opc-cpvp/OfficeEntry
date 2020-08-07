using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OfficeEntry.WebApp.Pages
{
    [Authorize]
    public partial class Index : ComponentBase
    {
        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
                return;

            var currentPageUrl = NavigationManager.Uri.Replace(NavigationManager.BaseUri, "");

            if (currentPageUrl == "entree-splash")
                return;

            var currentCulture = await JSRuntime.InvokeAsync<string>("blazorCulture.get");

            if (currentCulture is null)
                return;

            var culture = currentCulture;
            var localizedLandingPage = culture == "fr-CA" ? "demandes-d-acces" : "access-requests";
            var uri = new Uri(new Uri(NavigationManager.BaseUri), localizedLandingPage)
                .GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);
            var query = $"?culture={Uri.EscapeDataString(culture)}&" +
                $"redirectUri={Uri.EscapeDataString(uri)}";

            NavigationManager.NavigateTo("/Culture/SetCulture" + query, forceLoad: true);

            await base.OnAfterRenderAsync(firstRender);
        }

        public async Task SetLanguageToFrench()
        {
            await JSRuntime.InvokeVoidAsync("blazorCulture.set", "fr-CA");

            NavigationManager.NavigateTo("/", forceLoad: true);
        }

        public async Task SetLanguageToEnglish()
        {
            await JSRuntime.InvokeVoidAsync("blazorCulture.set", "en-CA");

            NavigationManager.NavigateTo("/", forceLoad: true);
        }
    }
}
