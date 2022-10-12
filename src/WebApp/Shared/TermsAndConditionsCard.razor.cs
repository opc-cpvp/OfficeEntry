using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using OfficeEntry.WebApp.Store.TermsAndConditionsUseCase;

namespace OfficeEntry.WebApp.Shared;

public partial class TermsAndConditionsCard
{
    [Inject] public IState<TermsAndConditionsState> TermsAndConditionsState { get; set; }
    [Inject] public IStringLocalizer<App> Localizer { get; set; }
}
