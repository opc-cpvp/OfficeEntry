using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OfficeEntry.Application.AccessRequests.Queries.GetSpotsAvailablePerHour
{
    public class CurrentCapacity
    {
        public int Hour { get; set; }
        public int Capacity { get; set; }
        public int SpotsReserved { get; set; }
    }

    public class GetSpotsAvailablePerHourQuery : IRequest<IEnumerable<CurrentCapacity>>
    {
        public Guid FloorId { get; set; }
        public DateTime SelectedDay { get; set; }
    }

    public class GetSpotsAvailablePerHourQueryHandler : IRequestHandler<GetSpotsAvailablePerHourQuery, IEnumerable<CurrentCapacity>>
    {
        private readonly IAccessRequestService _accessRequestService;
        private readonly ILocationService _locationService;

        public GetSpotsAvailablePerHourQueryHandler(IAccessRequestService accessRequestService, ILocationService locationService)
        {
            _accessRequestService = accessRequestService;
            _locationService = locationService;
        }

        public async Task<IEnumerable<CurrentCapacity>> Handle(GetSpotsAvailablePerHourQuery request, CancellationToken cancellationToken)
        {
            var accessRequests = await _accessRequestService.GetApprovedOrPendingAccessRequestsByFloor(request.FloorId, request.SelectedDay);
            var capacity = await _locationService.GetCapacityByFloorAsync(request.FloorId);

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
}
