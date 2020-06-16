using Microsoft.Extensions.Configuration;
using OfficeEntry.Domain.Contracts;
using OfficeEntry.Domain.Entities;
using System;
using System.Collections.Generic;

namespace OfficeEntry.Services.Xrm
{
    public class LocationService : XrmService, ILocationService
    {
        public LocationService(IConfiguration configuration) :
            base(configuration)
        {
        }

        public List<Building> GetBuildings()
        {
            throw new NotImplementedException();
        }

        public List<Floor> GetFloorsByBuilding(Guid buildingId)
        {
            throw new NotImplementedException();
        }
    }
}
