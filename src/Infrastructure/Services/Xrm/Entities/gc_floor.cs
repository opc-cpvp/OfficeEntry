using OfficeEntry.Domain.Entities;

namespace OfficeEntry.Infrastructure.Services.Xrm.Entities;

internal class gc_floor
{
    public Guid gc_floorid { get; set; }
    public gc_building gc_buildingfloorid { get; set; }
    public int gc_capacity { get; set; }
    public int gc_currentcapacity { get; set; }
    public string gc_englishname { get; set; }
    public string gc_frenchname { get; set; }

    public int statecode { get; set; }

    public static Floor Convert(gc_floor floor)
    {
        if (floor is null)
            return null;

        return new Floor
        {
            Id = floor.gc_floorid,
            Capacity = floor.gc_capacity,
            CurrentCapacity = floor.gc_currentcapacity,
            EnglishName = floor.gc_englishname,
            FrenchName = floor.gc_frenchname
        };
    }

    public static gc_floor MapFrom(Floor floor)
    {
        if (floor is null)
            return null;

        return new gc_floor
        {
            gc_floorid = floor.Id,
            gc_capacity = floor.Capacity,
            gc_currentcapacity = floor.CurrentCapacity,
            gc_englishname = floor.EnglishName,
            gc_frenchname = floor.FrenchName
        };
    }
}
