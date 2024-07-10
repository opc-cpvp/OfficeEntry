using MediatR;
using OfficeEntry.Application.Common.Interfaces;

namespace OfficeEntry.Application.User.Commands.UpdateHealthAndSafetyStatement;

public record UpdateHealthAndSafetyMeasuresStatementCommand : IRequest
{
    public bool IsHealthAndSafetyMeasuresAccepted { get; init; }
}

public class UpdateHealthAndSafetyMeasuresStatementCommandHandler : IRequestHandler<UpdateHealthAndSafetyMeasuresStatementCommand>
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
}
