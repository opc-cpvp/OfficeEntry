var interopJS = interopJS || {};

interopJS.datatables = {
    init: function (locale) {
        $(".datatables").DataTable({
            "language": {
                "url": getLanguageJson(locale)
            },
            "bSort": false,
            responsive: true
        });

        function getLanguageJson(locale) {
            if (locale === "fr")
                return "/js/datatables-fr.json"
            return "";
        }
    },

    destroy: function () {
        $(".datatables").DataTable().destroy();
    }
};

window.interop = interopJS;