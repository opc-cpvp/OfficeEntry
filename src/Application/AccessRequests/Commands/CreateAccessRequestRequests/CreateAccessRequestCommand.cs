using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeEntry.Application.AccessRequests.Queries.GetSpotsAvailablePerHour;
using OfficeEntry.Application.Locations.Queries.GetFloorPlan;

namespace OfficeEntry.Application.AccessRequests.Commands.CreateAccessRequestRequests
{
    public class CreateAccessRequestCommand : IRequest
    {
        public AccessRequest AccessRequest { get; set; }
    }

    public class CreateAccessRequestCommandHandler : IRequestHandler<CreateAccessRequestCommand>
    {
        private readonly IAccessRequestService _accessRequestService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILocationService _locationService;
        private readonly IUserService _userService;

        private IMediator _mediator;

        public CreateAccessRequestCommandHandler(IAccessRequestService accessRequestService, ICurrentUserService currentUserService, ILocationService locationService, IUserService userService, IMediator mediator)
        {
            _accessRequestService = accessRequestService;
            _currentUserService = currentUserService;
            _locationService = locationService;
            _userService = userService;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(CreateAccessRequestCommand request, CancellationToken cancellationToken)
        {
            var username = _currentUserService.UserId;
            var contactResult = await _userService.GetContact(username);

            if (contactResult.Contact?.UserSettings?.HealthSafety is null || contactResult.Contact?.UserSettings?.PrivacyStatement is null)
            {
                throw new Exception("Can't create an access request without accepting Privacy Act statement and Health and Safety measures");
            }

            var floorId = request.AccessRequest.FloorPlan.Floor.Id;
            var date = request.AccessRequest.StartTime;
            var requestContactCount = (request.AccessRequest.Visitors?.Count ?? 0) + 1;

            var currentCapacities = await _mediator.Send(new GetSpotsAvailablePerHourQuery { FloorId = floorId, SelectedDay = date }, cancellationToken);

            var hasAvailableCapacity = currentCapacities.Where(x => x.Hour >= request.AccessRequest.StartTime.Hour && x.Hour < request.AccessRequest.EndTime.Hour)
                .All(x => x.Capacity - x.SpotsReserved - requestContactCount >= 0);

            if (!hasAvailableCapacity)
            {
                throw new Exception("Your request exceeds the floor capacity");
            }

            var contact = new Contact
            {
                Id = contactResult.Contact.Id,
                FirstName = contactResult.Contact.FirstName,
                LastName = contactResult.Contact.LastName
            };

            if (request.AccessRequest.Employee is null)
            {
                request.AccessRequest.Employee = contact;
            }
            else
            {
                request.AccessRequest.Delegate = contact;
            }

            await _accessRequestService.CreateAccessRequest(request.AccessRequest);

            return Unit.Value;
        }
    }
}
