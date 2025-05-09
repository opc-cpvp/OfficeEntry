using OfficeEntry.Domain.Entities;
using OfficeEntry.Domain.ValueObjects;
using System.Collections.Immutable;

namespace OfficeEntry.Application.Common.Interfaces;

public interface IAccessRequestService
{
    Task<(Result Result, AccessRequest AccessRequest)> CreateAccessRequest(AccessRequest accessRequest);
    Task<(Result Result, AccessRequest AccessRequest)> GetAccessRequest(Guid accessRequestId);
    Task<(Result Result, IEnumerable<AccessRequest> AccessRequests)> GetAccessRequestsFor(Guid contactId);
    Task<ImmutableArray<AccessRequest>> GetApprovedOrPendingAccessRequestsByFloorPlan(Guid floorPlanId, DateOnly date);
    Task<(Result Result, IEnumerable<AccessRequest> AccessRequests)> GetDelegateAccessRequestsFor(Guid contactId);
    Task<Result> UpdateAccessRequest(AccessRequest accessRequest);
}
