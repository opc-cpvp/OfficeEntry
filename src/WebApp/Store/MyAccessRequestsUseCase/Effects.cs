using Fluxor;
using MediatR;
using OfficeEntry.Application.AccessRequests.Queries.GetAccessRequests;
using System.Linq;
using System.Threading.Tasks;

namespace OfficeEntry.WebApp.Store.MyAccessRequestsUseCase
{
    public class Effects
    {
        private readonly IMediator _mediator;

        public Effects(IMediator mediator)
        {
            _mediator = mediator;
        }

        [EffectMethod]
        public async Task HandleFetchDataAction(GetMyAccessRequestsAction action, IDispatcher dispatcher)
        {
            var accessRequests = (await _mediator.Send(new GetAccessRequestsQuery())).ToArray();
            dispatcher.Dispatch(new GetMyAccessRequestsResultAction(accessRequests));
        }
    }
}
