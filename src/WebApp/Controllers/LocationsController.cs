using Microsoft.AspNetCore.Mvc;
using OfficeEntry.Application.Locations.Queries.GetBuildings;
using OfficeEntry.Application.Locations.Queries.GetFloors;
using OfficeEntry.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OfficeEntry.WebApp.Controllers
{
    public class LocationsController : ApiController
    {
        private const int DayInSeconds = 86_400;

        [HttpGet("{locale}")]
        [ResponseCache(VaryByQueryKeys = new string[] { "locale" }, Duration = DayInSeconds)]
        public async Task<IEnumerable<Building>> Get(string locale)
        {
            return await Mediator.Send(new GetBuildingsQuery { Locale = locale });
        }

        [HttpGet("{locale}/{id}")]
        [ResponseCache(VaryByQueryKeys = new string[] { "id", "locale" }, Duration = DayInSeconds)]
        public async Task<IEnumerable<Floor>> Get(Guid id, string locale)
        {
            return await Mediator.Send(new GetFloorsQuery { BuildingId = id, Locale = locale });
        }
    }
}