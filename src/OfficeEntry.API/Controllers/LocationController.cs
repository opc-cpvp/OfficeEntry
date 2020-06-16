using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OfficeEntry.Domain.Contracts;
using OfficeEntry.Domain.Entities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OfficeEntry.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        // GET: api/<LocationController>
        [HttpGet("/api/locations")]
        public Task<IEnumerable<Building>> Get()
        {
            return _locationService.GetBuildingsAsync();
        }

        // GET api/<LocationController>/5
        [HttpGet("{id}")]
        public Task<IEnumerable<Floor>> Get(Guid id)
        {
            return _locationService.GetFloorsByBuildingAsync(id);
        }
    }
}
