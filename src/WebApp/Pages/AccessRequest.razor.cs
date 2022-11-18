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

    public bool IsCancelled => accessRequest.Status == Domain.Entities.AccessRequest.ApprovalStatus.Cancelled;

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
            CreatedOn = accessRequest.CreatedOn,
            Employee = new Contact
            {
                Id = accessRequest.EmployeeId,
                FirstName = accessRequest.EmployeeFirstName,
                LastName = accessRequest.EmployeeLastName
            },
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
            },
            FloorPlan = new FloorPlan { Id = accessRequest.FloorPlanId },
            StartTime = accessRequest.StartTime,
            EndTime = accessRequest.EndTime,
            Status = new OptionSet { Key = (int)accessRequest.Status }
        };

        if (accessRequest.DelegateId.HasValue)
        {
            accessRequestMessage.Delegate = new Contact { Id = accessRequest.DelegateId.Value };
        }

        await Mediator.Send(new UpdateAccessRequestCommand
        {
            BaseUrl = NavigationManager.BaseUri,
            AccessRequest = accessRequestMessage
        });

        Dispatcher.Dispatch(new GetAccessRequestsAction());
        Dispatcher.Dispatch(new GetDelegateAccessRequestsAction());

        if (IsEmployee)
        {
            NavigationManager.NavigateTo(Localizer["my-requests"]);
        }
        else if (IsDelegate)
        {
            NavigationManager.NavigateTo(Localizer["requests-for-my-colleagues"]);
        }
    }
}
