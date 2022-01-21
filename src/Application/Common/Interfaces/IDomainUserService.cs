using OfficeEntry.Domain.ValueObjects;

namespace OfficeEntry.Application.Common.Interfaces;

public interface IDomainUserService
{
    Task<string> GetUserNameAsync(AdAccount adAccount);
}
