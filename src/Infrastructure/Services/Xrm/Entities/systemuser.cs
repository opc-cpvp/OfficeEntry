using OfficeEntry.Domain.Entities;

namespace OfficeEntry.Infrastructure.Services.Xrm.Entities;

internal class systemuser
{
    public Guid systemuserid { get; set; }
    public string domainname { get; set; }
    public bool isdisabled { get; set; }

    public static SystemUser Convert(systemuser systemUser)
    {
        if (systemUser is null)
            return null;

        return new SystemUser
        {
            Id = systemUser.systemuserid,
            DomainName = systemUser.domainname
        };
    }
}
