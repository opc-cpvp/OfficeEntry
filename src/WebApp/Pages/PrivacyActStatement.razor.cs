using Fluxor;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using OfficeEntry.Application.User.Commands.UpdatePrivacyStatementRequests;
using OfficeEntry.WebApp.Store.MyTermsAndConditionsUseCase;
using System.Text.Json;

namespace OfficeEntry.WebApp.Pages;

[Authorize]
public partial class PrivacyActStatement
{
    [Inject] public NavigationManager NavigationManager { get; set; }
    [Inject] public IMediator Mediator { get; set; }
    [Inject] public IStringLocalizer<App> Localizer { get; set; }
    [Inject] public IState<MyTermsAndConditionsState> MyTermsAndConditionsState { get; set; }
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

    protected override void Dispose(bool disposing)
    {
        MyTermsAndConditionsState.StateChanged -= MyTermsAndConditionsState_StateChanged;
        base.Dispose(disposing);
    }

    protected override void OnInitialized()
    {
        if (!MyTermsAndConditionsState.Value.IsLoading)
        {
            SetSurveyData(MyTermsAndConditionsState.Value.IsPrivacyActStatementAccepted);
        }

        MyTermsAndConditionsState.StateChanged += MyTermsAndConditionsState_StateChanged;
        base.OnInitialized();
    }

    private void MyTermsAndConditionsState_StateChanged(object sender, EventArgs e)
    {
        if (MyTermsAndConditionsState.Value.IsLoading)
        {
            return;
        }

        SetSurveyData(MyTermsAndConditionsState.Value.IsPrivacyActStatementAccepted);
        StateHasChanged();
    }

    private void SetSurveyData(bool isPrivacyActStatementAccepted)
    {
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
