using OfficeEntry.Domain.Entities;
using System;
using System.Collections.Generic;

namespace OfficeEntry.Domain.Contracts
{
    public interface ILocationService
    {
        public List<Building> GetBuildings();
        public List<Floor> GetFloorsByBuilding(Guid buildingId);
    }
}
