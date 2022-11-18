using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;

namespace OfficeEntry.Application.User.Queries.GetMyTermsAndConditions;

public record GetTermsAndConditionsQuery : IRequest<TermsAndConditions>
{
    public static readonly GetTermsAndConditionsQuery Instance = new();

    private GetTermsAndConditionsQuery()
    {
    }
}

public class GetTermsAndConditionsQueryHandler : IRequestHandler<GetTermsAndConditionsQuery, TermsAndConditions>
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
}
