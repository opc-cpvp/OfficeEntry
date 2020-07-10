var interopJS = interopJS || {}

interopJS.spotsAvailablePerHours = {
    init: function (id, classStyle, languageCode, data) {

        document
            .querySelector('#blazor-spotsAvailablePerHours-wraper')
            .innerHTML = `<div id="${id}" class="${classStyle}"></div>`;

        google.charts.load('current', { 'packages': ['bar'] });
        google.charts.setOnLoadCallback(drawStuff);

        function drawStuff() {
            var data = google.visualization.arrayToDataTable([
                [
                    languageCode === "en" ? 'Spots available per hour' : 'Places disponibles par heure',
                    languageCode === "en" ? 'Spots reserved'           : 'Places réservés',
                    languageCode === "en" ? 'Spots available'          : 'Places disponibles',
                    { role: 'annotation' }
                ],
                // todo: replace fake data with data argument
                [languageCode === "en" ?  "7AM" :  '7h' , 10, 24, ''],
                [languageCode === "en" ?  "8AM" :  '8h' , 16, 22, ''],
                [languageCode === "en" ?  "9AM" :  '9h' , 16, 22, ''],
                [languageCode === "en" ? "10AM" : '10h' , 16, 22, ''],
                [languageCode === "en" ? "11AM" : '11h' , 16, 22, ''],
                [languageCode === "en" ? "12PM" : '12h' , 16, 22, ''],
                [languageCode === "en" ?  "1PM" : '13h' , 28, 19, ''],
                [languageCode === "en" ?  "2PM" : '14h' , 28, 19, ''],
                [languageCode === "en" ?  "3PM" : '15h' , 28, 19, ''],
                [languageCode === "en" ?  "4PM" : '16h' , 28, 19, ''],
                [languageCode === "en" ?  "5PM" : '17h' , 28, 19, ''],
            ]);

            var options = {
                //legend: { position: 'top', maxLines: 3 },
                legend: { position: "none" },
                bar: { groupWidth: '75%' },
                isStacked: true,
                series: {
                    0: { color: 'Black' },
                    1: { color: 'MediumSeaGreen' },
                }
            };

            var chart = new google.charts.Bar(document.getElementById(id));
            // Convert the Classic options to Material options.
            chart.draw(data, google.charts.Bar.convertOptions(options));
        };
    }
};

window.interop = interopJS;