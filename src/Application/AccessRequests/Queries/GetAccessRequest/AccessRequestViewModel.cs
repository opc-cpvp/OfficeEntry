using OfficeEntry.Domain.Entities;

namespace OfficeEntry.Application.AccessRequests.Queries.GetAccessRequest;

public class AccessRequestViewModel
{
    public Guid Id { get; set; }
    public bool IsDelegate { get; set; }

    public bool IsEmployee { get; set; }
    public bool IsManager { get; set; }

    public string Building { get; set; }
    public string Floor { get; set; }
    public string Workspace { get; set; }
    public string EmployeeName { get; set; }
    public string ManagerName { get; set; }
    public string Details { get; set; }
    public string Reason { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public AccessRequest.ApprovalStatus Status { get; set; }

    public AssetRequest[] AssetRequests { get; set; }
    public Visitor[] Visitors { get; set; }
}
