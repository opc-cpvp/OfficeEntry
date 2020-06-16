using Microsoft.Extensions.Configuration;
using OfficeEntry.Domain.Contracts;
using OfficeEntry.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OfficeEntry.Services.Xrm
{
    public class UserService : XrmService, IUserService
    {

        public UserService(string odataUrl) :
            base(odataUrl)
        {
        }

        public async Task<IEnumerable<Contact>> GetContactsByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<UserSettings> GetUserSettingsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
