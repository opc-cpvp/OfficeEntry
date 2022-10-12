using MediatR;
using OfficeEntry.Application.Common.Interfaces;

namespace OfficeEntry.Application.AccessRequests.Queries.GetDelegateAccessRequests;

public record GetDelegateAccessRequestsQuery : IRequest<IEnumerable<Domain.Entities.AccessRequest>>;

public class GetDelegateAccessRequestsQueryHandler : IRequestHandler<GetDelegateAccessRequestsQuery, IEnumerable<Domain.Entities.AccessRequest>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IAccessRequestService _accessRequestService;
    private readonly IUserService _userService;

    public GetDelegateAccessRequestsQueryHandler(ICurrentUserService currentUserService, IAccessRequestService accessRequestService, IUserService userService)
    {
        _currentUserService = currentUserService;
        _accessRequestService = accessRequestService;
        _userService = userService;
    }

    public async Task<IEnumerable<Domain.Entities.AccessRequest>> Handle(GetDelegateAccessRequestsQuery request, CancellationToken cancellationToken)
    {
        var username = _currentUserService.UserId;
        var userResult = await _userService.GetUserId(username);
        var result = await _accessRequestService.GetDelegateAccessRequestsFor(userResult.UserId);

        // TODO: what should we do with the result
        if (!result.Result.Succeeded)
        {
        }

        return result.AccessRequests
            .OrderBy(x => x.GetStatusOrder())
            .ThenByDescending(x => x.StartTime);
    }
}
