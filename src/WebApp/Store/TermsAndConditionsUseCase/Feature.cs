using Fluxor;

namespace OfficeEntry.WebApp.Store.TermsAndConditionsUseCase
{
    public class Feature : Feature<TermsAndConditionsState>
    {
        public override string GetName() => "Terms and Conditions";

        protected override TermsAndConditionsState GetInitialState() =>
            new TermsAndConditionsState(isLoading: true, isHealthAndSafetyMeasuresStatementAccepted: false, isPrivacyActStatementAccepted: false);
    }
}
