using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OfficeEntry.Application.Locations.Queries.GetFloors
{
    public class GetOfficeReservationsByFloorQuery : IRequest<IDictionary<Office, IEnumerable<AccessRequest>>>
    {
        public Guid FloorId { get; set; }
        public DateTime ReservationDate { get; set; }
        public string Locale { get; set; }
    }

    public class GetOfficeReservationsByFloorQueryHandler : IRequestHandler<GetOfficeReservationsByFloorQuery, IDictionary<Office, IEnumerable<AccessRequest>>>
    {
        private readonly ILocationService _locationService;
        private readonly IAccessRequestService _accessRequestService;

        public GetOfficeReservationsByFloorQueryHandler(ILocationService locationService, IAccessRequestService accessRequestService)
        {
            _locationService = locationService;
            _accessRequestService = accessRequestService;
        }

        public async Task<IDictionary<Office, IEnumerable<AccessRequest>>> Handle(GetOfficeReservationsByFloorQuery request, CancellationToken cancellationToken)
        {
            var offices = await _locationService.GetOfficesByFloorAsync(request.FloorId, request.Locale);
            var accessRequests = await _accessRequestService.GetApprovedOrPendingAccessRequestsByFloor(request.FloorId);

            // Filter the access requests to only those that fall on the same day
            accessRequests =
                accessRequests.Where(a => a.StartTime >= request.ReservationDate && a.EndTime <= request.ReservationDate.AddDays(1));

            var officeReservations = offices.GroupJoin(accessRequests, office => office.Id, accessRequest => accessRequest.Office?.Id,
                (office, accessRequests) => new { Office = office, AccessRequests = accessRequests ?? Enumerable.Empty<AccessRequest>() })
                .ToDictionary(r => r.Office, r => r.AccessRequests);

            return officeReservations;
        }
    }
}