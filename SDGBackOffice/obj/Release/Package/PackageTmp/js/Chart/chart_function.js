var monthNames = ["January", "February", "March", "April", "May", "June",
            "July", "August", "September", "October", "November", "December"
];
var dateObj = new Date();
//var month = dateObj.getUTCMonth() + 1; //months from 1-12
//var day = dateObj.getUTCDate();
var year = dateObj.getUTCFullYear();
var randomScalingFactor = function () { return Math.round(Math.random() * 1000000) };

//Start chart1
function getChart1(date, r, m, b, p) {

    var date = new Date(date);

    var day = date.getDate();
    var month = date.getMonth();
    var year = date.getUTCFullYear();

    $.ajax({
        url: rootDir + 'Ajax/GetDailyTransactionCount',
        type: 'POST',
        data: { 'date': (month + 1) + "/" + day + "/" + year, 'r': r, 'm': m, 'b': b, 'p': p },
        success: function (data) {
            var cnt = [];

            var l = 0;

            var d = 0;

            var dt = new Date(data.data1[0].Count);
            var dt1 = new Date(data.data1[1].Count);
            var dt2 = new Date(data.data1[2].Count);
            var dt3 = new Date(data.data1[3].Count);
            var dt4 = new Date(data.data1[4].Count);
            var dt5 = new Date(data.data1[5].Count);
            var dt6 = new Date(data.data1[6].Count);

            for (i = 0; i < 7; i++) {

                try { var cl = data.data2[i].Column.toString().split(",")[0]; } catch (error) { cl = 0; }
                try { var count = data.data2[i].Count; } catch (error) { count = 0; }

                if (cl != 0) {
                    var dDate = new Date(cl);
                    d = dDate.getDate();
                } else {
                    d = 0;
                }

                if (dt.getDate() == d) {
                    cnt[0] = count;
                } else if (dt1.getDate() == d) {
                    cnt[1] = count;
                } else if (dt2.getDate() == d) {
                    cnt[2] = count;
                } else if (dt3.getDate() == d) {
                    cnt[3] = count;
                } else if (dt4.getDate() == d) {
                    cnt[4] = count;
                } else if (dt5.getDate() == d) {
                    cnt[5] = count;
                } else if (dt6.getDate() == d) {
                    cnt[6] = count;
                }
            }

            for (i = 0; i < 7; i++) {
                if (typeof (cnt[i]) === 'undefined') {
                    cnt[i] = 0;
                } else { }
            }

            var lineChartData = {
                labels: [dt.getDate() + "-" + monthNames[dt.getMonth()], dt1.getDate() + "-" + monthNames[dt1.getMonth()], dt2.getDate() + "-" + monthNames[dt2.getMonth()], dt3.getDate() + "-" + monthNames[dt3.getMonth()], 
                    dt4.getDate() + "-" + monthNames[dt4.getMonth()], dt5.getDate() + "-" + monthNames[dt5.getMonth()], dt6.getDate() + "-" + monthNames[dt6.getMonth()]],
                datasets: [
                    {
                        label: "Data set 1",
                        fillColor: "rgba(220,220,220,0.2)",
                        strokeColor: "rgb(23,127,255)",
                        pointColor: "rgb(23,127,255)",
                        pointStrokeColor: "#fff",
                        pointHighlightFill: "#fff",
                        pointHighlightStroke: "rgba(220,220,220,1)",
                        data: [cnt[0], cnt[1], cnt[2], cnt[3], cnt[4], cnt[5], cnt[6]]
                    }
                ]

            }

            var options =
            {
                tooltipTemplate: "<%= addCommas(value) %>",
                showTooltips: true,
                onAnimationComplete: function () { this.showTooltip(this.datasets[0].points, true); render1(); },
                tooltipEvents: [],
                responsive: true,
                scaleFontColor: "#5BC236",
                scaleColor: "#000"
            }

            var ctx = document.getElementById("graph1").getContext("2d");
            var chart = new Chart(ctx).Line(lineChartData, options);
        }
    });
}
//End chart 1


