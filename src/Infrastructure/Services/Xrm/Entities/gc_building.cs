using OfficeEntry.Domain.Entities;

namespace OfficeEntry.Infrastructure.Services.Xrm.Entities;

internal class gc_building
{
    public Guid gc_buildingid { get; set; }
    public string gc_address { get; set; }
    public string gc_city { get; set; }
    public string gc_englishdescription { get; set; }
    public string gc_englishname { get; set; }
    public string gc_frenchdescription { get; set; }
    public string gc_frenchname { get; set; }
    public string gc_timezone { get; set; }
    public double gc_timezoneoffset { get; set; }
    public IEnumerable<contact> gc_building_contact_firstaidattendants { get; set; }
    public IEnumerable<contact> gc_building_contact_flooremergencyofficers { get; set; }
    public IEnumerable<contact> gc_building_contact_mentalhealthtraining { get; set; }
    public int statecode { get; set; }

    public static Building Convert(gc_building building)
    {
        if (building is null)
            return null;

        return new Building
        {
            Id = building.gc_buildingid,
            Address = building.gc_address,
            City = building.gc_city,
            EnglishDescription = building.gc_englishdescription,
            FrenchDescription = building.gc_frenchdescription,
            EnglishName = building.gc_englishname,
            FrenchName = building.gc_frenchname,
            Timezone = building.gc_timezone,
            TimezoneOffset = building.gc_timezoneoffset
        };
    }

    public static gc_building MapFrom(Building building)
    {
        if (building is null)
            return null;

        return new gc_building
        {
            gc_buildingid = building.Id,
            gc_address = building.Address,
            gc_city = building.City,
            gc_englishdescription = building.EnglishDescription,
            gc_frenchdescription = building.FrenchDescription,
            gc_englishname = building.EnglishName,
            gc_frenchname = building.FrenchName,
            gc_timezone = building.Timezone,
            gc_timezoneoffset = building.TimezoneOffset
        };
    }
}
