using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using OfficeEntry.WebApp.Store.MyTermsAndConditionsUseCase;

namespace OfficeEntry.WebApp.Shared
{
    public partial class TermsAndConditionsCard
    {
        [Inject] public IState<MyTermsAndConditionsState> MyTermsAndConditionsState { get; set; }
        [Inject] public IStringLocalizer<App> Localizer { get; set; }
    }
}
