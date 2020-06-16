using OfficeEntry.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OfficeEntry.Domain.Contracts
{
    public interface IUserService
    {
        public Task<IEnumerable<Contact>> GetContactsByNameAsync(string name);
        public Task<UserSettings> GetUserSettingsAsync();
    }
}
