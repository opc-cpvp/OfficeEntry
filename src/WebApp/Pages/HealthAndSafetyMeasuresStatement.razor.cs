using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using OfficeEntry.Application.TermsAndConditions.Commands.UpdateHealthAndSafetyStatementRequests;
using OfficeEntry.Application.TermsAndConditions.Queries.GetHealthAndSafetyMeasuresRequests;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace OfficeEntry.WebApp.Pages
{
    [Authorize]
    public abstract class HealthAndSafetyMeasuresStatementBase : ComponentBase
    {
        [Inject]
        public HttpClient Http { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IMediator Mediator { get; set; }

        public bool SurveyCompleted { get; set; }

        protected string SurveyData { get; set; }

        public async Task OnSurveyCompleted(string surveyResult)
        {
            SurveyCompleted = true;

            var surveyData = JsonSerializer.Deserialize<HealthAndSafetyMeasuresStatementSurveyData>(surveyResult);

            bool healthAndSafetyMeasuresAccepted = surveyData.questionAcceptHsmStatement.Any();

            await Mediator.Send(new UpdateHealthAndSafetyMeasuresStatementForCurrentUserCommand { IsHealthAndSafetyMeasuresAccepted = healthAndSafetyMeasuresAccepted });

            NavigationManager.NavigateTo("/");
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var isHealthAndSafetyStatementAccepted = await Mediator.Send(new GetHealthAndSafetyMeasuresForCurrentUserQuery());

                var surveyData = new HealthAndSafetyMeasuresStatementSurveyData
                {
                    questionAcceptHsmStatement = isHealthAndSafetyStatementAccepted
                        ? new string[] { "iAcceptHsmStatement" }
                        : new string[0]
                };

                SurveyData = JsonSerializer.Serialize(surveyData);
                StateHasChanged();
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        private protected class HealthAndSafetyMeasuresStatementSurveyData
        {
            public string[] questionAcceptHsmStatement { get; set; }
        }
    }
}