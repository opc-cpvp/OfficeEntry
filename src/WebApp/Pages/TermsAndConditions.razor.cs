using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;
using OfficeEntry.WebApp.Store.MyTermsAndConditionsUseCase;
using Fluxor;

namespace OfficeEntry.WebApp.Pages
{
    [Authorize]
    public partial class TermsAndConditions
    {
        [Inject] private IState<MyTermsAndConditionsState> MyTermsAndConditionsState { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] public IMediator Mediator { get; set; }
        [Inject] public IStringLocalizer<App> Localizer { get; set; }
    }
}
