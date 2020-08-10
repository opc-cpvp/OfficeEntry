var interopJS = interopJS || {};

interopJS.accessRequest = {
    getNextDayDate: function () {
        var today = new Date();
        // replace function fixes bug in IE where toLocaleDateString generates unicode characters
        return new Date(today.setDate(today.getDate() + 1)).toLocaleDateString("en-US").replace(/\u200E/g, '');
    },

    init: function() {
         window.interop.survey.survey
             .onAfterRenderQuestion
             .add(function(survey, options) {
                 if (options.question.name === "startDate") {
                     options.question.defaultValue = window.interop.accessRequest.getNextDayDate();
                 }                      
             });


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
    }
};

window.interop = interopJS;