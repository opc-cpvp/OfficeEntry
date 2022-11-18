using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;
using OfficeEntry.Domain.Enums;
using OfficeEntry.Infrastructure.Services.Xrm.Entities;
using Simple.OData.Client;
using System.Collections.Immutable;

namespace OfficeEntry.Infrastructure.Services.Xrm;

public class LocationService : ILocationService
{
    private const int ThresholdFirstAidAttendant = 50;
    private const int ThresholdFloorEmergencyOfficer = 50;
    private const int MinimumFirstAidAttendant = 5;
    private const int MinimumFloorEmergencyOfficer = 10;

    private readonly IODataClient _client;
    private readonly IAccessRequestService _accessRequestService;

    public LocationService(IODataClient client, IAccessRequestService accessRequestService)
    {
        _client = client;
        _accessRequestService = accessRequestService;
    }

    public async Task<IEnumerable<Building>> GetBuildingsAsync(string locale)
    {
        var buildings = await _client.For<gc_building>()
            .Filter(a => a.statecode == (int)StateCode.Active)
            .FindEntriesAsync();

        return buildings.Select(b => new Building
        {
            Id = b.gc_buildingid,
            Address = b.gc_address,
            City = b.gc_city,
            EnglishDescription = b.gc_englishdescription,
            FrenchDescription = b.gc_frenchdescription,
            EnglishName = b.gc_englishname,
            FrenchName = b.gc_frenchname,
            Name = (locale == Locale.French) ? b.gc_frenchname : b.gc_englishname,
            Timezone = b.gc_timezone,
            TimezoneOffset = b.gc_timezoneoffset
        });
    }

    public async Task<FloorPlanCapacity> GetCapacityByFloorPlanAsync(Guid floorPlanId, DateOnly date)
    {
        var accessRequests = await _accessRequestService.GetApprovedOrPendingAccessRequestsByFloorPlan(floorPlanId, date);
        var approvedAccessRequests = accessRequests
            .Where(a => a.Status.Key == (int)AccessRequest.ApprovalStatus.Approved)
            .DistinctBy(a => a.Employee.Id)
            .ToList();

        var firstAidAttendants = approvedAccessRequests.Count(x => x.FirstAidAttendant);
        var maxFirstAidAttendantCapacity = Math.Max(firstAidAttendants * ThresholdFirstAidAttendant, MinimumFirstAidAttendant);

        var floorEmergencyOfficers = approvedAccessRequests.Count(x => x.FloorEmergencyOfficer);
        var maxFloorEmergencyOfficerCapacity = Math.Max(floorEmergencyOfficers * ThresholdFloorEmergencyOfficer, MinimumFloorEmergencyOfficer);

        var currentCapacity = approvedAccessRequests.Count;
        var totalCapacity = accessRequests.DistinctBy(a => a.Employee.Id).Count();

        return new FloorPlanCapacity
        {
            CurrentCapacity = currentCapacity,
            MaxFirstAidAttendantCapacity = maxFirstAidAttendantCapacity,
            MaxFloorEmergencyOfficerCapacity = maxFloorEmergencyOfficerCapacity,
            TotalCapacity = totalCapacity
        };
    }

    public async Task<FloorPlan> GetFloorPlanAsync(Guid floorPlanId)
    {
        var floorPlan = await _client.For<gc_floorplan>()
            .Key(floorPlanId)
            .Expand(f => new { f.gc_building, f.gc_floor, f.gc_floorplan_gc_workspaces })
            .FindEntryAsync();

        return gc_floorplan.Convert(floorPlan);
    }

    public async Task<ImmutableArray<FloorPlan>> GetFloorPlansAsync(string keyword)
    {
        var floorPlans = await _client.For<gc_floorplan>()
            .Filter(x => x.statecode == (int)StateCode.Active)
            .Filter(x => x.gc_name.Contains(keyword))
            .FindEntriesAsync();

        return floorPlans.Select(gc_floorplan.Convert).ToImmutableArray();
    }

    public async Task<Workspace> GetWorkspaceAsync(Guid workspaceId)
    {
        var workspace = await _client.For<gc_workspace>()
            .Key(workspaceId)
            .FindEntryAsync();

        return gc_workspace.Convert(workspace);
    }

    public async Task UpdateFloorPlan(FloorPlan floorPlan)
    {
        var old = await _client.For<gc_floorplan>()
            .Expand(f => new { f.gc_floorplan_gc_workspaces })
            .Key(floorPlan.Id)
            .FindEntryAsync();

        if (!floorPlan.FloorPlanImage.Equals(old.gc_base64))
        {
            // Update floorplan image
            _ = await _client
                .For<gc_floorplan>()
                .Key(floorPlan.Id)
                .Set(new { gc_base64 = floorPlan.FloorPlanImage })
                .UpdateEntryAsync();
        }

        // Insert and update workspaces
        var workspaces = floorPlan
            .Workspaces
            .Select(x =>
            {
                x.FloorPlan = floorPlan;
                return gc_workspace.MapFrom(x);
            })
            .ToArray();

        // https://stackoverflow.com/questions/59983903/simple-odata-client-batch-processing
        var batch = new ODataBatch(_client);

        var entryCount = 0;
        foreach (var workspace in workspaces)
        {
            entryCount++;

            // Update
            if (old.gc_floorplan_gc_workspaces.Any(w => w.gc_workspaceid == workspace.gc_workspaceid))
            {
                batch += async _ => await _client
                    .For<gc_workspace>()
                    .Key(workspace.gc_workspaceid)
                    .Set(workspace)
                    .UpdateEntryAsync(false);
            }
            // Insert
            else
            {
                batch += async _ => await _client
                    .For<gc_workspace>()
                    .Set(workspace)
                    .InsertEntryAsync(false);
            }

            // batch of max 100 request per batch
            if (entryCount % 100 is not 0 && entryCount != workspaces.Length)
            {
                continue;
            }

            await batch.ExecuteAsync();
            batch = new ODataBatch(_client);
        }
    }

    public async Task<IEnumerable<Contact>> GetFirstAidAttendantsAsync(Guid buildingId)
    {
        var firstAidAttendants = await _client
            .For<gc_building>()
            .Key(buildingId)
            .NavigateTo(x => x.gc_building_contact_firstaidattendants)
            .Filter(x => x.statecode == (int)StateCode.Active)
            .FindEntriesAsync();

        return firstAidAttendants.Select(contact.Convert);
    }

    public async Task<IEnumerable<Contact>> GetFloorEmergencyOfficersAsync(Guid buildingId)
    {
        var floorEmergencyOfficers = await _client
            .For<gc_building>()
            .Key(buildingId)
            .NavigateTo(x => x.gc_building_contact_flooremergencyofficers)
            .Filter(x => x.statecode == (int)StateCode.Active)
            .FindEntriesAsync();

        return floorEmergencyOfficers.Select(contact.Convert);
    }
}
