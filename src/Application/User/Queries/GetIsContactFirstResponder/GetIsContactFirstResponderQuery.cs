using MediatR;
using Microsoft.Extensions.Logging;
using OfficeEntry.Application.Common.Interfaces;

namespace OfficeEntry.Application.User.Queries.GetIsContactFirstResponder;

public record GetIsContactFirstResponderQuery(string userId) : IRequest<bool>
{
    public static readonly GetIsContactFirstResponderQuery Instance = new();

    public GetIsContactFirstResponderQuery() : this(string.Empty)
    {
    }
}

public class GetIsContactFirstResponderQueryHandler : IRequestHandler<GetIsContactFirstResponderQuery, bool>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserService _userService;
    private readonly ILogger<GetIsContactFirstResponderQueryHandler> _logger;

    public GetIsContactFirstResponderQueryHandler(ICurrentUserService currentUserService, IUserService userService, ILogger<GetIsContactFirstResponderQueryHandler> logger)
    {
        _currentUserService = currentUserService;
        _userService = userService;
        _logger = logger;
    }

    public async Task<bool> Handle(GetIsContactFirstResponderQuery request, CancellationToken cancellationToken)
    {
        var userIdToCheck = request.userId;

        if (string.IsNullOrEmpty(userIdToCheck)) 
        {
            var username = _currentUserService.UserId;
            var result = await _userService.IsContactFirstResponder(username);

            if (!result.Result.Succeeded)
            {
                _logger.LogError("Failed to get first responder status for user {Username}", username);
                return false;
            }

            return result.isFirstResponder;
        }
        else
        {
            var result = await _userService.IsContactFirstResponder(new Guid(userIdToCheck));

            if (!result.Result.Succeeded)
            {
                _logger.LogError("Failed to get first responder status for user {Username}", userIdToCheck);
                return false;
            }

            return result.isFirstResponder;
        }



    }
}
