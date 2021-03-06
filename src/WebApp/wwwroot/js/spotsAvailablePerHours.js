﻿var interopJS = interopJS || {}

interopJS.spotsAvailablePerHours = {
    chartData : null,
    init: function (id, classStyle, languageCode, capacities) {

        chartData = capacities;

        document
            .querySelector('#blazor-spotsAvailablePerHours-wraper')
            .innerHTML = "<div id=" + id + " class=" + classStyle + "></div>";
            //.innerHTML = `<div id="${id}" class="${classStyle}"></div>`;

        google.charts.load('current', { 'packages': ['bar'] });
        google.charts.setOnLoadCallback(drawStuff);

        function drawStuff() {
            let array1 = [[
                languageCode === "fr" ? 'Places disponibles par heure' : 'Spots available per hour',
                languageCode === "fr" ? 'Places réservés' : 'Spots reserved',
                languageCode === "fr" ? 'Places disponibles' : 'Spots available',
                { role: 'annotation' }
            ]];
            let array3 = array1.concat(chartData);
            var data = google.visualization.arrayToDataTable(array3);

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