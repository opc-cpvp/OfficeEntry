using System;

namespace OfficeEntry.Domain.Entities
{
    public class Floor
    {
        public Guid BuildingId { get; set; }
        public int Capacity { get; set; }
        public int CurrentCapacity { get; set; }
        public string Name { get; set; }
    }
}
