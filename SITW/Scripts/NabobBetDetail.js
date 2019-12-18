$(document).ready(function () {
    setInterval(checkTopicTimeOut, 3000);

});


function betcheck(arr, $form, options) {
    var msg = "";
    $form.find("[name=money]").each(function (key, value) {
        if ($(value).val() != "") {
            var m = $(value).val();
            if (!$.isNumeric(m)) {
                alert("請輸入數字");
                return false;
            }
            if (m <= 0) {
                alert("請輸入正確預測金額");
                return false;
            }
        } 
    });
    $form.find("#betButton").attr("disabled", true);
}

function betresult(responseText, statusText, xhr, $form) {
    if (responseText.isTrue) {
        $form.find("input[name$='.money']").val("");
        $form.find(".libutton").removeClass("selected");
        $form.find("#dgonmoney").val(1);
        $form.find("#betModal").modal('toggle');
        $("#bet_m").text($("#dgonmoney").val() * 100);
        //$form.find("div[name='ddTopic']").scrollTop(0);
        $form.find("div[name='ddTopic']").animate({ scrollTop: 0 }, 'fast');
        updateAll();
        myAlert('預測成功', '恭喜你已成功預測此局！');    
    }
    else {
        Swal.fire({
            title: '錯誤',
            html: responseText.ErrorMsg,
            type: 'error',
            confirmButtonText: '前往認證!',
            showCancelButton: true
        }).then((conf) => {
            if (conf.value) {
                window.location.href = 'http://' + location.hostname + "/Manage";
            }


        });
    }
    $form.find("#betButton").removeAttr("disabled");

}

function TopicHtmlGenerator(value,i,j)
{
    var datego = value.edate;
    var newDate = datego.replace(/T/g, ' ');
    var col = "";
    var htmlstr = "";
    if (i < 4) {
        col = "4";
    } else {
        col = "6";
    }
    htmlstr += ' \
            <div id='+ value.md5TopicSn + ' class="item col-xs-12 col-sm-6 col-md-' + col + ' menucolor" name="tbet['+ (i-1) +'].topics"> \
        <div id="internal">\
                    <div class="sportsmagazine-fancy-title4" style="height:55px;background-color: rgba(0, 0, 0, 0.25);border:0px">\
                     <img style="width: 60px;position: absolute;top: -10px;" src="'+ value.image + '"/><h2 style="border:0px">' + value.title +'</h2> \
                    </div>\
       \
                      \
                    \
                    <div class="progres-title">\
                        <div class=info>';
    htmlstr += '\
                            <span>';
    htmlstr += '\
                            </span>\
                        </div>\
                    </div>\
                    <h4 class="card-title" style="">\
                        ' + (value.comment == null ? '' : '題目說明：' +value.comment) + ' \
                    </h4>\
                    <div class="card-body"> \
                        <div class="betin"> \
                            \
                            <input type="hidden" name="edate" class="canbet" value="'+ newDate +'"> \
    <table style="border:5px solid #0000;"><tdody><tr><td>\
    <ul class="ui-choose choose-type-right" id="uc_03">\
';

    var canbet = value.canbet;

    $.each(value.choiceList, function (ckey, cvalue) {
        ckey += j; 
        htmlstr += ' \
                                \
                                    \
                                        \
                                      \
                                   \
                                    \
';
        if (canbet) {
            var OddsFunction = false;
            if (cvalue.Odds != null)
                OddsFunction = true;
            htmlstr += ' \
                                        <input id="EchoiceSn" name="betList['+ ckey + '].EchoiceSn" type="hidden" value="' + cvalue.EchoiceSn + '"> \
                                        <input name="Odds" type="hidden" value="' + cvalue.Odds + '"> \
                                       <li class="libutton">' + cvalue.choiceStr + '<input id="money" name="betList[' + ckey + '].money" type="hidden" value=""></li>\
                                        \
                                           \
                                                \
                                            \
                                        \
                ';
        }
        else {
            htmlstr += ' \
                ';
        }
        htmlstr += ' \
                                    \
                                \
            ';
    });
    htmlstr += '</ul></td></tr></tbody></table> \
                                <div class="row">\
                                    <div class="col-md-3"> \
                                    </div> \
                                    <div class="col-md-6">';  
    htmlstr +='\
                                    </div> \
                                </div>\
                            \
                        </div>\
                        <p></p>\
                    </div>\
                 </div>\
            </div>\
    ';
    return htmlstr;
}

