namespace OfficeEntry.Domain.Entities;

public class AssetRequest
{
    public Guid Id { get; set; }
    public OptionSet Asset { get; set; }
    public string Other { get; set; }
}
