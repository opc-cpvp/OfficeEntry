using MagicOnion;
using MagicOnion.Server;
using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Services;

namespace OfficeEntry.Application.User.Commands.UpdateHealthAndSafetyStatement;

public class UpdateHealthAndSafetyMeasuresStatementCommandHandler :
    ServiceBase<IUpdateHealthAndSafetyMeasuresStatementCommandService>,
    IUpdateHealthAndSafetyMeasuresStatementCommandService,
    IRequestHandler<UpdateHealthAndSafetyMeasuresStatementCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ITermsAndConditionsService _termsAndConditionsService;

    public UpdateHealthAndSafetyMeasuresStatementCommandHandler(ICurrentUserService currentUserService, ITermsAndConditionsService termsAndConditionsService)
    {
        _currentUserService = currentUserService;
        _termsAndConditionsService = termsAndConditionsService;
    }

    public async Task Handle(UpdateHealthAndSafetyMeasuresStatementCommand request, CancellationToken cancellationToken)
    {
        var username = _currentUserService.UserId;

        await _termsAndConditionsService.SetHealthAndSafetyMeasuresFor(username, request.IsHealthAndSafetyMeasuresAccepted);

        return;
    }

    public async UnaryResult HandleAsync(UpdateHealthAndSafetyMeasuresStatementCommand request)
    {
        await Handle(request, CancellationToken.None);
    }
}
