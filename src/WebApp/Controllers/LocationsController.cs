using Microsoft.AspNetCore.Mvc;
using OfficeEntry.Application.Locations.Queries.GetBuildings;
using OfficeEntry.Application.Locations.Queries.GetFloors;
using OfficeEntry.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OfficeEntry.WebApp.Models;

namespace OfficeEntry.WebApp.Controllers
{
    public class LocationsController : ApiController
    {
        private const int DayInSeconds = 86_400;

        [HttpGet("{locale}")]
        [ResponseCache(VaryByQueryKeys = new string[] { "locale" }, Duration = DayInSeconds)]
        public async Task<IEnumerable<Building>> Get(string locale)
        {
            return await Mediator.Send(new GetBuildingsQuery { Locale = locale });
        }

        [HttpGet("{locale}/{id}")]
        [ResponseCache(VaryByQueryKeys = new string[] { "id", "locale" }, Duration = DayInSeconds)]
        public async Task<IEnumerable<Floor>> Get(Guid id, string locale)
        {
            return await Mediator.Send(new GetFloorsQuery { BuildingId = id, Locale = locale });
        }

        [HttpGet("{locale}/{buildingId}/{floorId}")]
        public async Task<IEnumerable<MapArea>> Get(Guid buildingId, Guid floorId, string locale, DateTime date, int start, int end)
        {
            var startTime = date.AddHours(start);
            var endTime = date.AddHours(end);

            var officeReservations = await Mediator.Send(new GetOfficeReservationsByFloorQuery { FloorId = floorId, ReservationDate = date, Locale = locale });
            return officeReservations.Select(GetReservationsArea);

            MapArea GetReservationsArea(KeyValuePair<Office, IEnumerable<AccessRequest>> officeReservations)
            {
                var office = officeReservations.Key;

                return new MapArea
                {
                    Id = GetReservationGrouping(officeReservations),
                    Name = office.Name,
                    Shape = office.GetShape(),
                    Coordinates = office.Coordinates
                };
            }

            string GetReservationGrouping(KeyValuePair<Office, IEnumerable<AccessRequest>> officeReservations)
            {
                var office = officeReservations.Key;
                var accessRequests = officeReservations.Value;

                // Check if the reservation overlaps with any existing reservations
                if (accessRequests.Any(a => a.StartTime < endTime && startTime < a.EndTime))
                    return "unavailable";

                return office.Id.ToString();
            }
        }
    }
}