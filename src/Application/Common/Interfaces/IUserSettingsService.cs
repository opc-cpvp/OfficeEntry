using OfficeEntry.Domain.Entities;
using OfficeEntry.Domain.ValueObjects;

namespace OfficeEntry.Application.Common.Interfaces
{
    public interface IUserSettingsService
    {
        Task<(Result Result, UserSettings UserSettings)> GetUserSettingsFor(Guid contactId);
    }
}
