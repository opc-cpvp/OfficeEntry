using OfficeEntry.Application.Common.Models;
using OfficeEntry.Domain.Entities;

namespace OfficeEntry.Application.Common.Interfaces;

public interface IUserService
{
    Task<(Result Result, Contact Contact)> GetContact(Guid contactId);
    Task<(Result Result, Contact Contact)> GetContactByUsername(string username);

    Task<(Result Result, IEnumerable<Contact> Contacts)> GetContacts(string excludeUsername);
    Task<(Result Result, SystemUser SystemUser)> GetSystemUser(Guid systemUserId);
    Task<(Result Result, SystemUser SystemUser)> GetSystemUserByUsername(string username);
    Task<(Result Result, Guid SystemUserId)> GetSystemUserId(string username);

    Task<(Result Result, Guid UserId)> GetUserId(string username);

    Task<(Result Result, bool isFirstResponder)> IsContactFirstResponder(string username);
    Task<(Result Result, bool isFirstResponder)> IsContactFirstResponder(Guid userId);
}
