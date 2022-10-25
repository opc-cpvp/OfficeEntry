using MediatR;
using OfficeEntry.Application.Common.Interfaces;

namespace OfficeEntry.Application.Locations.Queries.GetEmergencyContacts
{
    public record GetEmergencyContactsQuery : IRequest<EmergencyContactsViewModel>
    {
        public Guid BuildingId { get; init; }
    }

    public class GetEmergencyContactsQueryHandler : IRequestHandler<GetEmergencyContactsQuery, EmergencyContactsViewModel>
    {
        private readonly ILocationService _locationService;

        public GetEmergencyContactsQueryHandler(ILocationService locationService)
        {
            _locationService = locationService;
        }

        public async Task<EmergencyContactsViewModel> Handle(GetEmergencyContactsQuery request, CancellationToken cancellationToken)
        {
            var firstAidAttendants = await _locationService.GetFirstAidAttendantsAsync(request.BuildingId);
            var floorEmergencyOfficers = await _locationService.GetFloorEmergencyOfficersAsync(request.BuildingId);

            return new EmergencyContactsViewModel
            {
                FirstAidAttendants = firstAidAttendants,
                FloorEmergencyOfficers = floorEmergencyOfficers
            };
        }
    }
}
