using Bunit;

namespace WebApp.UnitTests;

public class MapPageTests
{
    [Fact]
    public void When_no_workstation_id_return_default_guid()
    {
        // Arrange
        var data = JsonSerializer.Serialize(new { });

        // Act
        var result = OfficeEntry.WebApp.Pages.FloorPlans.Map.ExtractWorkspaceId(data);

        // Assert
        Assert.Equal(Guid.Empty, result);
    }

    [Fact]
    public void When_workstation_id_exists_return_value_as_guid()
    {
        // Arrange
        var id = Guid.NewGuid();
        var data = JsonSerializer.Serialize(new { date = DateTime.Now.ToString("yyyy-MM-dd"), workspace = id });

        // Act
        var result = OfficeEntry.WebApp.Pages.FloorPlans.Map.ExtractWorkspaceId(data);

        // Assert
        Assert.Equal(id, result);
    }
}