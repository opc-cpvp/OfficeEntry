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

            survey.onValueChanged.add((sender, options) => {
                dotNet.invokeMethodAsync("ValueChanged", JSON.stringify(survey.data), JSON.stringify(options))
                        .catch(function (error) {
                            console.error(error);
                            dotNet.invokeMethodAsync("ShowError");
                        });

                // if (options.name === "floor") {
                //     //console.debug(sender);
                //     //console.debug(options.name);
                //     //console.debug(options.question);
                //     //console.debug(options.value);
                // }
            });

            survey
                .onTextMarkdown
                .add(function (survey, options) {
                    options.html = options.text;
                });

            // ===============================================================
            // accessRequest.js <START>
            // ===============================================================
            function CapacityValidator() {
                var that = this;

                return dotNet.invokeMethodAsync("HasAvailableCapacity", JSON.stringify(survey.data, null, 3))
                    .then(function (r) {
                        return that.returnResult(r);
                    });
            }

            Survey
                .FunctionFactory
                .Instance
                .register("CapacityValidator", CapacityValidator, true);
            // ===============================================================
            // accessRequest.js <END>
            // ===============================================================

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
