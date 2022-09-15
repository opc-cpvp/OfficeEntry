namespace OfficeEntry.Domain.Entities
{
    public class BuildingRole
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public OptionSet Role { get; set; }
        public Building Building { get; set; }
        public Floor Floor { get; set; }

        public enum BuildingRoles
        {
            FirstAidAttendant = 948160000,
            FloorEmergencyOfficer = 948160001
        }
    }
}
