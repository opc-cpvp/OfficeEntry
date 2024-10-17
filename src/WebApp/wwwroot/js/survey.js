// This is a JavaScript module that is loaded on demand. It can export any number of
// functions, and may import other JavaScript modules if required.


export let dotNet = null;
export let survey = null;
export let locale = null;

export function register(dotNetReference, loc) {
    dotNet = dotNetReference;
    locale = loc;
}

export function dispose(_) {
    document
        .querySelector('#blazor-survey-wraper')
        .innerHTML = "";
}

export function data(_) {
    return JSON.stringify(survey.data, null, 3);
}

export function setData(data) {
    if (data) {
        survey.data = JSON.parse(data);
    }
    // TODO: Set page to 0
}

export function setValue(questionName, value) {
    survey.setValue(questionName, value);
}

export function init(id, classStyle, surveyUrl, data) {
    Survey.settings.minWidth = "100%";
    Survey.StylesManager.Styles[".sv_row"] = "clear:both;";

    // Override the color used style the flip switch
    Survey.StylesManager.ThemeColors["bootstrap"]["$main-color"] = "#007bff";

    // The background color of selected item in dropdown is not set in survey-vue
    Survey.StylesManager.Styles[".sv-list__item--selected"] =
        "background-color: #007bff; color: #fff;";

    // The background color of the boolean switch is not set for some reason
    Survey.StylesManager.Styles[".sv-boolean__switch"] =
        "background-color: rgb(0, 98, 126)";

    // The boolean thumb ghost class is in diplay: block, causing line breaks we don't want for boolean switches
    Survey.StylesManager.Styles[".sv-boolean__thumb-ghost"] = "display: inline";

    // The background color of the boolean switch slider is not set for some reason
    Survey.StylesManager.Styles[".sv-boolean__slider"] = "background-color: white";

    // Better indentation of descriptions
    Survey.StylesManager.Styles[".sv_p_description"] = "padding-left: 16px";

    // Edit padding for action bar
    Survey.StylesManager.Styles[".sv_main .sv-action-bar"] =
        "padding: 16px;";

    // Apply bootstrap classes
    Survey.StylesManager.applyTheme("bootstrap");

    // Override classes to match bootstrap
    Survey.defaultBootstrapCss.navigationButton = "btn btn-primary";
    Survey.defaultBootstrapCss.matrixdynamic.buttonAdd = "btn btn-secondary";
    Survey.defaultBootstrapCss.matrixdynamic.buttonRemove = "btn btn-danger";

    fetch(surveyUrl)
        .then(function (response) { return response.json(); })
        .then(function (json) {
            document
                .querySelector('#blazor-survey-wraper')
                .innerHTML = "<div id=" + id + " class=" + classStyle + "><survey :survey=\"survey\"/></div>";

            if (json.locale)
                json.locale = locale;

            survey = new Survey.Model(json);

            if (data) {
                survey.data = JSON.parse(data);
            }

            survey.locale = locale;

            survey
                .onComplete
                .add(function (result) {
                    dotNet.invokeMethodAsync("SurveyCompleted", JSON.stringify(result.data))
                        .then(function (data) {
                            console.log("### surveyjs was sent to .NET.");
                        })
                        .catch(function (error) {
                            console.error(error);
                            dotNet.invokeMethodAsync("ShowError");
                        });
                });

            survey
                .onCurrentPageChanging
                .add(function (sender, option) {
                    dotNet.invokeMethodAsync("CurrentPageChanging", JSON.stringify(sender.data), option.oldCurrentPage.name, option.newCurrentPage.name)
                        .catch(function (error) {
                            console.error(error);
                            dotNet.invokeMethodAsync("ShowError");
                        });
                });

            survey  
                .onCurrentPageChanged
                .add(function (survey, options) {
                    dotNet.invokeMethodAsync("PageChanged", JSON.stringify(survey.data), options.newCurrentPage.name)
                        .catch(function (error) {
                            console.error(error);
                            dotNet.invokeMethodAsync("ShowError");
                        });
                });

            // Set date picker format when the value is changing for expected formatting
            survey.onValueChanging.add((sender, options) => {
                if (options.question && options.question.getType && options.question.getType() == "datepicker") {
                    const date = new Date(options.value);
                    var datestring = `${(date.getMonth() + 1)}/${date.getDate()}/${date.getFullYear()}`
                    options.value = datestring;
                }
            });

            // Set date picker format when the value is changing for expected formatting
            survey.onValueChanging.add((sender, options) => {
                if (options.question && options.question.getType() == "datepicker") {
                    const date = new Date(options.value);
                    var datestring = `${(date.getMonth() + 1)}/${date.getDate()}/${date.getFullYear()}`
                    options.value = datestring;
                }
            });

            survey.onValueChanged.add((sender, options) => {
                // We need to manually set the startDate for the datepicker when trying to change its max days
                if (options.name == "numberOfDaysAllowed") {
                    const startDatePicker = $("div[data-name='startDate'] input");
                    const daysAllowed = parseInt(survey.data.numberOfDaysAllowed);

                    var maxCalendarDays = new Date();
                    maxCalendarDays.setDate(maxCalendarDays.getDate() + daysAllowed);
                    startDatePicker.datepicker('option', 'maxDate', maxCalendarDays);
                }
                dotNet.invokeMethodAsync("ValueChanged", JSON.stringify(survey.data), JSON.stringify(options))
                    .catch(function (error) {
                        console.error(error);
                        dotNet.invokeMethodAsync("ShowError");
                    });
            });

            survey
                .onTextMarkdown
                .add(function (survey, options) {
                    options.html = options.text;
                });

            // Fix some styling issues that cannot be fixed directly with css
            survey.onAfterRenderQuestion.add((_, options) => {
                const html = options.htmlElement;
                const parent = html.parentElement;

                html.style.minWidth = "100%";
                parent.style.minWidth = "100%";

                // Added spacing to radio label
                if (options.question.getType() === "radiogroup") {
                    html
                        .querySelectorAll("fieldset span.sv-string-viewer")
                        .forEach((value) => {
                            value.innerText = ` ${value.innerText}`;
                        });
                }
            });

            // Apply button group class to actions in the navigation bar.
            const container = survey.navigationBarValue;
            const actions = container.actions;
            actions.forEach((x) => {
                x.css = "btn-group";
            });

            var app = new Vue({
                el: "#" + id,
                data: {
                    survey: survey
                }
            });
        })
        .then(function () {
            var that = this;
            console.log("### surveyjs loaded.");
            // As circular references are not supported, you can't pass "this" back to blazor.
            dotNet.invokeMethodAsync("SurveyLoaded")
                .catch(function (error) {
                    console.error(error);
                    dotNet.invokeMethodAsync("ShowError");
                });
        });
}
