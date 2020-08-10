var initializeDatatables = function (locale) {
    $('.datatables').DataTable({
        "language": {
            "url": getLanguageJson(locale)
        }
    });
};

var destroyDatatables = function () {
    $('.datatables').DataTable().destroy();
};

function getLanguageJson(locale) {
    if (locale === "fr")
        return "/js/datatables-fr.json"
    return "";
}
