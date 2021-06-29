function init(Survey, $) {
    $ = $ || window.$;
    var widget = {
        activatedBy: "property",
        name: "imagemapster",
        widgetIsLoaded: function() {
            return typeof $ === "function" && !!$.fn.mapster;
        },
        isFit: function(question) {
            if (widget.activatedBy === "property")
                return (
                    question["renderAs"] === "imagemapster" &&
                    question.getType() === "checkbox"
                );
            if (widget.activatedBy === "customtype")
                return question.getType() === "imagemapster";
            return false;
        },
        activatedByChanged: function(activatedBy) {
            if (!this.widgetIsLoaded()) return;
            widget.activatedBy = activatedBy;
            Survey.JsonObject.metaData.removeProperty("checkbox", "renderAs");
            if (activatedBy === "property") {
                Survey.JsonObject.metaData.addProperty("checkbox", {
                    name: "renderAs",
                    category: "general",
                    default: "default",
                    choices: ["imagemapster", "default"]
                });
                Survey.JsonObject.metaData.addProperty("checkbox", {
                    dependsOn: "renderAs",
                    category: "general",
                    name: "config",
                    visibleIf: function (obj) {
                        return obj.renderAs === "imagemapster";
                    }
                });
                Survey.JsonObject.metaData.addProperty("checkbox", {
                    dependsOn: "renderAs",
                    category: "general",
                    name: "imageLink",
                    visibleIf: function (obj) {
                        return obj.renderAs === "imagemapster";
                    }
                });
            }
            if (activatedBy === "customtype") {
                Survey.JsonObject.metaData.addClass("imagemapster", [], null, "checkbox");
                Survey.JsonObject.metaData.addProperty("imagemapster", {
                    name: "config",
                    category: "general",
                    default: null
                });
                Survey.JsonObject.metaData.addProperty("imagemapster", {
                    category: "general",
                    name: "imageLink"
                });
            }
            Survey.JsonObject.metaData.addProperty("itemvalue", {
                category: "general",
                name: "coordinates"
            });
            Survey.JsonObject.metaData.addProperty("itemvalue", {
                category: "general",
                name: "shape"
            });
        },
        htmlTemplate: "<div><img/><map></map></div>",
        afterRender: function (question, el) {
            var config = question.config;
            var settings = config && typeof config === "string"
                ? JSON.parse(config)
                : config;
            if (!settings) settings = {};

            var $img = $(el).find("img");
            $img.attr("usemap", "#" + question.name);
            $img.attr("src", question.imageLink);
            $img.attr("alt", question.name);

            var $map = $(el).find("map");
            $map.attr("name", question.name);

            var isSettingValue = false;
            var updateValueHandler = function() {
                if (isSettingValue) return;
                isSettingValue = true;
                $img.mapster("set", true, question.value);
                isSettingValue = false;
            };
            var updateChoices = function () {
                $img.mapster("unbind");
                $map.text("");
                question.visibleChoices.forEach(choice => {
                    var area = document.createElement("area");
                    area.setAttribute("href", "#");
                    area.setAttribute("alt", choice.text);
                    area.setAttribute("shape", choice.shape);
                    area.setAttribute("coords", choice.coordinates);
                    area.setAttribute(settings.mapKey, choice.value);

                    $map.append(area);
                });
                question.clearIncorrectValues();
                $img.mapster(settings);
                updateValueHandler();
            };
            var onStateChangeHandler = function (data) {
                if (isSettingValue) return;
                if (data.state !== "select") return;

                isSettingValue = true;
                var selectedAreas = $img.mapster("get");
                question.value = selectedAreas.length > 0 ? selectedAreas.split(",") : null;
                isSettingValue = false;
            };

            question.registerFunctionOnPropertyValueChanged(
                "visibleChoices",
                function() {
                    updateChoices();
                }
            );

            settings.mapKey = "data-key";
            settings.onStateChange = onStateChangeHandler;

            updateChoices();

            question.valueChangedCallback = updateValueHandler;
            updateValueHandler();
        },
        willUnmount: function (question, el) {
            question.valueChangedCallback = null;
            var $img = $(el).find("img");
            $img.mapster("unbind");
        }
    };

    Survey.CustomWidgetCollection.Instance.addCustomWidget(widget);
}

if (typeof Survey !== "undefined") {
    init(Survey, window.$);
}