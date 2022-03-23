using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Application.Common.Models;
using OfficeEntry.Domain.Entities;
using OfficeEntry.Infrastructure.Services.Xrm.Entities;
using Simple.OData.Client;

namespace OfficeEntry.Infrastructure.Services.Xrm;

public class UserService : IUserService
{
    private readonly IODataClient _client;

    public UserService(IODataClient client)
    {
        _client = client;
    }

    public async Task<(Result Result, Contact Contact)> GetContact(string username)
    {
        var contacts = await _client.For<contact>()
            .Filter(c => c.statecode == (int)StateCode.Active)
            .Filter(c => c.gc_username == username)
            .Expand(c => c.gc_usersettings)
            .FindEntriesAsync();

        // TODO: Should we replace this with a .Single()?

        if (contacts.Count() == 0)
        {
            return (Result.Failure(new[] { $"No contacts with username '{username}'." }), default(Contact));
        }

        if (contacts.Count() > 1)
        {
            return (Result.Failure(new[] { $"More than one contacts with username '{username}'." }), default(Contact));
        }

        return (Result.Success(), contact.Convert(contacts.First()));
    }

    public async Task<(Result Result, IEnumerable<Contact> Contacts)> GetContacts(string excludeUsername)
    {
        var contacts = await _client.For<contact>()
            .Select(c => new { c.contactid, c.firstname, c.lastname })
            .Filter(c => c.statecode == (int)StateCode.Active)
            .Filter(c => c.gc_username != null && c.gc_username != excludeUsername)
            .Filter(c => !(c.gc_username.Contains("scanner") || c.gc_username.Contains("student")))
            .OrderBy(c => c.lastname)                  
            .FindEntriesAsync();

        return (Result.Success(), contacts.Select(c => contact.Convert(c)));
    }

    public async Task<(Result Result, Guid UserId)> GetUserId(string username)
    {
        var (result, contact) = await GetContact(username);

        if (!result.Succeeded)
            return (result, default(Guid));

        return (Result.Success(), contact.Id);
    }
}
