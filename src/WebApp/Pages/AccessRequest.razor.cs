using Fluxor;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using OfficeEntry.Application.AccessRequests.Commands.UpdateAccessRequestRequests;
using OfficeEntry.Application.AccessRequests.Queries.GetAccessRequest;
using OfficeEntry.Domain.Entities;
using OfficeEntry.Domain.Enums;
using OfficeEntry.WebApp.Store.AccessRequestsUseCase;
using OfficeEntry.WebApp.Store.DelegateAccessRequestsUseCase;
using System.Globalization;

namespace OfficeEntry.WebApp.Pages;

[Authorize]
public partial class AccessRequest
{
    [Parameter]
    public Guid Id { get; set; }

    [Inject]
    public IStringLocalizer<App> Localizer { get; set; }

    [Inject]
    public IMediator Mediator { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    private IDispatcher Dispatcher { get; set; }

    public bool IsDelegate { get; set; }
    public bool IsEmployee { get; set; }

    public bool IsApproved => accessRequest.Status == Domain.Entities.AccessRequest.ApprovalStatus.Approved;
    public bool IsCancelled => accessRequest.Status == Domain.Entities.AccessRequest.ApprovalStatus.Cancelled;
    public bool IsDeclined => accessRequest.Status == Domain.Entities.AccessRequest.ApprovalStatus.Declined;

    private AccessRequestViewModel accessRequest;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        var locale = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        locale = (locale == Locale.French) ? locale : Locale.English;

        var result = await Mediator.Send(new GetAccessRequestQuery { AccessRequestId = Id, Locale = locale });
        IsDelegate = result.IsDelegate;
        IsEmployee = result.IsEmployee;
        accessRequest = result;

        StateHasChanged();

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task CancelRequest()
    {
        accessRequest.Status = Domain.Entities.AccessRequest.ApprovalStatus.Cancelled;

        var accessRequestMessage = new Domain.Entities.AccessRequest
        {
            Id = accessRequest.Id,
            Employee = new Contact { Id = accessRequest.EmployeeId },
            FloorPlan = new FloorPlan
            {
                Id = accessRequest.FloorPlanId,
                Building = new Building
                {
                    Id = accessRequest.BuildingId,
                    EnglishName = accessRequest.BuildingEnglishName,
                    FrenchName = accessRequest.BuildingFrenchName
                },
                Floor = new Floor
                {
                    Id = accessRequest.FloorId,
                    EnglishName = accessRequest.FloorEnglishName,
                    FrenchName = accessRequest.FloorFrenchName
                }
            },
            StartTime = accessRequest.StartTime,
            Status = new OptionSet { Key = (int)accessRequest.Status }
        };
        await Mediator.Send(new UpdateAccessRequestCommand { AccessRequest = accessRequestMessage });

        if (IsEmployee)
        {
            Dispatcher.Dispatch(new GetAccessRequestsAction());
            NavigationManager.NavigateTo(Localizer["my-requests"]);
        }
        else if (IsDelegate)
        {
            Dispatcher.Dispatch(new GetDelegateAccessRequestsAction());
            NavigationManager.NavigateTo(Localizer["requests-for-others"]);
        }
    }
}
