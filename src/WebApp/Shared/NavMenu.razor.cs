using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace OfficeEntry.WebApp.Shared
{
    public partial class NavMenu : ComponentBase
    {
        [Inject]
        IStringLocalizer<App> Localizer { get; set; }

        private bool collapseNavMenu = true;

        private string NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }
    }
}
