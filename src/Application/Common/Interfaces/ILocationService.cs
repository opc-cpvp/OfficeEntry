using OfficeEntry.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OfficeEntry.Application.Common.Interfaces
{
    public interface ILocationService
    {
        public Task<IEnumerable<Building>> GetBuildingsAsync(string locale);

        public Task<IEnumerable<Floor>> GetFloorsByBuildingAsync(Guid buildingId, string locale);

        public Task<int> GetCapacityByFloorAsync(Guid floorId);
    }
}