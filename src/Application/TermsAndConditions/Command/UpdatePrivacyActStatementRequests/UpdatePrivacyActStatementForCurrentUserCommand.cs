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
        private readonly IDomainUserService _domainUserService;
        private readonly ITermsAndConditionsService _termsAndConditionsService;

        public UpdatePrivacyActStatementForCurrentUserCommandHandler(ICurrentUserService currentUserService, IDomainUserService domainUserService, ITermsAndConditionsService termsAndConditionsService)
        {
            _currentUserService = currentUserService;
            _domainUserService = domainUserService;
            _termsAndConditionsService = termsAndConditionsService;
        }

        public async Task<Unit> Handle(UpdatePrivacyActStatementForCurrentUserCommand request, CancellationToken cancellationToken)
        {
            var fullname = await _domainUserService.GetUserNameAsync(AdAccount.For(_currentUserService.UserId));

            await _termsAndConditionsService.SetPrivacyActStatementFor(fullname, request.IsPrivacyActStatementAccepted);

            return Unit.Value;
        }
    }
}