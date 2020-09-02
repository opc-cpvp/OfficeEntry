using Fluxor;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using OfficeEntry.Domain.Enums;
using OfficeEntry.WebApp.Store.ManagerApprovalsUseCase;
using System.Globalization;
using System.Threading.Tasks;

namespace OfficeEntry.WebApp.Pages
{
    [Authorize]
    public partial class ReviewAccessRequests
    {
        [Inject]
        private IState<ManagerApprovalsState> ApprovalsState { get; set; }

        [Inject] public IStringLocalizer<App> Localizer { get; set; }
        [Inject] public IMediator Mediator { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            ApprovalsState.StateChanged += ApprovalsState_StateChanged;
        }

        private void ApprovalsState_StateChanged(object sender, ManagerApprovalsState e)
        {
            var locale = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            locale = (locale == Locale.French) ? locale : Locale.English;

            foreach (var accessRequest in e.AccessRequests)
            {
                accessRequest.Building.Name = (locale == Locale.French) ? accessRequest.Building.FrenchName : accessRequest.Building.EnglishName;
                accessRequest.Floor.Name = (locale == Locale.French) ? accessRequest.Floor.FrenchName : accessRequest.Floor.EnglishName;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
                return;

            var locale = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            locale = (locale == Locale.French) ? locale : Locale.English;

            await JSRuntime.InvokeAsync<object>("initializeDatatables", locale);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ApprovalsState.StateChanged -= ApprovalsState_StateChanged;
            JSRuntime.InvokeAsync<object>("destroyDatatables");
        }
    }
}
