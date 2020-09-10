using Fluxor;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using OfficeEntry.Application.TermsAndConditions.Commands.UpdatePrivacyStatementRequests;
using OfficeEntry.Application.TermsAndConditions.Queries.GetPrivacyStatementRequests;
using OfficeEntry.WebApp.Store.MyTermsAndConditionsUseCase;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace OfficeEntry.WebApp.Pages
{
    [Authorize]
    public partial class PrivacyActStatement
    {
        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] public IMediator Mediator { get; set; }
        [Inject] public IStringLocalizer<App> Localizer { get; set; }
        [Inject] public IDispatcher Dispatcher { get; set; }

        protected string SurveyData { get; set; }
        protected bool SurveyCompleted { get; set; }

        public async Task OnSurveyCompleted(string surveyResult)
        {
            SurveyCompleted = true;

            var surveyData = JsonSerializer.Deserialize<PrivacyActStatementSurveyData>(surveyResult);

            bool privateActStatementAccepted = surveyData.questionAcceptPaStatement.Any();

            await Mediator.Send(new UpdatePrivacyActStatementForCurrentUserCommand { IsPrivacyActStatementAccepted = privateActStatementAccepted });

            Dispatcher.Dispatch(new GetMyTermsAndConditions());

            NavigationManager.NavigateTo(Localizer["my-access-requests"]);
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