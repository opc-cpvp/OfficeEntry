using MemoryPack;

namespace OfficeEntry.Domain.Entities;

[MemoryPackable]
public partial class Contact
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName => string.Join(" ", FirstName, LastName);
    public string EmailAddress { get; set; }
    public string PhoneNumber { get; set; }
    public string Username { get; set; }
    public UserSettings UserSettings { get; set; }
}
