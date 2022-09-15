using OfficeEntry.Domain.Entities;

namespace OfficeEntry.Infrastructure.Services.Xrm.Entities
{
    internal class gc_buildingrole
    {
        public string gc_name { get; set; }
        public Guid gc_buildingroleid { get; set; }
        public BuildingRoles gc_role { get; set; }
        public gc_building gc_building { get; set; }
        public gc_floor gc_floor { get; set; }
        public int statecode { get; set; }

        public static BuildingRole Convert(gc_buildingrole buildingRole)
        {
            if (buildingRole is null)
                return null;

            return new BuildingRole
            {
                Id = buildingRole.gc_buildingroleid,
                Role = new OptionSet
                {
                    Key = (int)buildingRole.gc_role,
                    Value = Enum.GetName(typeof(BuildingRoles), buildingRole.gc_role)
                },
                Building = gc_building.Convert(buildingRole.gc_building),
                Floor = gc_floor.Convert(buildingRole.gc_floor)
            };
        }
    }
}
