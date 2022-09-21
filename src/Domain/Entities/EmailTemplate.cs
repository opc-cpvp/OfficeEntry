namespace OfficeEntry.Domain.Entities
{
    public class EmailTemplate
    {
        public string Language { get; set; } = "fr";
        public string Title { get; set; }
    }

    public static class EmailTemplates
    {
        public const string Notification = "NotificationEmail";
    }
}
