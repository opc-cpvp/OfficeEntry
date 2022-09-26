namespace OfficeEntry.Domain.Entities
{
    public class CapacityNotification : EmailTemplate
    {
        public override string Title => "Entrée au bureau - [FR] Capacity has been reached or exceeded / Office Entry - Capacity has been reached or exceeded";
        public int Capacity { get; set; }
        public DateTime Date { get; set; }
        public string RoleEnglishName { get; set; }
        public string RoleFrenchName { get; set; }
        public Building Building { get; set; }
        public Floor Floor { get; set; }
    }
}
