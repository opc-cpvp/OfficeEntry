using Fluxor;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using OfficeEntry.WebApp.Store.TermsAndConditionsUseCase;

namespace OfficeEntry.WebApp.Pages;

[Authorize]
public partial class TermsAndConditions
{
    [Inject] private IState<TermsAndConditionsState> TermsAndConditionsState { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    [Inject] public IMediator Mediator { get; set; }
    [Inject] public IStringLocalizer<App> Localizer { get; set; }
}
