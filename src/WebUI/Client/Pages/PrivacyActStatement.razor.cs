using Microsoft.AspNetCore.Components;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace OfficeEntry.WebUI.Client.Pages
{
    public abstract class PrivacyActStatementBase : ComponentBase
    {
        [Inject]
        public HttpClient Http { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        protected string SurveyData { get; set; }

        public bool SurveyCompleted { get; set; }

        public async Task OnSurveyCompleted(string surveyResult)
        {
            SurveyCompleted = true;

            var surveyData = JsonSerializer.Deserialize<PrivacyActStatementSurveyData>(surveyResult);

            bool privateActStatementAccepted = surveyData.questionAcceptPaStatement.Any();

            await Http.PutAsJsonAsync("api/PrivacyStatement", privateActStatementAccepted);

            NavigationManager.NavigateTo("/");
        }

        protected override async Task OnInitializedAsync()
        {
            var isPrivacyActStatementAccepted = await Http.GetFromJsonAsync<bool>("api/PrivacyStatement");

            var surveyData = new PrivacyActStatementSurveyData
            {
                questionAcceptPaStatement = isPrivacyActStatementAccepted
                    ? new string[] { "iAcceptPaStatement" }
                    : new string[0]
            };

            SurveyData = JsonSerializer.Serialize(surveyData);
        }

        private protected class PrivacyActStatementSurveyData
        {
            public string[] questionAcceptPaStatement { get; set; }
        }
    }
}