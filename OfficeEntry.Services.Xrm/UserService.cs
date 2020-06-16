using Microsoft.Extensions.Configuration;
using OfficeEntry.Domain.Contracts;
using OfficeEntry.Domain.Entities;
using System;
using System.Collections.Generic;

namespace OfficeEntry.Services.Xrm
{
    public class UserService : XrmService, IUserService
    {

        public UserService (IConfiguration configuration) :
            base(configuration)
        {
        }

        public List<Contact> GetContactsByName(string name)
        {
            throw new NotImplementedException();
        }

        public UserSettings GetUserSettings()
        {
            throw new NotImplementedException();
        }
    }
}
