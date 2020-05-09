//bounding c# obj to js obj async
let scriptPath = "", htmlPath = "", chart_champion_1, chart_champion_2, color = { red: "#c45850", green:"#3cba9f"};
let serviceData, Rotate = ["our turn", "enemy turn"], BP_ing = ["Banning", "Picking", "Banning", "Picking"], Lanes = ["Top", "Mid", "Jungle", "ADCarry", "Support"], BP=["ban","pick","ban","pick"];
let instruction;
const chart_config_1 = {
    type: 'bar',
    data: {
        labels: ["weight","win rate","ban rate"],
        datasets: [{
            label: "Champion",
            backgroundColor: [color.red,color.green, color.red],
            data:[0.5,0.5,0.5]
        }]
    },
    options: {
        responsive: true,
        maintainAspectRatio: false,
        legend: { display: false },
        title: {
            display: false
        },
         scales: {
            yAxes: [{
                ticks: {
                    suggestedMin: -0.8,
                    suggestedMax: 0.8
                }
            }]
        }
    }
};
const chart_config_2 = {
    type: 'bar',
    data: {
        labels: ["assistant", "counter", "enemy afraid"],
        datasets: [{
            label: "Champion",
            backgroundColor: [ color.green, color.red, color.green],
            data: [ 0.1, -0.1, 0.15]
        }]
    },
    options: {
        responsive: true,
        maintainAspectRatio: false,
        legend: { display: false },
        title: {
            display: false
        },
        scales: {
            yAxes: [{
                ticks: {
                    suggestedMin: -0.15,
                    suggestedMax: 0.15
                }
            }]
        }
    }
};
(async function() {

    await CefSharp.BindObjectAsync("boundedObj", "bound");


    var promiseOne = boundedObj.getContentScript();//<- get content html path from bounded object
    var promiseTwo = boundedObj.getContentHtml();//<- get content script path from bounded object

    [scriptPath, htmlPath] = await Promise.all([promiseOne, promiseTwo]);
 
    $("#pageContent").load(htmlPath, function (data) { //<- load content
        $.loadScript(scriptPath, function () {
            initToolTip();
            getAvatars();
            chart_champion_1 = initChart($('#championChart_1')[0], chart_config_1);
            chart_champion_2 = initChart($('#championChart_2')[0], chart_config_2);
            var elems = document.querySelectorAll('.fixed-action-btn');
            var instances = M.FloatingActionButton.init(elems, {
                direction: 'left',
                hoverEnabled: false
            });
        });       
    }); 
			
    //register events 
    $("#showMainPage").click(function () {
        $('#pageContent').fadeToggle('fast', function () {
            $("#pageContent").load(htmlPath, function (data) { //<- load content
                $.loadScript(scriptPath, function () {
                    if (chart_champion_1 != null)
                    {
                        chart_champion_1.destroy();
                    }
                    chart_champion_1 = initChart($('#championChart_1')[0], chart_config_1);
                    if (chart_champion_2 != null) {
                        chart_champion_2.destroy();
                    }
                    chart_champion_2 = initChart($('#championChart_2')[0], chart_config_2);
                    initToolTip();
                    getAvatars();
                    $('#pageContent').fadeToggle('fast');
                });                
            });
        })		
    });

    $("#webCrawler").click(function () {
        $('#pageContent').fadeToggle('fast', function () {
            $("#pageContent").load('crawler.html', function (data) { //<- load content
                $('#pageContent').fadeToggle('fast');
                $.loadScript('../js/html_js/crawlerPage.js', function () {

                    initToolTip();
                    chart_champion_1.destroy();
                    chart_champion_2.destroy();
                    var elems = document.querySelectorAll('.fixed-action-btn');
                    var instances = M.FloatingActionButton.init(elems, {
                        direction: 'left',
                        hoverEnabled: false
                    });

                });//<- load script after html was loaded

            });
        });
    });

    $("#showWebsite").click(function () {       
        var promise = boundedObj.requestService(JSON.stringify({ 'message': "website source" }));
    });
})();


function initToolTip() {
    $('.material-tooltip').remove();//<-material add these elements when every time $('.tooltipped').tooltip() called 
    $('.tooltipped').tooltip();//<-reactivate tool tip 
}

function initChart(element, chart_config, chartHeight) {
    let chartCanvas = element;
    let ctx = chartCanvas.getContext('2d');
    if (chartHeight != null) {
        ctx.height = chartHeight;
    }
    let chart = new Chart(ctx, chart_config);
    return chart;
}
function removeData(chart) {
    while (chart.data.datasets.length > 0)
    {
        chart.data.datasets.pop();
    }
    chart.update();
}
function addData(chart, dataset) {
    chart.data.datasets.push(dataset);
    chart.update();
}
function generateDataset(champion,chartID)
{
    console.log(champion)
    console.log(chartID)
    var dataset;
    var bop = BP[instruction.bPPhase];
    var backgroundColor = [color.red, color.green, color.red];
    if (chartID == 1) {
        if (bop == "pick") {
            backgroundColor[0] = color.green;
        }
        else if (bop == "ban") {
            backgroundColor[0] = color.red;
        }
        dataset = {
            label: champion.Name,
            backgroundColor: backgroundColor,
            data: [champion.Weight, champion.WinRate, champion.BanRate]
        }
    }
    else if (chartID == 2)
    {
        if (bop == "pick") {
            backgroundColor[0] = color.green;
            backgroundColor[1] = color.green;
            backgroundColor[2] = color.green;
        }
        else if (bop == "ban") {
            backgroundColor[0] = color.red;
            backgroundColor[1] = color.red;
            backgroundColor[2] = color.green;
        }
        dataset = {
            label: champion.Name,
            backgroundColor: backgroundColor,
            data: [champion.Assistant, champion.Counter, champion.EnemyAfraid]
        }
    }
    
    return dataset;
    
}
jQuery.loadScript = function (url, callback) {
    jQuery.ajax({
        url: url,
        dataType: 'script',
        success: callback,
        async: true
    });
}
