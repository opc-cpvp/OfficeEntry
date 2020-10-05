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
using OfficeEntry.WebApp.Store.MyTermsAndConditionsUseCase;

namespace OfficeEntry.WebApp.Pages
{
    [Authorize]
    public partial class MyAccessRequests
    {
        [Inject] public IState<MyAccessRequestsState> MyAccessRequestsState { get; set; }
        [Inject] public IState<MyTermsAndConditionsState> MyTermsAndConditionsState { get; set; }
        [Inject] public IDispatcher Dispatcher { get; set; }
        [Inject] public IStringLocalizer<App> Localizer { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (!MyAccessRequestsState.Value.AccessRequests.Any())
            {
                Dispatcher.Dispatch(new GetMyAccessRequestsAction());
            }
        }

        private void Refresh()
        {
            Dispatcher.Dispatch(new GetMyAccessRequestsAction());
        }
    }
}