//Start chart 2
function getChart2(date, r, m, b, p) {
    var date = new Date(date);

    var day = date.getDate();
    var month = date.getMonth();
    var year = date.getUTCFullYear();

    $.ajax({
        url: rootDir + 'Ajax/GetDailySalesTransaction',
        type: 'POST',
        data: { 'date': (month + 1) + "/" + day + "/" + year, 'r': r, 'm': m, 'b': b, 'p': p },
        success: function (data) {

            var amt = [];

            var l = 0;

            var d = 0;

            var dt = new Date(data.data1[0].Count);
            var dt1 = new Date(data.data1[1].Count);
            var dt2 = new Date(data.data1[2].Count);
            var dt3 = new Date(data.data1[3].Count);
            var dt4 = new Date(data.data1[4].Count);
            var dt5 = new Date(data.data1[5].Count);
            var dt6 = new Date(data.data1[6].Count);

            for (i = 0; i < 7; i++) {

                try { var cl = data.data2[i].Column.toString().split(",")[0]; } catch (error) { cl = 0; }
                try { var count = data.data2[i].Amount; } catch (error) { count = 0; }

                if (cl != 0) {
                    var dDate = new Date(cl);
                    d = dDate.getDate();
                } else {
                    d = 0;
                }

                if (dt.getDate() == d) {
                    amt[0] = count;
                } else if (dt1.getDate() == d) {
                    amt[1] = count;
                } else if (dt2.getDate() == d) {
                    amt[2] = count;
                } else if (dt3.getDate() == d) {
                    amt[3] = count;
                } else if (dt4.getDate() == d) {
                    amt[4] = count;
                } else if (dt5.getDate() == d) {
                    amt[5] = count;
                } else if (dt6.getDate() == d) {
                    amt[6] = count;
                }
            }

            for (i = 0; i < 7; i++) {
                if (typeof (amt[i]) === 'undefined') {
                    amt[i] = 0;
                } else { }
            }

            var lineChartData2 = {
                labels: [dt.getDate() + "-" + monthNames[dt.getMonth()], dt1.getDate() + "-" + monthNames[dt1.getMonth()], dt2.getDate() + "-" + monthNames[dt2.getMonth()], dt3.getDate() + "-" + monthNames[dt3.getMonth()],
                    dt4.getDate() + "-" + monthNames[dt4.getMonth()], dt5.getDate() + "-" + monthNames[dt5.getMonth()], dt6.getDate() + "-" + monthNames[dt6.getMonth()]],
                datasets: [
                    {
                        label: "Data set 2",
                        fillColor: "rgba(220,220,220,0.2)",
                        strokeColor: "rgb(23,127,255)",
                        pointColor: "rgb(23,127,255)",
                        pointStrokeColor: "#fff",
                        pointHighlightFill: "#fff",
                        pointHighlightStroke: "rgba(220,220,220,1)",
                        data: [amt[0], amt[1], amt[2], amt[3], amt[4], amt[5], amt[6]]
                    }
                ]

            }

            var options2 =
            {
                tooltipTemplate: "<%= addCommas(value) %>",
                showTooltips: true,
                onAnimationComplete: function () { this.showTooltip(this.datasets[0].points, true); render2() },
                tooltipEvents: [],
                responsive: true,
                scaleFontColor: "#5BC236",
                scaleColor: "#000"
            }

            var ctx2 = document.getElementById("graph2").getContext("2d");
            var chart2 = new Chart(ctx2).Line(lineChartData2, options2);
        }
    });
}
//End chart 2


