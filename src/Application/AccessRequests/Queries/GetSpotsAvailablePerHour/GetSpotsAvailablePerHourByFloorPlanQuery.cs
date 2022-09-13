using MediatR;
using OfficeEntry.Application.Common.Interfaces;

namespace OfficeEntry.Application.AccessRequests.Queries.GetSpotsAvailablePerHour;

public class GetSpotsAvailablePerHourByFloorPlanQuery : IRequest<IEnumerable<CurrentCapacity>>
{
    public Guid FloorPlanId { get; set; }
    public DateTime SelectedDay { get; set; }
}

public class GetSpotsAvailablePerHourByFloorPlanQueryHandler : IRequestHandler<GetSpotsAvailablePerHourByFloorPlanQuery, IEnumerable<CurrentCapacity>>
{
    private readonly IAccessRequestService _accessRequestService;
    private readonly IFloorPlanService _floorPlanService;
    private readonly ILocationService _locationService;

    public GetSpotsAvailablePerHourByFloorPlanQueryHandler(IAccessRequestService accessRequestService, IFloorPlanService floorPlanService, ILocationService locationService)
    {
        _accessRequestService = accessRequestService;
        _floorPlanService = floorPlanService;
        _locationService = locationService;
    }

    public async Task<IEnumerable<CurrentCapacity>> Handle(GetSpotsAvailablePerHourByFloorPlanQuery request, CancellationToken cancellationToken)
    {
        var accessRequests = await _accessRequestService.GetApprovedOrPendingAccessRequestsByFloorPlan(request.FloorPlanId, DateOnly.FromDateTime(request.SelectedDay));
        var floorPlan = await _floorPlanService.GetFloorPlanByIdAsync(request.FloorPlanId);
        var capacity = await _locationService.GetCapacityByFloorAsync(floorPlan.Floor.Id);

        return Enumerable
            .Range(0, 24)
            .Select(x => request.SelectedDay.AddHours(x))
            .Select(x => new CurrentCapacity
            {
                Hour = x.Hour,
                Capacity = capacity,
                SpotsReserved = GetSumOfSpotsReservedWithinHour(x)
            });

        int GetSumOfSpotsReservedWithinHour(DateTime hour)
        {
            var sum = accessRequests
                .Where(a => hour >= a.StartTime && hour < a.EndTime)
                .Select(x => x.Visitors.Count + 1)
                .Sum();

            return sum;
        }
    }
}
