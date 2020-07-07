using Newtonsoft.Json.Linq;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Application.Common.Models;
using OfficeEntry.Domain.Entities;
using OfficeEntry.Infrastructure.Services.Xrm.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OfficeEntry.Infrastructure.Services.Xrm
{
    public class AccessRequestService : XrmService, IAccessRequestService
    {
        public readonly IHttpClientFactory _httpClientFactory;

        public AccessRequestService(IHttpClientFactory httpClientFactory)
            : base(httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<(Result Result, AccessRequest AccessRequest)> GetAccessRequest(Guid accessRequestId)
        {
            var accessRequest = await Client.For<gc_accessrequest>()
                .Key(accessRequestId)
                .Expand(a => new { a.gc_employee, a.gc_building, a.gc_floor, a.gc_manager, a.gc_accessrequest_contact_visitors, a.gc_accessrequest_assetrequest })
                .FindEntryAsync();

            return (Result.Success(), gc_accessrequest.Convert(accessRequest));
        }

        public async Task<(Result Result, IEnumerable<AccessRequest> AccessRequests)> GetAccessRequestsFor(Guid contactId)
        {
            var accessRequests = await Client.For<gc_accessrequest>()
                .Filter(a => a.statecode == (int)StateCode.Active)
                .Filter(a => a.gc_employee.contactid == contactId)
                .Expand(a => new { a.gc_employee, a.gc_building, a.gc_floor, a.gc_manager, a.gc_accessrequest_assetrequest })
                .OrderByDescending(a => a.gc_starttime)
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
                .Expand(a => new { a.gc_employee, a.gc_building, a.gc_floor, a.gc_manager, a.gc_accessrequest_assetrequest })
                .OrderByDescending(a => a.gc_starttime)
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
            var access = gc_accessrequest.MapFrom(accessRequest);
            access.gc_accessrequestid = Guid.NewGuid();

            access = await Client.For<gc_accessrequest>()
                .Set(access)
                .InsertEntryAsync();

            var visitors = await GetContactForVisitors().ToListAsync();

            await AssociateAccessRequestWithVisitors(access, visitors);

            await InsertAssetRequests();

            return Result.Success();

            async Task InsertAssetRequests()
            {
                var assetRequests = accessRequest
                    .AssetRequests
                    .Select(assetRequest => new gc_assetrequest
                    {
                        gc_assetrequestid = Guid.NewGuid(),
                        gc_name           = $"{access.gc_accessrequestid} - {Enum.GetName(typeof(Domain.Enums.Asset), assetRequest.Asset.Key)}",
                        gc_assetsid       = access,
                        gc_asset          = (Asset)assetRequest.Asset.Key,
                        gc_other          = assetRequest.Other
                    });

                foreach (var assetRequest in assetRequests)
                {
                    await Client
                        .For<gc_assetrequest>()
                        .Set(assetRequests)
                        .InsertEntryAsync();
                }
            }
            
            async IAsyncEnumerable<contact> GetContactForVisitors()
            {
                var visitors = accessRequest.Visitors.Select(x => contact.MapFrom(x)).ToList();

                foreach (var visitor in visitors)
                {
                    var contacts = await Client.For<contact>()
                        .Filter(x => x.emailaddress1 == visitor.emailaddress1)
                        .FindEntriesAsync();

                    var contact = contacts.FirstOrDefault();

                    // If no contact has been found, let's create it.
                    if (contact is null)
                    {
                        visitor.contactid = Guid.NewGuid();

                        contact = await Client.For<contact>()
                            .Set(visitor)
                            .InsertEntryAsync();
                    }

                    yield return contact;
                }
            }

            /// <remarks>
            /// Unfortunately, this is the only supported way of associating existing entity instances.
            /// 
            /// For more information, please see:
            /// https://docs.microsoft.com/en-us/powerapps/developer/common-data-service/webapi/samples/basic-operations-csharp
            /// https://himbap.com/blog/?p=2063
            /// </remarks>
            async Task AssociateAccessRequestWithVisitors(gc_accessrequest accessRequest, IEnumerable<contact> visitors)
            {
                using var httpClient = _httpClientFactory.CreateClient(NamedHttpClients.Dynamics365ServiceDesk);

                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");

                var accessRequestId = accessRequest.gc_accessrequestid.ToString("D");

                var relationshipObjects = visitors
                    .Select(x => x.contactid.ToString("D"))
                    .Select(id => new JObject
                    {
                    { "@odata.id", new Uri(httpClient.BaseAddress + $"contacts({id})") }
                    })
                    .Select(x => x.ToString());

                foreach (var relationshipObject in relationshipObjects)
                {
                    using var content = new StringContent(relationshipObject.ToString(), Encoding.UTF8, "application/json");

                    using var response = await httpClient.PostAsync($"gc_accessrequests({accessRequestId})/gc_accessrequest_contact_visitors/$ref", content);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception(response.ToString());
                    }
                }
            }
        }

        public async Task<Result> UpdateAccessRequest(AccessRequest accessRequest)
        {
            await Client.For<gc_accessrequest>()
                .Key(accessRequest.Id)
                .Set(new
                {
                    gc_approvalstatus = (ApprovalStatus)accessRequest.Status.Key
                })
                .UpdateEntryAsync();

            return Result.Success();
        }
    }
}