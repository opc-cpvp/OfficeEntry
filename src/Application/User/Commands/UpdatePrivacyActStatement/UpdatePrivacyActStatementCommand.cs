using MagicOnion;
using MagicOnion.Server;
using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Services;

namespace OfficeEntry.Application.User.Commands.UpdatePrivacyStatement;

public class UpdatePrivacyActStatementCommandHandler :
    ServiceBase<IUpdatePrivacyActStatementCommandService>,
    IUpdatePrivacyActStatementCommandService,
    IRequestHandler<UpdatePrivacyActStatementCommand>
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

    public async UnaryResult HandleAsync(UpdatePrivacyActStatementCommand request)
    {
        await Handle(request, CancellationToken.None);
    }
}
