using MemoryPack;
using OfficeEntry.Domain.Enums;

namespace OfficeEntry.Domain.Entities
{
    [MemoryPackable]
    public partial class EmployeeRole
    {
        public EmployeeRoleType RoleType { get; set; }
        public string FrenchName { get; set; }
        public string EnglishName { get; set; }
    }
}
