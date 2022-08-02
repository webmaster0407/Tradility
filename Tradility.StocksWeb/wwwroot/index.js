"use strict"
function getLang(){
    let r = window.location.pathname.replace('/','');
    return r;
}

let widget;
let chart;
let symbol;
let lines = [];
let ts;
function initOnReady() {
    widget = new TradingView.widget({
        debug: true, // uncomment this line to see Library errors and warnings in the console
        fullscreen: true,
        symbol: symbol || 'AAPL',
        interval: '1D',
        container: "tv_chart_container",

        //	BEWARE: no trailing slash is expected in feed URL
        datafeed: new Datafeeds.UDFCompatibleDatafeed("http://localhost:9999/udf"), // new Datafeeds.UDFCompatibleDatafeed("https://demo-feed-data.tradingview.com"),// 
        library_path: "charting_library/",
        locale: "en",
        
        disabled_features: ["use_localstorage_for_settings"],
        enabled_features: ["study_templates"],
        charts_storage_url: 'https://saveload.tradingview.com',
        charts_storage_api_version: "1.1",
        client_id: 'tradingview.com',
        user_id: 'public_user_id',
    });   

    widget.onChartReady(() => {

        console.log('ChartReady');
        chart = widget.activeChart();
        ts = chart.getTimeScale();
        
        debugger;
         var points = chart._chartWidget._model._model().timeScale()._points._items;
         var lastDate = new Date(points[points.length - 1] * 1000);
        for (let index = 0; index < 365; index++) {
             lastDate.setDate(lastDate.getDate() + 1);
             if ((lastDate.getDay() % 6))
                 points.push(lastDate.getTime() / 1000);
                 //points.push(0);
        }
        //ts._timeScale.setRightOffset(-120);
        ts._timeScale.setRightOffset(0);

        lines.forEach(x => {
            chart.createMultipointShape(x.points, {
                shape: 'ray',
                //disableSelection: true,
                lock: true,
                disableUndo: true,
                zOrder: x.highOrder ? 'top' : 'bottom',
                overrides: {
                    linecolor: x.lineColor,
                    linewidth: x.lineWidths,
                    linestyle: x.lineStyle,
                    showLabel: true,
                    horzLabelsAlign: x.textAlign,
                    text: x.text,
                    bold: true,
                    showPrice: true,
                    textcolor: x.textColor,
                    extendLeft: x.extLeft,
                    extendRight: x.extRight
                }
            });

            
        });    

        console.log("onChartReady()//////////////////////////////////////////////");
    });
};

function setSymbol(newSymbol) {
    symbol = newSymbol;
    console.log("setSymbol() //////////////////////////////////////////////");
    console.log(symbol + " " + newSymbol);
}

function addLine(points, lineStyle, lineColor, lineWidths, text, textColor, textAlign, extLeft, extRight, highOrder) {
    let line = {
        points,
        lineStyle,
        lineColor,
        lineWidths,
        text,
        textColor,
        textAlign,
        extLeft,
        extRight,
        highOrder
    };
    console.log("addLine() //////////////////////////////////////////////");
    console.log(line);
    lines.push(line);
}

function addHLine(price, time, text, color) {
    var points = [
        {
            price,
            time
        },
        {
            price,
            time: (new Date()).getTime() / 1000
        }
    ];
    addLine(points, 0, color, 5, text, color, 'center', true, true, false);

    console.log("addHLine() //////////////////////////////////////////////");
}

function addOPT(price, date, text, color) {
    var points = [
        {
            price,
            time: 0
        },
        {
            price,
            time: date
        }
    ];
    addLine(points, 0, color, 5, text, color, 'right', true, false, true);

    console.log("addOPT() //////////////////////////////////////////////");
}

function addOPTExp(price, time, text) {
    var points = [
        {
            price: price - 1,
            time
        },
        {
            price: price + 1,
            time
        }
    ];
    let color = '#000000';
    addLine(points, 2, color, 3, text, color, 'center', true, true, true);

    console.log("addOPTExp() //////////////////////////////////////////////");
    console.log(line);
}

function addVLine(price, time, text, textAlign, color, extLeft, extRight) {
    debugger;
    var points = [
        {
            price: price - 1,
            time
        },
        {
            price: price + 1,
            time
        }
    ];

    addLine(points, 0, color, 3, text, color, textAlign, extLeft, extRight, true);

    console.log("addVLine() //////////////////////////////////////////////");
}

function addTrade(price, time, text, color) {
    debugger;
    var points = [
        {
            price,
            time
        },
        {
            price,
            time: (new Date()).getTime() / 1000
        }
    ];

    addLine(points, 0, color, 5, text, color, false, true, false);

    console.log("addTrade() //////////////////////////////////////////////");
}

function cleanLines() {
    lines = [];
    console.log("cleanLines() //////////////////////////////////////////////");
}

function save() {
    initOnReady();
    console.log("save() //////////////////////////////////////////////");
}

window.addEventListener('DOMContentLoaded', initOnReady, false);

//function setOrderLineParams(price, text) {
//    params.orderLine = {};
//    params.orderLine.price = price;
//    params.orderLine.quantity = price;
//    params.orderLine.text = text;

//    console.log("setOrderLineParams()//////////////////////////////////////////////");
//}

//function setChartSymbolAndPrice(symbol, price, time) {
//    params = {};
//    params.symbol = symbol;

//    if (price > 0 && time > 0)
//        setRayParams(price, time);
//    else if (price > 0)
//        setOrderLineParams(price, 'Average Cost');
//    initOnReady();
//    console.log("setChartSymbolAndPrice()//////////////////////////////////////////////");
//    return true;
//}

//if (params?.orderLine) {
        //    chart
        //    .createOrderLine()
        //    .setLineLength(3)               
        //    .setLineStyle(0)
        //    .setPrice(params.orderLine.price)
        //    .setQuantity(params.orderLine.quantity)
        //    .setText(params.orderLine.text);
        //}