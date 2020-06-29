using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace OfficeEntry.Application.TermsAndConditions.Queries.GetHealthAndSafetyMeasuresRequests
{
    public class GetHealthAndSafetyMeasuresForCurrentUserQuery : IRequest<bool>
    {
    }

    public class GetHealthAndSafetyMeasuresForCurrentUserQueryHandler : IRequestHandler<GetHealthAndSafetyMeasuresForCurrentUserQuery, bool>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ITermsAndConditionsService _termsAndConditionsService;

        public GetHealthAndSafetyMeasuresForCurrentUserQueryHandler(ICurrentUserService currentUserService, ITermsAndConditionsService termsAndConditionsService)
        {
            _currentUserService = currentUserService;
            _termsAndConditionsService = termsAndConditionsService;
        }

        public async Task<bool> Handle(GetHealthAndSafetyMeasuresForCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var username = _currentUserService.UserId;
            var result = await _termsAndConditionsService.GetHealthAndSafetyMeasuresFor(username);

            if (!result.Result.Succeeded)
            {
            }

            return result.IsHealthAndSafetyMeasuresAccepted;
        }
    }
}