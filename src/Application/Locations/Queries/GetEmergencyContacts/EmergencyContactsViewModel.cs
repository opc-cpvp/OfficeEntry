using OfficeEntry.Domain.Entities;

namespace OfficeEntry.Application.Locations.Queries.GetEmergencyContacts
{
    public class EmergencyContactsViewModel
    {
        public IEnumerable<Contact> FirstAidAttendants { get; set; } = Enumerable.Empty<Contact>();
        public IEnumerable<Contact> FloorEmergencyOfficers { get; set; } = Enumerable.Empty<Contact>();
    }
}
