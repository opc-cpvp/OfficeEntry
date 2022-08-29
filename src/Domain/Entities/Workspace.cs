﻿namespace OfficeEntry.Domain.Entities;

public class Workspace
{
    public Guid Id { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public string Name { get; set; }

    //public int NumberOfSpots { get; set; }


    public FloorPlan FloorPlan { get; set; }
}