using System;

namespace OfficeEntry.Domain.Entities
{
    public class AccessRequest
    {
        public Guid Id { get; set; }
        public Building Building { get; set; }
        public string Details { get; set; }
        public DateTime EndTime { get; set; }
        public Floor Floor { get; set; }
        public Contact Manager { get; set; }
        public OptionSet Reason { get; set; }
        public DateTime StartTime { get; set; }
        public OptionSet Status { get; set; }
    }
}
