using MemoryPack;

namespace OfficeEntry.Domain.Entities;

[MemoryPackable]
public partial class OptionSet
{
    public int Key { get; set; }
    public string Value { get; set; }
}
