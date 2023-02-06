using Fluxor;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using OfficeEntry.Application.User.Commands.UpdateHealthAndSafetyStatement;
using OfficeEntry.WebApp.Shared;
using OfficeEntry.WebApp.Store.TermsAndConditionsUseCase;
using System.Text.Json;

namespace OfficeEntry.WebApp.Pages;

[Authorize]
public partial class HealthAndSafetyMeasuresStatement
{
    [Inject] public NavigationManager NavigationManager { get; set; }
    [Inject] public IMediator Mediator { get; set; }
    [Inject] public IStringLocalizer<App> Localizer { get; set; }
    [Inject] public IState<TermsAndConditionsState> TermsAndConditionsState { get; set; }
    [Inject] public IDispatcher Dispatcher { get; set; }

    protected bool SurveyCompleted { get; set; }
    protected string SurveyData { get; set; }

    public async Task OnSurveyCompleted(SurveyCompletedEventArgs e)
    {
        SurveyCompleted = true;

        var surveyData = JsonSerializer.Deserialize<HealthAndSafetyMeasuresStatementSurveyData>(e.SurveyResult);

        bool healthAndSafetyMeasuresAccepted = surveyData.questionAcceptHsmStatement.Any();

        await Mediator.Send(new UpdateHealthAndSafetyMeasuresStatementCommand { IsHealthAndSafetyMeasuresAccepted = healthAndSafetyMeasuresAccepted });

        Dispatcher.Dispatch(new GetTermsAndConditions());

        NavigationManager.NavigateTo(Localizer["my-requests"]);
    }

    protected override void Dispose(bool disposing)
    {
        TermsAndConditionsState.StateChanged -= TermsAndConditionsState_StateChanged;
        base.Dispose(disposing);
    }

    protected override void OnInitialized()
    {
        if (!TermsAndConditionsState.Value.IsLoading)
        {
            SetSurveyData(TermsAndConditionsState.Value.IsHealthAndSafetyMeasuresAccepted);
        }

        TermsAndConditionsState.StateChanged += TermsAndConditionsState_StateChanged;
        base.OnInitialized();
    }

    private void TermsAndConditionsState_StateChanged(object sender, EventArgs e)
    {
        if (TermsAndConditionsState.Value.IsLoading)
        {
            return;
        }

        SetSurveyData(TermsAndConditionsState.Value.IsHealthAndSafetyMeasuresAccepted);
        StateHasChanged();
    }

    private void SetSurveyData(bool isHealthAndSafetyMeasuresAccepted)
    {
        var surveyData = new HealthAndSafetyMeasuresStatementSurveyData
        {
            questionAcceptHsmStatement = isHealthAndSafetyMeasuresAccepted
                ? new string[] { "iAcceptHsmStatement" }
                : Array.Empty<string>()
        };

        SurveyData = JsonSerializer.Serialize(surveyData);
    }

    private protected class HealthAndSafetyMeasuresStatementSurveyData
    {
        public string[] questionAcceptHsmStatement { get; set; }
    }
}
