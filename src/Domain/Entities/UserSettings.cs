using MemoryPack;

namespace OfficeEntry.Domain.Entities;

[MemoryPackable]
public partial class UserSettings
{
    public Guid Id { get; set; }
    public DateTime? HealthSafety { get; set; }
    public DateTime? PrivacyStatement { get; set; }
}
