using MediatR;
using OfficeEntry.Application.AccessRequests.Queries.GetSpotsAvailablePerHour;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;

namespace OfficeEntry.Application.AccessRequests.Commands.CreateAccessRequestRequests
{
    public class CreateAccessRequestForCurrentUserCommand : IRequest
    {
        public AccessRequest AccessRequest { get; set; }
    }

    public class CreateAccessRequestForCurrentUserCommandHandler : IRequestHandler<CreateAccessRequestForCurrentUserCommand>
    {
        private readonly IAccessRequestService _accessRequestService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILocationService _locationService;
        private readonly IUserService _userService;

        private IMediator _mediator;

        public CreateAccessRequestForCurrentUserCommandHandler(IAccessRequestService accessRequestService, ICurrentUserService currentUserService, ILocationService locationService, IUserService userService, IMediator mediator)
        {
            _accessRequestService = accessRequestService;
            _currentUserService = currentUserService;
            _locationService = locationService;
            _userService = userService;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(CreateAccessRequestForCurrentUserCommand request, CancellationToken cancellationToken)
        {
            var username = _currentUserService.UserId;
            var contactResult = await _userService.GetContact(username);

            if (contactResult.Contact?.UserSettings?.HealthSafety == null || contactResult.Contact?.UserSettings?.PrivacyStatement == null)
            {
                throw new Exception("Can't create an access request without accepting Privacy Act statement and Health and Safety measures");
            }

            var floorId = request.AccessRequest.Floor.Id;
            var date = request.AccessRequest.StartTime;
            var requestContactCount = (request.AccessRequest.Visitors?.Count ?? 0) + 1;

            var results = await _mediator.Send(new GetSpotsAvailablePerHourQuery { FloorId = floorId, SelectedDay = date });

            var hasAvailableCapacity = results.Where(x => x.Hour >= request.AccessRequest.StartTime.Hour && x.Hour < request.AccessRequest.EndTime.Hour)
                    .All(x => x.Capacity - x.SpotsReserved - requestContactCount >= 0);

            if (!hasAvailableCapacity)
            {
                throw new Exception("Your request exceeds the floor capacity");
            }    

            request.AccessRequest.Employee = new Contact
            {
                Id = contactResult.Contact.Id,
                FirstName = contactResult.Contact.FirstName,
                LastName = contactResult.Contact.LastName
            };

            await _accessRequestService.CreateAccessRequest(request.AccessRequest);

            return Unit.Value;
        }
    }
}