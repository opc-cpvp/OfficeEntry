using OfficeEntry.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OfficeEntry.Application.Common.Interfaces
{
    public interface IAccessRequestService
    {
        Task<(Result Result, IEnumerable<Domain.Entities.AccessRequest> AccessRequests)> GetAccessRequestsFor(Guid contactId);
    }
}
