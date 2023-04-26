using OfficeEntry.Domain.Entities;
using OfficeEntry.Domain.Enums;

namespace OfficeEntry.Application.Common.Interfaces
{
    public interface IEmployeeRoleService
    {
        EmployeeRole GetEmployeeRole(EmployeeRoleType roleType);
    }
}
