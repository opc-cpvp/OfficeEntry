using MagicOnion;
using MagicOnion.Server;
using MediatR;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;
using OfficeEntry.Domain.Enums;
using OfficeEntry.Domain.Services;
using OfficeEntry.Domain.ViewModels;
using static OfficeEntry.Domain.Entities.AccessRequest;

namespace OfficeEntry.Application.AccessRequests.Queries.GetAccessRequest;

//public record GetAccessRequestQuery : IRequest<AccessRequestViewModel>
//{
//    public Guid AccessRequestId { get; init; }
//    public string Locale { get; init; }
//}

public class GetAccessRequestQueryHandler : ServiceBase<IGetAccessRequestQueryService>,
    IGetAccessRequestQueryService,
    IRequestHandler<GetAccessRequestQuery, AccessRequestViewModel>
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
            CreatedOn = result.AccessRequest.CreatedOn,
            BuildingId = result.AccessRequest.Building.Id,
            IsDelegate = result.AccessRequest.Delegate?.Id == userResult.UserId,
            IsEmployee = result.AccessRequest.Employee.Id == userResult.UserId,
            DelegateId = result.AccessRequest.Delegate?.Id,
            EmployeeId = result.AccessRequest.Employee.Id,
            EmployeeFirstName = result.AccessRequest.Employee.FirstName,
            EmployeeLastName = result.AccessRequest.Employee.LastName,
            EmployeeName = result.AccessRequest.Employee.FullName,
            ManagerName = result.AccessRequest.Manager?.FullName,
            FloorPlanId = result.AccessRequest.FloorPlan?.Id ?? Guid.Empty,
            Building = (request.Locale == Locale.French) ? result.AccessRequest.Building.FrenchName : result.AccessRequest.Building.EnglishName,
            BuildingEnglishName = result.AccessRequest.Building.EnglishName,
            BuildingFrenchName = result.AccessRequest.Building.FrenchName,
            FloorId = result.AccessRequest.Floor.Id,
            Floor = (request.Locale == Locale.French) ? result.AccessRequest.Floor.FrenchName : result.AccessRequest.Floor.EnglishName,
            FloorEnglishName = result.AccessRequest.Floor.EnglishName,
            FloorFrenchName = result.AccessRequest.Floor.FrenchName,
            Details = result.AccessRequest.Details,
            StartTime = result.AccessRequest.StartTime,
            EndTime = result.AccessRequest.EndTime,
            Reason = result.AccessRequest.Reason?.Value,
            Workspace = result.AccessRequest.Workspace?.Name,
            Status = (ApprovalStatus)result.AccessRequest.Status.Key
        };
    }

    public async UnaryResult<AccessRequestViewModel> HandleAsync(GetAccessRequestQuery request)
    {
        return await Handle(request, new CancellationToken());
    }
}
