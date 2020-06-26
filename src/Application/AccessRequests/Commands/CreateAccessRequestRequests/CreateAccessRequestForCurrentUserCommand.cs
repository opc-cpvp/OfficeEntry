using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
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

        public CreateAccessRequestForCurrentUserCommandHandler(IAccessRequestService accessRequestService, ICurrentUserService currentUserService, IUserService userService)
        {
            _accessRequestService = accessRequestService;
            _currentUserService = currentUserService;
            _userService = userService;
        }

        public async Task<Unit> Handle(CreateAccessRequestForCurrentUserCommand request, CancellationToken cancellationToken)
        {
            var username = _currentUserService.UserId;
            var userResult = await _userService.GetUserId(username);

            // TODO: Should this be set here?
            request.AccessRequest.Employee = new Domain.Entities.Contact { Id = userResult.UserId };

            await _accessRequestService.CreateAccessRequest(request.AccessRequest);

            return Unit.Value;
        }
    }
}
