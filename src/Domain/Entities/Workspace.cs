using MemoryPack;
using System.Dynamic;

namespace OfficeEntry.Domain.Entities;

[MemoryPackable]
public partial class Workspace
{
    public Guid Id { get; set; }
    public string EnglishDescription { get; set; }
    public string FrenchDescription { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public string Name { get; set; }
    public FloorPlan FloorPlan { get; set; }
    public OptionSet StateCode { get; set; }
}
