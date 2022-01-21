using Fluxor;

namespace OfficeEntry.WebApp.Store.MyTermsAndConditionsUseCase;

public class Feature : Feature<MyTermsAndConditionsState>
{
    public override string GetName() => "My Terms and Conditions";

    protected override MyTermsAndConditionsState GetInitialState() =>
        new MyTermsAndConditionsState(isLoading: true, healthAndSafetyMeasuresAccepted: false, privacyActStatementAccepted: false);
}
