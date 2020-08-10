var initializeDatatables = function (locale) {
    $('.datatables').DataTable({
        "language": {
            "url": getLanguageJson(locale)
        },
        responsive: true
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
