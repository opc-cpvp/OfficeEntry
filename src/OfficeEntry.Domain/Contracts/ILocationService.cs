using OfficeEntry.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OfficeEntry.Domain.Contracts
{
    public interface ILocationService
    {
        public Task<IEnumerable<Building>> GetBuildingsAsync();
        public Task<IEnumerable<Floor>> GetFloorsByBuildingAsync(Guid buildingId);
    }
}
