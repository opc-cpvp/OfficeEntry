using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;

namespace OfficeEntry.Application.User.Queries.GetTermsAndConditions
{
    public class GetMyTermsAndConditionsQuery : IRequest<TermsAndConditions>
    {
    }

    public class GetMyTermsAndConditionsQueryHandler : IRequestHandler<GetMyTermsAndConditionsQuery, TermsAndConditions>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ITermsAndConditionsService _termsAndConditionsService;

        public GetMyTermsAndConditionsQueryHandler(ICurrentUserService currentUserService, ITermsAndConditionsService termsAndConditionsService)
        {
            _currentUserService = currentUserService;
            _termsAndConditionsService = termsAndConditionsService;
        }

        public async Task<TermsAndConditions> Handle(GetMyTermsAndConditionsQuery request, CancellationToken cancellationToken)
        {
            var username = _currentUserService.UserId;
            return await _termsAndConditionsService.GetTermsAndConditionsFor(username);
        }
    }
}