function createTopic(topicData) {
    var i = 0;
    var j = 0;
    $.each(JSON.parse(topicData), function (key, value) {
        var htmlstr = "";      
        i++;
        htmlstr = TopicHtmlGenerator(value,i,j);
        $("div[name=ddTopic]").append(htmlstr);
        if (i < 4) {
            j += 2;
        }
        else if (i == 4) {
            j += 4;
        }
        else {
            j += 5;
        }
            
    });
    $('#betButton').click(function () {
        var attr = $(this).attr("disabled");
        if (typeof attr == typeof undefined) {          
            $(this).parents('form').submit();
        }
    });
    $('form[name=betForm]').ajaxForm({
        beforeSubmit: betcheck,
        success: betresult
    });
    var $container = $(".masonry");
    $container.masonry({
        
    });
    /*$("input[name$='.money']").on('click', function () {
        
        $(this).parents(".betin").find("input[name$='.money']").val("");
        $(this).parents(".betin").find("input[name$='.money']").prop("checked", false);
        $(this).prop("checked", true);
        if ($(this).prop("checked")) {
            $(this).val(100);
            
        }

    });*/
    /*$("#betModal").on("hidden.bs.modal", function () {
        if (poolsubmit == 1) {
            $("#poolli").show();
        }
        poolsubmit = 0;
    });*/

    $("#betButtonClose").on('click', function () {
        $("#betModal").modal('toggle');


    });

    

    $(".libutton").on('click', function () {
        
        $(this).parents(".betin").find("input[name$='.money']").val("");
        $(this).parents(".betin").find(".libutton").removeClass("selected");
        $(this).addClass("selected");
        if ($(this).attr("class") == "libutton selected") {
            $(this).find("input[name$='.money']").val($("#dgonmoney").val() * 100);
                

        }
        //$(this).addClass("selected");


    });

    $("#betlimit").click(function () {
       // var bethtml = "";
        var betcount = 0;
        var topicstring = "";
        topicstring += "<table><tbody>";
        for (var i = 0; i < 5; i++) {
            //  alert('good');
            var img = $("div[name='tbet[" + i + "].topics']").find("img").attr("src");
            var choice = $("div[name='tbet[" + i + "].topics']").find(".selected").text();
            var tilte = $("div[name='tbet[" + i + "].topics']").find("h2").text();
            var count = $("div[name='tbet[" + i + "].topics']").find(".selected").length;
            betcount += count;
            topicstring += "<tr><td><img title=" + tilte + " class='topicsimg' src=" + img + " >  </td><td style='color:#fff;'>"+ choice +"</td></tr>";
           // var topicstring = "<img class='topicsimg' src=" + img + " >" + " <i style='display:inline;color:#fff' class='fa fa-arrows-h' aria-hidden='true'></i>" + "&nbsp;" + choice + "&nbsp;";
            

            //alert($("div[name='tbet[" + i + "].topics']").find("img").attr("src"));
           // bethtml += topicstring;
        }
        topicstring += "</tbody></table>";
        //alert(bethtml)
        if (betcount == 5) {
            $(this).attr("data-target", "#betModal");
        } else {
            $(this).attr("data-target", "");
            myAlert('填空單錯誤', '請選完題目，再點擊預測！');    
        }
        $("#bet_html").html(topicstring);
        
    });
    $("#bet_m").text($("#dgonmoney").val() * 100);   

    $("#dgonmoney").keyup(function () {
        var qtyx = $(this).val().indexOf(".") + 1;
        if (parseInt($(this).val()) <= 0) {
            alert("輸入數值有誤");
            $(this).val(1);
            $("#bet_m").text($(this).val() * 100);
        } else if (qtyx > 0) {
            alert("輸入數值有誤");
            $(this).val(1);
            $("#bet_m").text($(this).val() * 100);
        } else if (isNaN($(this).val())) {
            alert("輸入數值有誤");
            $(this).val(1);
        } else {
            $("#bet_m").text($(this).val() * 100);
            $(".selected").find("input[name$=money]").val($(this).val() * 100);

        }



    });
    $('.loading').hide();


}

