using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OfficeEntry.Application.TermsAndConditions.Command.UpdatePrivacyStatementRequests
{
    public class UpdatePrivacyActStatementForCurrentUserCommand : IRequest
    {
        public bool IsPrivacyActStatementAccepted { get; set; }
    }

    public class UpdatePrivacyActStatementForCurrentUserCommandHandler : IRequestHandler<UpdatePrivacyActStatementForCurrentUserCommand>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ITermsAndConditionsService _termsAndConditionsService;

        public UpdatePrivacyActStatementForCurrentUserCommandHandler(ICurrentUserService currentUserService, ITermsAndConditionsService termsAndConditionsService)
        {
            _currentUserService = currentUserService;
            _termsAndConditionsService = termsAndConditionsService;
        }

        public async Task<Unit> Handle(UpdatePrivacyActStatementForCurrentUserCommand request, CancellationToken cancellationToken)
        {
            var username = _currentUserService.UserId;

            await _termsAndConditionsService.SetPrivacyActStatementFor(username, request.IsPrivacyActStatementAccepted);

            return Unit.Value;
        }
    }
}