using System;

namespace OfficeEntry.Domain.Entities
{
    public class Floor
    {
        public Guid Id { get; set; }
        public Guid BuildingFloorId { get; set; }
        public Guid BuildingId { get; set; }
        public int Capacity { get; set; }
        public int CurrentCapacity { get; set; }
        public string Name { get; set; }

        //public Building Building { get; set; }
    }
}
