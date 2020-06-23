using OfficeEntry.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace OfficeEntry.Infrastructure.Services.Xrm.Entities
{
    internal class gc_building
    {
        public Guid gc_buildingid { get; set; }
        public string gc_address { get; set; }
        public IList<gc_floor> gc_building_floor { get; set; }
        public string gc_city { get; set; }
        public string gc_englishdescription { get; set; }
        public string gc_englishname { get; set; }
        public string gc_frenchdescription { get; set; }
        public string gc_frenchname { get; set; }
        public string gc_timezone { get; set; }
        public double gc_timezoneoffset { get; set; }
        public int statecode { get; set; }

        public static Building Convert(gc_building building)
        {
            return new Building
            {
                Id = building.gc_buildingid,
                Address = building.gc_address,
                City = building.gc_city,
                Description = building.gc_englishdescription,
                Name = building.gc_englishname,
                Timezone = building.gc_timezone,
                TimezoneOffset = building.gc_timezoneoffset
            };
        }
    }
}
