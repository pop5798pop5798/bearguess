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
   

    $form.find(".Btn").attr("disabled", true);
}

function betresult(responseText, statusText, xhr, $form) {
    if (responseText.isTrue) {
        $form.find("input[name$=money]").val("");
        updateAll();
        alert('預測此題成功');

    }
    else {
        if (responseText.ErrorCode== 2) {
            Swal.fire({
                title: '錯誤',
                html: responseText.ErrorMsg,
                type: 'error',
                confirmButtonText: '前往登入!',
                showCancelButton: true
            }).then((conf) => {
                if (conf.value) {
                    window.location.href = 'http://' + location.hostname + "/Account/Login?returnUrl=" + location.href;
                }


            });

        } else {
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

        
    }
    $form.find(".Btn").removeAttr("disabled");

}

function TopicHtmlGenerator(value)
{
    var datego = value.edate;
    var newDate = datego.replace(/T/g, ' ');
    var htmlstr = "";
    htmlstr += ' \
            <div name="card" id='+ value.md5TopicSn + ' class="item col-xs-12 col-sm-6 col-md-6 menucolor"> \
        <div id="internal">\
                    <div class="sportsmagazine-fancy-title4">\
                        <h2>\
                         ' + (value.walk == 1 ? '<img src="/Content/extra-images/rank-bet-100.png" style="width: 80px; position: absolute;right: 0;top: 0;">' : '') + value.title + ' \
                        </h2>\
                       '+ (value.live == 1 ? '<h5 style="float:right;font-size:14px;margin:-30px 0px;color:#ebdec2;padding:10px"> 預估' + newDate + '&nbsp;後停止預測</h5>' : '') + '\
                    </div>\
                     \
                        ' + (value.comment == null ? '' : '<h4 class="card-title">題目說明：' + value.comment + '</h4>') + ' \
                    \
                    <div '+ (value.betmodel == 1 ? "style='display:none'":'') +' class="progres-title">\
                        <div class=info>\
                            <div class="progress">';
    $.each(value.choiceList, function (ckey, cvalue) {
        htmlstr += '\
                                <div name="p' + cvalue.md5ChoiceSn + '" class="progress-bar progress-bar-striped active" role="progressbar"\
                                        aria-valuenow="'+ cvalue.betMoneyRate + '" aria-valuemin="0" aria-valuemax="100"\
                                        style="width: '+ cvalue.betMoneyRate + '%;">\
                                </div>';
    });
    htmlstr += '\
                            </div>\
                            <span>';
    $.each(value.choiceList, function (ckey, cvalue) {
        htmlstr += '\
                                <p name="p' + cvalue.md5ChoiceSn + '">' + cvalue.choiceStr + '： ' + cvalue.betMoney + ' (' + cvalue.betMoneyRate + '%)</p>';
    });
    htmlstr += '\
                            </span>\
                        </div>\
                    </div>\
                    \
                    <div class="card-body"> \
                        <div class="betin"> \
                            <form name="betForm" action="/bet/Create" method="post"> \
                            <input type="hidden" name="edate" class="canbet" value="'+ newDate +'"> \
                            <input name="walk" type="hidden" value="' + value.walk + '"> \
                            <input name="valid" type="hidden" value="' + value.valid + '"> \
';

    var canbet = value.canbet;
  //  var betModel = $("input[name$='game.betModel']").val();  
   // canbet = (betModel == 7) ? true : value.canbet;
    $.each(value.choiceList, function (ckey, cvalue) {
        htmlstr += ' \
                                <div class="row choiceRow">\
                                    <div class="col-xs-8 col-md-9 col-lg-9 detailscolor">\
                                        <p>'+ cvalue.choiceStr + '</p>\
                                    </div>\
                                    <div class="col-xs-4 col-md-3 col-lg-3">\
';
        if (canbet) {
            var OddsFunction = false;
            if (cvalue.Odds != null)
                OddsFunction = true;
            htmlstr += ' \
                                        <input id="EchoiceSn" name="betList['+ ckey + '].EchoiceSn" type="hidden" value="' + cvalue.EchoiceSn + '"> \
                                        <input name="Odds" type="hidden" value="' + cvalue.Odds + '"> \
                                        <input class="form-control canbet" id="money" name="betList['+ ckey + '].money" type="tel" pattern="^\+?[1-9][0-9]*0$|^$|^0$" \
                                            '+ (OddsFunction ? 'placeholder="' + cvalue.Odds + '" onfocus="showOddsPanel(this)" onblur="hideOddsPanel(this)" onkeyup="showOddsDetail(this)"' : "")
                                        +' inputmode="numeric" value=""> \
                ';
        }
        else {
            htmlstr += ' \
                ';
        }
        htmlstr += ' \
                                    </div> \
                                </div> \
            ';
    });
    htmlstr += '\
                                <div class="row" style="padding:10px 0px 10px 0px">\
                                    <div class="col-md-3"> \
                                    </div> \
                                    <div class="col-md-12" style="display:flex;align-items:center;justify-content:center;">';
    if (canbet) {
        htmlstr += '\
                                       <a class="Btn Btn--primary Btn--purple Btn--shadow Btn--wide canbet" href="#idontwanttogoanywhere">\
            <span class="Btn-inner">\
                <span class="Btn-innerContent">\
                    點擊預測\
                                                    </span>\
                                                </span >\
                                            </a> \ ';
    }
    htmlstr +='\
                                    </div> \
                                </div>\
                            </form>\
                        </div>\
                        <p></p>\
                    </div>\
                 </div>\
            </div>\
    ';
    return htmlstr;
}

function createTopic(topicData) {
    $.each(JSON.parse(topicData), function (key, value) {
        var htmlstr = "";
        htmlstr = TopicHtmlGenerator(value);
        $("div[name=ddTopic]").append(htmlstr);
    });
    $('.Btn').on('click', function () {       
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
    $('.loading').hide();
}

function updateTopic(topicData) {
    $.each(JSON.parse(topicData), function (key, value) {
        var htmlstr = "";
        htmlstr = TopicHtmlGenerator(value);
        if ($("#" + value.md5TopicSn).length >= 1) {
            $("#" + value.md5TopicSn).html($(htmlstr).html());
        }
        else {
            var $container = $("div[name=ddTopic]");
            var $content = $(htmlstr);
            $container.append($content).masonry('appended', $content);
        }
    });
    $('.Btn').on('click', function () {
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
    $(obj).parents(".choiceRow").next().find(".oddsDetail").html('<p style="border-radius: 20px;border:1px solid;color:#d6d6d6;font-size:14px;font-weight:bold;padding:0px 10px">此選項預知正確：' + '獲得<span style="color:#ea1300">' + Math.round(money * odds) + '</span>' + '鮭魚積分' +'</p>');
}

///檢查題目的時間是不是已經過了，自動關閉
function checkTopicTimeOut() {
   // var betModel = $("input[name$='game.betModel']").val();  
   
    $("input[name=edate]").each(function (i, obj) {
        var topicModel = $(obj).parents("form").find("input[name=walk]").val();
        //alert(topicModel);
            if (Date.parse($(obj).val()) < Date.now() && topicModel !== "1") {
               $(obj).parents("form").find(".canbet").remove();
            }
    });

    $("input[name=valid]").each(function (i, obj) {
        //alert(topicModel);
        if ($(obj).val() == 3 || $(obj).val() == 5) {
            $(obj).parents("form").find(".canbet").remove();
        }


    });
    
}
