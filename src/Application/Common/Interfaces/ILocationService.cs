using OfficeEntry.Domain.Entities;
using System.Collections.Immutable;

namespace OfficeEntry.Application.Common.Interfaces;

public interface ILocationService
{
    public Task<IEnumerable<Building>> GetBuildingsAsync(string locale);
    public Task<int> GetCapacityByFloorAsync(Guid floorId);
    Task<FloorPlanCapacity> GetCapacityByFloorPlanAsync(Guid floorPlanId, DateOnly date);
    Task<FloorPlan> GetFloorPlanAsync(Guid floorPlanId);
    Task<ImmutableArray<FloorPlan>> GetFloorPlansAsync(string keyword);
    public Task<IEnumerable<Floor>> GetFloorsByBuildingAsync(Guid buildingId, string locale);
    Task<Workspace> GetWorkspaceAsync(Guid workspaceId);
    Task UpdateFloorPlan(FloorPlan floorPlan);
}
