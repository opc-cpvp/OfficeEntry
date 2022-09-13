using System.Collections.Immutable;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;
using OfficeEntry.Infrastructure.Services.Xrm.Entities;
using Simple.OData.Client;

namespace OfficeEntry.Infrastructure.Services.Xrm;

public class FloorPlanService : IFloorPlanService
{
    private readonly IODataClient _client;

    public FloorPlanService(IODataClient client)
    {
        _client = client;
    }

    public async Task<FloorPlan> GetFloorPlanByIdAsync(Guid floorPlanId)
    {
        var floorPlan = await _client.For<gc_floorplan>()
            .Filter(x => x.statecode == (int)StateCode.Active)
            .Expand(f => new { f.gc_building, f.gc_floor, f.gc_floorplan_gc_workspaces })
            .Key(floorPlanId)
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