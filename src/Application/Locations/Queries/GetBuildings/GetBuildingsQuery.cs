using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OfficeEntry.Application.Locations.Queries.GetBuildings
{
    public class GetBuildingsQuery : IRequest<IEnumerable<Building>>
    {
    }

    public class GetBuildingsQueryHandler : IRequestHandler<GetBuildingsQuery, IEnumerable<Building>>
    {
        private readonly ILocationService _locationService;

        public GetBuildingsQueryHandler(ILocationService locationService)
        {
            _locationService = locationService;
        }

        public Task<IEnumerable<Building>> Handle(GetBuildingsQuery getBuildingsQuery, object cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Building>> Handle(GetBuildingsQuery request, CancellationToken cancellationToken)
        {
            return _locationService.GetBuildingsAsync();
        }
    }
}