//Start chart 3
function getChart3(date, r, m, b, p) {
    var date = new Date(date);

    var day = date.getDate();
    var month = date.getMonth();
    var year = date.getUTCFullYear();

    $.ajax({
        url: rootDir + 'Ajax/GetDailyDeclinedTransactionCount',
        type: 'POST',
        data: { 'date': (month + 1) + "/" + day + "/" + year, 'r': r, 'm': m, 'b': b, 'p': p },
        success: function (data) {
            var cnt = [];

            var l = 0;

            var d = 0;

            var dt = new Date(data.data1[0].Count);
            var dt1 = new Date(data.data1[1].Count);
            var dt2 = new Date(data.data1[2].Count);
            var dt3 = new Date(data.data1[3].Count);
            var dt4 = new Date(data.data1[4].Count);
            var dt5 = new Date(data.data1[5].Count);
            var dt6 = new Date(data.data1[6].Count);
            for (i = 0; i < 7; i++) {

                try { var cl = data.data2[i].Column.toString().split(",")[0]; } catch (error) { cl = 0; }
                try { var count = data.data2[i].Count; } catch (error) { count = 0; }

                if (cl != 0) {
                    var dDate = new Date(cl);
                    d = dDate.getDate();
                } else {
                    d = 0;
                }

                if (dt.getDate() == d) {
                    cnt[0] = count;
                } else if (dt1.getDate() == d) {
                    cnt[1] = count;
                } else if (dt2.getDate() == d) {
                    cnt[2] = count;
                } else if (dt3.getDate() == d) {
                    cnt[3] = count;
                } else if (dt4.getDate() == d) {
                    cnt[4] = count;
                } else if (dt5.getDate() == d) {
                    cnt[5] = count;
                } else if (dt6.getDate() == d) {
                    cnt[6] = count;
                }
            }

            for (i = 0; i < 7; i++) {
                if (typeof (cnt[i]) === 'undefined') {
                    cnt[i] = 0;
                } else { }
            }

            var lineChartData3 = {
                labels: [dt.getDate() + "-" + monthNames[dt.getMonth()], dt1.getDate() + "-" + monthNames[dt1.getMonth()], dt2.getDate() + "-" + monthNames[dt2.getMonth()], dt3.getDate() + "-" + monthNames[dt3.getMonth()],
                    dt4.getDate() + "-" + monthNames[dt4.getMonth()], dt5.getDate() + "-" + monthNames[dt5.getMonth()], dt6.getDate() + "-" + monthNames[dt6.getMonth()]],
                datasets: [
                    {
                        label: "Data set 3",
                        fillColor: "rgba(220,220,220,0.2)",
                        strokeColor: "rgb(23,127,255)",
                        pointColor: "rgb(23,127,255)",
                        pointStrokeColor: "#fff",
                        pointHighlightFill: "#fff",
                        pointHighlightStroke: "rgba(220,220,220,1)",
                        data: [cnt[0], cnt[1], cnt[2], cnt[3], cnt[4], cnt[5], cnt[6]]
                        //data: [cnt, cnt1, cnt2, cnt3, cnt4, cnt5, cnt6]
                    }
                ]

            }

            var options3 =
            {
                tooltipTemplate: "<%= addCommas(value) %>",
                showTooltips: true,
                onAnimationComplete: function () { this.showTooltip(this.datasets[0].points, true); render3() },
                tooltipEvents: [],
                responsive: true,
                scaleFontColor: "#5BC236",
                scaleColor: "#000"
            }

            var ctx3 = document.getElementById("graph3").getContext("2d");
            var chart3 = new Chart(ctx3).Line(lineChartData3, options3);
        }
    });
}
//End chart 3

//Start debit chart 1
function getDebitChart1(date, r, m, b, p) {

    var date = new Date(date);

    var day = date.getDate();
    var month = date.getMonth();
    var year = date.getUTCFullYear();

    $.ajax({
        url: rootDir + '/Ajax/GetDailyDebitTransactionCount',
        type: 'POST',
        data: { 'date': (month + 1) + "/" + day + "/" + year, 'r': r, 'm': m, 'b': b, 'p': p },
        success: function (data) {
            var cnt = [];

            var l = 0;

            var d = 0;

            var dt = new Date(data.data1[0].Count);
            var dt1 = new Date(data.data1[1].Count);
            var dt2 = new Date(data.data1[2].Count);
            var dt3 = new Date(data.data1[3].Count);
            var dt4 = new Date(data.data1[4].Count);
            var dt5 = new Date(data.data1[5].Count);
            var dt6 = new Date(data.data1[6].Count);

            for (i = 0; i < 7; i++) {

                try { var cl = data.data2[i].Column.toString().split(",")[0]; } catch (error) { cl = 0; }
                try { var count = data.data2[i].Count; } catch (error) { count = 0; }

                if (cl != 0) {
                    var dDate = new Date(cl);
                    d = dDate.getDate();
                } else {
                    d = 0;
                }

                if (dt.getDate() == d) {
                    cnt[0] = count;
                } else if (dt1.getDate() == d) {
                    cnt[1] = count;
                } else if (dt2.getDate() == d) {
                    cnt[2] = count;
                } else if (dt3.getDate() == d) {
                    cnt[3] = count;
                } else if (dt4.getDate() == d) {
                    cnt[4] = count;
                } else if (dt5.getDate() == d) {
                    cnt[5] = count;
                } else if (dt6.getDate() == d) {
                    cnt[6] = count;
                }
            }

            for (i = 0; i < 7; i++) {
                if (typeof (cnt[i]) === 'undefined') {
                    cnt[i] = 0;
                } else { }
            }

            var lineChartData = {
                labels: [dt.getDate() + "-" + monthNames[dt.getMonth()], dt1.getDate() + "-" + monthNames[dt1.getMonth()], dt2.getDate() + "-" + monthNames[dt2.getMonth()], dt3.getDate() + "-" + monthNames[dt3.getMonth()],
                    dt4.getDate() + "-" + monthNames[dt4.getMonth()], dt5.getDate() + "-" + monthNames[dt5.getMonth()], dt6.getDate() + "-" + monthNames[dt6.getMonth()]],
                datasets: [
                    {
                        label: "Data set 1",
                        fillColor: "rgba(220,220,220,0.2)",
                        strokeColor: "rgb(23,127,255)",
                        pointColor: "rgb(23,127,255)",
                        pointStrokeColor: "#fff",
                        pointHighlightFill: "#fff",
                        pointHighlightStroke: "rgba(220,220,220,1)",
                        data: [cnt[0], cnt[1], cnt[2], cnt[3], cnt[4], cnt[5], cnt[6]]
                    }
                ]

            }

            var options =
            {
                tooltipTemplate: "<%= addCommas(value) %>",
                showTooltips: true,
                onAnimationComplete: function () { this.showTooltip(this.datasets[0].points, true); render1(); },
                tooltipEvents: [],
                responsive: true,
                scaleFontColor: "#5BC236",
                scaleColor: "#000"
            }

            var ctx = document.getElementById("graph1").getContext("2d");
            var chart = new Chart(ctx).Line(lineChartData, options);
        }
    });
}
//End debit chart 1


