var initializeSelect2 = function (locale) {
    if (locale == null) {
        locale = "en";
    }

    $.fn.select2.defaults.set("language", locale);
}