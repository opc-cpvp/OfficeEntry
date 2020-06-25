using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Application.Common.Models;
using OfficeEntry.Domain.Entities;
using OfficeEntry.Infrastructure.Services.Xrm.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OfficeEntry.Infrastructure.Services.Xrm
{
    public class AccessRequestService : XrmService, IAccessRequestService
    {
        public AccessRequestService(IHttpClientFactory httpClientFactory)
            : base(httpClientFactory)
        {
        }

        public async Task<(Result Result, IEnumerable<AccessRequest> AccessRequests)> GetAccessRequestsFor(Guid contactId)
        {
            var accessRequests = await Client.For<gc_accessrequest>()
                .Filter(a => a.statecode == (int)StateCode.Active)
                .Filter(a => a.gc_employee.contactid == contactId)
                .Expand(a => new { a.gc_employee, a.gc_building, a.gc_floor, a.gc_manager })
                .FindEntriesAsync();

            accessRequests = accessRequests.ToList();

            foreach (var accessRequest in accessRequests.ToList())
            {
                var visitors = await Client.For<gc_accessrequest>()
                    .Key(accessRequest.gc_accessrequestid)
                    .NavigateTo(a => a.gc_accessrequest_contact_visitors)
                    .FindEntriesAsync();

                accessRequest.gc_accessrequest_contact_visitors = visitors.ToList();
            }

            return (Result.Success(), accessRequests.Select(a => gc_accessrequest.Convert(a)));
        }
    }
}
