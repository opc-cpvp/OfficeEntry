using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;
using OfficeEntry.Infrastructure.Services.Xrm.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace OfficeEntry.Infrastructure.Services.Xrm
{

    public class LocationService : XrmService, ILocationService
    {
        public LocationService(IHttpClientFactory httpClientFactory)
            : base(httpClientFactory)
        {
        }

        public async Task<IEnumerable<Building>> GetBuildingsAsync()
        {
            var buildings = await Client.For<gc_building>()
                .Filter(a => a.statecode == (int)StateCode.Active)
                .FindEntriesAsync();

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
                Id = f.gc_floorid,
                BuildingId = buildingId,
                Capacity = f.gc_capacity,
                CurrentCapacity = f.gc_currentcapacity,
                Name = f.gc_englishname
            });
        }
    }
}
