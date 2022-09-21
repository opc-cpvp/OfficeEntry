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

        if (userSettings is null)
        {
            return (Result.Success(), Enumerable.Empty<BuildingRole>());
        }

        var buildingRoles = await _client.For<gc_usersettingses>()
            .Key(userSettings.Id)
            .NavigateTo(u => u.gc_usersettings_buildingrole)
            .Expand(r => new { r.gc_building, r.gc_floor })
            .FindEntriesAsync();

        var map = buildingRoles.Select(gc_buildingrole.Convert).ToList();

        return (Result.Success(), map);
    }

    public async Task<(Result Result, IEnumerable<Contact> Contacts)> GetContactsByBuildingRole(Guid floorId, BuildingRole.BuildingRoles role)
    {
        var buildingRoles = await _client.For<gc_buildingrole>()
            .Filter(x => x.statecode == (int)StateCode.Active)
            .Filter($"{nameof(gc_buildingrole.gc_role)} eq {(int)role}")
            .Filter(x => x.gc_floor.gc_floorid == floorId)
            .Expand(
                "gc_usersettingsbuildingroleid/gc_usersettingsid"
            )
            .Select(x => x.gc_usersettingsbuildingroleid)
            .FindEntriesAsync();

        var contacts = new List<Contact>();
        foreach (var buildingRole in buildingRoles)
        {
            var userSettingsId = buildingRole.gc_usersettingsbuildingroleid.gc_usersettingsid;
            var contact = await _client.For<contact>()
                .Filter(x => x.gc_usersettings.gc_usersettingsid == userSettingsId)
                .FindEntryAsync();
            contacts.Add(contact.Convert(contact));
        }

        return (Result.Success(), contacts);
    }
}