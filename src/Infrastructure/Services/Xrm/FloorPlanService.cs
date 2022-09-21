using System.Collections.Immutable;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;
using OfficeEntry.Infrastructure.Services.Xrm.Entities;
using Simple.OData.Client;

namespace OfficeEntry.Infrastructure.Services.Xrm;

public class FloorPlanService : IFloorPlanService
{
    private const int ThresholdFirstAidAttendant = 50;
    private const int ThresholdFloorEmergencyOfficer = 50;
    private const int MinimumFirstAidAttendant = 5;
    private const int MinimumFloorEmergencyOfficer = 10;

    private readonly IODataClient _client;
    private readonly IAccessRequestService _accessRequestService;
    private readonly IBuildingRoleService _buildingRoleService;

    public FloorPlanService(IODataClient client, IAccessRequestService accessRequestService, IBuildingRoleService buildingRoleService)
    {
        _client = client;
        _accessRequestService = accessRequestService;
        _buildingRoleService = buildingRoleService;
    }

    public async Task<FloorPlan> GetFloorPlanByIdAsync(Guid floorPlanId)
    {
        var floorPlan = await _client.For<gc_floorplan>()
            .Key(floorPlanId)
            .Expand(f => new { f.gc_building, f.gc_floor, f.gc_floorplan_gc_workspaces })
            .FindEntryAsync();

        return gc_floorplan.Convert(floorPlan);
    }

    public async Task<FloorPlanCapacity> GetFloorPlanCapacityAsync(Guid floorPlanId, DateOnly date)
    {
        var floorPlan = await GetFloorPlanByIdAsync(floorPlanId);

        var accessRequests = await _accessRequestService.GetApprovedOrPendingAccessRequestsByFloorPlan(floorPlanId, date);
        var approvedAccessRequests = accessRequests
            .Where(a => a.Status.Key == (int)AccessRequest.ApprovalStatus.Approved)
            .GroupBy(a => a.Employee.Id)
            .ToDictionary(g => g.Key, g => g.ToList());

        var buildingRoleTasks = approvedAccessRequests.Keys
            .Select(x => _buildingRoleService.GetBuildingRolesFor(x));

        var assignmentCounts = (await Task.WhenAll(buildingRoleTasks))
            .SelectMany(r => r.BuildingRoles)
            .Where(r => r.Floor?.Id == floorPlan.Floor.Id)
            .GroupBy(r => (BuildingRole.BuildingRoles)r.Role.Key)
            .ToDictionary(g => g.Key, g => g.ToList().Count);

        assignmentCounts.TryGetValue(BuildingRole.BuildingRoles.FirstAidAttendant, out var count);
        var approvedFirstAidAttendants = count;

        assignmentCounts.TryGetValue(BuildingRole.BuildingRoles.FloorEmergencyOfficer, out count);
        var approvedFloorEmergencyOfficers = count;

        var maxFirstAidAttendantCapacity = Math.Max(approvedFirstAidAttendants * ThresholdFirstAidAttendant, MinimumFirstAidAttendant);
        var maxFloorEmergencyOfficerCapacity = Math.Max(approvedFloorEmergencyOfficers * ThresholdFloorEmergencyOfficer, MinimumFloorEmergencyOfficer);
        var maxCapacity = Math.Min(maxFirstAidAttendantCapacity, maxFloorEmergencyOfficerCapacity);
        var currentCapacity = approvedAccessRequests.Keys.Count;
        var totalCapacity = accessRequests.DistinctBy(a => a.Employee.Id).Count();

        return new FloorPlanCapacity
        {
            CurrentCapacity = currentCapacity,
            MaxCapacity = maxCapacity,
            MaxFirstAidAttendantCapacity = maxFirstAidAttendantCapacity,
            MaxFloorEmergencyOfficerCapacity = maxFloorEmergencyOfficerCapacity,
            TotalCapacity = totalCapacity
        };
    }

    public async Task<ImmutableArray<FloorPlan>> GetFloorPlansAsync(string keyword)
    {
        var floorPlans = await _client.For<gc_floorplan>()
            .Filter(x => x.statecode == (int)StateCode.Active)
            .Filter(x => x.gc_name.Contains(keyword))
            .FindEntriesAsync();

        return floorPlans.Select(gc_floorplan.Convert).ToImmutableArray();
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
            var updatedFloorplan = await _client
                .For<gc_floorplan>()
                .Key(floorPlan.Id)
                .Set(new { gc_base64 = floorPlan.FloorPlanImage })
                .UpdateEntryAsync();
        }

        // Insert and update workspaces
        var workspaces = floorPlan
            .Workspaces
            .Select(x => new gc_workspace
            {
                gc_workspaceid = x.Id,
                gc_x = x.X,
                gc_y = x.Y,
                gc_name = x.Name,
                gc_floorplanid = new gc_floorplan { gc_floorplanid = floorPlan.Id }
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
}