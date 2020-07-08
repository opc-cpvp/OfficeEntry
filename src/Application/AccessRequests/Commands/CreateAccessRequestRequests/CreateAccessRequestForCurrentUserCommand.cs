using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

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
        private readonly IUserService _userService;

        public CreateAccessRequestForCurrentUserCommandHandler(IAccessRequestService accessRequestService, ICurrentUserService currentUserService, ILocationService locationService, IUserService userService)
        {
            _accessRequestService = accessRequestService;
            _currentUserService = currentUserService;
            _userService = userService;
        }

        public async Task<Unit> Handle(CreateAccessRequestForCurrentUserCommand request, CancellationToken cancellationToken)
        {
            var username = _currentUserService.UserId;
            var contactResult = await _userService.GetContact(username);

            if (contactResult.Contact?.UserSettings?.HealthSafety == null || contactResult.Contact?.UserSettings?.PrivacyStatement == null)
            {
                throw new Exception("Can't create an access request without accepting Privacy Act statement and Health and Safety measures");
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