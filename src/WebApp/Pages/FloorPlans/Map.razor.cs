using Fluxor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json.Linq;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Domain.Entities;
using OfficeEntry.WebApp.Shared;
using OfficeEntry.WebApp.Store.FloorPlanUseCases.Map;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text.Json;
using static OfficeEntry.WebApp.Pages.FloorPlans.MapJsInterop;

namespace OfficeEntry.WebApp.Pages.FloorPlans;

[Authorize]
public partial class Map : IAsyncDisposable
{
    private Survey mySurvey;
    private Domain.Entities.AccessRequest _selectedAccessRequest;
    private DateOnly _selectedDate = DateOnly.FromDateTime(DateTime.Now);
    [Parameter] public Guid FloorPlanId { get; set; }

    [Inject] private IDispatcher Dispatcher { get; set; }
    [Inject] private IState<MapState> MapState { get; set; }
    [Inject] private IMapJsInterop MapJsInterop { get; set; }
    [Inject] private ILogger<Map> Logger { get; set; }

    private FloorPlan FloorPlanDto { get; set; } // ViewModel
    private IEnumerable<Domain.Entities.AccessRequest> AccessRequests { get; set; } // ViewModel

    protected async override Task OnInitializedAsync()
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
        var id = await GetSelectedWorkstationId(mySurvey);
        var circles = FloorPlanDto.Workspaces
            .Select(x => new
            {
                Id = x.Id,
                Name = x.Name,
                Position = new { Left = x.X, Top = x.Y },
                Selected = x.Id == id, // Check if the circle should be selected when changing the date
                Taken = AccessRequests.Any(a => a.Workspace.Id == x.Id),
                EmployeeFullName = AccessRequests.FirstOrDefault(a => a.Workspace.Id == x.Id)?.Employee.FullName ?? string.Empty,
            });

        var circlesJson = JsonSerializer.Serialize(circles.ToArray());
                
        await MapJsInterop.Start(FloorPlanDto.FloorPlanImage, circlesJson);

        _selectedAccessRequest = AccessRequests.FirstOrDefault(x => x.Workspace.Id == id);

        StateHasChanged();        
    }

    // Select survey data to check if a circle should be selected
    static internal async Task<Guid> GetSelectedWorkstationId(Survey survey)
    {
        if (survey is null)
        {
            return Guid.Empty;
        }

        var data = await survey.GetData();

        return ExtractWorkspaceId(data);
    }

    static internal Guid ExtractWorkspaceId(string surveyData)
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
        var circle = Newtonsoft.Json.JsonConvert.DeserializeObject<OfficeEntry.WebApp.Pages.FloorPlans.Edit.Circle>(data);

        await mySurvey.SetValueAsync("workspace", circle.Id.ToString());

        _selectedAccessRequest = AccessRequests.FirstOrDefault(x => x.Workspace.Id == circle.Id);

        StateHasChanged();

        await Task.CompletedTask;
    }

    public async Task OnSurveyCompleted(SurveyCompletedEventArgs e)
    {
        await Task.CompletedTask;
    }

    public async Task OnValueChanged(ValueChangedEventArgs e)
    {
        var options = JsonSerializer
            .Deserialize<Rootobject>(
                json: e.Options,
                options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (options.Name is "startDate")
        {
            _selectedDate = DateOnly.Parse(options.Value);            
            
            Dispatcher.Dispatch(new GetMapAction(FloorPlanId, _selectedDate));

            _selectedAccessRequest = null;

            StateHasChanged();
        }

        if (options.Name is "workspace")
        {
            _selectedAccessRequest = AccessRequests.FirstOrDefault(x => x.Workspace.Id == Guid.Parse(options.Value));
            await MapJsInterop.SetSelectedCircle(options.Value);
        }

        await Task.CompletedTask;
    }
}

public class ExampleModel
{
    //[Required]
    //[StringLength(10, ErrorMessage = "Name is too long.")]
    //public string? Name { get; set; }

    [Required]
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);

    public Guid WorkspaceId { get; set; }

    public RecurrencePattern RecurrencePattern { get; set; } = RecurrencePattern.DoesNotRepeate;
}

public enum RecurrencePattern
{
    DoesNotRepeate,
    Daily,
    Weekly,
    Monthly,
    Annually,
    EveryWeekday,
    //Custom
}

public class Rootobject
{
    public string Name { get; set; }
    public string Value { get; set; }
}
