using MediatR;
using OfficeEntry.Application.Common.Interfaces;

namespace OfficeEntry.Application.User.Queries.GetMyTermsAndConditions;

public record GetContactFirstResponderQuery : IRequest<bool>
{
    public static readonly GetContactFirstResponderQuery Instance = new();

    public GetContactFirstResponderQuery()
    {
    }
}

public class GetContactFirstResponderQueryHandler : IRequestHandler<GetContactFirstResponderQuery, bool>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserService _userService;

    public GetContactFirstResponderQueryHandler(ICurrentUserService currentUserService, IUserService userService)
    {
        _currentUserService = currentUserService;
        _userService = userService;
    }

    public async Task<bool> Handle(GetContactFirstResponderQuery request, CancellationToken cancellationToken)
    {
        var username = _currentUserService.UserId;
        var result = await _userService.IsContactFirstResponder(username); // Get a bool indicating if the user is a first responder

        // TODO: what should we do with the result
        if (!result.Result.Succeeded)
        {
        }

        return result.isFirstResponder;
    }
}
