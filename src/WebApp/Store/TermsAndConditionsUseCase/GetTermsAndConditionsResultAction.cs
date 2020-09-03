namespace OfficeEntry.WebApp.Store.TermsAndConditionsUseCase
{
    public class GetTermsAndConditionsResultAction
    {
        public bool IsHealthAndSafetyMeasuresStatementAccepted { get; }
        public bool IsPrivacyActStatementAccepted { get; }

        public GetTermsAndConditionsResultAction(bool isHealthAndSafetyMeasuresStatementAccepted, bool isPrivacyActStatementAccepted)
        {
            IsHealthAndSafetyMeasuresStatementAccepted = isHealthAndSafetyMeasuresStatementAccepted;
            IsPrivacyActStatementAccepted = isPrivacyActStatementAccepted;
        }
    }
}
