using OfficeEntry.Application.Common.Models;
using OfficeEntry.Domain.Entities;

namespace OfficeEntry.Application.Common.Interfaces
{
    public interface IUserSettingsService
    {
        Task<(Result Result, UserSettings UserSettings)> GetUserSettingsFor(Guid contactId);
    }
}
