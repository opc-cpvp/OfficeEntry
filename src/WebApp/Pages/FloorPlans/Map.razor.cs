using Fluxor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json.Linq;
using OfficeEntry.Domain.Entities;
using OfficeEntry.WebApp.Shared;
using OfficeEntry.WebApp.Store.FloorPlanUseCases.Map;
using System.Text.Json;
using Newtonsoft.Json;
using OfficeEntry.WebApp.Models;
using static OfficeEntry.WebApp.Pages.FloorPlans.MapJsInterop;
using JsonSerializer = System.Text.Json.JsonSerializer;
using MediatR;
using OfficeEntry.Application.AccessRequests.Commands.CreateAccessRequestRequests;
using OfficeEntry.WebApp.Store.ManagerApprovalsUseCase;
using OfficeEntry.WebApp.Store.MyAccessRequestsUseCase;
using Microsoft.Extensions.Localization;

namespace OfficeEntry.WebApp.Pages.FloorPlans;

[Authorize]
public partial class Map : IAsyncDisposable
{
    [Parameter] public Guid FloorPlanId { get; set; }

    [Inject] private IStringLocalizer<App> Localizer { get; set; }
    [Inject] private ILogger<Map> Logger { get; set; }
    [Inject] private IDispatcher Dispatcher { get; set; }
    [Inject] private IMediator Mediator { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }
    [Inject] private IState<MapState> MapState { get; set; }
    [Inject] private IMapJsInterop MapJsInterop { get; set; }

    private Survey mySurvey;
    private Domain.Entities.AccessRequest _selectedAccessRequest;
    private DateOnly _selectedDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
    private int _startTime = 9;
    private int _endTime = 17;
    private FloorPlan FloorPlanDto { get; set; } // ViewModel
    private IEnumerable<Domain.Entities.AccessRequest> AccessRequests { get; set; } // ViewModel

    public bool SurveyCompleted { get; set; }

    protected override async Task OnInitializedAsync()
    {
        SubscribeToAction<GetMapResultAction>(async x =>
        {
            FloorPlanDto = x.Dto;
            AccessRequests = x.AccessRequests;

            await UpdateCanvas();
        });

        await base.OnInitializedAsync();
    }

    protected override Task OnParametersSetAsync()
    {
        // Invalidate the DTO and reload the Map
        FloorPlanDto = null;
        Dispatcher.Dispatch(new GetMapAction(FloorPlanId, _selectedDate));

        return base.OnParametersSetAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        await MapJsInterop.Register(this);
    }

    private async Task UpdateCanvas()
    {
        var id = await GetSelectedWorkspaceId(mySurvey);
        var accessRequests = AccessRequests.Where(a =>
            a.StartTime < _selectedDate.ToDateTime(new TimeOnly(hour: _endTime, minute: 0)) &&
            a.EndTime > _selectedDate.ToDateTime(new TimeOnly(hour: _startTime, minute: 0))
        );

        var circles = FloorPlanDto.Workspaces
            .Select(x => new
            {
                Id = x.Id,
                Name = x.Name,
                Position = new { Left = x.X, Top = x.Y },
                Selected = x.Id == id, // Check if the circle should be selected when changing the date
                Taken = accessRequests.Any(a => a.Workspace.Id == x.Id),
                EmployeeFullName = accessRequests.FirstOrDefault(a => a.Workspace.Id == x.Id)?.Employee.FullName ?? string.Empty,
            });

        var circlesJson = JsonSerializer.Serialize(circles.ToArray());

        await MapJsInterop.Start(FloorPlanDto.FloorPlanImage, circlesJson);

        _selectedAccessRequest = accessRequests.FirstOrDefault(x => x.Workspace.Id == id);

        StateHasChanged();
    }

    // Select survey data to check if a circle should be selected
    internal static async Task<Guid> GetSelectedWorkspaceId(Survey survey)
    {
        if (survey is null)
        {
            return Guid.Empty;
        }

        var data = await survey.GetData();

        return ExtractWorkspaceId(data);
    }

    internal static Guid ExtractWorkspaceId(string surveyData)
    {
        var id = JToken.Parse(surveyData)
            .FirstOrDefault(x => x.Path is "workspace")
            ?.FirstOrDefault()?.Value<string>() ?? Guid.Empty.ToString();

        return Guid.Parse(id);
    }

