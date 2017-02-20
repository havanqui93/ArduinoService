// define colors of line chart
var colors = ["#FF0707", "#6E07FF", "#07FFFF", "#07FF51", "#E6FF07", "#FF7207", "#000000", "#F700EA"];
var myLineChart;
var dataChartLocal = null;

function GetTokenKey() {
    var currentLocation = window.location.pathname;
    return currentLocation.split('/')[3];
}

$('#filterDate').datetimepicker({
    showTodayButton: false
}).on('dp.change', function (ev) {
    //ShowLoading();
    $('#contentchart').html('');
    dataChartLocal = GetDataChart($(this).val());
    CreateChart(dataChartLocal, 'line');
    //HideLoading();
});

$('#btnToday').on('click', function () {
    //ShowLoading();
    $('#contentchart').html('');
    dataChartLocal = GetDataChart(GetDateToday());
    CreateChart(dataChartLocal, 'line');
    $('#filterDate').val(GetDateToday());
    //HideLoading();
});


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
    dataChartLocal = GetDataChart(GetDateToday()); // 1 : today
    CreateChart(dataChartLocal, "line");
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
    var arrChartType = { VIEW: "line" };
    switch (typechart) {
        case "line": arrChartType = { VIEW: "line" }; break;
        case "bar": arrChartType = { VIEW: "bar" }; break;
    }
    var START = 0, END = 100;

    $.each(data, function (index, data) {
        // check start/end
        if (data.GROUP_SENSOR_ID != 1) {
            START = 0;
            END = 100;
        }
        else {
            if (data.CHART_NAME.indexOf("[Nhiệt độ]") != -1) {
                START = -50;
                END = 50;
            }
            else {
                START = 0;
                END = 100;
            }
        }

        $('#contentchart').append('<label class="sensorchart">' + data.CHART_NAME + '</label><div id="chartbox_' + index + '" class="chartbox"></div>');

        var datajson = [];
        for (var i = 0; i < data.CHART_DATA.length; i++) {
            datajson.push({ VALUE: data.CHART_DATA[i].VALUE, DAY: data.CHART_DATA[i].DAY });
        }
        dataChart = {
            view: arrChartType.VIEW, //"spline",
            container: 'chartbox_' + index,
            value: "#VALUE#",
            //label:"#VALUE#",
            color: "#2b9fe4",
            width: 30,
            labelLines: true,
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
                title: "Thời gian (giờ)",
                template: "#DAY#"
            },
            yAxis: {
                title: '(' + data.UNIT_NAME + ')',
                start: START,
                step: 10,
                end: END,
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

        // Custom position
        //$('.dhx_axis_title_y').attr('style', 'top: 0px !important');



    });
}

