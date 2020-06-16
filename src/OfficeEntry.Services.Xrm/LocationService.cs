using Microsoft.Extensions.Configuration;
using OfficeEntry.Domain.Contracts;
using OfficeEntry.Domain.Entities;
using OfficeEntry.Services.Xrm.Entities;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OfficeEntry.Services.Xrm
{
    public class LocationService : XrmService, ILocationService
    {
        public LocationService(string odataUrl) :
            base(odataUrl)
        {
        }

        public async Task<IEnumerable<Building>> GetBuildingsAsync()
        {
            var client = GetODataClient();
            var buildings = await client.For<gc_building>().FindEntriesAsync();

            return buildings.Select(b => new Building
            {
                ID = b.gc_buildingid,
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
            throw new NotImplementedException();
        }
    }
}