//Start debit chart 2
function getDebitChart2(date, r, m, b, p) {
    var date = new Date(date);

    var day = date.getDate();
    var month = date.getMonth();
    var year = date.getUTCFullYear();

    $.ajax({
        url: rootDir + 'Ajax/GetDailyDebitSalesTransaction',
        type: 'POST',
        data: { 'date': (month + 1) + "/" + day + "/" + year, 'r': r, 'm': m, 'b': b, 'p': p },
        success: function (data) {

            var amt = [];

            var l = 0;

            var d = 0;

            var dt = new Date(data.data1[0].Count);
            var dt1 = new Date(data.data1[1].Count);
            var dt2 = new Date(data.data1[2].Count);
            var dt3 = new Date(data.data1[3].Count);
            var dt4 = new Date(data.data1[4].Count);
            var dt5 = new Date(data.data1[5].Count);
            var dt6 = new Date(data.data1[6].Count);

            for (i = 0; i < 7; i++) {

                try { var cl = data.data2[i].Column.toString().split(",")[0]; } catch (error) { cl = 0; }
                try { var count = data.data2[i].Amount; } catch (error) { count = 0; }

                if (cl != 0) {
                    var dDate = new Date(cl);
                    d = dDate.getDate();
                } else {
                    d = 0;
                }

                if (dt.getDate() == d) {
                    amt[0] = count;
                } else if (dt1.getDate() == d) {
                    amt[1] = count;
                } else if (dt2.getDate() == d) {
                    amt[2] = count;
                } else if (dt3.getDate() == d) {
                    amt[3] = count;
                } else if (dt4.getDate() == d) {
                    amt[4] = count;
                } else if (dt5.getDate() == d) {
                    amt[5] = count;
                } else if (dt6.getDate() == d) {
                    amt[6] = count;
                }
            }

            for (i = 0; i < 7; i++) {
                if (typeof (amt[i]) === 'undefined') {
                    amt[i] = 0;
                } else { }
            }

            var lineChartData2 = {
                labels: [dt.getDate() + "-" + monthNames[dt.getMonth()], dt1.getDate() + "-" + monthNames[dt1.getMonth()], dt2.getDate() + "-" + monthNames[dt2.getMonth()], dt3.getDate() + "-" + monthNames[dt3.getMonth()],
                    dt4.getDate() + "-" + monthNames[dt4.getMonth()], dt5.getDate() + "-" + monthNames[dt5.getMonth()], dt6.getDate() + "-" + monthNames[dt6.getMonth()]],
                datasets: [
                    {
                        label: "Data set 2",
                        fillColor: "rgba(220,220,220,0.2)",
                        strokeColor: "rgb(23,127,255)",
                        pointColor: "rgb(23,127,255)",
                        pointStrokeColor: "#fff",
                        pointHighlightFill: "#fff",
                        pointHighlightStroke: "rgba(220,220,220,1)",
                        data: [amt[0], amt[1], amt[2], amt[3], amt[4], amt[5], amt[6]]
                    }
                ]

            }

            var options2 =
            {
                tooltipTemplate: "<%= addCommas(value) %>",
                showTooltips: true,
                onAnimationComplete: function () { this.showTooltip(this.datasets[0].points, true); render2() },
                tooltipEvents: [],
                responsive: true,
                scaleFontColor: "#5BC236",
                scaleColor: "#000"
            }

            var ctx2 = document.getElementById("graph2").getContext("2d");
            var chart2 = new Chart(ctx2).Line(lineChartData2, options2);
        }
    });
}
//End debit chart 2