    public async ValueTask DisposeAsync()
    {
        Dispose();

        await MapJsInterop.Stop();
    }


    // TODO: rename method to something like `OccupiedWorkspaceLookup`
    public async Task OnSpying(SpyingEventArg e)
    {
        Logger.LogInformation("OfficeEntry UserSpying: {UserName} {Name} {Date} {Workspace} {Victim}",
            e.UserId,
            e.FullName,
            _selectedDate.ToString("yyyy-MM-dd"),
            e.Workspace,
            e.Victim);

        await Task.CompletedTask;
    }

    public async Task OnSelectedCircleChanged(string data)
    {
        var accessRequests = AccessRequests.Where(a =>
            a.StartTime < _selectedDate.ToDateTime(new TimeOnly(hour: _endTime, minute: 0)) &&
            a.EndTime > _selectedDate.ToDateTime(new TimeOnly(hour: _startTime, minute: 0))
        );
        var circle = Newtonsoft.Json.JsonConvert.DeserializeObject<OfficeEntry.WebApp.Pages.FloorPlans.Edit.Circle>(data);

        _selectedAccessRequest = accessRequests.FirstOrDefault(x => x.Workspace.Id == circle.Id);
        if (_startTime != _endTime && _selectedAccessRequest is null)
        {
            await mySurvey.SetValueAsync("workspace", circle.Id.ToString());
        }

        StateHasChanged();

        await Task.CompletedTask;
    }

    public async Task OnSurveyCompleted(SurveyCompletedEventArgs e)
    {
        SurveyCompleted = true;

        var submission = JsonConvert.DeserializeObject<AccessRequestSubmission>(e.SurveyResult);

        var accessRequest = new Domain.Entities.AccessRequest
        {
            AssetRequests = new List<AssetRequest>(),
            Building = FloorPlanDto.Building,
            Floor = FloorPlanDto.Floor,
            FloorPlan = FloorPlanDto,
            EndTime = submission.startDate.AddHours(submission.endTime),
            StartTime = submission.startDate.AddHours(submission.startTime),
            Status = new OptionSet { Key = (int)Domain.Entities.AccessRequest.ApprovalStatus.Pending },
            Visitors = new List<Contact>(),
            Workspace = new Workspace { Id = submission.workspace }
        };

        var isDelegate = submission.otherIndividual != Guid.Empty;
        if (isDelegate)
        {
            accessRequest.Employee = new Contact { Id = submission.otherIndividual };
        }

        await Mediator.Send(new CreateAccessRequestCommand { AccessRequest = accessRequest });

        Dispatcher.Dispatch(new GetMyAccessRequestsAction());
        Dispatcher.Dispatch(new GetManagerApprovalsAction());

        NavigationManager.NavigateTo(Localizer["my-access-requests"]);
    }

    public async Task OnValueChanged(ValueChangedEventArgs e)
    {
        var options = JsonSerializer
            .Deserialize<SurveyQuestion>(
                json: e.Options,
                options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (options.Name is "startDate")
        {
            _selectedDate = DateOnly.Parse(options.Value);
            Dispatcher.Dispatch(new GetMapAction(FloorPlanId, _selectedDate));
            _selectedAccessRequest = null;

            StateHasChanged();
        }

        if (options.Name is "startTime")
        {
            if (options.Value is null)
            {
                _startTime = _endTime;
                await mySurvey.SetValueAsync("workspace", null);
                return;
            }

            _startTime = int.Parse(options.Value);
            await UpdateCanvas();
        }

        if (options.Name is "endTime")
        {
            if (options.Value is null)
            {
                _endTime = _startTime;
                await mySurvey.SetValueAsync("workspace", null);
                return;
            }

            _endTime = int.Parse(options.Value);
            await UpdateCanvas();
        }

        if (options.Name is "workspace")
        {
            await MapJsInterop.SetSelectedCircle(options.Value);

            if (options.Value is null)
            {
                return;
            }

            await UpdateCanvas();
        }

        await Task.CompletedTask;
    }
}

public class SurveyQuestion
{
    public string Name { get; set; }
    public string Value { get; set; }
}
