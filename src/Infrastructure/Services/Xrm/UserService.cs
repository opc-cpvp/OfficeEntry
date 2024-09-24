using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Application.Common.Models;
using OfficeEntry.Domain.Entities;
using OfficeEntry.Infrastructure.Services.Xrm.Entities;
using Simple.OData.Client;
using System.Collections.Concurrent;

namespace OfficeEntry.Infrastructure.Services.Xrm;

public class UserService : IUserService
{
    private static readonly ConcurrentDictionary<string, Guid> _cachedUserIds = new();
    private static readonly ConcurrentDictionary<string, Guid> _cachedSystemUserIds = new();
    private readonly IODataClient _client;

    public UserService(IODataClient client)
    {
        _client = client;
    }

    public async Task<(Result Result, Contact Contact)> GetContact(Guid contactId)
    {
        var contact = await _client.For<contact>()
            .Key(contactId)
            .Select(c => new
            {
                c.contactid,
                c.firstname,
                c.lastname,
                c.gc_usersettings,
                c.gc_username
            })
            .Expand(c => c.gc_usersettings)
            .FindEntryAsync();

        var map = contact.Convert(contact);

        return (Result.Success(), map);
    }

    public async Task<(Result Result, Contact Contact)> GetContactByUsername(string username)
    {
        var (_, contactId) = await GetUserId(username);
        return await GetContact(contactId);
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

        return (Result.Success(), contacts.Select(contact.Convert));
    }

    public async Task<(Result Result, SystemUser SystemUser)> GetSystemUser(Guid systemUserId)
    {
        var systemUser = await _client.For<systemuser>()
            .Key(systemUserId)
            .Select(s => new
            {
                s.systemuserid,
                s.domainname
            })
            .FindEntryAsync();

        var map = systemuser.Convert(systemUser);

        return (Result.Success(), map);
    }

    public async Task<(Result Result, SystemUser SystemUser)> GetSystemUserByUsername(string username)
    {
        var (_, systemUserId) = await GetSystemUserId(username);
        return await GetSystemUser(systemUserId);
    }

    public async Task<(Result Result, Guid SystemUserId)> GetSystemUserId(string username)
    {
        if (_cachedSystemUserIds.ContainsKey(username))
        {
            return (Result.Success(), _cachedSystemUserIds[username]);
        }

        var systemUsers = await _client.For<systemuser>()
            .Select(s => s.systemuserid)
            .Filter(s => s.isdisabled == false)
            .Filter(s => s.domainname == username)
            .FindEntriesAsync();

        // TODO: Should we replace this with a .Single()?

        if (!systemUsers.Any())
        {
            throw new Exception($"No system users with username '{username}'.");
        }

        if (systemUsers.Count() > 1)
        {
            throw new Exception($"More than one system users with username '{username}'.");
        }

        _cachedSystemUserIds[username] = systemUsers.First().systemuserid;

        return (Result.Success(), _cachedSystemUserIds[username]);
    }

    public async Task<(Result Result, Guid UserId)> GetUserId(string username)
    {
        if (_cachedUserIds.ContainsKey(username))
        {
            return (Result.Success(), _cachedUserIds[username]);
        }

        var contacts = await _client.For<contact>()
            .Select(c => c.contactid)
            .Filter(c => c.statecode == (int)StateCode.Active)
            .Filter(c => c.gc_username == username)
            .FindEntriesAsync();

        // TODO: Should we replace this with a .Single()?
        if (!contacts.Any())
        {
            throw new Exception($"No contacts with username '{username}'.");
        }

        if (contacts.Count() > 1)
        {
            throw new Exception($"More than one contacts with username '{username}'.");
        }

        var contactId = contacts.First().contactid;
        if (contactId == Guid.Empty)
        {
            // Dynamics sometimes returns an empty GUID
            throw new Exception($"An error occurred when retrieving contactid for '{username}'.");
        }

        _cachedUserIds[username] = contactId;
        return (Result.Success(), _cachedUserIds[username]);
    }

    public async Task<(Result Result, bool isFirstResponder)> IsContactFirstResponder(string username)
    {
        throw new NotImplementedException();
    }
}
