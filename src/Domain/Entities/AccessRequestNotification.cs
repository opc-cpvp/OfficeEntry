﻿namespace OfficeEntry.Domain.Entities;

public class AccessRequestNotification : EmailTemplate
{
    public string BaseUrl { get; set; }
    public override string Title => $"Entrée au bureau - Demande de réservation au {AccessRequest?.Building?.FrenchName} {AccessRequest?.FrenchStatus} / Office Entry - Reservation request at {AccessRequest?.Building?.EnglishName} {AccessRequest?.EnglishStatus}";
    public AccessRequest AccessRequest { get; set; }
    public string EnglishUrl => $"{BaseUrl}en/access-requests/{AccessRequest.Id}";
    public string FrenchUrl => $"{BaseUrl}fr/demandes-d-acces/{AccessRequest.Id}";
}
