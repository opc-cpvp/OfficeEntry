using OfficeEntry.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System;
using System.DirectoryServices.AccountManagement;
using System.Threading.Tasks;
using OfficeEntry.Domain.Exceptions;
using OfficeEntry.Domain.ValueObjects;

namespace OfficeEntry.Infrastructure.Identity
{
    public class DomainUserService : IDomainUserService
    {
        private readonly string _domainName;

        public DomainUserService(string domainName)
        {
            _domainName = domainName;
        }

        public async Task<string> GetUserNameAsync(AdAccount adAccount)
        {
            return await Task.Run(() => {
                using var AD = new PrincipalContext(ContextType.Domain, _domainName);
                var userIdentity = UserPrincipal.FindByIdentity(AD, adAccount.Name);

                if (userIdentity == null)
                {
                    throw new AdAccountInvalidException(adAccount);
                }

                return userIdentity.DisplayName;
            });
        }
    }
}
