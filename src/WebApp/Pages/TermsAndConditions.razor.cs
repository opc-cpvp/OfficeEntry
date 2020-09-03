using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Fluxor;
using OfficeEntry.WebApp.Store.TermsAndConditionsUseCase;

namespace OfficeEntry.WebApp.Pages
{
    [Authorize]
    public partial class TermsAndConditions
    {
        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] public IMediator Mediator { get; set; }
        [Inject] public IStringLocalizer<App> Localizer { get; set; }
        [Inject] private IState<TermsAndConditionsState> TermsAndConditionsState { get; set; }


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
                return;


            await base.OnAfterRenderAsync(firstRender);
        }

    }
}
