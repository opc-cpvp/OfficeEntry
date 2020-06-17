using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace OfficeEntry.Infrastructure.Services
{

    public class LocationService : XrmService, ILocationService
    {
        public LocationService(IHttpClientFactory httpClientFactory)
            : base(httpClientFactory)
        {
        }

        public async Task<IEnumerable<Building>> GetBuildingsAsync()
        {
            var buildings = await Client.For<gc_building>().FindEntriesAsync();

            return buildings.Select(b => new Building
            {
                Id = b.gc_buildingid,
                Address = b.gc_address,
                City = b.gc_city,
                Description = b.gc_englishdescription,
                Name = b.gc_englishname,
                Timezone = b.gc_timezone,
                TimezoneOffset = b.gc_timezoneoffset
            });
        }

        public async Task<IEnumerable<Floor>> GetFloorsByBuildingAsync(Guid buildingId)
        {
            var floors = await Client.For<gc_building>()
                .Key(buildingId)
                .NavigateTo(b => b.gc_building_floor)
                .FindEntriesAsync();

            return floors.Select(f => new Floor
            {
                FloorId = f.gc_floorid,
                BuildingId = buildingId,
                Capacity = f.gc_capacity,
                CurrentCapacity = f.gc_currentcapacity,
                Name = f.gc_englishname
            });
        }

        private protected class gc_building
        {
            public Guid gc_buildingid { get; set; }
            public string gc_address { get; set; }
            public string gc_city { get; set; }
            public string gc_englishdescription { get; set; }
            public string gc_englishname { get; set; }
            public string gc_frenchdescription { get; set; }
            public string gc_frenchname { get; set; }
            public string gc_timezone { get; set; }
            public double gc_timezoneoffset { get; set; }

            public IList<gc_floor> gc_building_floor { get; set; }
        }

        private protected class gc_floor
        {
            public Guid gc_floorid { get; set; }
            public gc_building gc_buildingfloorid { get; set; }
            public int gc_capacity { get; set; }
            public int gc_currentcapacity { get; set; }
            public string gc_englishname { get; set; }
            public string gc_frenchname { get; set; }
        }
    }
}
