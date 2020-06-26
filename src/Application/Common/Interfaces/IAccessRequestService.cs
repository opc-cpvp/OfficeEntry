using OfficeEntry.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OfficeEntry.Application.Common.Interfaces
{
    public interface IAccessRequestService
    {
        Task<(Result Result, Domain.Entities.AccessRequest AccessRequest)> GetAccessRequestFor(Guid contactId, Guid accessRequestId);
        Task<(Result Result, IEnumerable<Domain.Entities.AccessRequest> AccessRequests)> GetAccessRequestsFor(Guid contactId);
        Task<Result> CreateAccessRequest(Domain.Entities.AccessRequest accessRequest);
    }
}
