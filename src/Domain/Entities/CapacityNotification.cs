namespace OfficeEntry.Domain.Entities
{
    public class CapacityNotification : EmailTemplate
    {
        public int Capacity { get; set; }
        public DateTime Date { get; set; }
        public string RoleEnglishName { get; set; }
        public string RoleFrenchName { get; set; }
        public Building Building { get; set; }
        public Floor Floor { get; set; }
        public NotificationType Type { get; init; }
        public override string Title => GetTitle(Type);

        public CapacityNotification(NotificationType notificationType)
        {
            Type = notificationType;
        }
        public enum NotificationType
        {
            Available = 1,
            Maximum = 2
        }

        private string GetTitle(NotificationType type)
        => type switch
        {
            NotificationType.Available => $"Entrée au bureau - Un {RoleFrenchName} n'est plus requis / Office Entry - A {RoleEnglishName} is no longer required",
            NotificationType.Maximum => "Entrée au bureau - La capacité maximale a été atteinte ou dépassée / Office Entry - Capacity has been reached or exceeded",
            _ => null
        };
    }
}
