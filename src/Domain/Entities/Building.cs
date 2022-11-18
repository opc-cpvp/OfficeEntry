namespace OfficeEntry.Domain.Entities;

public class Building
{
    public Guid Id { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string EnglishDescription { get; set; }
    public string FrenchDescription { get; set; }
    public string EnglishName { get; set; }
    public string FrenchName { get; set; }
    public string Name { get; set; }
    public string Timezone { get; set; }
    public double TimezoneOffset { get; set; }
}
