using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using OfficeEntry.Application;
using OfficeEntry.WebApp;
using OfficeEntry.WebApp.Shared;
using System.Xml.Linq;

namespace WebApp.UnitTests;

public class SurveyTests : TestContext
{
    private readonly IServiceProvider ServiceProvider;
    private readonly JSRuntimeInvocationHandler _plannedRegister;
    private readonly JSRuntimeInvocationHandler _plannedInit;

    public SurveyTests()
    {
        var services = new ServiceCollection();
        services.AddApplication();

        ServiceProvider = services.BuildServiceProvider();

        var surveyInterop = new Mock<ISurveyInterop>();

        Services.AddSingleton(ServiceProvider.GetRequiredService<IMediator>());
        Services.AddSingleton(surveyInterop.Object);

        var moduleInterop = JSInterop.SetupModule("/js/survey.js");

        _plannedRegister = moduleInterop
            .SetupVoid(invocation => invocation.Identifier == "register")
            .SetVoidResult();

        _plannedInit = moduleInterop
            .SetupVoid(invocation => invocation.Identifier == "init")
            .SetVoidResult();

        var plannedInitializeDatepicker = JSInterop.Setup<object>("initializeDatepicker", "en");
        var plannedinitializeSelect2 = JSInterop.Setup<object>("initializeSelect2");
    }

    [Fact]
    public void When_render()
    {
        var cut = RenderComponent<Survey>(ComponentParameter.CreateParameter(nameof(Survey.SurveyUrl), "example.com"));

        cut.MarkupMatches("<div id=\"blazor-survey-wraper\"></div>");
    }

    [Fact]
    public void When_data_parameter_is_null_or_empty_an_empty_object_is_passed_to_the_survey()
    {
        var cut = RenderComponent<Survey>(ComponentParameter.CreateParameter(nameof(Survey.SurveyUrl), "example.com"));

        cut.MarkupMatches("<div id=\"blazor-survey-wraper\"></div>");

        // register
        {
            _plannedRegister.Invocations.Count.Should().Be(1);
            var invocation = _plannedRegister.Invocations["register"][0];
            invocation.Identifier.Should().Be("register");
            invocation.Arguments[0].Should().BeAssignableTo<DotNetObjectReference<Survey>>();
            invocation.Arguments[1].Should().Be("en");
        }

        // init
        {
            const int Id = 0, Class = 1, SurveyUrl = 2, Data = 3;

            _plannedInit.Invocations.Count.Should().Be(1);
            var invocation = _plannedInit.Invocations["init"][0];
            invocation.Identifier.Should().Be("init");
            invocation.Arguments[Id].Should().BeAssignableTo<string>().And.Be("surveyElement", because: "no Id was provided, default value is used");
            invocation.Arguments[Class].Should().BeAssignableTo<string>().And.Be(string.Empty, because: "no css class provided");
            invocation.Arguments[SurveyUrl].Should().BeAssignableTo<string>();
            invocation.Arguments[Data].Should().BeAssignableTo<string>().And.Be("{}");
        }
    }

    [Fact]
    public void When_data_parameter_is_not_null_it_is_passed_to_the_survey()
    {
        var id = Guid.NewGuid();
        var data = JsonSerializer.Serialize(new { date = DateTime.Now.ToString("yyyy-MM-dd"), workspace = id });

        _ = RenderComponent<Survey>
        (
            ComponentParameter.CreateParameter(nameof(Survey.SurveyUrl), "example.com"),
            ComponentParameter.CreateParameter(nameof(Survey.Data), data)
        );

        // init
        {
            const int Id = 0, Class = 1, SurveyUrl = 2, Data = 3;

            _plannedInit.Invocations.Count.Should().Be(1);
            var invocation = _plannedInit.Invocations["init"][0];
            invocation.Arguments[Data].Should().BeAssignableTo<string>().And.Be(data);
        }
    }

    public class EventCallbackTests : SurveyTests
    {
        private const string NAME = nameof(NAME);
        private const string EXPECTED = nameof(EXPECTED);
        private string Actual { get; set; } = default!;

        [Fact]
        public async Task OnSurveyCompletedtTest()
        {
            // Arrange
            Action<SurveyCompletedEventArgs> action = _ => Actual = EXPECTED;

            var callback = EventCallback.Factory.Create(NAME, action);

            var cut = RenderComponent<Survey>
            (
                ComponentParameter.CreateParameter(nameof(Survey.SurveyUrl), "example.com"),
                ComponentParameter.CreateParameter(nameof(Survey.OnSurveyCompleted), callback)
            );

            // Act
            await cut.InvokeAsync(() => cut.Instance.SurveyCompleted(It.IsAny<string>()));

            // Assert
            Actual.Should().Be(EXPECTED);
        }

        [Fact]
        public async Task When_PageChanged_invoke_OnPageChanged_callback()
        {
            // Arrange
            Action<PageChangedEventArgs> action = _ => Actual = EXPECTED;

            var callback = EventCallback.Factory.Create(NAME, action);

            var cut = RenderComponent<Survey>
            (
                ComponentParameter.CreateParameter(nameof(Survey.SurveyUrl), "example.com"),
                ComponentParameter.CreateParameter(nameof(Survey.OnPageChanged), callback)
            );

            // Act
            await cut.InvokeAsync(() => cut.Instance.PageChanged(It.IsAny<string>(), It.IsAny<string>()));

            // Assert
            Actual.Should().Be(EXPECTED);
        }

        [Fact]
        public async Task When_ValueChanged_invoke_OnValueChanged_callback()
        {
            // Arrange
            Action<ValueChangedEventArgs> action = _ => Actual = EXPECTED;

            var callback = EventCallback.Factory.Create(NAME, action);

            var cut = RenderComponent<Survey>
            (
                ComponentParameter.CreateParameter(nameof(Survey.SurveyUrl), "example.com"),
                ComponentParameter.CreateParameter(nameof(Survey.OnValueChanged), callback)
            );

            // Act
            await cut.InvokeAsync(() => cut.Instance.ValueChanged(It.IsAny<string>(), It.IsAny<string>()));

            // Assert
            Actual.Should().Be(EXPECTED);
        }

        [Fact]
        public async Task When_CurrentPageChanging_invoke_OnCurrentPageChanging_callback()
        {
            // Arrange
            Action<CurrentPageChangingEventArgs> action = _ => Actual = EXPECTED;

            var callback = EventCallback.Factory.Create(NAME, action);

            var cut = RenderComponent<Survey>
            (
                ComponentParameter.CreateParameter(nameof(Survey.SurveyUrl), "example.com"),
                ComponentParameter.CreateParameter(nameof(Survey.OnCurrentPageChanging), callback)
            );

            // Act
            await cut.InvokeAsync(() => cut.Instance.CurrentPageChanging(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            // Assert
            Actual.Should().Be(EXPECTED);
        }

        [Fact]
        public async Task When_SurveyLoaded_invoke_OnSurveyLoaded_callback()
        {
            // Arrange
            Action<EventArgs> action = _ => Actual = EXPECTED;

            var callback = EventCallback.Factory.Create(NAME, action);

            var cut = RenderComponent<Survey>
            (
                ComponentParameter.CreateParameter(nameof(Survey.SurveyUrl), "example.com"),
                ComponentParameter.CreateParameter(nameof(Survey.OnSurveyLoaded), callback)
            );

            // Act
            await cut.InvokeAsync(() => cut.Instance.SurveyLoaded());

            // Assert
            Actual.Should().Be(EXPECTED);
        }
    }
}
