using OfficeEntry.Domain.Entities;

namespace OfficeEntry.Infrastructure.Services.Xrm.Entities;

internal class gc_workspace
{
    public Guid gc_workspaceid { get; set; }
    public string gc_name { get; set; }
    public int gc_x { get; set; }
    public int gc_y { get; set; }
    public gc_floorplan gc_floorplanid { get; set; }

    public static Workspace Convert(gc_workspace workspace)
    {
        if (workspace is null)
            return null;

        return new Workspace
        {
            Id = workspace.gc_workspaceid,
            Name = workspace.gc_name,
            X = workspace.gc_x,
            Y = workspace.gc_y
        };
    }

    public static gc_workspace MapFrom(Workspace workspace)
    {
        if (workspace is null)
            return null;

        return new gc_workspace
        {
            gc_workspaceid = workspace.Id,
            gc_name = workspace.Name,
            gc_x = workspace.X,
            gc_y = workspace.Y,
            gc_floorplanid = gc_floorplan.MapFrom(workspace.FloorPlan)
        };
    }
}