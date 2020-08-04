using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using OfficeEntry.Application.TermsAndConditions.Commands.UpdatePrivacyStatementRequests;
using OfficeEntry.Application.TermsAndConditions.Queries.GetPrivacyStatementRequests;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace OfficeEntry.WebApp.Pages
{
    [Authorize]
    public abstract class PrivacyActStatementBase : ComponentBase
    {
        [Inject]
        public HttpClient Http { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IMediator Mediator { get; set; }

        protected string SurveyData { get; set; }

        public bool SurveyCompleted { get; set; }

        public async Task OnSurveyCompleted(string surveyResult)
        {
            SurveyCompleted = true;

            var surveyData = JsonSerializer.Deserialize<PrivacyActStatementSurveyData>(surveyResult);

            bool privateActStatementAccepted = surveyData.questionAcceptPaStatement.Any();

            await Mediator.Send(new UpdatePrivacyActStatementForCurrentUserCommand { IsPrivacyActStatementAccepted = privateActStatementAccepted });

            NavigationManager.NavigateTo("/");
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var isPrivacyActStatementAccepted = await Mediator.Send(new GetPrivacyStatementForCurrentUserQuery());

                var surveyData = new PrivacyActStatementSurveyData
                {
                    questionAcceptPaStatement = isPrivacyActStatementAccepted
                        ? new string[] { "iAcceptPaStatement" }
                        : new string[0]
                };

                SurveyData = JsonSerializer.Serialize(surveyData);

                StateHasChanged();
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        private protected class PrivacyActStatementSurveyData
        {
            public string[] questionAcceptPaStatement { get; set; }
        }
    }
}