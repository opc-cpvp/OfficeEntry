using Fluxor;
using MediatR;
using OfficeEntry.Application.User.Queries.GetTermsAndConditions;

namespace OfficeEntry.WebApp.Store.MyTermsAndConditionsUseCase;

public record MyTermsAndConditionsState
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

public class Feature : Feature<MyTermsAndConditionsState>
{
    public override string GetName() => "My Terms and Conditions";

    protected override MyTermsAndConditionsState GetInitialState() =>
        new MyTermsAndConditionsState(isLoading: true, healthAndSafetyMeasuresAccepted: false, privacyActStatementAccepted: false);
}

public record struct GetMyTermsAndConditions
{
}

public record GetMyTermsAndConditionsResultAction
{
    public bool IsHealthAndSafetyMeasuresAccepted { get; }
    public bool IsPrivacyActStatementAccepted { get; }

    public GetMyTermsAndConditionsResultAction(bool healthAndSafetyMeasuresAccepted, bool privacyActStatementAccepted)
    {
        IsHealthAndSafetyMeasuresAccepted = healthAndSafetyMeasuresAccepted;
        IsPrivacyActStatementAccepted = privacyActStatementAccepted;
    }
}

public class Reducers
{
    [ReducerMethod]
    public static MyTermsAndConditionsState ReduceGetMyTermsAndConditions(MyTermsAndConditionsState state, GetMyTermsAndConditions action) =>
        new MyTermsAndConditionsState(isLoading: true, healthAndSafetyMeasuresAccepted: false, privacyActStatementAccepted: false);

    [ReducerMethod]
    public static MyTermsAndConditionsState ReduceGetMyTermsAndConditionsResultAction(MyTermsAndConditionsState state, GetMyTermsAndConditionsResultAction action) =>
        new MyTermsAndConditionsState(isLoading: false, action.IsHealthAndSafetyMeasuresAccepted, action.IsPrivacyActStatementAccepted);
}

public class Effects
{
    private readonly IMediator _mediator;

    public Effects(IMediator mediator)
    {
        _mediator = mediator;
    }

    [EffectMethod]
    public async Task HandleFetchDataAction(GetMyTermsAndConditions action, IDispatcher dispatcher)
    {
        var result = await _mediator.Send(new GetMyTermsAndConditionsQuery());

        dispatcher.Dispatch(new GetMyTermsAndConditionsResultAction(result.IsHealthAndSafetyMeasuresAccepted, result.IsPrivacyActStatementAccepted));
    }
}
