using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;
using OfficeEntry.Domain.Enums;

namespace OfficeEntry.Infrastructure.Services
{
    public class EmployeeRoleService : IEmployeeRoleService
    {
        private static IEnumerable<EmployeeRole> EmployeeRoles => new List<EmployeeRole>
        {
            new EmployeeRole
            {
                RoleType = EmployeeRoleType.FloorEmergencyOfficer,
                EnglishName = "Floor Emergency Officer",
                FrenchName = "Agent de secours d’étage"
            },
            new EmployeeRole
            {
                RoleType = EmployeeRoleType.FirstAidAttendant,
                EnglishName = "First Aid Attendant",
                FrenchName = "Secouriste"
            }
        };

        public EmployeeRole GetEmployeeRole(EmployeeRoleType roleType)
        {
            return EmployeeRoles.First(x => x.RoleType == roleType);
        }
    }
}
