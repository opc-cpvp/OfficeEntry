using Fluxor;

namespace OfficeEntry.WebApp.Store.TermsAndConditionsUseCase
{
    public class Reducers
    {
        [ReducerMethod]
        public static TermsAndConditionsState ReduceGetMyAccessRequestsAction(TermsAndConditionsState state, GetTermsAndConditionsAction action) =>
            new TermsAndConditionsState(isLoading: true, isHealthAndSafetyMeasuresStatementAccepted: false, isPrivacyActStatementAccepted: false);

        [ReducerMethod]
        public static TermsAndConditionsState ReduceGetMyAccessRequestsResultAction(TermsAndConditionsState state, GetTermsAndConditionsResultAction action) =>
            new TermsAndConditionsState(isLoading: false, isHealthAndSafetyMeasuresStatementAccepted: action.IsHealthAndSafetyMeasuresStatementAccepted, isPrivacyActStatementAccepted: action.IsPrivacyActStatementAccepted);
    }
}
