using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;
using System.Collections.Immutable;

namespace OfficeEntry.Application.AccessRequests.Queries.GetAccessRquestPerFloorPlan;

public class GetAccessRequestPerFloorPlanQuery : IRequest<ImmutableArray<AccessRequest>>
{
    public Guid FloorPlanId { get; init; }
    public DateOnly Date { get; init; }
}

public class GetAccessRequestPerFloorPlanQueryHandler : IRequestHandler<GetAccessRequestPerFloorPlanQuery, ImmutableArray<AccessRequest>>
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
}
