﻿namespace OfficeEntry.Domain.Entities;

public class FloorPlan
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string FloorPlanImage { get; set; }
    public Building Building { get; set; }
    public Floor Floor { get; set; }
    public virtual ICollection<Workspace> Workspaces { get; set; } = new HashSet<Workspace>();

    public override string ToString()
    {
        return $"FloorPlan {{ Id: {Id}, Name: \"{Name}\" }}";
    }
}
