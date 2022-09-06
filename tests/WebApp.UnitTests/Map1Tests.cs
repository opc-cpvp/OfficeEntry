using Bunit;
using FluentAssertions;
using Fluxor;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Moq;
using OfficeEntry.Application;
using OfficeEntry.Domain.Entities;
using OfficeEntry.WebApp;
using OfficeEntry.WebApp.Pages.FloorPlans;
using OfficeEntry.WebApp.Shared;
using OfficeEntry.WebApp.Store.FloorPlanUseCases.Map;
using Serilog;
using System.Collections.Immutable;
using System.Numerics;
using Xunit;

namespace WebApp.UnitTests;

public class Map1Tests
{
    private readonly IServiceProvider ServiceProvider;
    private readonly IStore Store;
    private readonly IState<MapState> State;

    public Map1Tests()
    {
        var services = new ServiceCollection();
        services.AddFluxor(x => x.ScanAssemblies(GetType().Assembly).ScanTypes(typeof(MapState), typeof(Reducers)));

        services.AddApplication();

        services.AddLocalization(options => options.ResourcesPath = "Resources");
        //services.AddSingleton<ILoggerFactory>(new LoggerFactory().AddSerilog(serilogLogger, dispose: true));
        services.AddSingleton<ILoggerFactory>(new LoggerFactory());
        services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

        ServiceProvider = services.BuildServiceProvider();
        Store = ServiceProvider.GetRequiredService<IStore>();
        State = ServiceProvider.GetRequiredService<IState<MapState>>();
        Store.InitializeAsync().Wait();
    }

    [Fact]
    public void Test1()
    {
        // Arrange
        using var ctx = new TestContext();

        // Register the a stub substitution.
        ctx.ComponentFactories.AddStub<Loading>();
        ctx.ComponentFactories.AddStub<Survey>();

        var mapJsInterop = new Mock<IMapJsInterop>();        

        var dispatcher = ServiceProvider.GetRequiredService<IDispatcher>();
        var actionSubscriber = ServiceProvider.GetRequiredService<IActionSubscriber>();

        ctx.Services.AddSingleton(dispatcher);
        ctx.Services.AddSingleton(State);
        ctx.Services.AddSingleton(actionSubscriber);
        ctx.Services.AddSingleton(mapJsInterop.Object);

        var textService = new TaskCompletionSource<string>();
        var cut = ctx.RenderComponent<Map>(parameters => parameters.Add(p => p.FloorPlanId, Guid.NewGuid()));

        var accessRequests = new AccessRequest[]
        {
            //new AccessRequest { Id =  }
        }.ToImmutableArray();

        // Act
        dispatcher.Dispatch(new GetMapResultAction(new FloorPlan {  }, accessRequests));

        // Assert
        Assert.False(State.Value.IsLoading);
    }
}
