﻿using OfficeEntry.Domain.Entities;
using System.Collections.Immutable;

namespace OfficeEntry.Application.Common.Interfaces;

public interface IFloorPlanService
{
    Task<FloorPlan> GetFloorPlanByIdAsync(Guid floorPlanId);
    Task<FloorPlanCapacity> GetFloorPlanCapacityAsync(Guid floorPlanId, DateOnly date);
    Task<ImmutableArray<FloorPlan>> GetFloorPlansAsync(string keyword);
    Task UpdateFloorPlan(FloorPlan floorPlan);
}