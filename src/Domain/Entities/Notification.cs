namespace OfficeEntry.Domain.Entities
{
    public class Notification : EmailTemplate
    {
        public int Capacity { get; set; }
        public string RoleEnglishName { get; set; }
        public string RoleFrenchName { get; set; }

        public Building Building { get; set; }
        public Floor Floor { get; set; }
    }
}
