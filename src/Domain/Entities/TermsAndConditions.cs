using MemoryPack;

namespace OfficeEntry.Domain.Entities;

[MemoryPackable]
public partial class TermsAndConditions
{
    public bool IsHealthAndSafetyMeasuresAccepted { get; set; }
    public bool IsPrivacyActStatementAccepted { get; set; }
}
