using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace OfficeEntry.WebUI.Client.Pages
{
    public abstract class HealthAndSafetyMeasuresStatementBase : ComponentBase
    {
        [Inject]
        public HttpClient Http { get; set; }

        public async Task OnSurveyCompleted(string surveyResult)
        {
            var surveyData = System.Text.Json.JsonSerializer.Deserialize<Rootobject>(surveyResult);

            bool healthAndSafetyMeasuresAccepted = surveyData.questionAcceptHsmStatement.Any();

            await Http.PutAsJsonAsync("api/HealthAndSafetyMeasures", healthAndSafetyMeasuresAccepted);
        }

        private protected class Rootobject
        {
            public string[] questionAcceptHsmStatement { get; set; }
        }
    }
}
