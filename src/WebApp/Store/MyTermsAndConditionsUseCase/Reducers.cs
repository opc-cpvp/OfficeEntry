using Fluxor;

namespace OfficeEntry.WebApp.Store.MyTermsAndConditionsUseCase
{
    public class Reducers
    {
        [ReducerMethod]
        public static MyTermsAndConditionsState ReduceGetMyTermsAndConditions(MyTermsAndConditionsState state, GetMyTermsAndConditions action) =>
            new MyTermsAndConditionsState(isLoading: true, healthAndSafetyMeasuresAccepted: false, privacyActStatementAccepted: false);

        [ReducerMethod]
        public static MyTermsAndConditionsState ReduceGetMyTermsAndConditionsResultAction(MyTermsAndConditionsState state, GetMyTermsAndConditionsResultAction action) =>
            new MyTermsAndConditionsState(isLoading: false, action.IsHealthAndSafetyMeasuresAccepted, action.IsPrivacyActStatementAccepted);
    }
}
