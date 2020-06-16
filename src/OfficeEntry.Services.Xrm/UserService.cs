using OfficeEntry.Domain.Contracts;
using OfficeEntry.Domain.Entities;
using OfficeEntry.Services.Xrm.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OfficeEntry.Services.Xrm
{
    public class UserService : XrmService, IUserService
    {
        public UserService(string odataUrl) :
            base(odataUrl)
        {
        }

        public async Task<IEnumerable<Contact>> GetContactsAsync()
        {
            var client = GetODataClient();
            var contacts = await client.For<contact>().FindEntriesAsync();

            return contacts.Select(c => new Contact
            {
                ID = c.contactid,
                FirstName = c.firstname,
                LastName = c.lastname,
                EmailAddress = c.emailaddress1,
                PhoneNumber = c.telephone1,
                Username = c.gc_username
            });
        }

        public async Task<UserSettings> GetUserSettingsAsync(string username)
        {
            var client = GetODataClient();
            var contact = await client.For<contact>()
                .Filter(c => c.gc_username == username)
                .Expand(c => c.gc_usersettings)
                .FindEntryAsync();

            if (contact.gc_usersettings is null)
                return new UserSettings();

            return new UserSettings
            {
                HealthSafety = contact.gc_usersettings.gc_healthsafety,
                PrivacyStatement = contact.gc_usersettings.gc_privacystatement
            };
        }
    }
}