using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using OfficeEntry.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace OfficeEntry.WebApp.Shared
{
    public partial class AccessRequestDatatable : IDisposable
    {
        [Parameter]
        public IEnumerable<Domain.Entities.AccessRequest> AccessRequests { get; set; }

        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] public IStringLocalizer<App> Localizer { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
                return;

            var locale = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            locale = (locale == Locale.French) ? locale : Locale.English;

            foreach (var accessRequest in AccessRequests)
            {
                accessRequest.Building.Name = (locale == Locale.French) ? accessRequest.Building.FrenchName : accessRequest.Building.EnglishName;
                accessRequest.Floor.Name = (locale == Locale.French) ? accessRequest.Floor.FrenchName : accessRequest.Floor.EnglishName;
            }

            StateHasChanged();

            await JSRuntime.InvokeVoidAsync("interop.datatables.init", locale);
        }

        public void Dispose()
        {
            JSRuntime.InvokeVoidAsync("interop.datatables.destroy");
        }
    }
}
