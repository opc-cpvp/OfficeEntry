using MagicOnion;
using MagicOnion.Server;
using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;
using OfficeEntry.Domain.Services;

namespace OfficeEntry.Application.Users.Queries.GetContactsRequests;

public class GetContactsQueryHandler :
    ServiceBase<IGetContactsQueryService>,
    IGetContactsQueryService,
    IRequestHandler<GetContactsQuery, IEnumerable<Domain.Entities.Contact>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserService _userService;

    public GetContactsQueryHandler(ICurrentUserService currentUserService, IUserService userService)
    {
        _currentUserService = currentUserService;
        _userService = userService;
    }

    public async Task<IEnumerable<Domain.Entities.Contact>> Handle(GetContactsQuery request, CancellationToken cancellationToken)
    {
        var excludeUsername = _currentUserService.UserId;
        var result = await _userService.GetContacts(excludeUsername);

        // TODO: what should we do with the result
        if (!result.Result.Succeeded)
        {
        }

        return result.Contacts;
    }

    public async UnaryResult<Contact[]> HandleAsync(GetContactsQuery request)
    {
        return [.. await Handle(request, CancellationToken.None)];
    }
}
