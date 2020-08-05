var interopJS = interopJS || {}

interopJS.survey = {
    dotNet: null,
    survey: null,

    save: function(name, value) {
        window.localStorage["surveyjs-blazor"] = value;
    },

    register: function(dotNetReference) {
        dotNet = dotNetReference;
    },

    dispose: function(_) {
        document
            .querySelector('#blazor-survey-wraper')
            .innerHTML = "";
    },

    data: function(_) {
        return JSON.stringify(survey.data, null, 3);
    },

    setData: function (data) {
        if (data) {
            survey.data = JSON.parse(data);
        }
        // TODO: Set page to 0
    },

    getNextDayDate: function () {
        var today = new Date();
        // replace function fixes bug in IE where toLocaleDateString generates unicode characters
        return new Date(today.setDate(today.getDate() + 1)).toLocaleDateString("en-US").replace(/\u200E/g, '');
    },

    setDatepickerLocale: function (locale) {
        if (locale === 'fr') {
            return $.datepicker.setDefaults($.datepicker.regional['fr-CA']);
        }
        return $.datepicker.setDefaults($.datepicker.regional['']);
    },

    init: function (id, classStyle, surveyUrl, data) {
        // Override the color used style the flip switch
        Survey.StylesManager.ThemeColors["bootstrap"]["$main-color"] = "#007bff";

        // Apply bootstrap classes
        Survey.StylesManager.applyTheme("bootstrap");

        // Override classes to match bootstrap
        Survey.defaultBootstrapCss.navigationButton = "btn btn-primary";
        Survey.defaultBootstrapCss.matrixdynamic.buttonAdd = "btn btn-secondary";
        Survey.defaultBootstrapCss.matrixdynamic.buttonRemove = "btn btn-danger";

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
            
        fetch(surveyUrl)
            .then(function (response) { return response.json(); })
            .then(function(json) {
                document
                    .querySelector('#blazor-survey-wraper')
                    .innerHTML = "<div id=" + id + " class=" + classStyle + "><survey :survey=\"survey\"/></div>";

                if (json.locale)
                    json.locale = window.localStorage['BlazorCulture'] === "fr-CA" ? "fr" : "en";

                survey = new Survey.Model(json);

                survey.locale = window.localStorage['BlazorCulture'] === "fr-CA" ? "fr" : "en";

                window.interop.survey.setDatepickerLocale(survey.locale);

                survey
                    .onComplete
                    .add(function(result) {
                        dotNet.invokeMethodAsync("SurveyCompleted", JSON.stringify(result.data, null, 3))
                            .then(function(data) {
                                console.log("### surveyjs was sent to .NET.");
                            });
                    });

                var surveyValueChanged = function (sender, options) {
                    //alert("surveyValueChanged");
                    window.localStorage["surveyjs-blazor"] = JSON.stringify(survey.data, null, 3);
                };

                survey
                    .onAfterRenderQuestion
                    .add(function(survey, options) {
                        if (options.question.name === "startDate") {
                            options.question.defaultValue = window.interop.survey.getNextDayDate();
                        }                      
                    });

                survey
                    .onCurrentPageChanged
                    .add(function(survey, options) {

                    dotNet.invokeMethodAsync("PageChanged", JSON.stringify(survey.data), options.newCurrentPage.name);
                });

                survey
                    .onValueChanged
                    .add(surveyValueChanged);

                if (data) {
                    survey.data = JSON.parse(data);
                }

                var app = new Vue({
                    el: "#" + id,
                    data: {
                        survey: survey
                    }
                });
            });
    }
}

window.ShowAlert = function(message) {
    alert(message);
}

window.interop = interopJS;