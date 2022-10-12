namespace OfficeEntry.Domain.Entities
{
    public abstract class EmailTemplate
    {
        public string Language { get; set; } = Enums.Locale.French;
        public virtual string Title { get; }
    }

    public static class EmailTemplates
    {
        public const string CapacityNotification = "CapacityNotificationEmail";
        public const string AccessRequestNotification = "AccessRequestNotificationEmail";
    }
}
