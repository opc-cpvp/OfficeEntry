using OfficeEntry.Application.Common.Models;
using OfficeEntry.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OfficeEntry.Application.Common.Interfaces
{
    public interface IAccessRequestService
    {
        Task<(Result Result, AccessRequest AccessRequest)> GetAccessRequestFor(Guid contactId, Guid accessRequestId);

        Task<(Result Result, IEnumerable<AccessRequest> AccessRequests)> GetAccessRequestsFor(Guid contactId);

        Task<(Result Result, IEnumerable<AccessRequest> AccessRequests)> GetManagerAccessRequestsFor(Guid contactId);

        Task<Result> CreateAccessRequest(AccessRequest accessRequest);

        Task<Result> UpdateAccessRequest(AccessRequest accessRequest);
    }
}