using Microsoft.AspNetCore.Mvc;
using OfficeEntry.Application.Locations.Queries.GetBuildings;
using OfficeEntry.Application.Locations.Queries.GetFloors;
using OfficeEntry.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace OfficeEntry.WebUI.Server.Controllers
{
    public class LocationsController : ApiController
    {

        [HttpGet("{locale}")]
        public async Task<IEnumerable<Building>> Get(string locale)
        {
            return await Mediator.Send(new GetBuildingsQuery { Locale = locale });
        }

        [HttpGet("{locale}/{id}")]
        public async Task<IEnumerable<Floor>> Get(Guid id, string locale)
        {
            return await Mediator.Send(new GetFloorsQuery { BuildingId = id, Locale = locale });
        }
    }
}