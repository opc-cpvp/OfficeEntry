using MagicOnion;
using MagicOnion.Server;
using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;
using OfficeEntry.Domain.Services;
using System.Collections.Immutable;

namespace OfficeEntry.Application.AccessRequests.Queries.GetAccessRequestPerFloorPlan;

public class GetAccessRequestPerFloorPlanQueryHandler : 
    ServiceBase<IGetAccessRequestPerFloorPlanQueryService>,
    IGetAccessRequestPerFloorPlanQueryService,
    IRequestHandler<GetAccessRequestPerFloorPlanQuery, ImmutableArray<AccessRequest>>
{
    private readonly IAccessRequestService _accessRequestService;

    public GetAccessRequestPerFloorPlanQueryHandler(IAccessRequestService accessRequestService)
    {
        _accessRequestService = accessRequestService;
    }

    public async Task<ImmutableArray<AccessRequest>> Handle(GetAccessRequestPerFloorPlanQuery request, CancellationToken cancellationToken)
    {
        var accessRequests = await _accessRequestService.GetApprovedOrPendingAccessRequestsByFloorPlan(request.FloorPlanId, request.Date);

        return accessRequests.ToImmutableArray();
    }

    public async UnaryResult<AccessRequest[]> HandleAsync(GetAccessRequestPerFloorPlanQuery request)
    {
        return [.. await Handle(request, new CancellationToken())];
    }
}
