using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OfficeEntry.Application.Locations.Queries.GetFloors
{
    public class GetFloorsQuery : IRequest<IEnumerable<Floor>>
    {
        public Guid BuildingId { get; set; }
        public string Locale { get; set; }
    }

    public class GetFloorsQueryHandler : IRequestHandler<GetFloorsQuery, IEnumerable<Floor>>
    {
        private readonly ILocationService _locationService;

        public GetFloorsQueryHandler(ILocationService locationService)
        {
            _locationService = locationService;
        }

        public Task<IEnumerable<Floor>> Handle(GetFloorsQuery request, CancellationToken cancellationToken)
        {
            return _locationService.GetFloorsByBuildingAsync(request.BuildingId, request.Locale);
        }
    }
}