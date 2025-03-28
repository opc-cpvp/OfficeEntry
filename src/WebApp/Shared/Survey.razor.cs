﻿using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OfficeEntry.Domain.Enums;
using System.Globalization;

namespace OfficeEntry.WebApp.Shared;

public partial class Survey
{
    [Parameter]
    public string Id { get; set; } = "surveyElement";

    [Parameter]
    public string Class { get; set; } = "";

    // The [EditorRequired] attribute is enforced at design-time and as part of the build.
    // It isn’t enforced at runtime and it doesn’t guarantee the the parameter value cannot be null.
    [EditorRequired]
    [Parameter]
    public string SurveyUrl { get; set; } = default!;

    [Parameter]
    public EventCallback<EventArgs> OnSurveyLoaded { get; set; }

    [Parameter]
    public EventCallback<SurveyCompletedEventArgs> OnSurveyCompleted { get; set; }

    [Parameter]
    public EventCallback<PageChangedEventArgs> OnPageChanged { get; set; }

    [Parameter]
    public EventCallback<ValueChangedEventArgs> OnValueChanged { get; set; }

    [Parameter]
    public EventCallback<CurrentPageChangingEventArgs> OnCurrentPageChanging { get; set; }

    [Parameter]
    public string Data { get; set; }

    private DotNetObjectReference<Survey> _objectReference;

    private bool _isErrorActive;
    private string _locale;

    [Inject]
    public MediatR.IMediator Mediator { get; set; }

    private IJSObjectReference _module;

    public override Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);

        if (SurveyUrl == null)
        {
            throw new InvalidOperationException($"{nameof(Survey)} requires a {nameof(SurveyUrl)} parameter.");
        }

        return base.SetParametersAsync(ParameterView.Empty);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        _module = await JSRuntime
           .InvokeAsync<IJSObjectReference>("import", "/js/survey.js");

        var locale = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        _locale = locale == Locale.French ? locale : Locale.English;

        _objectReference = DotNetObjectReference.Create(this);

        await _module.InvokeVoidAsync("register", _objectReference, _locale);
        var dataValue = string.IsNullOrWhiteSpace(Data) ? "{}" : Data;
        await _module.InvokeVoidAsync("init", Id, Class, SurveyUrl, dataValue);
        await JSRuntime.InvokeVoidAsync("initializeDatepicker", _locale);
        await JSRuntime.InvokeVoidAsync("initializeSelect2", _locale);
    }

    public async Task<string> GetData()
    {
        var surveyData = await _module.InvokeAsync<string>("data");
        return surveyData;
    }

    public async Task SetData(string data)
    {
        await _module.InvokeVoidAsync("setData", data);
    }

    public async Task SetValueAsync(string questionName, string value)
    {
        await _module.InvokeVoidAsync("setValue", questionName, value);
    }

    [JSInvokable]
    public async Task SurveyCompleted(string results)
    {
        await OnSurveyCompleted.InvokeAsync(new SurveyCompletedEventArgs { SurveyResult = results });
    }

    [JSInvokable]
    public async Task PageChanged(string surveyData, string newCurrentPageName)
    {
        await OnPageChanged.InvokeAsync(new PageChangedEventArgs { SurveyData = surveyData, NewCurrentPageName = newCurrentPageName });
    }

    [JSInvokable]
    public async Task CurrentPageChanging(string surveyData, string oldCurrentPageName, string newCurrentPageName)
    {
        await OnCurrentPageChanging.InvokeAsync(new CurrentPageChangingEventArgs
        {
            SurveyData = surveyData,
            OldCurrentPageName = oldCurrentPageName,
            NewCurrentPageName = newCurrentPageName
        });
    }

    [JSInvokable]
    public async Task ValueChanged(string surveyData, string options)
    {
        await OnValueChanged.InvokeAsync(new ValueChangedEventArgs
        {
            SurveyData = surveyData,
            Options = options
        });
    }

    [JSInvokable]

    public void ShowError()
    {
        _isErrorActive = true;
        StateHasChanged();
    }

    [JSInvokable]

    private void HideError()
    {
        _isErrorActive = false;
        StateHasChanged();
    }

    [JSInvokable]
    public async Task SurveyLoaded()
    {
        await OnSurveyLoaded.InvokeAsync(EventArgs.Empty);
    }

    public void Dispose()
    {
        _objectReference?.Dispose();
    }
}

public class SurveyCompletedEventArgs : EventArgs
{
    public string SurveyResult { get; init; }
}

public class PageChangedEventArgs : EventArgs
{
    public string SurveyData { get; init; }
    public string NewCurrentPageName { get; init; }
}

public class CurrentPageChangingEventArgs : EventArgs
{
    public string SurveyData { get; init; }
    public string NewCurrentPageName { get; init; }
    public string OldCurrentPageName { get; init; }
}

public class ValueChangedEventArgs : EventArgs
{
    public string SurveyData { get; init; }
    public string Options { get; init; }
}