using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using OfficeEntry.Domain.Enums;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Fluxor;
using OfficeEntry.WebApp.Store.MyAccessRequestsUseCase;

namespace OfficeEntry.WebApp.Pages
{
    [Authorize]
    public partial class MyAccessRequests
    {
        [Inject] private IState<MyAccessRequestsState> MyAccessRequestsState { get; set; }
        [Inject] private IDispatcher Dispatcher { get; set; }
        [Inject] public IStringLocalizer<App> Localizer { get; set; }
        [Inject] public IMediator Mediator { get; set; }
        [Inject] public IJSRuntime JSRuntime { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            MyAccessRequestsState.StateChanged += MyAccessRequestsState_StateChanged;

            if (!MyAccessRequestsState.Value.AccessRequests.Any())
            {
                Dispatcher.Dispatch(new GetMyAccessRequestsAction());
            }
        }

        private void MyAccessRequestsState_StateChanged(object sender, MyAccessRequestsState e)
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
            await base.OnAfterRenderAsync(firstRender);

            if (!firstRender)
            {
                return;
            }

            var locale = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            locale = (locale == Locale.French) ? locale : Locale.English;            

            await JSRuntime.InvokeAsync<object>("initializeDatatables", locale);          
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            MyAccessRequestsState.StateChanged -= MyAccessRequestsState_StateChanged;
            JSRuntime.InvokeAsync<object>("destroyDatatables");
        }
    }
}
