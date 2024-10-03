using Fluxor;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OfficeEntry.Application;
using OfficeEntry.Application.Common.Interfaces;
using OfficeEntry.Application.Common.Models;
using OfficeEntry.Domain.Entities;
using OfficeEntry.WebApp;
using OfficeEntry.WebApp.Pages.FloorPlans;
using OfficeEntry.WebApp.Shared;
using OfficeEntry.WebApp.Store.FloorPlanUseCases.Map;
using System.Collections.Immutable;

namespace WebApp.UnitTests;

public class MapComponentTests
{
    private readonly IServiceProvider ServiceProvider;
    private readonly IStore Store;
    private readonly IState<MapState> State;

    public MapComponentTests()
    {
        var services = new ServiceCollection();
        services.AddFluxor(x => x.ScanAssemblies(GetType().Assembly).ScanTypes(typeof(MapState), typeof(Reducers)));

        services.AddApplication();

        services.AddLocalization(options => options.ResourcesPath = "Resources");
        services.AddSingleton<ILoggerFactory>(new LoggerFactory());

        var domainUserService = new Mock<IDomainUserService>();
        services.AddSingleton(domainUserService.Object);

        var currentUserService = new Mock<ICurrentUserService>();
        services.AddSingleton(currentUserService.Object);

        var userService = new Mock<IUserService>();
        userService.Setup(x => x.IsContactFirstResponder(It.IsAny<string>())).ReturnsAsync((Result.Success(), true));
        services.AddSingleton(userService.Object);

        services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

        ServiceProvider = services.BuildServiceProvider();
        Store = ServiceProvider.GetRequiredService<IStore>();
        State = ServiceProvider.GetRequiredService<IState<MapState>>();
        Store.InitializeAsync().Wait();
    }

    [Fact]
    public void When_component_loads_the_state_gets_updated()
    {
        // Arrange
        using var ctx = new TestContext();

        // Register the a stub substitution.
        ctx.ComponentFactories.AddStub<Loading>();
        ctx.ComponentFactories.AddStub<Survey>();

        var stringLocalizer = new Mock<IStringLocalizer<App>>();
        var mapJsInterop = new Mock<IMapJsInterop>();

        var dispatcher = ServiceProvider.GetRequiredService<IDispatcher>();
        var mediator = ServiceProvider.GetRequiredService<IMediator>();
        var actionSubscriber = ServiceProvider.GetRequiredService<IActionSubscriber>();
        var currentUserService = ServiceProvider.GetRequiredService<ICurrentUserService>();

        ctx.Services.AddSingleton(dispatcher);
        ctx.Services.AddSingleton(mediator);
        ctx.Services.AddSingleton(currentUserService);
        ctx.Services.AddSingleton(State);
        ctx.Services.AddSingleton(actionSubscriber);
        ctx.Services.AddSingleton(mapJsInterop.Object);
        ctx.Services.AddSingleton(stringLocalizer.Object);

        var textService = new TaskCompletionSource<string>();
        var cut = ctx.RenderComponent<Map>(parameters => parameters.Add(p => p.FloorPlanId, Guid.NewGuid()));

        var accessRequests = new AccessRequest[] { }.ToImmutableArray();

        // Act
        dispatcher.Dispatch(new GetMapResultAction(new FloorPlan {  }, accessRequests));

        // Assert
        Assert.False(State.Value.IsLoading);
    }
}
