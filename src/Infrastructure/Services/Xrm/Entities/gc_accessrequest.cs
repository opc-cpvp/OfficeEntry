using OfficeEntry.Domain.Entities;

namespace OfficeEntry.Infrastructure.Services.Xrm.Entities;

internal class gc_accessrequest
{
    public string gc_name { get; set; }
    public Guid gc_accessrequestid { get; set; }
    public ApprovalStatus gc_approvalstatus { get; set; }
    public gc_building gc_building { get; set; }
    public contact gc_delegate { get; set; }
    public string gc_details { get; set; }
    public contact gc_employee { get; set; }
    public DateTime gc_endtime { get; set; }
    public gc_floor gc_floor { get; set; }
    public contact gc_manager { get; set; }
    public AccessReasons? gc_accessreason { get; set; }
    public DateTime gc_starttime { get; set; }
    public IList<contact> gc_accessrequest_contact_visitors { get; set; } = new List<contact>();
    public IList<gc_assetrequest> gc_accessrequest_assetrequest { get; set; } = new List<gc_assetrequest>();
    public int statecode { get; set; }
    public DateTime createdon { get; set; }

    public gc_floorplan gc_floorplan { get; set; }
    public gc_workspace gc_workspace { get; set; }

    public static AccessRequest Convert(gc_accessrequest accessRequest)
    {
        if (accessRequest is null)
            return null;

        var request = new AccessRequest
        {
            Id = accessRequest.gc_accessrequestid,
            CreatedOn = accessRequest.createdon.ToLocalTime(),
            AssetRequests = accessRequest.gc_accessrequest_assetrequest.Select(gc_assetrequest.Convert).ToList(),
            Building = gc_building.Convert(accessRequest.gc_building),
            Delegate = contact.Convert(accessRequest.gc_delegate),
            Details = accessRequest.gc_details,
            Employee = contact.Convert(accessRequest.gc_employee),
            EndTime = accessRequest.gc_endtime.ToLocalTime(),
            Floor = gc_floor.Convert(accessRequest.gc_floor),
            FloorPlan = gc_floorplan.Convert(accessRequest.gc_floorplan),
            Manager = contact.Convert(accessRequest.gc_manager),
            StartTime = accessRequest.gc_starttime.ToLocalTime(),
            Status = new OptionSet
            {
                Key = (int)accessRequest.gc_approvalstatus,
                Value = Enum.GetName(typeof(ApprovalStatus), accessRequest.gc_approvalstatus)
            },
            Visitors = accessRequest.gc_accessrequest_contact_visitors.Select(contact.Convert).ToList(),
            Workspace = gc_workspace.Convert(accessRequest.gc_workspace)
        };

        if (accessRequest.gc_accessreason.HasValue)
        {
            request.Reason = new OptionSet
            {
                Key = (int)accessRequest.gc_accessreason,
                Value = Enum.GetName(typeof(AccessReasons), accessRequest.gc_accessreason)
            };
        }

        return request;
    }

    public static gc_accessrequest MapFrom(AccessRequest accessRequest)
    {
        return new gc_accessrequest
        {
            gc_name = $"{accessRequest.Employee.FullName} - {accessRequest.StartTime:yyyy-MM-dd}",
            gc_accessreason = (AccessReasons?)accessRequest.Reason?.Key,
            gc_approvalstatus = (ApprovalStatus?)accessRequest.Status?.Key ?? ApprovalStatus.Pending,
            gc_building = gc_building.MapFrom(accessRequest.Building),
            gc_delegate = contact.MapFrom(accessRequest.Delegate),
            gc_details = accessRequest.Details,
            gc_employee = contact.MapFrom(accessRequest.Employee),
            gc_endtime = accessRequest.EndTime,
            gc_floor = gc_floor.MapFrom(accessRequest.Floor),
            gc_floorplan = gc_floorplan.MapFrom(accessRequest.FloorPlan),
            gc_manager = contact.MapFrom(accessRequest.Manager),
            gc_starttime = accessRequest.StartTime,
            gc_workspace = gc_workspace.MapFrom(accessRequest.Workspace)
        };
    }
}
