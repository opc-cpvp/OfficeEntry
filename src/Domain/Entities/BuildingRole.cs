namespace OfficeEntry.Domain.Entities
{
    public class BuildingRole
    {
        public OptionSet Role { get; set; }
        public Building Building { get; set; }
        public Floor Floor { get; set; }

        public enum BuildingRoleRole
        {
            FirstAidAttendant = 948160000,
            FloorEmergencyOfficer = 948160001
        }
    }
}
