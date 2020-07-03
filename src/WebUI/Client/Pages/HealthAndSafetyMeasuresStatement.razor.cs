using Microsoft.AspNetCore.Components;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace OfficeEntry.WebUI.Client.Pages
{
    public abstract class HealthAndSafetyMeasuresStatementBase : ComponentBase
    {
        [Inject]
        public HttpClient Http { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public bool SurveyCompleted { get; set; }

        protected string SurveyData { get; set; }

        public async Task OnSurveyCompleted(string surveyResult)
        {
            SurveyCompleted = true;

            var surveyData = JsonSerializer.Deserialize<HealthAndSafetyMeasuresStatementSurveyData>(surveyResult);

            bool healthAndSafetyMeasuresAccepted = surveyData.questionAcceptHsmStatement.Any();

            await Http.PutAsJsonAsync("api/HealthAndSafetyMeasures", healthAndSafetyMeasuresAccepted);

            NavigationManager.NavigateTo("/");
        }

        protected override async Task OnInitializedAsync()
        {
            var isPrivacyActStatementAccepted = await Http.GetFromJsonAsync<bool>("api/HealthAndSafetyMeasures");

            var surveyData = new HealthAndSafetyMeasuresStatementSurveyData
            {
                questionAcceptHsmStatement = isPrivacyActStatementAccepted
                    ? new string[] { "iAcceptHsmStatement" }
                    : new string[0]
            };

            SurveyData = JsonSerializer.Serialize(surveyData);
        }

        private protected class HealthAndSafetyMeasuresStatementSurveyData
        {
            public string[] questionAcceptHsmStatement { get; set; }
        }
    }
}