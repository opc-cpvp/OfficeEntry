using OfficeEntry.Domain.Entities;

namespace OfficeEntry.Infrastructure.Services.Xrm.Entities;

// TODO: floorplan or zoneplan?? if building for mobile, a full floorplan is too big to be displayed.
internal class gc_floorplan
{
    public Guid gc_floorplanid { get; set; }
    public string gc_name { get; set; }
    public int statecode { get; set; }
    public string gc_base64 { get; set; }
    public gc_building gc_building { get; set; }
    public gc_floor gc_floor { get; set; }
    public ICollection<gc_workspace> gc_floorplan_gc_workspaces { get; set; } = new List<gc_workspace>();

    public static FloorPlan Convert(gc_floorplan floorPlan)
    {
        if (floorPlan is null)
            return null;

        return new FloorPlan
        {
            Id = floorPlan.gc_floorplanid,
            Name = floorPlan.gc_name,
            FloorPlanImage = floorPlan.gc_base64,
            Building = gc_building.Convert(floorPlan.gc_building),
            Floor = gc_floor.Convert(floorPlan.gc_floor),
            Workspaces = floorPlan.gc_floorplan_gc_workspaces.Select(gc_workspace.Convert).ToList()
        };
    }

    public static gc_floorplan MapFrom(FloorPlan floorPlan)
    {
        if (floorPlan is null)
            return null;

        return new gc_floorplan
        {
            gc_floorplanid = floorPlan.Id,
            gc_name = floorPlan.Name,
            gc_base64 = floorPlan.FloorPlanImage,
            gc_building = gc_building.MapFrom(floorPlan.Building),
            gc_floor = gc_floor.MapFrom(floorPlan.Floor)
        };
    }
}