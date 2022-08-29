using Fluxor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json.Linq;
using OfficeEntry.Domain.Entities;
using OfficeEntry.WebApp.Shared;
using OfficeEntry.WebApp.Store.FloorPlanUseCases.Map;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace OfficeEntry.WebApp.Pages.FloorPlans;

[Authorize]
public partial class Map : IAsyncDisposable
{
    private Survey mySurvey;
    private IJSObjectReference module;
    private DotNetObjectReference<Map> objRef;
    private Domain.Entities.AccessRequest _selectedAccessRequest;

    [Parameter] public Guid FloorPlanId { get; set; }

    [Inject] private IJSRuntime JSRuntime { get; set; }
    [Inject] private IDispatcher Dispatcher { get; set; }
    [Inject] private IState<MapState> MapState { get; set; }

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

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        objRef = DotNetObjectReference.Create(this);

        module = await JSRuntime
            .InvokeAsync<IJSObjectReference>("import", "/js/floorplan.js");

        await module.InvokeVoidAsync("register", objRef, "en");

        Dispatcher.Dispatch(new GetMapAction(FloorPlanId, DateOnly.FromDateTime(DateTime.Now)));
    }

    private async Task UpdateCanvas()
    {
        var id = Guid.Empty;

        // Select survey data to check if a circle should be selected
        if (mySurvey is not null)
        {
            var data = await mySurvey.GetData();

            id = ExtractWorkspaceId(data);
        }        

        var circles = FloorPlanDto.Workspaces
            .Select(x => new
            {
                Id = x.Id,
                Name = x.Name,
                Position = new { Left = x.X, Top = x.Y },
                Selected = x.Id == id, // Check if the circle should be selected when changing the date
                Taken = AccessRequests.Any(a => a.Workspace.Id == x.Id)
            });

        var circlesJson = JsonSerializer.Serialize(circles.ToArray());

        await module.InvokeVoidAsync("start", FloorPlanDto.FloorPlanImage, circlesJson);

        _selectedAccessRequest = AccessRequests.FirstOrDefault(x => x.Workspace.Id == id);

        StateHasChanged();

        // Create unit test
        Guid ExtractWorkspaceId(string surveyData)
        {
            var id = JToken.Parse(surveyData)
                .FirstOrDefault(x => x.Path is "workspace")
                ?.FirstOrDefault()?.Value<string>() ?? Guid.Empty.ToString();

            return Guid.Parse(id);
        }
    }

    public async ValueTask DisposeAsync()
    {
        Dispose();

        await module.InvokeVoidAsync("stop");

        await module.DisposeAsync();

        objRef?.Dispose();
    }

    [JSInvokable]
    public async Task OnSelectedCircleChanged(string data)
    {
        var circle = Newtonsoft.Json.JsonConvert.DeserializeObject<OfficeEntry.WebApp.Pages.FloorPlans.Edit.Circle>(data);

        await mySurvey.SetValueAsync("workspace", circle.Id.ToString());

        _selectedAccessRequest = AccessRequests.FirstOrDefault(x => x.Workspace.Id == circle.Id);

        StateHasChanged();

        await Task.CompletedTask;
    }

    public async Task OnSurveyCompleted(string data)
    {
        await Task.CompletedTask;
    }

    public async Task OnValueChanged((string data, string options) p)
    {
        var options = System.Text.Json.JsonSerializer
            .Deserialize<Rootobject>(
                json: p.options,
                options: new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (options.Name is "startDate")
        {
            var date = DateOnly.Parse(options.Value);            
            
            Dispatcher.Dispatch(new GetMapAction(FloorPlanId, date));

            _selectedAccessRequest = null;

            StateHasChanged();
        }

        if (options.Name is "workspace")
        {
            _selectedAccessRequest = AccessRequests.FirstOrDefault(x => x.Workspace.Id == Guid.Parse(options.Value));
            await module.InvokeVoidAsync("setSelectedCircle", options.Value);
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