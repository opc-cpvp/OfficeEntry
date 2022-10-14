namespace OfficeEntry.Domain.Entities;

public class AccessRequest
{
    public Guid Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public Contact Employee { get; set; }
    public Contact Manager { get; set; }
    public FloorPlan FloorPlan { get; set; }
    public Workspace Workspace { get; set; }
    public Building Building { get; set; }
    public Floor Floor { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Details { get; set; }
    public Contact Delegate { get; set; }
    public OptionSet Reason { get; set; }
    public OptionSet Status { get; set; }
    public bool IsPending => Status?.Key == (int)ApprovalStatus.Pending;

    public string EnglishStatus => Status?.Key switch
    {
        (int)ApprovalStatus.Approved => "Approved",
        (int)ApprovalStatus.Cancelled => "Cancelled",
        (int)ApprovalStatus.Declined => "Declined",
        (int)ApprovalStatus.Pending => "Pending",
        _ => string.Empty
    };

    public string FrenchStatus => Status?.Key switch
    {
        (int)ApprovalStatus.Approved => "Approuvée",
        (int)ApprovalStatus.Cancelled => "Annulée",
        (int)ApprovalStatus.Declined => "Refusée",
        (int)ApprovalStatus.Pending => "En attente",
        _ => string.Empty
    };

    public enum ApprovalStatus
    {
        Pending = 948160000,
        Approved = 948160001,
        Declined = 948160002,
        Cancelled = 948160003
    }

    public int GetStatusOrder()
        => GetStatusOrder((ApprovalStatus)Status.Key);

    private static int GetStatusOrder(ApprovalStatus status)
        => status switch
        {
            ApprovalStatus.Pending => 1,
            ApprovalStatus.Approved => 2,
            ApprovalStatus.Declined => 3,
            ApprovalStatus.Cancelled => 4,
            _ => 5
        };
}
