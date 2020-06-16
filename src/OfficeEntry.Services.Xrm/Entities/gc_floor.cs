using System;

namespace OfficeEntry.Services.Xrm.Entities
{
    internal class gc_floor
    {
        public Guid gc_floorid { get; set; }
        public gc_building gc_buildingfloorid { get; set; }
        public int gc_capacity { get; set; }
        public int gc_currentcapacity { get; set; }
        public string gc_englishname { get; set; }
        public string gc_frenchname { get; set; }
    }
}