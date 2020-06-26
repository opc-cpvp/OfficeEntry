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

        public async Task<(Result Result, AccessRequest AccessRequest)> GetAccessRequestFor(Guid contactId, Guid accessRequestId)
        {
            var accessRequest = await Client.For<gc_accessrequest>()
                .Key(accessRequestId)
                .Expand(a => new { a.gc_employee, a.gc_building, a.gc_floor, a.gc_manager, a.gc_accessrequest_contact_visitors })
                .FindEntryAsync();

            // TODO: Ensure that the user is only able to view the request if they're the employee / manager

            return (Result.Success(), gc_accessrequest.Convert(accessRequest));
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

        public async Task<(Result Result, IEnumerable<AccessRequest> AccessRequests)> GetManagerAccessRequestsFor(Guid contactId)
        {
            var accessRequests = await Client.For<gc_accessrequest>()
                .Filter(a => a.statecode == (int)StateCode.Active)
                .Filter(a => a.gc_manager.contactid == contactId)
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

        public async Task<Result> CreateAccessRequest(AccessRequest accessRequest)
        {
            var request = new gc_accessrequest {
                gc_accessrequestid = Guid.NewGuid(),
                gc_name = "new submission",
                gc_accessreason = (AccessReasons)accessRequest.Reason.Key,
                gc_approvalstatus = ApprovalStatus.Pending,
                gc_building = new gc_building { gc_buildingid = accessRequest.Building.Id },
                gc_details = accessRequest.Details,
                gc_employee = new contact { contactid = accessRequest.Employee.Id },
                gc_endtime = accessRequest.EndTime,
                gc_floor = new gc_floor { gc_floorid = accessRequest.Floor.Id },
                gc_manager = new contact { contactid = accessRequest.Manager.Id },
                gc_starttime = accessRequest.StartTime
            };

            await Client.For<gc_accessrequest>()
                .Set(request)
                .InsertEntryAsync();

            // TODO: Create / link visitors

            return Result.Success();
        }

        public async Task<Result> UpdateAccessRequest(AccessRequest accessRequest)
        {
            await Client.For<gc_accessrequest>()
                .Key(accessRequest.Id)
                .Set(new {
                    gc_approvalstatus = (ApprovalStatus)accessRequest.Status.Key
                })
                .UpdateEntryAsync();

            return Result.Success();
        }
    }
}
