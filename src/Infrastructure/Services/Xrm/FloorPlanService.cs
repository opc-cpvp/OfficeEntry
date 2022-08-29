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
        var x = await _client.For<gc_floorplan>()
            .Filter(x => x.statecode == (int)StateCode.Active)
            .Expand(f => new { f.gc_floorplan_gc_workspaces })
            .Key(floorPlanId)
            .FindEntryAsync();

        return new FloorPlan
        {
            Id = x.gc_floorplanid,
            Name = x.gc_name,
            FloorPlanImage = x.gc_base64,
            Workspaces = x.gc_floorplan_gc_workspaces
                .Select(w => new Workspace
                {
                    Id = w.gc_workspaceid,
                    Name = w.gc_name,
                    X = w.gc_x,
                    Y = w.gc_y
                })
                .ToList()
        };
    }

    public async Task<ImmutableArray<FloorPlan>> GetFloorPlansAsync(string keyword)
    {
        var floorplans = await _client.For<gc_floorplan>()
            .Filter(x => x.statecode == (int)StateCode.Active)
            .Filter(x => x.gc_name.Contains(keyword))
            .FindEntriesAsync();

        return floorplans
            .Select(x => new FloorPlan
            {
                Id = x.gc_floorplanid,
                Name = x.gc_name
            }).ToImmutableArray();        
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


// TODO: floorplan or zoneplan?? if building for mobile, a full floorplan is too big to be displayed.
public class gc_floorplan
{
    public Guid gc_floorplanid { get; set; }

    public string gc_name { get; set; }

    public int statecode { get; set; }

    public string gc_base64 { get; set; }

    public ICollection<gc_workspace> gc_floorplan_gc_workspaces { get; set; } = new List<gc_workspace>();
}

public class gc_workspace
{
    public Guid gc_workspaceid { get; set; }
    public string gc_name { get; set; }
    public int gc_x { get; set; }
    public int gc_y { get; set; }

    public gc_floorplan gc_floorplanid { get; set; }
}