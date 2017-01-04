// define colors of line chart
var colors = ["#FF0707", "#6E07FF", "#07FFFF", "#07FF51", "#E6FF07", "#FF7207", "#000000", "#F700EA"];
var myLineChart;
var dataChartLocal = null;

function GetTokenKey() {
    var currentLocation = window.location.pathname;
    return currentLocation.split('/')[3];
}


doOnLoad();

$('#typechart').on('change', function () {
    // clear chart
    $('#contentchart').html('');

    var val = $(this).val();
    CreateChart(dataChartLocal, val);
});

$('#datechart').on('change', function () {
    $('#contentchart').html('');

    dataChartLocal = GetDataChart($(this).val());
    var val = $('#typechart').val();
    CreateChart(dataChartLocal, val);
});

function doOnLoad() {
    dataChartLocal = GetDataChart("1"); // 1 : today
    CreateChart(dataChartLocal, "spline");
}

// convert mm/dd/yyyy to new Date
function toDate(dateStr) {
    var parts = dateStr.split("/");
    return new Date(parts[2], parts[0] - 1, parts[1]);
}

function GetDataChart(datechart) {
    var tokenkey = GetTokenKey();
    var result;
    $.ajax({
        url: '/Home/GetDataChart',
        type: 'GET',
        async: false,
        dataType: 'json',
        data: { 'tokenkey': tokenkey, 'datechart': datechart },
        success: function (data) {
            result = data;
        }
    });
    return result;
}

function CreateChart(data, typechart) {
    // define when change type char
    var arrChartType = { VIEW: "spline" };
    switch (typechart) {
        case "spline": arrChartType = { VIEW: "spline" }; break;
        case "bar": arrChartType = { VIEW: "bar" }; break;
    }

    $.each(data, function (index, data) {
        $('#contentchart').append('<label class="sensorchart">' + data.CHART_NAME + '</label><div id="chartbox_' + index + '" class="chartbox"></div>');

        var datajson = [];
        for (var i = 0; i < data.CHART_DATA.length; i++) {
            datajson.push({ VALUE: data.CHART_DATA[i].VALUE, DAY: data.CHART_DATA[i].DAY });
        }
        dataChart = {
            view: arrChartType.VIEW, //"spline",
            container: 'chartbox_' + index,
            value: "#VALUE#",
            color: "#2b9fe4",
            width: 30,
            item: {
                borderColor: "#1293f8",
                color: "#ffffff"
            },
            line: {
                color: "#1293f8",
                width: 2
            },
            tooltip: {
                template: "#VALUE#"
            },
            offset: 0,
            xAxis: {
                template: "#DAY#"
            },
            yAxis: {
                start: 0,
                step: 50,
                end: 500,
                template: function (value) {
                    return value % 5 ? "" : value
                }
            },
            padding: {
                left: 35,
                bottom: 50,
                right: 50
            },
            origin: 0,
            //legend: {
            //    layout: "x",
            //    width: 75,
            //    align: "center",
            //    valign: "bottom",
            //    margin: 10
            //}
        };
        // add new line
        myLineChart = new dhtmlXChart(dataChart);
        myLineChart.parse(datajson, "json");

    });
}

