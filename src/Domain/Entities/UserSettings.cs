namespace OfficeEntry.Domain.Entities
{
    public class UserSettings
    {
        public Guid Id { get; set; }
        public DateTime? HealthSafety { get; set; }
        public DateTime? PrivacyStatement { get; set; }
    }
}