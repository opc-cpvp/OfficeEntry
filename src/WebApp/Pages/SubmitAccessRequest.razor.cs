using Fluxor;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using OfficeEntry.Application.AccessRequests.Commands.CreateAccessRequestRequests;
using OfficeEntry.Application.AccessRequests.Queries.GetSpotsAvailablePerHour;
using OfficeEntry.Domain.Entities;
using OfficeEntry.Domain.Enums;
using OfficeEntry.WebApp.Models;
using OfficeEntry.WebApp.Shared;
using OfficeEntry.WebApp.Store.ManagerApprovalsUseCase;
using OfficeEntry.WebApp.Store.MyAccessRequestsUseCase;
using System.Text.RegularExpressions;

namespace OfficeEntry.WebApp.Pages;

[Authorize]
public partial class SubmitAccessRequest : ComponentBase
{
    [Inject] private NavigationManager NavigationManager { get; set; }
    [Inject] private IDispatcher Dispatcher { get; set; }
    [Inject] private IStringLocalizer<App> Localizer { get; set; }
    [Inject] private IMediator Mediator { get; set; }

    public bool SurveyCompleted { get; set; }
    public bool ShowSpotsAvailablePerHours { get; set; }
    public CurrentCapacity[] FloorCapacity { get; set; }

    [Inject] private IJSRuntime JSRuntime { get; set; }

    public async Task OnSurveyCompleted(SurveyCompletedEventArgs e)
    {
        SurveyCompleted = true;

        var submission = JsonConvert.DeserializeObject<AccessRequestSubmission>(e.SurveyResult);

        var accessRequest = new Domain.Entities.AccessRequest
        {
            AssetRequests = new List<AssetRequest>(),
            Building = new Building { Id = submission.building },
            Details = submission.details,
            EndTime = submission.startDate.AddHours(submission.endTime),
            Floor = new Floor { Id = submission.floor },
            Manager = new Contact { Id = submission.manager },
            Reason = new OptionSet
            {
                Key = submission.reason
            },
            StartTime = submission.startDate.AddHours(submission.startTime),
            Visitors = new List<Contact>()
        };

        if (submission.visitors != null)
        {
            foreach (var visitor in submission.visitors)
            {
                var match = Regex.Match(visitor.name.Trim(), @"^(?<first>[^\s]+)\s*(?<last>.*)$");

                var firstName = match.Groups["first"].Value;
                var lastName = match.Groups["last"].Value;

                accessRequest.Visitors.Add(
                    new Contact
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        EmailAddress = visitor.email,
                        PhoneNumber = visitor.telephone
                    }
                );
            }
        }

        accessRequest.AssetRequests.AddRange(Repeat((int)Asset.Chair, submission.chair));
        accessRequest.AssetRequests.AddRange(Repeat((int)Asset.Laptop, submission.laptop));
        accessRequest.AssetRequests.AddRange(Repeat((int)Asset.Tablet, submission.tablet));
        accessRequest.AssetRequests.AddRange(Repeat((int)Asset.Monitor, submission.monitor));
        accessRequest.AssetRequests.AddRange(Repeat((int)Asset.DockingStation, submission.dockingStation));
        accessRequest.AssetRequests.AddRange(Repeat((int)Asset.Keyboard, submission.keyboard));
        accessRequest.AssetRequests.AddRange(Repeat((int)Asset.Mouse, submission.mouse));
        accessRequest.AssetRequests.AddRange(Repeat((int)Asset.Cables, submission.cables));
        accessRequest.AssetRequests.AddRange(Repeat((int)Asset.Headset, submission.headset));
        accessRequest.AssetRequests.AddRange(Repeat((int)Asset.Printer, submission.printer));

        if (!string.IsNullOrWhiteSpace(submission.other))
        {
            accessRequest.AssetRequests.Add(new AssetRequest
            {
                Asset = new OptionSet
                {
                    Key = (int)Asset.Other
                },
                Other = submission.other
            });
        }

        await Mediator.Send(new CreateAccessRequestForCurrentUserCommand { AccessRequest = accessRequest });

        Dispatcher.Dispatch(new GetMyAccessRequestsAction());
        Dispatcher.Dispatch(new GetManagerApprovalsAction());

        NavigationManager.NavigateTo(Localizer["my-access-requests"]);

        static IEnumerable<AssetRequest> Repeat(int value, int count)
            => Enumerable.Repeat(new AssetRequest
            {
                Asset = new OptionSet
                {
                    Key = value
                }
            }, count);
    }

    public async Task OnPageChanged(PageChangedEventArgs e)
    {
        if (e.NewCurrentPageName is "page4")
        {
            var submission = JsonConvert.DeserializeObject<AccessRequestSubmission>(e.SurveyData);

            var floorId = submission.floor;
            var date = submission.startDate;

            var results = await Mediator.Send(new GetSpotsAvailablePerHourQuery { FloorId = floorId, SelectedDay = date });

            FloorCapacity = results.ToArray();

            ShowSpotsAvailablePerHours = true;

            return;
        }

        await Task.Run(() => ShowSpotsAvailablePerHours = e.NewCurrentPageName is "page4");
    }
}
