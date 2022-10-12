using Newtonsoft.Json.Linq;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Application.Common.Models;
using OfficeEntry.Domain.Entities;
using OfficeEntry.Infrastructure.Services.Xrm.Entities;
using Simple.OData.Client;
using System.Collections.Immutable;
using System.Text;

namespace OfficeEntry.Infrastructure.Services.Xrm;

public class AccessRequestService : IAccessRequestService
{
    private readonly IODataClient _client;
    private readonly IHttpClientFactory _httpClientFactory;

    public AccessRequestService(IODataClient client, IHttpClientFactory httpClientFactory)
    {
        _client = client;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ImmutableArray<AccessRequest>> GetApprovedOrPendingAccessRequestsByFloorPlan(Guid floorPlanId, DateOnly date)
    {
        var startOfDay = date.ToDateTime(TimeOnly.MinValue);
        var endOfDay = date.ToDateTime(TimeOnly.MaxValue);

        var accessRequests = await _client.For<gc_accessrequest>()
            .Filter(a => a.gc_floorplan.gc_floorplanid == floorPlanId)
            .Filter(x => x.gc_starttime <= endOfDay)
            .Filter(x => x.gc_endtime >= startOfDay)
            .Filter(a => a.statecode == (int)StateCode.Active)
            .Expand(
                "gc_delegate/contactid",
                "gc_delegate/firstname",
                "gc_delegate/lastname",

                "gc_employee/contactid",
                "gc_employee/firstname",
                "gc_employee/lastname",

                "gc_building/gc_buildingid",
                "gc_building/gc_englishname", "gc_building/gc_frenchname",

                "gc_floor/gc_floorid",
                "gc_floor/gc_englishname", "gc_floor/gc_frenchname",

                "gc_floorplan/gc_floorplanid",

                "gc_workspace/gc_workspaceid",
                "gc_workspace/gc_name"
            )
            .FindEntriesAsync();

        // TODO: add visitors for shared workspace and boardrooms
        return accessRequests
            .Where(accessRequest =>
                accessRequest.gc_approvalstatus == (ApprovalStatus)AccessRequest.ApprovalStatus.Approved ||
                accessRequest.gc_approvalstatus == (ApprovalStatus)AccessRequest.ApprovalStatus.Pending
            )
            .Select(gc_accessrequest.Convert)
            .ToImmutableArray();
    }

    public async Task<IEnumerable<AccessRequest>> GetApprovedOrPendingAccessRequestsByFloor(Guid floorId, DateTime? date = null)
    {
        var startOfDay = date.Value.Date;
        var endOfDay = date.Value.AddDays(1).Date.AddSeconds(-1);

        var accessRequests = await _client.For<gc_accessrequest>()
            .Filter(a => a.gc_floor.gc_floorid == floorId && a.gc_starttime >= startOfDay && a.gc_starttime <= endOfDay)
            .FindEntriesAsync();

        return accessRequests.Where(x =>
            x.gc_approvalstatus == (ApprovalStatus)AccessRequest.ApprovalStatus.Approved ||
            x.gc_approvalstatus == (ApprovalStatus)AccessRequest.ApprovalStatus.Pending
        ).Select(gc_accessrequest.Convert);
    }

    public async Task<(Result Result, AccessRequest AccessRequest)> GetAccessRequest(Guid accessRequestId)
    {
        var accessRequest = await _client.For<gc_accessrequest>()
            .Key(accessRequestId)
            .Expand(a => new
            {
                a.gc_employee,
                a.gc_delegate,
                a.gc_building,
                a.gc_floor,
                a.gc_floorplan,
                a.gc_manager,
                a.gc_workspace,
            })
            .FindEntryAsync();

        var map = gc_accessrequest.Convert(accessRequest);

        return (Result.Success(), map);
    }

    public async Task<(Result Result, IEnumerable<AccessRequest> AccessRequests)> GetAccessRequestsFor(Guid contactId)
    {
        var accessRequests = await _client.For<gc_accessrequest>()
            // Only grab the required properties
            .Select(a => new
            {
                a.gc_accessrequestid,
                a.gc_approvalstatus,
                a.gc_starttime,
                a.gc_endtime,
                a.gc_employee,
                a.gc_building,
                a.gc_floor,
                a.gc_floorplan,
                a.gc_workspace
            })
            .Filter(a => a.statecode == (int)StateCode.Active)
            .Filter(a => a.gc_employee.contactid == contactId)
            // Only grab the required properties of the navigation properties
            .Expand(
                "gc_employee/contactid",
                "gc_employee/firstname",
                "gc_employee/lastname",

                "gc_building/gc_buildingid",
                "gc_building/gc_englishname", "gc_building/gc_frenchname",

                "gc_floor/gc_floorid",
                "gc_floor/gc_englishname", "gc_floor/gc_frenchname",

                "gc_floorplan/gc_floorplanid",

                "gc_workspace/gc_workspaceid",
                "gc_workspace/gc_name")
            .OrderByDescending(a => a.gc_starttime)
            .FindEntriesAsync();

        var map = accessRequests.Select(gc_accessrequest.Convert).ToList();

        return (Result.Success(), map);
    }

    public async Task<(Result Result, IEnumerable<AccessRequest> AccessRequests)> GetDelegateAccessRequestsFor(Guid contactId)
    {
        var accessRequests = await _client.For<gc_accessrequest>()
            // Only grab the required properties
            .Select(a => new
            {
                a.gc_accessrequestid,
                a.gc_approvalstatus,
                a.gc_starttime,
                a.gc_endtime,
                a.gc_employee,
                a.gc_building,
                a.gc_floor,
                a.gc_floorplan,
                a.gc_workspace
            })
            .Filter(a => a.statecode == (int)StateCode.Active)
            .Filter(a => a.gc_delegate.contactid == contactId)
            // Only grab the required properties of the navigation properties
            .Expand(
                "gc_employee/contactid",
                "gc_employee/firstname",
                "gc_employee/lastname",

                "gc_building/gc_buildingid",
                "gc_building/gc_englishname", "gc_building/gc_frenchname",

                "gc_floor/gc_floorid",
                "gc_floor/gc_englishname", "gc_floor/gc_frenchname",

                "gc_floorplan/gc_floorplanid",

                "gc_workspace/gc_workspaceid",
                "gc_workspace/gc_name")
            .OrderByDescending(a => a.gc_starttime)
            .FindEntriesAsync();

        var map = accessRequests.Select(gc_accessrequest.Convert).ToList();

        return (Result.Success(), map);
    }

    public async Task<(Result Result, AccessRequest AccessRequest)> CreateAccessRequest(AccessRequest accessRequest)
    {
        var access = gc_accessrequest.MapFrom(accessRequest);
        access.gc_accessrequestid = Guid.NewGuid();

        access.gc_starttime = access.gc_starttime.ToUniversalTime();
        access.gc_endtime = access.gc_endtime.ToUniversalTime();

        access = await _client.For<gc_accessrequest>()
            .Set(access)
            .InsertEntryAsync();

        var map = gc_accessrequest.Convert(access);

        return (Result.Success(), map);
    }

    public async Task<Result> UpdateAccessRequest(AccessRequest accessRequest)
    {
        await _client.For<gc_accessrequest>()
            .Key(accessRequest.Id)
            .Set(new
            {
                gc_approvalstatus = (ApprovalStatus)accessRequest.Status.Key
            })
            .UpdateEntryAsync();

        return Result.Success();
    }
}
