using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Application.Common.Models;
using OfficeEntry.Domain.Entities;
using OfficeEntry.Infrastructure.Services.Xrm.Entities;
using Simple.OData.Client;

namespace OfficeEntry.Infrastructure.Services.Xrm;

public class BuildingRoleService : IBuildingRoleService
{
    private readonly IODataClient _client;
    private readonly IUserSettingsService _userSettingsService;

    public BuildingRoleService(IODataClient client, IUserSettingsService userSettingsService)
    {
        _client = client;
        _userSettingsService = userSettingsService;
    }

    public async Task<(Result Result, IEnumerable<BuildingRole> BuildingRoles)> GetBuildingRolesFor(Guid contactId)
    {
        var (_, userSettings) = await _userSettingsService.GetUserSettingsFor(contactId);

        var buildingRoles = await _client.For<gc_usersettingses>()
            .Key(userSettings.Id)
            .NavigateTo(u => u.gc_usersettings_buildingrole)
            .Expand(r => new { r.gc_building, r.gc_floor })
            .FindEntriesAsync();

        var map = buildingRoles.Select(gc_buildingrole.Convert).ToList();

        return (Result.Success(), map);
    }
}