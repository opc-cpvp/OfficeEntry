using MagicOnion;
using MagicOnion.Server;
using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;
using OfficeEntry.Domain.Services;

namespace OfficeEntry.Application.User.Queries.GetMyTermsAndConditions;

public class GetTermsAndConditionsQueryHandler
    : ServiceBase<IGetTermsAndConditionsQueryService>,
    IGetTermsAndConditionsQueryService,
    IRequestHandler<GetTermsAndConditionsQuery, TermsAndConditions>

{
    private readonly ICurrentUserService _currentUserService;
    private readonly ITermsAndConditionsService _termsAndConditionsService;

    public GetTermsAndConditionsQueryHandler(ICurrentUserService currentUserService, ITermsAndConditionsService termsAndConditionsService)
    {
        _currentUserService = currentUserService;
        _termsAndConditionsService = termsAndConditionsService;
    }

    public async Task<TermsAndConditions> Handle(GetTermsAndConditionsQuery request, CancellationToken cancellationToken)
    {
        var username = _currentUserService.UserId;
        return await _termsAndConditionsService.GetTermsAndConditionsFor(username);
    }

    public async UnaryResult<TermsAndConditions> HandleAsync(GetTermsAndConditionsQuery request)
    {
        return await Handle(request, new CancellationToken());
    }
}