function updateTopic(topicData) {
    var i = 0;
    var j = 0;
    $.each(JSON.parse(topicData), function (key, value) {
        var htmlstr = "";
        i++;
        htmlstr = TopicHtmlGenerator(value, i, j);     

        if ($("#" + value.md5TopicSn).length >= 1) {
            $("#" + value.md5TopicSn).html($(htmlstr).html());
        }
        else {
            var $container = $("div[name=ddTopic]");
            var $content = $(htmlstr);
            $container.append($content).masonry('appended', $content);
        }
        if (i < 4) {
            j += 2;
        }
        else if (i == 4) {
            j += 4;
        }
        else {
            j += 5;
        }

    });
    $('#betButton').click(function () {
        var attr = $(this).attr("disabled");
        if (typeof attr == typeof undefined) {           
            $(this).parents('form').submit();
        }
    });
    $('form[name=betForm]').ajaxForm({
        beforeSubmit: betcheck,
        success: betresult
    });
}


function updateBetMoney(topicData) {
    
    $.each(JSON.parse(topicData), function (key, value) {
        
        $.each(value.choiceList, function (ckey, cvalue) {
            $("div[name=p" + cvalue.md5ChoiceSn + "]").css("width", cvalue.betMoneyRate + "%");
            $("p[name=p" + cvalue.md5ChoiceSn + "]").html(cvalue.choiceStr + '： ' + cvalue.betMoney + ' (' + cvalue.betMoneyRate + '%)');

        });
    });
    
    /*
    chartstring = "[" + chartstring + "]";

    暫時移除highcharts
    Highcharts.setOptions({
        colors: ['#058DC7', '#50B432', '#ED561B', '#DDDF00', '#24CBE5', '#64E572', '#FF9655', '#FFF263', '#6AF9C4']
    });

    Highcharts.chart('container', {
        chart: {
            renderTo: 'chart',
            margin: 0,
            defaultSeriesType: 'areaspline',
            backgroundColor: 'rgba(255, 255, 255, 0.0)',
            type: 'bar'
        },
        yAxis: {
            gridLineWidth: 0,
            visible: false
        },
        xAxis: {
            gridLineWidth: 0,
            visible: false
        },
        title: {
            text: ""
        },
        exporting: { enabled: false },
        credits: {
            enabled: false
        },
        legend: {
            enabled: false
        },
        plotOptions: {
            series: {
                stacking: 'percent'
            },
            bar: {
                borderRadius: 5
            }
        },
        tooltip: {
            pointFormat: '<span style="color:{series.color}">{series.name}</span>: <b>{point.y}</b> ({point.percentage:.0f}%)<br/>',
            headerFormat: "",
            shared: true
        },
        series: JSON.parse(chartstring)
    });
    */


}

function showOddsPanel(obj) {
    if ($(obj).parents(".choiceRow").next().attr("name") != "OddsPanel") {
        var oddspanelhtml = '<div name="OddsPanel" class="row OddsPanel detailscolor"> \
            <div class="col-xs-12 col-md-12 col-lg-12 oddsDetail">\
                \
            </div>\
        </div>';

        $(obj).parents(".choiceRow").after(oddspanelhtml);
        showOddsDetail(obj);

        var $container = $(".masonry");
        $container.masonry({

        });
    }
}
function hideOddsPanel(obj) {
    if ($(obj).val() == "" || $(obj).val() == "0") {
        if ($(obj).parents(".choiceRow").next().attr("name") == "OddsPanel") {
            $(obj).parents(".choiceRow").next().remove();
            var $container = $(".masonry");
            $container.masonry({

            });
        }
    }
}

function showOddsDetail(obj) {
    var odds = $(obj).siblings("input[name=Odds]").val();
    var money = $(obj).val();
    var divname = $(obj).siblings("[name$=EchoiceSn]").val();
    $(obj).parents(".choiceRow").next().find(".oddsDetail").html('<p style="color:#d6d6d6;font-size:14px;font-weight:bold;">猜中獲利：' + '<span style="color:#ea1300">' + money + '</span>' + ' × ' + odds + ' = ' + ' × ' + odds + ' = ' + '<span style="color:#ea1300">' + Math.round(money * odds) + '</span>' + '</p>');
}

