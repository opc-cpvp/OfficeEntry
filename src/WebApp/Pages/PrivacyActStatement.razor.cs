using Fluxor;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using OfficeEntry.Application.User.Commands.UpdatePrivacyStatement;
using OfficeEntry.WebApp.Shared;
using System.Text.Json;
using OfficeEntry.WebApp.Store.TermsAndConditionsUseCase;

namespace OfficeEntry.WebApp.Pages;

[Authorize]
public partial class PrivacyActStatement
{
    [Inject] public NavigationManager NavigationManager { get; set; }
    [Inject] public IMediator Mediator { get; set; }
    [Inject] public IStringLocalizer<App> Localizer { get; set; }
    [Inject] public IState<TermsAndConditionsState> TermsAndConditionsState { get; set; }
    [Inject] public IDispatcher Dispatcher { get; set; }

    protected string SurveyData { get; set; }
    protected bool SurveyCompleted { get; set; }

    public async Task OnSurveyCompleted(SurveyCompletedEventArgs e)
    {
        SurveyCompleted = true;

        var surveyData = JsonSerializer.Deserialize<PrivacyActStatementSurveyData>(e.SurveyResult);

        var privateActStatementAccepted = surveyData.questionAcceptPaStatement.Any();

        await Mediator.Send(new UpdatePrivacyActStatementCommand { IsPrivacyActStatementAccepted = privateActStatementAccepted });

        Dispatcher.Dispatch(new GetTermsAndConditions());

        NavigationManager.NavigateTo(Localizer["my-access-requests"]);
    }

    protected override void Dispose(bool disposing)
    {
        TermsAndConditionsState.StateChanged -= MyTermsAndConditionsState_StateChanged;
        base.Dispose(disposing);
    }

    protected override void OnInitialized()
    {
        if (!TermsAndConditionsState.Value.IsLoading)
        {
            SetSurveyData(TermsAndConditionsState.Value.IsPrivacyActStatementAccepted);
        }

        TermsAndConditionsState.StateChanged += MyTermsAndConditionsState_StateChanged;
        base.OnInitialized();
    }

    private void MyTermsAndConditionsState_StateChanged(object sender, EventArgs e)
    {
        if (TermsAndConditionsState.Value.IsLoading)
        {
            return;
        }

        SetSurveyData(TermsAndConditionsState.Value.IsPrivacyActStatementAccepted);
        StateHasChanged();
    }

    private void SetSurveyData(bool isPrivacyActStatementAccepted)
    {
        var surveyData = new PrivacyActStatementSurveyData
        {
            questionAcceptPaStatement = isPrivacyActStatementAccepted
                ? new[] { "iAcceptPaStatement" }
                : Array.Empty<string>()
        };

        SurveyData = JsonSerializer.Serialize(surveyData);
    }

    private protected class PrivacyActStatementSurveyData
    {
        public string[] questionAcceptPaStatement { get; set; }
    }
}
