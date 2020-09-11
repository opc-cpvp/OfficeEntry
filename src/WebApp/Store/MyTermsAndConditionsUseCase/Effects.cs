using Fluxor;
using MediatR;
using OfficeEntry.Application.User.Queries.GetTermsAndConditions;
using System.Threading.Tasks;

namespace OfficeEntry.WebApp.Store.MyTermsAndConditionsUseCase
{
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
}
