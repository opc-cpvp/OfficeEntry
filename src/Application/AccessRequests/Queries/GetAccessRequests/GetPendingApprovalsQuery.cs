using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace OfficeEntry.Application.AccessRequests.Queries.GetPendingApprovals
{
    public class GetPendingApprovalsQuery : IRequest<int>
    {
    }

    public class GetPendingApprovalsQueryHandler : IRequestHandler<GetPendingApprovalsQuery, int>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IAccessRequestService _accessRequestService;
        private readonly IUserService _userService;

        public GetPendingApprovalsQueryHandler(ICurrentUserService currentUserService, IAccessRequestService accessRequestService, IUserService userService)
        {
            _currentUserService = currentUserService;
            _accessRequestService = accessRequestService;
            _userService = userService;
        }

        public async Task<int> Handle(GetPendingApprovalsQuery request, CancellationToken cancellationToken)
        {
            var username = _currentUserService.UserId;
            var userResult = await _userService.GetUserId(username);
            var result = await _accessRequestService.GetPendingApprovalsFor(userResult.UserId);

            // TODO: what should we do with the result
            if (!result.Result.Succeeded)
            {
            }

            return result.PendingApprovals;
        }
    }
}