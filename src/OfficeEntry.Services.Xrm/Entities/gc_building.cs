using System;
using System.Collections.Generic;
using System.Text;

namespace OfficeEntry.Services.Xrm.Entities
{
    class gc_building
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
}
