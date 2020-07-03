using Microsoft.AspNetCore.Components;
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

        public bool IsApproved => accessRequest.Status.Key == (int)Domain.Entities.AccessRequest.ApprovalStatus.Approved;
        public bool IsCancelled => accessRequest.Status.Key == (int)Domain.Entities.AccessRequest.ApprovalStatus.Cancelled;
        public bool IsDeclined => accessRequest.Status.Key == (int)Domain.Entities.AccessRequest.ApprovalStatus.Declined;

        private Domain.Entities.AccessRequest accessRequest;

        protected override async Task OnInitializedAsync()
        {
            var result = await Http.GetFromJsonAsync<Application.AccessRequests.Queries.GetAccessRequest.AccessRequestViewModel>($"api/AccessRequests/{Id}");
            IsEmployee = result.IsEmployee;
            IsManager = result.IsManager;
            accessRequest = result.AccessRequest;
        }

        private async Task ApproveRequest()
        {
            accessRequest.Status.Key = (int)Domain.Entities.AccessRequest.ApprovalStatus.Approved;
            await Http.PostAsJsonAsync("api/accessrequests/update", accessRequest);
            accessRequest.Status.Value = "Approved";
        }

        private async Task CancelRequest()
        {
            accessRequest.Status.Key = (int)Domain.Entities.AccessRequest.ApprovalStatus.Cancelled;
            await Http.PostAsJsonAsync("api/accessrequests/update", accessRequest);
            accessRequest.Status.Value = "Cancelled";
        }

        private async Task DeclineRequest()
        {
            accessRequest.Status.Key = (int)Domain.Entities.AccessRequest.ApprovalStatus.Declined;
            await Http.PostAsJsonAsync("api/accessrequests/update", accessRequest);
            accessRequest.Status.Value = "Declined";
        }
    }
}
