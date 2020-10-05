using Fluxor;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using OfficeEntry.Domain.Enums;
using OfficeEntry.WebApp.Store.ManagerApprovalsUseCase;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace OfficeEntry.WebApp.Pages
{
    [Authorize]
    public partial class ReviewAccessRequests
    {
        [Inject] public IState<ManagerApprovalsState> ApprovalsState { get; set; }
        [Inject] public IDispatcher Dispatcher { get; set; }
        [Inject] public IStringLocalizer<App> Localizer { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (!ApprovalsState.Value.AccessRequests.Any())
            {
                Dispatcher.Dispatch(new GetManagerApprovalsAction());
            }
        }

        private void Refresh()
        {
            Dispatcher.Dispatch(new GetManagerApprovalsAction());
        }
    }
}
