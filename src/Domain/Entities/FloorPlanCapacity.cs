namespace OfficeEntry.Domain.Entities
{
    public class FloorPlanCapacity
    {
        // Represents the total capacity of approved requests for a given floor plan
        public int CurrentCapacity { get; init; }

        // Flag to indicate if the floor plan has reached its maximum capacity
        public bool HasCapacity => RemainingCapacity > 0;

        // Flag to indicate if the floor plan needs a First Aid Attendant
        public bool NeedsFirstAidAttendant => CurrentCapacity >= MaxFirstAidAttendantCapacity;

        // Flag to indicate if the floor plan needs a Floor Emergency Officer
        public bool NeedsFloorEmergencyOfficer => CurrentCapacity >= MaxFloorEmergencyOfficerCapacity;

        // Represents the remaining capacity for a given floor plan
        public int RemainingCapacity => MaxCapacity - CurrentCapacity;

        // Represents the maximum capacity for a given floor plan
        public int MaxCapacity => Math.Min(MaxFirstAidAttendantCapacity, MaxFloorEmergencyOfficerCapacity);

        // Represents the maximum capacity for a given floor plan based on the number of First Aid Attendants
        public int MaxFirstAidAttendantCapacity { get; init; }

        // Represents the maximum capacity for a given floor plan based on the number of Floor Emergency Officers
        public int MaxFloorEmergencyOfficerCapacity { get; init; }

        // Represents the total capacity of approved and pending requests for a given floor plan
        public int TotalCapacity { get; init; }
    }
}
