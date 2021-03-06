﻿using Fluxor;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using OfficeEntry.Application.User.Commands.UpdateHealthAndSafetyStatementRequests;
using OfficeEntry.WebApp.Store.MyTermsAndConditionsUseCase;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace OfficeEntry.WebApp.Pages
{
    [Authorize]
    public partial class HealthAndSafetyMeasuresStatement
    {
        [Inject] public NavigationManager NavigationManager { get; set; }
        [Inject] public IMediator Mediator { get; set; }
        [Inject] public IStringLocalizer<App> Localizer { get; set; }
        [Inject] public IState<MyTermsAndConditionsState> MyTermsAndConditionsState { get; set; }
        [Inject] public IDispatcher Dispatcher { get; set; }

        protected bool SurveyCompleted { get; set; }
        protected string SurveyData { get; set; }

        public async Task OnSurveyCompleted(string surveyResult)
        {
            SurveyCompleted = true;

            var surveyData = JsonSerializer.Deserialize<HealthAndSafetyMeasuresStatementSurveyData>(surveyResult);

            bool healthAndSafetyMeasuresAccepted = surveyData.questionAcceptHsmStatement.Any();

            await Mediator.Send(new UpdateHealthAndSafetyMeasuresStatementForCurrentUserCommand { IsHealthAndSafetyMeasuresAccepted = healthAndSafetyMeasuresAccepted });

            Dispatcher.Dispatch(new GetMyTermsAndConditions());

            NavigationManager.NavigateTo(Localizer["my-access-requests"]);
        }

        protected override void Dispose(bool disposing)
        {
            MyTermsAndConditionsState.StateChanged -= MyTermsAndConditionsState_StateChanged;
            base.Dispose(disposing);
        }

        protected override void OnInitialized()
        {
            if (!MyTermsAndConditionsState.Value.IsLoading)
            {
                SetSurveyData(MyTermsAndConditionsState.Value.IsHealthAndSafetyMeasuresAccepted);
            }

            MyTermsAndConditionsState.StateChanged += MyTermsAndConditionsState_StateChanged;
            base.OnInitialized();
        }

        private void MyTermsAndConditionsState_StateChanged(object sender, MyTermsAndConditionsState e)
        {
            if (e.IsLoading)
            {
                return;
            }

            SetSurveyData(e.IsHealthAndSafetyMeasuresAccepted);
            StateHasChanged();
        }

        private void SetSurveyData(bool isHealthAndSafetyMeasuresAccepted)
        {
            var surveyData = new HealthAndSafetyMeasuresStatementSurveyData
            {
                questionAcceptHsmStatement = isHealthAndSafetyMeasuresAccepted
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