using MagicOnion;
using MediatR;
using MemoryPack;
using OfficeEntry.Domain.Entities;
using OfficeEntry.Domain.ValueObjects;
using OfficeEntry.Domain.ViewModels;
using System.Collections.Immutable;

namespace OfficeEntry.Domain.Services;

/*
 * AccessRequests
 */


[MemoryPackable]
public partial record CreateAccessRequestCommand : IRequest<Result>
{
    public string BaseUrl { get; init; }
    public AccessRequest AccessRequest { get; init; }
}
public interface ICreateAccessRequestCommandService : IService<ICreateAccessRequestCommandService>
{
    UnaryResult HandleAsync(CreateAccessRequestCommand request);
}



[MemoryPackable]
public partial record UpdateAccessRequestCommand : IRequest
{
    public string BaseUrl { get; init; }
    public AccessRequest AccessRequest { get; init; }
}

public interface IUpdateAccessRequestCommandService : IService<IUpdateAccessRequestCommandService>
{
    UnaryResult HandleAsync(UpdateAccessRequestCommand request);
}

[MemoryPackable]
public partial record GetAccessRequestQuery : IRequest<AccessRequestViewModel>
{
    public Guid AccessRequestId { get; init; }
    public string Locale { get; init; }
}

public interface IGetAccessRequestQueryService : IService<IGetAccessRequestQueryService>
{
    UnaryResult<AccessRequestViewModel> HandleAsync(GetAccessRequestQuery request);
}






[MemoryPackable]
public partial record GetAccessRequestPerFloorPlanQuery : IRequest<ImmutableArray<AccessRequest>>
{
    public Guid FloorPlanId { get; init; }
    public DateOnly Date { get; init; }
}
public interface IGetAccessRequestPerFloorPlanQueryService : IService<IGetAccessRequestPerFloorPlanQueryService>
{
    UnaryResult<AccessRequest[]> HandleAsync(GetAccessRequestPerFloorPlanQuery request);
}






[MemoryPackable]
public partial record GetAccessRequestsQuery : IRequest<IEnumerable<Domain.Entities.AccessRequest>>
{
    public static readonly GetAccessRequestsQuery Instance = new();

    private GetAccessRequestsQuery()
    {
    }
}

public interface IGetAccessRequestsQueryService : IService<IGetAccessRequestsQueryService>
{
    UnaryResult<AccessRequest[]> HandleAsync(GetAccessRequestsQuery request);
}




[MemoryPackable]
public partial record GetDelegateAccessRequestsQuery : IRequest<IEnumerable<Domain.Entities.AccessRequest>>
{
    public static readonly GetDelegateAccessRequestsQuery Instance = new();

    private GetDelegateAccessRequestsQuery()
    {
    }
}

public interface IGetDelegateAccessRequestsQueryService : IService<IGetDelegateAccessRequestsQueryService>
{
    UnaryResult<AccessRequest[]> HandleAsync(GetDelegateAccessRequestsQuery request);
}

/*
 * Locations
 */
[MemoryPackable]
public partial record UpdateFloorPlanCommand : IRequest
{
    public FloorPlan FloorPlan { get; init; }
}

public interface IUpdateFloorPlanCommandService : IService<IUpdateFloorPlanCommandService>
{
    UnaryResult HandleAsync(UpdateFloorPlanCommand request);
}




[MemoryPackable]
public partial record GetAvailableWorkspacesQuery : IRequest<IEnumerable<AvailableWorkspaceViewModel>>
{
    public string Locale { get; init; }
    public Guid FloorPlanId { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime EndTime { get; init; }
}
public interface IGetAvailableWorkspacesQueryService : IService<IGetAvailableWorkspacesQueryService>
{
    UnaryResult<AvailableWorkspaceViewModel[]> HandleAsync(GetAvailableWorkspacesQuery request);
}






[MemoryPackable]
public partial record GetFloorPlanQuery : IRequest<FloorPlan>
{
    public Guid FloorPlanId { get; init; }
}
public interface IGetFloorPlanQueryService : IService<IGetFloorPlanQueryService>
{
    UnaryResult<FloorPlan> HandleAsync(GetFloorPlanQuery request);
}




[MemoryPackable]
public partial record GetFloorPlansQuery : IRequest<ImmutableArray<FloorPlan>>
{
    public string Keyword { get; init; }
}
public interface IGetFloorPlansQueryService : IService<IGetFloorPlansQueryService>
{
    UnaryResult<FloorPlan[]> HandleAsync(GetFloorPlansQuery request);
}

/*
 * User
 */

[MemoryPackable]
public partial record UpdateHealthAndSafetyMeasuresStatementCommand : IRequest
{
    public bool IsHealthAndSafetyMeasuresAccepted { get; init; }
}
public interface IUpdateHealthAndSafetyMeasuresStatementCommandService : IService<IUpdateHealthAndSafetyMeasuresStatementCommandService>
{
    UnaryResult HandleAsync(UpdateHealthAndSafetyMeasuresStatementCommand request);
}



[MemoryPackable]
public partial record UpdatePrivacyActStatementCommand : IRequest
{
    public bool IsPrivacyActStatementAccepted { get; init; }
}
public interface IUpdatePrivacyActStatementCommandService : IService<IUpdatePrivacyActStatementCommandService>
{
    UnaryResult HandleAsync(UpdatePrivacyActStatementCommand request);
}




[MemoryPackable]
public partial record GetIsContactFirstResponderQuery() : IRequest<bool>
{
    public string UserId { get; init; }
}
public interface IGetIsContactFirstResponderQueryService : IService<IGetIsContactFirstResponderQueryService>
{
    UnaryResult<bool> HandleAsync(GetIsContactFirstResponderQuery request);
}




[MemoryPackable]
public partial record GetTermsAndConditionsQuery : IRequest<TermsAndConditions>
{
    public static readonly GetTermsAndConditionsQuery Instance = new();

    private GetTermsAndConditionsQuery()
    {
    }
}

public interface IGetTermsAndConditionsQueryService : IService<IGetTermsAndConditionsQueryService>
{
    UnaryResult<TermsAndConditions> HandleAsync(GetTermsAndConditionsQuery request);
}

/*
 * Users
 */

[MemoryPackable]
public partial record GetContactsQuery : IRequest<IEnumerable<Domain.Entities.Contact>>
{
    public static readonly GetContactsQuery Instance = new();

    private GetContactsQuery()
    {
    }
}
public interface IGetContactsQueryService : IService<IGetContactsQueryService>
{
    UnaryResult<Contact[]> HandleAsync(GetContactsQuery request);
}



public class SpyingEventArg : EventArgs
{
    public string Victim { get; init; }
    public string Workspace { get; init; }
    public string FullName { get; set; }
    public string UserId { get; set; }
}

/// <summary>
/// Service for handling workspace spying activities
/// </summary>
public interface IRecordSpyingEventService : IService<IRecordSpyingEventService>
{
    /// <summary>
    /// Records a workspace spying event
    /// </summary>
    /// <param name="event">Spying event information</param>
    /// <param name="selectedDate">The date of the spying event</param>
    /// <returns>A Task representing the asynchronous operation</returns>
    UnaryResult HandleAsync(SpyingEventArg @event, DateOnly selectedDate);
}