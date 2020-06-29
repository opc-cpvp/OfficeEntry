using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OfficeEntry.Application.AccessRequests.Queries.GetAccessRequest
{
    public class GetAccessRequestQuery : IRequest<AccessRequestViewModel>
    {
        public Guid AccessRequestId { get; set; }
    }

    public class GetAccessRequestQueryHandler : IRequestHandler<GetAccessRequestQuery, AccessRequestViewModel>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IAccessRequestService _accessRequestService;
        private readonly IUserService _userService;

        public GetAccessRequestQueryHandler(ICurrentUserService currentUserService, IAccessRequestService accessRequestService, IUserService userService)
        {
            _currentUserService = currentUserService;
            _accessRequestService = accessRequestService;
            _userService = userService;
        }

        public async Task<AccessRequestViewModel> Handle(GetAccessRequestQuery request, CancellationToken cancellationToken)
        {
            var username = _currentUserService.UserId;
            var userResult = await _userService.GetUserId(username);

            // TODO: Ensure that the user is only able to view the request if they're the employee / manager
            var result = await _accessRequestService.GetAccessRequest(request.AccessRequestId);

            // TODO: what should we do with the result
            if (!result.Result.Succeeded)
            {
            }

            return new AccessRequestViewModel
            {
                IsEmployee = result.AccessRequest.Employee.Id == userResult.UserId,
                IsManager = result.AccessRequest.Manager.Id == userResult.UserId,
                AccessRequest = result.AccessRequest
            };
        }
    }
}