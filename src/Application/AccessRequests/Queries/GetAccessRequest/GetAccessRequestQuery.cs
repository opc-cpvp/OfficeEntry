using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OfficeEntry.Application.AccessRequests.Queries.GetAccessRequest
{
    public class GetAccessRequestQuery : IRequest<Domain.Entities.AccessRequest>
    {
        public Guid AccessRequestId { get; set; }
    }

    public class GetAccessRequestQueryHandler : IRequestHandler<GetAccessRequestQuery, Domain.Entities.AccessRequest>
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

        public async Task<Domain.Entities.AccessRequest> Handle(GetAccessRequestQuery request, CancellationToken cancellationToken)
        {
            var username = _currentUserService.UserId;
            var userResult = await _userService.GetUserId(username);
            var result = await _accessRequestService.GetAccessRequestFor(userResult.UserId, request.AccessRequestId);

            // TODO: what should we do with the
            if (!result.Result.Succeeded)
            {
            }

            return result.AccessRequest;
        }
    }
}