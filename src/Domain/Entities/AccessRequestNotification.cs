namespace OfficeEntry.Domain.Entities;

public class AccessRequestNotification : EmailTemplate
{
    public override string Title => $"Requête d'accès au {AccessRequest?.Building?.FrenchName} {FrenchStatus} / Access request at {AccessRequest?.Building?.EnglishName} {EnglishStatus}";
    public AccessRequest AccessRequest { get; set; }

    public string EnglishStatus => AccessRequest?.Status?.Key switch
    {
        (int)AccessRequest.ApprovalStatus.Approved => "Approved",
        (int)AccessRequest.ApprovalStatus.Cancelled => "Cancelled",
        (int)AccessRequest.ApprovalStatus.Declined => "Declined",
        (int)AccessRequest.ApprovalStatus.Pending => "Pending",
        _ => string.Empty
    };

    public string FrenchStatus => AccessRequest?.Status?.Key switch
    {
        (int)AccessRequest.ApprovalStatus.Approved => "Approuvée",
        (int)AccessRequest.ApprovalStatus.Cancelled => "Annulée",
        (int)AccessRequest.ApprovalStatus.Declined => "Refusée",
        (int)AccessRequest.ApprovalStatus.Pending => "En attente",
        _ => string.Empty
    };
}
