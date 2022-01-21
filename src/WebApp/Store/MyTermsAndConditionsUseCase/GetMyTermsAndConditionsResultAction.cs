namespace OfficeEntry.WebApp.Store.MyTermsAndConditionsUseCase;

public class GetMyTermsAndConditionsResultAction
{
    public bool IsHealthAndSafetyMeasuresAccepted { get; }
    public bool IsPrivacyActStatementAccepted { get; }

    public GetMyTermsAndConditionsResultAction(bool healthAndSafetyMeasuresAccepted, bool privacyActStatementAccepted)
    {
        IsHealthAndSafetyMeasuresAccepted = healthAndSafetyMeasuresAccepted;
        IsPrivacyActStatementAccepted = privacyActStatementAccepted;
    }
}
