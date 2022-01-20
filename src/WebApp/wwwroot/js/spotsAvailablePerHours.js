var interopJS = interopJS || {}

interopJS.spotsAvailablePerHours = {
    chartData: null,
    init: function (id, title, labels, datasetLabels, datasetsData) {

        document
            .querySelector("#blazor-spotsAvailablePerHours-wraper")
            .innerHTML = '<canvas id="spots-available-per-hours-chart"></canvas>';

        var ctx = document.getElementById("spots-available-per-hours-chart").getContext("2d");

        var chart = new Chart(ctx, {
            type: "bar",
            data: {
                labels: labels, // responsible for how many bars are gonna show on the chart
                datasets: [{
                    label: datasetLabels[0],
                    data: datasetsData[0],
                    backgroundColor: "Black",
                    hoverBackgroundColor: "Black" // Not needed on chart.js version 3
                }, {
                    label: datasetLabels[1],
                    data: datasetsData[1],
                    backgroundColor: "MediumSeaGreen",
                    hoverBackgroundColor: "MediumSeaGreen" // Not needed on chart.js version 3
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                legend: {
                    position: "top" // place legend on the top of chart
                },
                title: {
                    display: true,
                    text: title
                },
                ////// TODO: Uncomment when we no longer need to support IE and can update chart.js to version 3
                ////scales: {
                ////    x: {
                ////        stacked: true // this should be set to make the bars stacked
                ////    },
                ////    y: {
                ////        stacked: true, // this also..
                ////        ticks: {
                ////            //stepSize: 1
                ////            beginAtZero: true,
                ////            callback: function (value) { if (value % 1 === 0) { return value; } }
                ////        }
                ////    }
                ////}
                // TODO: Remove once we update to version 3 of chart.js
                scales: {
                    xAxes: [{
                        stacked: true
                    }],
                    yAxes: [{
                        stacked: true,
                        ticks: {
                            //stepSize: 1
                            beginAtZero: true,
                            callback: function (value) { if (value % 1 === 0) { return value; } }
                        }
                    }]
                }
            }
        });
    }
};

window.interop = interopJS;