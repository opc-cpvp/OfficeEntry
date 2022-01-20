using OfficeEntry.Domain.Entities;

namespace OfficeEntry.Application.Common.Interfaces
{
    public interface ILocationService
    {
        public Task<IEnumerable<Building>> GetBuildingsAsync(string locale);

        public Task<IEnumerable<Floor>> GetFloorsByBuildingAsync(Guid buildingId, string locale);

        public Task<int> GetCapacityByFloorAsync(Guid floorId);
    }
}