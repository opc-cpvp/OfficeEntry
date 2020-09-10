using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using OfficeEntry.WebApp.Store.MyTermsAndConditionsUseCase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OfficeEntry.WebApp.Shared
{
    public partial class TermsAndConditionsCard
    {
        [Inject] public IState<MyTermsAndConditionsState> MyTermsAndConditionsState { get; set; }
        [Inject] public IStringLocalizer<App> Localizer { get; set; }
    }
}
