using Fluxor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using OfficeEntry.Domain.Entities;
using OfficeEntry.Domain.Enums;
using OfficeEntry.WebApp.Store.FloorPlanUseCases.Edit;
using System.Globalization;
using System.Text.Json;

namespace OfficeEntry.WebApp.Pages.FloorPlans;

[Authorize(Policy = "EditUser")]
public partial class Edit : IAsyncDisposable
{
    private EditContext EditContext;
    private IJSObjectReference module;
    private DotNetObjectReference<Edit> objRef;

    [Parameter] public Guid FloorPlanId { get; set; }

    [Inject] private IJSRuntime JSRuntime { get; set; }
    [Inject] private IDispatcher Dispatcher { get; set; }
    [Inject] private IState<FloorPlanState> FloorPlanState { get; set; }

    private FloorPlan FloorPlanDto { get; set; } // ViewModel
    private Workspace WorkspaceDto { get; set; } // ViewModel

    protected async override Task OnInitializedAsync()
    {
        SubscribeToAction<GetFloorPlanResultAction>(async x =>
        {
            FloorPlanDto = x.Dto;

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

        await module.InvokeVoidAsync("register", objRef, "en", "editMode");

        Dispatcher.Dispatch(new GetFloorPlanAction(FloorPlanId));
    }

    private async Task UpdateCanvas()
    {
        var locale = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        locale = (locale == Locale.French) ? locale : Locale.English;

        var circles = FloorPlanDto.Workspaces
            .Select(x =>
            {
                var workspaceDescription = (locale == Locale.French) ? x.FrenchDescription : x.EnglishDescription;

                return new
                {
                    Id = x.Id,
                    Name = x.Name,
                    Position = new { Left = x.X, Top = x.Y },
                    EmployeeFullName = workspaceDescription,
                    Selected = false
                };
            });

        var circlesJson = JsonSerializer.Serialize(circles.ToArray());

        await module.InvokeVoidAsync("start", FloorPlanDto.FloorPlanImage, circlesJson);
    }

    private async Task ShowCanvas(string imageSource)
    {
        FloorPlanDto.FloorPlanImage = imageSource;

        await UpdateCanvas();
    }

    [JSInvokable]
    public Guid GetGuid() => Guid.NewGuid();

    [JSInvokable]
    public void OnSelectedCircleChanged(string data)
    {
        var circle = JsonSerializer.Deserialize<Circle>(data);

        if (!FloorPlanDto.Workspaces.Any(x => x.Id == circle.Id))
        {
            FloorPlanDto.Workspaces.Add(new Workspace
            {
                Id = circle.Id,
                FloorPlan = new FloorPlan { Id = FloorPlanDto.Id },
                Name = circle.Name,
                X = circle.Position.Left,
                Y = circle.Position.Top,
            });
        }

        WorkspaceDto = FloorPlanDto.Workspaces.First(x => x.Id == circle.Id);

        if (EditContext is not null)
        {
            EditContext.OnFieldChanged -= EditContext_OnFieldChangedAsync;
        }

        EditContext = new EditContext(WorkspaceDto);
        EditContext.OnFieldChanged += EditContext_OnFieldChangedAsync;

        // Note: The OnFieldChanged event is raised for each field in the model
        void EditContext_OnFieldChangedAsync(object sender, FieldChangedEventArgs e)
        {
            if (EditContext.Validate())
            {
                var locale = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                locale = (locale == Locale.French) ? locale : Locale.English;

                var workspaceDescription = (locale == Locale.French) ? WorkspaceDto.FrenchDescription : WorkspaceDto.EnglishDescription;

                var data = JsonSerializer.Serialize(new
                    {
                        Id = WorkspaceDto.Id,
                        Name = WorkspaceDto.Name,
                        Position = new { Left = WorkspaceDto.X, Top = WorkspaceDto.Y },
                        EmployeeFullName = workspaceDescription,
                    });

                _ = module.InvokeVoidAsync("updateCircle", data); // no await on purpose, only the animation run async
            }
        }

        StateHasChanged();
    }

    private void Save()
    {
        Dispatcher.Dispatch(new UpdateFloorPlanAction(FloorPlanDto));
    }

    public async ValueTask DisposeAsync()
    {
        Dispose();

        await module.InvokeVoidAsync("stop");

        await module.DisposeAsync();

        objRef?.Dispose();
    }

    public class Circle
    {
        public Guid Id { get; set; }
        public Position Position { get; set; } = new();
        public string Name { get; set; }
    }

    public class Position
    {
        public int Top { get; set; }
        public int Left { get; set; }
    }
}
