﻿using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;
using OfficeEntry.Domain.Enums;
using OfficeEntry.Infrastructure.Services.Xrm.Entities;
using Simple.OData.Client;

namespace OfficeEntry.Infrastructure.Services.Xrm;

public class LocationService : ILocationService
{
    private readonly IODataClient _client;

    public LocationService(IODataClient client)
    {
        _client = client;
    }

    public async Task<IEnumerable<Building>> GetBuildingsAsync(string locale)
    {
        var buildings = await _client.For<gc_building>()
            .Filter(a => a.statecode == (int)StateCode.Active)
            .FindEntriesAsync();

        return buildings.Select(b => new Building
        {
            Id = b.gc_buildingid,
            Address = b.gc_address,
            City = b.gc_city,
            EnglishDescription = b.gc_englishdescription,
            FrenchDescription = b.gc_frenchdescription,
            EnglishName = b.gc_englishname,
            FrenchName = b.gc_frenchname,
            Name = (locale == Locale.French) ? b.gc_frenchname : b.gc_englishname,
            Timezone = b.gc_timezone,
            TimezoneOffset = b.gc_timezoneoffset
        });
    }

    public async Task<IEnumerable<Floor>> GetFloorsByBuildingAsync(Guid buildingId, string locale)
    {
        var floors = await _client.For<gc_building>()
            .Key(buildingId)
            .NavigateTo(b => b.gc_building_floor)
            .FindEntriesAsync();

        return floors.Select(f => new Floor
        {
            Id = f.gc_floorid,
            BuildingId = buildingId,
            Capacity = f.gc_capacity,
            CurrentCapacity = f.gc_currentcapacity,
            EnglishName = f.gc_englishname,
            FrenchName = f.gc_frenchname,
            Name = (locale == Locale.French) ? f.gc_frenchname : f.gc_englishname,
        });
    }

    public async Task<int> GetCapacityByFloorAsync(Guid floorId)
    {
        var floor = await _client.For<gc_floor>()
            .Key(floorId)
            .FindEntryAsync();

        return floor.gc_capacity;
    }
}
