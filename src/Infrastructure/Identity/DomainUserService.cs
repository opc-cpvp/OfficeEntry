using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Exceptions;
using OfficeEntry.Domain.ValueObjects;
using System.Collections.Immutable;
using System.DirectoryServices.AccountManagement;
using System.Runtime.Versioning;

namespace OfficeEntry.Infrastructure.Identity;

public class DomainUserService : IDomainUserService
{
    private readonly string _domainName;

    public DomainUserService(string domainName)
    {
        _domainName = domainName;
    }

    [SupportedOSPlatform("windows")]
    public async Task<string> GetUserNameAsync(AdAccount adAccount)
    {
        return await Task.Run(() =>
        {
            using var AD = new PrincipalContext(ContextType.Domain, _domainName);
            var userIdentity = UserPrincipal.FindByIdentity(AD, IdentityType.SamAccountName, adAccount.Name);

            if (userIdentity is null)
            {
                throw new AdAccountInvalidException(adAccount);
            }

            return userIdentity.DisplayName;
        });
    }

    [SupportedOSPlatform("windows")]
    public ImmutableArray<string> GetUserGroupsFor(AdAccount adAccount)
    {
        using var AD = new PrincipalContext(ContextType.Domain, _domainName);
        var userIdentity = UserPrincipal.FindByIdentity(AD, IdentityType.SamAccountName, adAccount.Name);

        if (userIdentity is null)
        {
            throw new AdAccountInvalidException(adAccount);
        }

        var groups = userIdentity.GetGroups().ToArray();

        return groups.Select(x => x.Name).ToImmutableArray();
    }
}
