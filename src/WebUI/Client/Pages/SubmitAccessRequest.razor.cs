using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Threading.Tasks;

namespace OfficeEntry.WebUI.Client.Pages
{
    public abstract class SubmitAccessRequestBase : ComponentBase
    {
        [Inject]
        public HttpClient Http { get; set; }

        public async Task OnSurveyCompleted(string surveyResult)
        {
        }
    }
}
