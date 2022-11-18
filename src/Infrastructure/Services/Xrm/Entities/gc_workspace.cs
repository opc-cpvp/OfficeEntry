using OfficeEntry.Domain.Entities;

namespace OfficeEntry.Infrastructure.Services.Xrm.Entities;

internal class gc_workspace
{
    public Guid gc_workspaceid { get; set; }
    public string gc_name { get; set; }
    public string gc_englishdescription { get; set; }
    public string gc_frenchdescription { get; set; }
    public int gc_x { get; set; }
    public int gc_y { get; set; }
    public gc_floorplan gc_floorplanid { get; set; }
    public int statecode { get; set; }

    public static Workspace Convert(gc_workspace workspace)
    {
        if (workspace is null)
            return null;

        return new Workspace
        {
            Id = workspace.gc_workspaceid,
            Name = workspace.gc_name,
            EnglishDescription = workspace.gc_englishdescription,
            FrenchDescription = workspace.gc_frenchdescription,
            X = workspace.gc_x,
            Y = workspace.gc_y,
            StateCode = new OptionSet
            {
                Key = workspace.statecode,
                Value = Enum.GetName(typeof(StateCode), workspace.statecode)
            }
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
            gc_englishdescription = workspace.EnglishDescription,
            gc_frenchdescription = workspace.FrenchDescription,
            gc_x = workspace.X,
            gc_y = workspace.Y,
            gc_floorplanid = gc_floorplan.MapFrom(workspace.FloorPlan)
        };
    }
}