using MemoryPack;
using OfficeEntry.Domain.Entities;

namespace OfficeEntry.Domain.ViewModels;

[MemoryPackable]
public partial class AccessRequestViewModel
{
    public Guid Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid BuildingId { get; set; }
    public Guid? DelegateId { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeFirstName { get; set; }
    public string EmployeeLastName { get; set; }
    public Guid FloorPlanId { get; set; }
    public Guid FloorId { get; set; }
    public bool IsDelegate { get; set; }
    public bool IsEmployee { get; set; }
    public string Building { get; set; }
    public string BuildingEnglishName { get; set; }
    public string BuildingFrenchName { get; set; }
    public string Floor { get; set; }
    public string FloorEnglishName { get; set; }
    public string FloorFrenchName { get; set; }
    public string Workspace { get; set; }
    public string EmployeeName { get; set; }
    public string ManagerName { get; set; }
    public string Details { get; set; }
    public string Reason { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public AccessRequest.ApprovalStatus Status { get; set; }
}

[MemoryPackable]
public partial class AvailableWorkspaceViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}