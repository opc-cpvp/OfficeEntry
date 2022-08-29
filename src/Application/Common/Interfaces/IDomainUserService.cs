using OfficeEntry.Domain.ValueObjects;
using System.Collections.Immutable;

namespace OfficeEntry.Application.Common.Interfaces;

public interface IDomainUserService
{
    Task<string> GetUserNameAsync(AdAccount adAccount);

    ImmutableArray<string> GetUserGroupsFor(AdAccount adAccount);
}
