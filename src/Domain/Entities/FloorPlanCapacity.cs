namespace OfficeEntry.Domain.Entities
{
    public class FloorPlanCapacity
    {
        public int CurrentCapacity { get; init; }
        public bool HasCapacity => RemainingCapacity > 0;
        public int RemainingCapacity => MaxCapacity - CurrentCapacity;
        public int MaxCapacity => Math.Min(MaxFirstAidAttendantCapacity, MaxFloorEmergencyOfficerCapacity);
        public int MaxFirstAidAttendantCapacity { get; init; }
        public int MaxFloorEmergencyOfficerCapacity { get; init; }
        public int TotalCapacity { get; init; }
    }
}
