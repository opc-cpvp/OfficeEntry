using OfficeEntry.Application.Common.Models;
using OfficeEntry.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OfficeEntry.Application.Common.Interfaces
{
    public interface IUserService
    {
        Task<(Result Result, Contact Contact)> GetContact(string username);

        Task<(Result Result, IEnumerable<Contact> Contacts)> GetContacts(string excludeUsername);

        Task<(Result Result, Guid UserId)> GetUserId(string username);
    }
}