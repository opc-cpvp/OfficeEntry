using OfficeEntry.Domain.Entities;

namespace OfficeEntry.Infrastructure.Services.Xrm.Entities;

internal class contact
{
    public Guid contactid { get; set; }
    public string firstname { get; set; }
    public string lastname { get; set; }
    public string emailaddress1 { get; set; }
    public string gc_username { get; set; }
    public gc_usersettingses gc_usersettings { get; set; }
    public string telephone1 { get; set; }
    public int statecode { get; set; }

    public static Contact Convert(contact contact)
    {
        if (contact is null)
            return null;

        return new Contact
        {
            Id = contact.contactid,
            FirstName = contact.firstname,
            LastName = contact.lastname,
            EmailAddress = contact.emailaddress1,
            PhoneNumber = contact.telephone1,
            Username = contact.gc_username,
            UserSettings = gc_usersettingses.Convert(contact.gc_usersettings)
        };
    }

    public static contact MapFrom(Contact contact)
    {
        if (contact is null)
            return null;

        return new contact
        {
            contactid = contact.Id,
            firstname = contact.FirstName,
            lastname = contact.LastName,
            emailaddress1 = contact.EmailAddress?.ToLower(),
            telephone1 = contact.PhoneNumber,
            gc_username = contact.Username
        };
    }
}
