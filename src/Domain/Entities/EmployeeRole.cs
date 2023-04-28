using OfficeEntry.Domain.Enums;

namespace OfficeEntry.Domain.Entities
{
    public class EmployeeRole
    {
        public EmployeeRoleType RoleType { get; set; }
        public string FrenchName { get; set; }
        public string EnglishName { get; set; }
    }
}
