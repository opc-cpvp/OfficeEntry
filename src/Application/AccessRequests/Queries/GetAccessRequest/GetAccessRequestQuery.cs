﻿using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;
using OfficeEntry.Domain.Enums;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static OfficeEntry.Domain.Entities.AccessRequest;

namespace OfficeEntry.Application.AccessRequests.Queries.GetAccessRequest
{
    public class GetAccessRequestQuery : IRequest<AccessRequestViewModel>
    {
        public Guid AccessRequestId { get; set; }
        public string Locale { get; set; }
    }

    public class GetAccessRequestQueryHandler : IRequestHandler<GetAccessRequestQuery, AccessRequestViewModel>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IAccessRequestService _accessRequestService;
        private readonly IUserService _userService;

        public GetAccessRequestQueryHandler(ICurrentUserService currentUserService, IAccessRequestService accessRequestService, IUserService userService)
        {
            _currentUserService = currentUserService;
            _accessRequestService = accessRequestService;
            _userService = userService;
        }

        public async Task<AccessRequestViewModel> Handle(GetAccessRequestQuery request, CancellationToken cancellationToken)
        {
            var username = _currentUserService.UserId;
            var userResult = await _userService.GetUserId(username);

            // TODO: Ensure that the user is only able to view the request if they're the employee / manager
            var result = await _accessRequestService.GetAccessRequest(request.AccessRequestId);

            // TODO: what should we do with the result
            if (!result.Result.Succeeded)
            {
            }

            return new AccessRequestViewModel
            {
                Id = result.AccessRequest.Id,
                IsEmployee = result.AccessRequest.Employee.Id == userResult.UserId,
                IsManager = result.AccessRequest.Manager.Id == userResult.UserId,
                EmployeeName = result.AccessRequest.Employee.FullName,
                ManagerName = result.AccessRequest.Manager.FullName,
                Building = (request.Locale == Locale.French) ? result.AccessRequest.Building.FrenchName : result.AccessRequest.Building.EnglishName,
                Floor = (request.Locale == Locale.French) ? result.AccessRequest.Floor.FrenchName : result.AccessRequest.Floor.EnglishName,
                Details = result.AccessRequest.Details,
                StartTime = result.AccessRequest.StartTime,
                EndTime = result.AccessRequest.EndTime,
                Reason = result.AccessRequest.Reason.Value.ToString(),
                Status = (ApprovalStatus)result.AccessRequest.Status.Key,

                Visitors = result.AccessRequest.Visitors.Select(x => new Visitor
                {
                    FullName = x.FullName,
                    EmailAddress = x.EmailAddress,
                    PhoneNumber = x.PhoneNumber
                }).ToArray(),
                AssetRequests = result.AccessRequest.AssetRequests.ToArray()
            };
        }
    }
}