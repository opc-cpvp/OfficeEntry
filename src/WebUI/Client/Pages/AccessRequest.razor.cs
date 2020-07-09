using Microsoft.AspNetCore.Components;
using OfficeEntry.Application.AccessRequests.Queries.GetAccessRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace OfficeEntry.WebUI.Client.Pages
{
    public partial class AccessRequest : ComponentBase
    {
        [Inject]
        public HttpClient Http { get; set; }

        [Parameter]
        public Guid Id { get; set; }

        public bool IsEmployee { get; set; }
        public bool IsManager { get; set; }

        public bool IsApproved => accessRequest.Status  == Domain.Entities.AccessRequest.ApprovalStatus.Approved;
        public bool IsCancelled => accessRequest.Status == Domain.Entities.AccessRequest.ApprovalStatus.Cancelled;
        public bool IsDeclined => accessRequest.Status  == Domain.Entities.AccessRequest.ApprovalStatus.Declined;

        private AccessRequestViewModel accessRequest;

        protected override async Task OnInitializedAsync()
        {
            var result = await Http.GetFromJsonAsync<AccessRequestViewModel>($"api/AccessRequests/{Id}");
            IsEmployee = result.IsEmployee;
            IsManager = result.IsManager;
            accessRequest = result;
        }

        private async Task ApproveRequest()
        {
            accessRequest.Status = Domain.Entities.AccessRequest.ApprovalStatus.Approved;

            var accessRequestMessage = new Domain.Entities.AccessRequest
            {
                Id = accessRequest.Id,
                Status = new Domain.Entities.OptionSet { Key = (int)accessRequest.Status }
            };
            await Http.PostAsJsonAsync("api/accessrequests/update", accessRequestMessage);
        }

        private async Task CancelRequest()
        {
            accessRequest.Status = Domain.Entities.AccessRequest.ApprovalStatus.Cancelled;

            var accessRequestMessage = new Domain.Entities.AccessRequest
            {
                Id = accessRequest.Id,
                Status = new Domain.Entities.OptionSet { Key = (int)accessRequest.Status }
            };
            await Http.PostAsJsonAsync("api/accessrequests/update", accessRequestMessage);
        }

        private async Task DeclineRequest()
        {
            accessRequest.Status = Domain.Entities.AccessRequest.ApprovalStatus.Declined;

            var accessRequestMessage = new Domain.Entities.AccessRequest
            {
                Id = accessRequest.Id,
                Status = new Domain.Entities.OptionSet { Key = (int)accessRequest.Status }
            };

            await Http.PostAsJsonAsync("api/accessrequests/update", accessRequestMessage);
        }
    }
}
