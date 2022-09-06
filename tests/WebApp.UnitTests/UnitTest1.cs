using Bunit;

namespace WebApp.UnitTests;

public class Map2Tests
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

public class Map3Tests
{
    [Fact]
    public void Test1()
    {
        using var ctx = new TestContext();
        var moduleInterop = ctx.JSInterop.SetupModule("/js/floorplan.js");
    }
}