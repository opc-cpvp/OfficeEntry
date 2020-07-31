using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using OfficeEntry.Application.AccessRequests.Commands.UpdateAccessRequestRequests;
using OfficeEntry.Application.AccessRequests.Queries.GetAccessRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace OfficeEntry.WebApp.Pages
{
    [Authorize]
    public partial class AccessRequest : ComponentBase
    {
        [Inject]
        public HttpClient Http { get; set; }

        [Parameter]
        public Guid Id { get; set; }

        [Inject]
        public IMediator Mediator { get; set; }

        public bool IsEmployee { get; set; }
        public bool IsManager { get; set; }

        public bool IsApproved => accessRequest.Status  == Domain.Entities.AccessRequest.ApprovalStatus.Approved;
        public bool IsCancelled => accessRequest.Status == Domain.Entities.AccessRequest.ApprovalStatus.Cancelled;
        public bool IsDeclined => accessRequest.Status  == Domain.Entities.AccessRequest.ApprovalStatus.Declined;

        private AccessRequestViewModel accessRequest;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                return;
            }

            var result = await Mediator.Send(new GetAccessRequestQuery { AccessRequestId = Id });
            IsEmployee = result.IsEmployee;
            IsManager = result.IsManager;
            accessRequest = result;

            StateHasChanged();

            await base.OnAfterRenderAsync(firstRender);
        }

        private async Task ApproveRequest()
        {
            accessRequest.Status = Domain.Entities.AccessRequest.ApprovalStatus.Approved;

            var accessRequestMessage = new Domain.Entities.AccessRequest
            {
                Id = accessRequest.Id,
                Status = new Domain.Entities.OptionSet { Key = (int)accessRequest.Status }
            };
            await Mediator.Send(new UpdateAccessRequestCommand { AccessRequest = accessRequestMessage });
        }

        private async Task CancelRequest()
        {
            accessRequest.Status = Domain.Entities.AccessRequest.ApprovalStatus.Cancelled;

            var accessRequestMessage = new Domain.Entities.AccessRequest
            {
                Id = accessRequest.Id,
                Status = new Domain.Entities.OptionSet { Key = (int)accessRequest.Status }
            };
            await Mediator.Send(new UpdateAccessRequestCommand { AccessRequest = accessRequestMessage });
        }

        private async Task DeclineRequest()
        {
            accessRequest.Status = Domain.Entities.AccessRequest.ApprovalStatus.Declined;

            var accessRequestMessage = new Domain.Entities.AccessRequest
            {
                Id = accessRequest.Id,
                Status = new Domain.Entities.OptionSet { Key = (int)accessRequest.Status }
            };

            await Mediator.Send(new UpdateAccessRequestCommand { AccessRequest = accessRequestMessage });
        }
    }
}
