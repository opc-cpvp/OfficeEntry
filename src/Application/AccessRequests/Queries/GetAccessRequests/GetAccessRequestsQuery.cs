using MediatR;
using OfficeEntry.Application.Common.Interfaces;

namespace OfficeEntry.Application.AccessRequests.Queries.GetAccessRequests;

public class GetAccessRequestsQuery : IRequest<IEnumerable<Domain.Entities.AccessRequest>>
{
}

public class GetAccessRequestsQueryHandler : IRequestHandler<GetAccessRequestsQuery, IEnumerable<Domain.Entities.AccessRequest>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IAccessRequestService _accessRequestService;
    private readonly IUserService _userService;

    public GetAccessRequestsQueryHandler(ICurrentUserService currentUserService, IAccessRequestService accessRequestService, IUserService userService)
    {
        _currentUserService = currentUserService;
        _accessRequestService = accessRequestService;
        _userService = userService;
    }

    public async Task<IEnumerable<Domain.Entities.AccessRequest>> Handle(GetAccessRequestsQuery request, CancellationToken cancellationToken)
    {
        var username = _currentUserService.UserId;
        var userResult = await _userService.GetUserId(username);
        var result = await _accessRequestService.GetAccessRequestsFor(userResult.UserId);

        // TODO: what should we do with the result
        if (!result.Result.Succeeded)
        {
        }

        return result.AccessRequests
            .OrderByDescending(x => x.StartTime)
            .ThenBy(x => x.GetStatusOrder());
    }
}
