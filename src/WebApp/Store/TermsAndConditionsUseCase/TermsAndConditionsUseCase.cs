using Fluxor;
using MediatR;
using OfficeEntry.Application.User.Queries.GetMyTermsAndConditions;
using OfficeEntry.Domain.Services;

namespace OfficeEntry.WebApp.Store.TermsAndConditionsUseCase;

public record TermsAndConditionsState
{
    public bool IsLoading { get; }
    public bool IsHealthAndSafetyMeasuresAccepted { get; }
    public bool IsPrivacyActStatementAccepted { get; }
    public bool AreTermsAndConditionsAccepted => IsHealthAndSafetyMeasuresAccepted && IsPrivacyActStatementAccepted;

    public TermsAndConditionsState(bool isLoading, bool healthAndSafetyMeasuresAccepted, bool privacyActStatementAccepted)
    {
        IsLoading = isLoading;
        IsHealthAndSafetyMeasuresAccepted = healthAndSafetyMeasuresAccepted;
        IsPrivacyActStatementAccepted = privacyActStatementAccepted;
    }
}

public class Feature : Feature<TermsAndConditionsState>
{
    public override string GetName() => "My Terms and Conditions";

    protected override TermsAndConditionsState GetInitialState() =>
        new TermsAndConditionsState(isLoading: true, healthAndSafetyMeasuresAccepted: false, privacyActStatementAccepted: false);
}

public record struct GetTermsAndConditions
{
}

public record GetTermsAndConditionsResultAction
{
    public bool IsHealthAndSafetyMeasuresAccepted { get; }
    public bool IsPrivacyActStatementAccepted { get; }

    public GetTermsAndConditionsResultAction(bool healthAndSafetyMeasuresAccepted, bool privacyActStatementAccepted)
    {
        IsHealthAndSafetyMeasuresAccepted = healthAndSafetyMeasuresAccepted;
        IsPrivacyActStatementAccepted = privacyActStatementAccepted;
    }
}

public class Reducers
{
    [ReducerMethod]
    public static TermsAndConditionsState ReduceGetMyTermsAndConditions(TermsAndConditionsState state, GetTermsAndConditions action) =>
        new TermsAndConditionsState(isLoading: true, healthAndSafetyMeasuresAccepted: false, privacyActStatementAccepted: false);

    [ReducerMethod]
    public static TermsAndConditionsState ReduceGetMyTermsAndConditionsResultAction(TermsAndConditionsState state, GetTermsAndConditionsResultAction action) =>
        new TermsAndConditionsState(isLoading: false, action.IsHealthAndSafetyMeasuresAccepted, action.IsPrivacyActStatementAccepted);
}

public class Effects
{
    private readonly IMediator _mediator;

    public Effects(IMediator mediator)
    {
        _mediator = mediator;
    }

    [EffectMethod]
    public async Task HandleFetchDataAction(GetTermsAndConditions action, IDispatcher dispatcher)
    {
        var result = await _mediator.Send(GetTermsAndConditionsQuery.Instance);

        dispatcher.Dispatch(new GetTermsAndConditionsResultAction(result.IsHealthAndSafetyMeasuresAccepted, result.IsPrivacyActStatementAccepted));
    }
}
