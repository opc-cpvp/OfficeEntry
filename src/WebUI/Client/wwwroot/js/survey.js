var interopJS = interopJS || {}

interopJS.survey = {
    dotNet: null,
    survey: null,

    save: (name, value) => {
        window.localStorage["surveyjs-blazor"] = value;
    },

    register: dotNetReference => {
        dotNet = dotNetReference;
    },

    dispose: _ => {
        document
            .querySelector('#blazor-survey-wraper')
            .innerHTML = "";
    },

    data: _ => {
        return JSON.stringify(survey.data, null, 3);
    },

    setData: function (data) {
        if (data) {
            survey.data = JSON.parse(data);
        }
        // TODO: Set page to 0
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

        fetch(surveyUrl)
            .then(response => response.json())
            .then(json => {

                document
                    .querySelector('#blazor-survey-wraper')
                    .innerHTML = `<div id="${id}" class="${classStyle}"><survey :survey="survey"/></div>`;

                survey = new Survey.Model(json);

                survey
                    .onComplete
                    .add(function (result) {
                        dotNet.invokeMethodAsync("SurveyCompleted", JSON.stringify(result.data, null, 3))
                            .then(data => {
                                console.log("### surveyjs was sent to .NET.");
                            });
                    });

                var surveyValueChanged = function (sender, options) {
                    //alert("surveyValueChanged");
                    window.localStorage["surveyjs-blazor"] = JSON.stringify(survey.data, null, 3);
                };

                survey
                    .onValueChanged
                    //.add( (s,e) => window.localStorage["surveyjs-blazor"] = survey.data );
                    .add(surveyValueChanged);

                if (data) {
                    survey.data = JSON.parse(data);
                }

                var app = new Vue({
                    el: `#${id}`,
                    data: {
                        survey: survey
                    }
                });
            });
    }
}

window.ShowAlert = (message) => {
    alert(message);
}

window.interop = interopJS;