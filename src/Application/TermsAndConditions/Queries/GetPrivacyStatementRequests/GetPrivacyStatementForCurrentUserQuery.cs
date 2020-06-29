using MediatR;
using OfficeEntry.Application.Common.Interfaces;
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
        private readonly ITermsAndConditionsService _termsAndConditionsService;

        public GetPrivacyStatementForCurrentUserQueryHandler(ICurrentUserService currentUserService, ITermsAndConditionsService termsAndConditionsService)
        {
            _currentUserService = currentUserService;
            _termsAndConditionsService = termsAndConditionsService;
        }

        public async Task<bool> Handle(GetPrivacyStatementForCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var username = _currentUserService.UserId;
            var result = await _termsAndConditionsService.GetPrivacyActStatementFor(username);

            // TODO: what should we do with the
            if (!result.Result.Succeeded)
            {
            }

            return result.IsPrivacyActStatementAccepted;
        }
    }
}