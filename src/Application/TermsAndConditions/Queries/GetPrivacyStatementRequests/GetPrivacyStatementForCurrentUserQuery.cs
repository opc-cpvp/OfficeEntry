using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OfficeEntry.Application.TermsAndConditions.Queries.GetPrivacyStatementRequests
{
    public class GetPrivacyStatementForCurrentUserQuery : IRequest<bool>
    {
    }

    public class GetPrivacyStatementForCurrentUserQueryHandler : IRequestHandler<GetPrivacyStatementForCurrentUserQuery, bool>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IDomainUserService _domainUserService;
        private readonly ITermsAndConditionsService _termsAndConditionsService;

        public GetPrivacyStatementForCurrentUserQueryHandler(ICurrentUserService currentUserService, IDomainUserService domainUserService, ITermsAndConditionsService termsAndConditionsService)
        {
            _currentUserService = currentUserService;
            _domainUserService = domainUserService;
            _termsAndConditionsService = termsAndConditionsService;
        }

        public async Task<bool> Handle(GetPrivacyStatementForCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var fullname = await _domainUserService.GetUserNameAsync(AdAccount.For(_currentUserService.UserId));
            var result = await _termsAndConditionsService.GetPrivacyActStatementFor(fullname);

            // TODO: what should we do with the 
            if (!result.Result.Succeeded)
            {

            }

            return result.IsPrivacyActStatementAccepted;
        }
    }
}
