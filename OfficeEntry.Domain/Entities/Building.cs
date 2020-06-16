using System;

namespace OfficeEntry.Domain.Entities
{
    public class Building
    {
        public Guid ID { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
    }
}
