namespace OfficeEntry.Domain.Entities;

public class AccessRequestNotification : EmailTemplate
{
    public override string Title => $"Entrée au bureau - Demande de réservation au {AccessRequest?.Building?.FrenchName} {AccessRequest?.FrenchStatus} / Office Entry - Reservation request at {AccessRequest?.Building?.EnglishName} {AccessRequest?.EnglishStatus}";
    public AccessRequest AccessRequest { get; set; }
}
