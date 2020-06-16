using OfficeEntry.Domain.Entities;
using System.Collections.Generic;

namespace OfficeEntry.Domain.Contracts
{
    public interface IUserService
    {
        public List<Contact> GetContactsByName(string name);
        public UserSettings GetUserSettings();
    }
}
