using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace OfficeEntry.WebApp.Pages;

[Authorize]
public partial class Index
{
    private bool loading = true;

    [Inject] public NavigationManager NavigationManager { get; set; }

    protected override void OnInitialized()
    {
        var baseUriRange = NavigationManager.BaseUri.Length..;
        var uri = NavigationManager.Uri[baseUriRange];
        switch (uri.Trim('/'))
        {
            case "en":
                SetLanguageToEnglish();
                return;
            case "fr":
                SetLanguageToFrench();
                return;
            default:
                break;
        }

        base.OnInitialized();
        loading = false;
    }

    public void SetLanguageToFrench()
    {
        NavigationManager.NavigateTo("/fr/mes-demandes-d-acces", forceLoad: true);
    }

    public void SetLanguageToEnglish()
    {
        NavigationManager.NavigateTo("/en/my-access-requests", forceLoad: true);
    }
}
