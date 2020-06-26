using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OfficeEntry.Application.AccessRequests.Queries.GetAccessRequests
{
    public class GetManagerAccessRequestsQuery : IRequest<IEnumerable<Domain.Entities.AccessRequest>>
    {
    }

    public class GetManagerAccessRequestsQueryHandler : IRequestHandler<GetManagerAccessRequestsQuery, IEnumerable<Domain.Entities.AccessRequest>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IAccessRequestService _accessRequestService;
        private readonly IUserService _userService;

        public GetManagerAccessRequestsQueryHandler(ICurrentUserService currentUserService, IAccessRequestService accessRequestService, IUserService userService)
        {
            _currentUserService = currentUserService;
            _accessRequestService = accessRequestService;
            _userService = userService;
        }

        public async Task<IEnumerable<Domain.Entities.AccessRequest>> Handle(GetManagerAccessRequestsQuery request, CancellationToken cancellationToken)
        {
            var username = _currentUserService.UserId;
            var userResult = await _userService.GetUserId(username);
            var result = await _accessRequestService.GetManagerAccessRequestsFor(userResult.UserId);

            // TODO: what should we do with the
            if (!result.Result.Succeeded)
            {

            }

            return result.AccessRequests;
        }
    }
}
