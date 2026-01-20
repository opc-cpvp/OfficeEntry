using OfficeEntry.Domain.Entities;
using OfficeEntry.Domain.Enums;
using System.Collections.Immutable;

namespace OfficeEntry.Application.Common.Interfaces;

public interface ILocationService
{
    public Task<IEnumerable<Building>> GetBuildingsAsync(string locale);
    Task<FloorPlanCapacity> GetCapacityByFloorPlanAsync(Guid floorPlanId, DateOnly date);
    Task<FloorPlan> GetFloorPlanAsync(Guid floorPlanId);
    Task<ImmutableArray<FloorPlan>> GetFloorPlansAsync(string keyword);
    Task<Workspace> GetWorkspaceAsync(Guid workspaceId);
    Task<ImmutableArray<Workspace>> GetWorkspacesAsync(Guid floorPlanId);
    Task UpdateFloorPlan(FloorPlan floorPlan);
    Task<IEnumerable<Contact>> GetFirstAidAttendantsAsync(Guid buildingId);
    Task<IEnumerable<Contact>> GetFloorEmergencyOfficersAsync(Guid buildingId);
    Task<IEnumerable<Contact>> GetMentalHealthTrainingAsync(Guid buildingId);
    Task<IEnumerable<Contact>> GetContactsForBuildingByRole(Guid buildingId, EmployeeRoleType roleType);
}
