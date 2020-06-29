﻿using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace OfficeEntry.Application.TermsAndConditions.Commands.UpdateHealthAndSafetyStatementRequests
{
    public class UpdateHealthAndSafetyMeasuresStatementForCurrentUserCommand : IRequest
    {
        public bool IsHealthAndSafetyMeasuresAccepted { get; set; }
    }

    public class UpdateHealthAndSafetyMeasuresStatementForCurrentUserCommandHandler : IRequestHandler<UpdateHealthAndSafetyMeasuresStatementForCurrentUserCommand>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IDomainUserService _domainUserService;
        private readonly ITermsAndConditionsService _termsAndConditionsService;

        public UpdateHealthAndSafetyMeasuresStatementForCurrentUserCommandHandler(ICurrentUserService currentUserService, ITermsAndConditionsService termsAndConditionsService)
        {
            _currentUserService = currentUserService;
            _termsAndConditionsService = termsAndConditionsService;
        }

        public async Task<Unit> Handle(UpdateHealthAndSafetyMeasuresStatementForCurrentUserCommand request, CancellationToken cancellationToken)
        {
            var username = _currentUserService.UserId;

            await _termsAndConditionsService.SetHealthAndSafetyMeasuresFor(username, request.IsHealthAndSafetyMeasuresAccepted);

            return Unit.Value;
        }
    }
}