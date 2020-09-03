var interopJS = interopJS || {}

interopJS.survey = {
    dotNet: null,
    survey: null,
    locale: null,

    save: function(name, value) {
        window.localStorage["surveyjs-blazor"] = value;
    },

    register: function(dotNetReference, locale) {
        dotNet = dotNetReference;
        this.locale = locale;
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


    init: function (id, classStyle, surveyUrl, data) {
        var that = this;

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
            .then(function(json) {
                document
                    .querySelector('#blazor-survey-wraper')
                    .innerHTML = "<div id=" + id + " class=" + classStyle + "><survey :survey=\"survey\"/></div>";

                if (json.locale)
                    json.locale = that.locale;

                survey = new Survey.Model(json);

                survey.locale = that.locale;

                survey
                    .onComplete
                    .add(function(result) {
                        dotNet.invokeMethodAsync("SurveyCompleted", JSON.stringify(result.data))
                            .then(function(data) {
                                console.log("### surveyjs was sent to .NET.");
                            });
                    });

                survey
                    .onCurrentPageChanged
                    .add(function(survey, options) {
                        dotNet.invokeMethodAsync("PageChanged", JSON.stringify(survey.data), options.newCurrentPage.name);
                });

                interopJS.survey.survey = survey;

                if (data) {
                    survey.data = JSON.parse(data);
                }

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
                dotNet.invokeMethodAsync("SurveyLoaded");
            });;
    }
}

window.interop = interopJS;