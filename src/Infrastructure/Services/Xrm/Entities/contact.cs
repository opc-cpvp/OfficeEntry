using OfficeEntry.Domain.Entities;
using System;

namespace OfficeEntry.Infrastructure.Services.Xrm.Entities
{
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

        public static contact MapFrom(Contact x)
        {
            return new contact
            {
                firstname = x.FirstName,
                lastname = x.LastName,
                emailaddress1 = x.EmailAddress.ToLower(),
                telephone1 = x.PhoneNumber
            };
        }
    }
}