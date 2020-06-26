﻿using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OfficeEntry.Application.AccessRequests.Commands.UpdateAccessRequestRequests
{
    public class UpdateAccessRequestCommand : IRequest
    {
        public AccessRequest AccessRequest { get; set; }
    }

    public class UpdateAccessRequestCommandHandler : IRequestHandler<UpdateAccessRequestCommand>
    {
        private readonly IAccessRequestService _accessRequestService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserService _userService;

        public UpdateAccessRequestCommandHandler(IAccessRequestService accessRequestService, ICurrentUserService currentUserService, IUserService userService)
        {
            _accessRequestService = accessRequestService;
            _currentUserService = currentUserService;
            _userService = userService;
        }

        public async Task<Unit> Handle(UpdateAccessRequestCommand request, CancellationToken cancellationToken)
        {
            var username = _currentUserService.UserId;
            var userResult = await _userService.GetUserId(username);

            await _accessRequestService.UpdateAccessRequest(request.AccessRequest);

            return Unit.Value;
        }
    }
}
