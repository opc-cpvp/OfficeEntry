using Microsoft.JSInterop;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Infrastructure.Identity;
using OfficeEntry.WebApp.Area.Identity.Services;
using System.Text.Json;

namespace OfficeEntry.WebApp.Pages.FloorPlans;

public interface IMapJsInterop
{
    Task OnSelectedCircleChanged(string data);
    Task Register(Map map);
    Task SetSelectedCircle(string data);
    Task Start(string floorplanImage, string circlesJson);
}

public sealed class MapJsInterop : IAsyncDisposable, IMapJsInterop
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDomainUserService _domainUserService;

    private readonly Lazy<Task<IJSObjectReference>> _moduleTask;
    private readonly DotNetObjectReference<MapJsInterop> _objRef;
    private Map? _mapInstance;

    public MapJsInterop(IJSRuntime jsRuntime, ICurrentUserService currentUserService, IDomainUserService domainUserService)
    {
        _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
           "import", "/js/floorplan.js").AsTask());

        _objRef = DotNetObjectReference.Create(this);

        _currentUserService = currentUserService;
        _domainUserService = domainUserService;
    }

    public async Task Register(Map map)
    {
        _mapInstance = map;

        var module = await _moduleTask.Value;

        await module.InvokeVoidAsync("register", _objRef, "en");
    }

    [JSInvokable]
    public async Task OnSelectedCircleChanged(string data)
    {
        await _mapInstance.OnSelectedCircleChanged(data);
    }

    [JSInvokable]
    public async Task OnSpying(string data)
    {
        var e = JsonSerializer.Deserialize<SpyingEventArg>(data);
        e.UserId = _currentUserService.UserId;
        e.FullName = await _domainUserService.GetUserNameAsync(Domain.ValueObjects.AdAccount.For(_currentUserService.UserId));

        await _mapInstance.OnSpying(e);
    }

    public async Task Start(string floorplanImage, string circlesJson)
    {
        var module = await _moduleTask.Value;

        await module.InvokeVoidAsync("start", floorplanImage, circlesJson);    
    }

    public async Task SetSelectedCircle(string data)
    {
        var module = await _moduleTask.Value;

        await module.InvokeVoidAsync("setSelectedCircle", data);
    }

    public async ValueTask DisposeAsync()
    {
        if (_moduleTask.IsValueCreated)
        {
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("stop");
            await module.DisposeAsync();
        }

        _objRef?.Dispose();
    }

    public class SpyingEventArg : EventArgs
    {
        public string Victim { get; init; }
        public string Workspace { get; init; }
        public string FullName { get; set; }
        public string UserId { get; set; }
    }
}