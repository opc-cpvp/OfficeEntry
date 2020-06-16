using System;

namespace OfficeEntry.Services.Xrm.Entities
{
    internal class gc_accessrequest
    {
        public Guid gc_accessrequestid { get; set; }
        public gc_building gc_building { get; set; }
        public string gc_details { get; set; }
        public DateTime gc_endtime { get; set; }
        public gc_floor gc_floor { get; set; }
        public contact contact { get; set; }
        public contact gc_manager { get; set; }
        public DateTime gc_starttime { get; set; }
    }
}