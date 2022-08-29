namespace OfficeEntry.Domain.Entities;

public class FloorPlan
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid FloorId { get; set; }
    public string FloorPlanImage { get; set; }
    public virtual ICollection<Workspace> Workspaces { get; set; } = new HashSet<Workspace>();
}
