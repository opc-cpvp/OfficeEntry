using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Application.Common.Models;
using OfficeEntry.Infrastructure.Services.Xrm.Entities;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OfficeEntry.Infrastructure.Services.Xrm
{
    public class ContactsService : XrmService
    {
        public ContactsService(IHttpClientFactory httpClientFactory)
            : base(httpClientFactory)
        {
        }

        internal async Task<(Result Result, contact contact)> GetContact(string username)
        {
            var contacts = await Client.For<contact>()
                .Filter(c => c.gc_username == username)
                //.Filter(c => c.fullname == fullname)
                .Expand(c => c.gc_usersettings)
                .FindEntriesAsync();

            // TODO: Should we replace this with a .Single()?

            if (contacts.Count() == 0)
            {
                return (Result.Failure(new[] { $"No contacts with username '{username}'." }), default(contact));
            }

            if (contacts.Count() > 1)
            {
                return (Result.Failure(new[] { $"More than one contacts with username '{username}'." }), default(contact));
            }

            return (Result.Success(), contacts.First());
        }

        public async Task<(Result Result, Guid UserId)> GetUserId(string username)
        {
            var (result, contact) = await GetContact(username);

            if (!result.Succeeded)
                return (result, default(Guid));

            return (Result.Success(), contact.contactid);
        }
    }
}
