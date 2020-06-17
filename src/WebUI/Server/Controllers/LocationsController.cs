using OfficeEntry.Application.Locations.Queries.GetBuildings;
using OfficeEntry.Application.Locations.Queries.GetFloors;
using OfficeEntry.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OfficeEntry.WebUI.Server.Controllers
{
    public class LocationsController : ApiController
    {
        [HttpGet]
        public async Task<IEnumerable<Building>> Get()
        {
            return await Mediator.Send(new GetBuildingsQuery());
        }

        // GET api/<LocationController>/5
        [HttpGet("{id}")]
        public async Task<IEnumerable<Floor>> Get(Guid id)
        {
            return await Mediator.Send(new GetFloorsQuery { BuildingId = id });
        }
    }
}
