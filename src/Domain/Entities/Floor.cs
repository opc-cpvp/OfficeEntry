using MemoryPack;

namespace OfficeEntry.Domain.Entities;

[MemoryPackable]
public partial class Floor
{
    public Guid Id { get; set; }
    public Guid BuildingId { get; set; }
    public int Capacity { get; set; }
    public int CurrentCapacity { get; set; }
    public string EnglishName { get; set; }
    public string FrenchName { get; set; }
    public string Name { get; set; }
}
