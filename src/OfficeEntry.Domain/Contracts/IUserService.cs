using OfficeEntry.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OfficeEntry.Domain.Contracts
{
    public interface IUserService
    {
        public Task<IEnumerable<Contact>> GetContactsAsync();
        public Task<UserSettings> GetUserSettingsAsync(string username);
    }
}
