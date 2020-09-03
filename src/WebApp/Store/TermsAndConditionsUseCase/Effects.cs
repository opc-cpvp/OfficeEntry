using Fluxor;
using MediatR;
using OfficeEntry.Application.TermsAndConditions.Queries.GetHealthAndSafetyMeasuresRequests;
using OfficeEntry.Application.TermsAndConditions.Queries.GetPrivacyStatementRequests;
using System.Threading.Tasks;

namespace OfficeEntry.WebApp.Store.TermsAndConditionsUseCase
{
    public class Effects
    {
        private readonly IMediator _mediator;

        public Effects(IMediator mediator)
        {
            _mediator = mediator;
        }

        [EffectMethod]
        public async Task HandleFetchDataAction(GetTermsAndConditionsAction action, IDispatcher dispatcher)
        {
            var isHealthAndSafetyMeasuresStatementAccepted = await _mediator.Send(new GetHealthAndSafetyMeasuresForCurrentUserQuery());
            var isPrivacyActStatementAccepted = await _mediator.Send(new GetPrivacyStatementForCurrentUserQuery());

            dispatcher.Dispatch(new GetTermsAndConditionsResultAction(isHealthAndSafetyMeasuresStatementAccepted, isPrivacyActStatementAccepted));
        }
    }
}
