/* Canadian-French initialisation for the jQuery UI date picker plugin. */
(function (factory) {
	if (typeof define === "function" && define.amd) {

		// AMD. Register as an anonymous module.
		define(["../widgets/datepicker"], factory);
	} else {

		// Browser globals
		factory(jQuery.datepicker);
	}
}(function (datepicker) {
	fetch('/json/datepicker-fr.json')
		.then(function (response) { return response.json(); })
		.then(function (data) {
			datepicker.regional["fr-CA"] = data;
		});
}));

var initializeDatepicker = function (locale) {
	if (locale === 'fr') {
		return $.datepicker.setDefaults($.datepicker.regional['fr-CA']);
	}
	return $.datepicker.setDefaults($.datepicker.regional['']);
}