//Start debit chart 3
function getDebitChart3(date, r, m, b, p) {
    var date = new Date(date);

    var day = date.getDate();
    var month = date.getMonth();
    var year = date.getUTCFullYear();

    $.ajax({
        url: rootDir + 'Ajax/GetDailyDebitDeclinedTransactionCount',
        type: 'POST',
        data: { 'date': (month + 1) + "/" + day + "/" + year, 'r': r, 'm': m, 'b': b, 'p': p },
        success: function (data) {
            var cnt = [];

            var l = 0;

            var d = 0;

            var dt = new Date(data.data1[0].Count);
            var dt1 = new Date(data.data1[1].Count);
            var dt2 = new Date(data.data1[2].Count);
            var dt3 = new Date(data.data1[3].Count);
            var dt4 = new Date(data.data1[4].Count);
            var dt5 = new Date(data.data1[5].Count);
            var dt6 = new Date(data.data1[6].Count);
            for (i = 0; i < 7; i++) {

                try { var cl = data.data2[i].Column.toString().split(",")[0]; } catch (error) { cl = 0; }
                try { var count = data.data2[i].Count; } catch (error) { count = 0; }

                if (cl != 0) {
                    var dDate = new Date(cl);
                    d = dDate.getDate();
                } else {
                    d = 0;
                }

                if (dt.getDate() == d) {
                    cnt[0] = count;
                } else if (dt1.getDate() == d) {
                    cnt[1] = count;
                } else if (dt2.getDate() == d) {
                    cnt[2] = count;
                } else if (dt3.getDate() == d) {
                    cnt[3] = count;
                } else if (dt4.getDate() == d) {
                    cnt[4] = count;
                } else if (dt5.getDate() == d) {
                    cnt[5] = count;
                } else if (dt6.getDate() == d) {
                    cnt[6] = count;
                }
            }

            for (i = 0; i < 7; i++) {
                if (typeof (cnt[i]) === 'undefined') {
                    cnt[i] = 0;
                } else { }
            }

            var lineChartData3 = {
                labels: [dt.getDate() + "-" + monthNames[dt.getMonth()], dt1.getDate() + "-" + monthNames[dt1.getMonth()], dt2.getDate() + "-" + monthNames[dt2.getMonth()], dt3.getDate() + "-" + monthNames[dt3.getMonth()],
                    dt4.getDate() + "-" + monthNames[dt4.getMonth()], dt5.getDate() + "-" + monthNames[dt5.getMonth()], dt6.getDate() + "-" + monthNames[dt6.getMonth()]],
                datasets: [
                    {
                        label: "Data set 3",
                        fillColor: "rgba(220,220,220,0.2)",
                        strokeColor: "rgb(23,127,255)",
                        pointColor: "rgb(23,127,255)",
                        pointStrokeColor: "#fff",
                        pointHighlightFill: "#fff",
                        pointHighlightStroke: "rgba(220,220,220,1)",
                        data: [cnt[0], cnt[1], cnt[2], cnt[3], cnt[4], cnt[5], cnt[6]]
                        //data: [cnt, cnt1, cnt2, cnt3, cnt4, cnt5, cnt6]
                    }
                ]

            }

            var options3 =
            {
                tooltipTemplate: "<%= addCommas(value) %>",
                showTooltips: true,
                onAnimationComplete: function () { this.showTooltip(this.datasets[0].points, true); render3() },
                tooltipEvents: [],
                responsive: true,
                scaleFontColor: "#5BC236",
                scaleColor: "#000"
            }

            var ctx3 = document.getElementById("graph3").getContext("2d");
            var chart3 = new Chart(ctx3).Line(lineChartData3, options3);
        }
    });
}
//End Debit Chart3

window.onload = function () {
    //Original Code for Chart.js
    //window.myLine = new Chart(ctx).Line(lineChartData, {
    //responsive: true,
    //showTooltips: true,
    //tooltipTemplate: "<%= datasetLabel %> - <%= value %>",
    //multiTooltipTemplate: "<%= datasetLabel %> - <%= value %>"
    //});
}

function addCommas(nStr) {
    nStr += '';
    x = nStr.split('.');
    x1 = x[0];
    x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }
    return x1 + x2;
}

function render1() {
    var url = document.getElementById("graph1").toDataURL();
    document.getElementById("url1").src = url;
}

function render2() {
    var url = document.getElementById("graph1").toDataURL();
    document.getElementById("url2").src = url;
}

function render3() {
    var url = document.getElementById("graph1").toDataURL();
    document.getElementById("url3").src = url;
}
