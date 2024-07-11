using MediatR;
using OfficeEntry.Application.Common.Interfaces;

namespace OfficeEntry.Application.User.Commands.UpdatePrivacyStatement;

public record UpdatePrivacyActStatementCommand : IRequest
{
    public bool IsPrivacyActStatementAccepted { get; init; }
}

public class UpdatePrivacyActStatementCommandHandler : IRequestHandler<UpdatePrivacyActStatementCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ITermsAndConditionsService _termsAndConditionsService;

    public UpdatePrivacyActStatementCommandHandler(ICurrentUserService currentUserService, ITermsAndConditionsService termsAndConditionsService)
    {
        _currentUserService = currentUserService;
        _termsAndConditionsService = termsAndConditionsService;
    }

    public async Task Handle(UpdatePrivacyActStatementCommand request, CancellationToken cancellationToken)
    {
        var username = _currentUserService.UserId;

        await _termsAndConditionsService.SetPrivacyActStatementFor(username, request.IsPrivacyActStatementAccepted);

        return;
    }
}
