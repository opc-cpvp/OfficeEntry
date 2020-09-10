namespace OfficeEntry.WebApp.Store.MyTermsAndConditionsUseCase
{
    public class MyTermsAndConditionsState
    {
        public bool IsLoading { get; }
        public bool IsHealthAndSafetyMeasuresAccepted { get; }
        public bool IsPrivacyActStatementAccepted { get; }
        public bool AreTermsAndConditionsAccepted => IsHealthAndSafetyMeasuresAccepted && IsPrivacyActStatementAccepted;

        public MyTermsAndConditionsState(bool isLoading, bool healthAndSafetyMeasuresAccepted, bool privacyActStatementAccepted)
        {
            IsLoading = isLoading;
            IsHealthAndSafetyMeasuresAccepted = healthAndSafetyMeasuresAccepted;
            IsPrivacyActStatementAccepted = privacyActStatementAccepted;
        }
    }
}
