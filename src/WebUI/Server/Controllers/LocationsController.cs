using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Application.Locations.Queries.GetBuildings;
using OfficeEntry.Application.Locations.Queries.GetFloors;
using OfficeEntry.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OfficeEntry.WebUI.Server.Controllers
{
    public class LocationsController : ApiController
    {
        private readonly ILocationService _locationService;

        public LocationsController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet]
        public async Task<IEnumerable<Building>> Get(CancellationToken cancellationToken)
        {
            //return await Mediator.Send(new GetBuildingsQuery());

            var handler = new GetBuildingsQueryHandler(_locationService);
            return await handler.Handle(new GetBuildingsQuery(), cancellationToken);
        }

        // GET api/<LocationController>/5
        [HttpGet("{id}")]
        public async Task<IEnumerable<Floor>> Get(Guid id, CancellationToken cancellationToken)
        {
            //return await Mediator.Send(new GetFloorsQuery { BuildingId = id });

            var handler = new GetFloorsQueryHandler(_locationService);
            return await handler.Handle(new GetFloorsQuery { BuildingId = id }, cancellationToken);
        }
    }
}