///檢查題目的時間是不是已經過了，自動關閉
function checkTopicTimeOut() {
    $("input[name=edate]").each(function (i,obj) {
        if (Date.parse($(obj).val()) < Date.now())
        {
            $("#mask_css").css("display", "block");
            $(obj).parents("form").find(".canbet").remove();
        }
    });
}
(function ($) {
    $.alerts = {
        alert: function (title, message, callback) {
            if (title == null) title = 'Alert';
            $.alerts._show(title, message, null, 'alert', function (result) {
                if (callback) callback(result);
            });
        },

        confirm: function (title, message, callback) {
            if (title == null) title = 'Confirm';
            $.alerts._show(title, message, null, 'confirm', function (result) {
                if (callback) callback(result);
            });
        },


        _show: function (title, msg, value, type, callback) {

            var _html = "";

            _html += '<div id="mb_box"></div><div id="mb_con"><span id="mb_tit">' + title + '</span>';
            _html += '<div id="mb_msg">' + msg + '</div><div id="mb_btnbox">';
            if (type == "alert") {
                _html += '<input id="mb_btn_ok" type="button" value="確定" />';
            }
            if (type == "confirm") {
                _html += '<input id="mb_btn_no" type="button" value="取消" />';
                _html += '<input id="mb_btn_ok" type="button" value="確定" />';
            }
            _html += '</div></div>';

            //必須先將_html添加到body，再設置Css樣式  
            $("body").append(_html); GenerateCss();

            switch (type) {
                case 'alert':

                    $("#mb_btn_ok").click(function () {
                        $.alerts._hide();
                        callback(true);
                    });
                    $("#mb_btn_ok").focus().keypress(function (e) {
                        if (e.keyCode == 13 || e.keyCode == 27) $("#mb_btn_ok").trigger('click');
                    });
                    break;
                case 'confirm':

                    $("#mb_btn_ok").click(function () {
                        $.alerts._hide();
                        if (callback) callback(true);
                    });
                    $("#mb_btn_no").click(function () {
                        $.alerts._hide();
                        if (callback) callback(false);
                    });
                    $("#mb_btn_no").focus();
                    $("#mb_btn_ok, #mb_btn_no").keypress(function (e) {
                        if (e.keyCode == 13) $("#mb_btn_ok").trigger('click');
                        if (e.keyCode == 27) $("#mb_btn_no").trigger('click');
                    });
                    break;
            }
        },
        _hide: function () {
            $("#mb_box,#mb_con").remove();
        }
    }
    // Shortuct functions  
    myAlert = function (title, message, callback) {
        $.alerts.alert(title, message, callback);
    }

    myConfirm = function (title, message, callback) {
        $.alerts.confirm(title, message, callback);
    };



    //生成Css  
    var GenerateCss = function () {

        $("#mb_box").css({
            width: '100%', height: '100%', zIndex: '99999', position: 'fixed',
            filter: 'Alpha(opacity=60)', backgroundColor: 'black', top: '0', left: '0', opacity: '0.6'
        });

        $("#mb_con").css({
            zIndex: '999999', width: '350px', height: '200px', position: 'fixed',
            backgroundColor: 'White',
        });

        $("#mb_tit").css({
            display: 'block', fontSize: '14px', color: '#444', padding: '10px 15px',
            backgroundColor: '#fff', borderRadius: '15px 15px 0 0',
            fontWeight: 'bold'
        });

        $("#mb_msg").css({
            padding: '10px', lineHeight: '40px', textAlign: 'center',
            fontSize: '18px', color: '#4c4c4c'
        });

        $("#mb_ico").css({
            display: 'block', position: 'absolute', right: '10px', top: '9px',
            border: '1px solid Gray', width: '18px', height: '18px', textAlign: 'center',
            lineHeight: '16px', cursor: 'pointer', borderRadius: '12px', fontFamily: '微軟雅黑'
        });

        $("#mb_btnbox").css({ margin: '15px 0px 10px 0', textAlign: 'center' });
        $("#mb_btn_ok,#mb_btn_no").css({ width: '80px', height: '30px', color: 'white', border: 'none', borderRadius: '4px' });
        $("#mb_btn_ok").css({ backgroundColor: '#41a259' });
        $("#mb_btn_no").css({ backgroundColor: 'gray', marginRight: '40px' });


        //右上角關閉按鈕hover樣式  
        $("#mb_ico").hover(function () {
            $(this).css({ backgroundColor: 'Red', color: 'White' });
        }, function () {
            $(this).css({ backgroundColor: '#DDD', color: 'black' });
        });

        var _widht = document.documentElement.clientWidth; //屏幕寬  
        var _height = document.documentElement.clientHeight; //屏幕高  

        var boxWidth = $("#mb_con").width();
        var boxHeight = $("#mb_con").height();

        //讓提示框居中  
        $("#mb_con").css({ top: (_height - boxHeight) / 2 + "px", left: (_widht - boxWidth) / 2 + "px" });
    }


})(jQuery); 