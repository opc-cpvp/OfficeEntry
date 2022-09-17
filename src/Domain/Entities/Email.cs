namespace OfficeEntry.Domain.Entities
{
    public class Email
    {
        public SystemUser From { get; set; }
        public IList<Contact> To { get; set; }
        public IList<string> Cc { get; set; } = new List<string>();
        public string Subject { get; set; }
        public string Description { get; set; }
    }
}
