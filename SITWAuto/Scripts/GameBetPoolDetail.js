var countbetall = 0;
var orderall = 0;
var poolsubmit = 0;

$(document).ready(function () {
    setInterval(checkTopicTimeOut, 3000);
    $("#loginModal").modal();
});


function betcheck(arr, $form, options) {
    var msg = "";
    $form.find("[name=money]").each(function (key, value) {
        if ($(value).val() != "")
        {
            var m = $(value).val();
            if (!$.isNumeric(m)) {
                alert("請輸入數字");
                return false;
            }
            if (m <= 0) {
                alert("請輸入正確下注金額");
                return false;
            }
        }
    });
   
    $form.find("#betButton").attr("disabled", true);
};

function betresult(responseText, statusText, xhr, $form) {
    if (responseText.isTrue) {      
        $form.find("input[name$=money]").val("");
        $form.find("input[name$=count]").val("0");
        $("div[name$=betnumber]").find("img").attr("src", "/Content/extra-images/dgon/dgon_null.png");
        $("div[name$=betnumber]").find("img").attr("alt", "");
        $("div[name$=betnumber]").removeClass('hover_dgon__portrait').addClass('dgon__portrait');
        $(".hover_dgon_number__portrait").removeClass('hover_dgon_number__portrait').addClass('dgon_number__portrait');
        $(".dgon-box").hide();
        $(".fromnumber").find("span").text("0");
        $form.find("#dgonmoney").val(1);
        $form.find("#betModal").modal('toggle');
        countbetall = 0;
        //$(".letters").html();
        $("#poolli").hide();
        poolsubmit = 1;
        
        updateAll();
        
    }
    else {
        myAlert('錯誤', responseText.ErrorMsg); 
        //alert(responseText.ErrorMsg);
    }
    $form.find("#betButton").removeAttr("disabled");

}
function ball(value,betan,money) {
    var htmlstr = "";

    var canbet = value.canbet;
    htmlstr += '\
            <div class="bear_head">\
            <span class="'+ betan + '">+' + money + '</span>\
            <span class="betballstyle">龍的傳人</span>\
            <div class="main clearfix" style="position:absolute;top:80px;left:25px;cursor:pointer;">\
                <div class="drop">\
                    <div class="dropafter" style="height:'+ value.choiceList[0].betball + "em" + '"> \
                              <div class="illustration">  \
   <div class="i-medium"></div> \
   <div class="i-small"></div> \
  </div>  \
    </div>\
                </div>\
                        </div >\
            <div class="dgon_bonus">\
                 <table>\
                    <thead>\
                        <tr>\
                            <th>\
                                勝隊擊殺小龍數\
                                        </th>\
                            <th>\
                                額外獎勵\
                                        </th>\
                            <th>\
                                預估獎金\
                                        </th>\
                        </tr>\
                    </thead>\
                    <tbody style="color: #fff;">\
                        <tr>\
                            <td>0</td>\
                            <td>---</td>\
                            <td>25000</td>\
                        </tr>\
                        <tr>\
                            <td>1</td>\
                            <td>---</td>\
                            <td>15000</td>\
                        </tr>\
                        <tr>\
                            <td>2</td>\
                            <td>---</td>\
                            <td>10000</td>\
                        </tr>\
                        <tr>\
                            <td>3</td>\
                            <td>---</td>\
                            <td>6000</td>\
                        </tr>\
                        <tr>\
                            <td>4</td>\
                            <td>X</td>\
                            <td>'+ value.poolall * 10 / 100 + '</td>\
                        </tr>\
                        <tr>\
                            <td>4</td>\
                            <td>O</td>\
                            <td>'+ value.poolall * 20 / 100 + '</td>\
                        </tr>\
                        <tr>\
                            <td>5</td>\
                            <td>X</td>\
                            <td>'+ value.poolall * 30 / 100 + '</td>\
                        </tr>\
                        <tr>\
                            <td>5</td>\
                            <td>O</td>\
                            <td>'+ value.poolall * 40 / 100 +'</td>\
                        </tr>\
                        <tr>\
                            <td colspan="3">\
                                額外獎勵:猜中此場勝隊擊殺大龍數量(巴龍、遠古龍)，詳情請看遊戲規則\
                                        </td>\
                        </tr>\
                    </tbody>\
                </table> \
</div> \
     </div > \
        ';

   
    //animate({ padding: "10px 10px 10px 10px" }, 150);
    return htmlstr;
}



function TopicHtmlGenerator(value)
{
    var datego = value.edate;
    var newDate = datego.replace(/T/g, ' ');
    var htmlstr = "";
    htmlstr += '      \
            <div id='+ value.md5TopicSn + ' class="item col-xs-12 col-sm-12 col-md-12 menucolor"> \
       <div id="mask_css"> </div> \
        <div id="internal"> \
                    <h4 class="card-title" style="">\
                        ' + (value.comment == null ? '' : '題目說明：' + value.comment) + ' \
                    </h4>\
                    <div class="card-body"> \
                        <div class="donbetin"> \
                            \
    <div class="dgon_from">\
        <div class="dgon-list">\
    <span style="font-size:19px;color:#ffdc11;margin-bottom:10px;margin-top:20px">注單</span>\
    <div class="fromnumber"><div style="float:left;color:#fff;margin-right:10px;">勝隊擊殺：</div>火龍 * <span id="fire_dnnumber">0</span>地龍 * <span id="ground_dnnumber">0</span>風龍 * <span id="wind_dnnumber">0</span>水龍 * <span id="water_dnnumber">0</span>大龍 * <span id="big_dnnumber">0</span></div>\
    </div>\
        <div class="dgon-list">\
            <div class="dgon"> \
                <div class="dgon__portrait imageWrapper" id="bet00" name="betnumber">\
         <div class="dgon-box"><span>X</span></div >\
                    <img class="dgonimg Tile-image" src="/Content/extra-images/dgon/dgon_null.png" alt="li-li">\
                </div>\
            </div>\
            <div class="dgon">\
                <div class="dgon__portrait imageWrapper" id="bet01" name="betnumber">\
                            <div class="dgon-box"><span>X</span></div >\
                     <img class="dgonimg Tile-image" src="/Content/extra-images/dgon/dgon_null.png" alt="li-li">\
                </div>\
             </div>\
                  <div class="dgon">  \
                  <div class="dgon__portrait imageWrapper" id="bet02" name="betnumber">\
        <div class="dgon-box"><span>X</span></div >\
                      <img class="dgonimg Tile-image" src="/Content/extra-images/dgon/dgon_null.png" alt="li-li">\
                  </div>\
                        </div>\
                        <div class="dgon">\
                            <div class="dgon__portrait imageWrapper" id="bet03" name="betnumber">\
        <div class="dgon-box"><span>X</span></div >\
                                <img class="dgonimg Tile-image" src="/Content/extra-images/dgon/dgon_null.png" alt="li-li">\
                                            </div>\
                            </div>\
                            <div class="dgon">\
                                <div class="dgon__portrait imageWrapper" id="bet04" name="betnumber">\
                                    <div class="dgon-box"><span>X</span></div >\
                                    <img class="dgonimg Tile-image" src="/Content/extra-images/dgon/dgon_null.png" alt="li-li">\
                                            </div>\
                                </div>\
        </div>\
        <div class="dgon-list">\
             <span style="font-size:19px;color:#ffdc11;margin-bottom:10px;">請選擇小龍</span>\
        </div>\
        <div class="dgon-list">\
    <div class="dgon" id="fire">\
        <span class="dgon-text">火龍</span>\
                                    <div class="dgon__portrait imageWrapper" >\
                                        <img class="dgonimg Tile-image" src="/Content/extra-images/dgon/dgon-fire.png" alt="li-li">\
                                            </div>\
                                    </div>\
                                    <div class="dgon" id="ground">\
        <span class="dgon-text">地龍</span>\
                                        <div class="dgon__portrait imageWrapper">\
                                            <img class="dgonimg Tile-image" src="/Content/extra-images/dgon/dgon-ground.png" alt="li-li">\
                                            </div>\
                                        </div>\
                                        <div class="dgon" id="wind">\
        <span class="dgon-text">風龍</span>\
                                            <div class="dgon__portrait imageWrapper">\
                                                <img class="dgonimg Tile-image" src="/Content/extra-images/dgon/dgon-wind.png" alt="li-li">\
                                            </div>\
                                            </div>\
                                            <div class="dgon" id="water">\
        <span class="dgon-text">水龍</span>\
                                                <div class="dgon__portrait imageWrapper">\
                                                    <img class="dgonimg Tile-image" src="/Content/extra-images/dgon/dgon-water.png" alt="li-li">\
                                            </div>\
                                                </div>\
                                                <div class="dgon" id="nix">\
        <span class="dgon-text">無擊殺</span>\
                                                    <div class="dgon__portrait imageWrapper">\
                                                        <img class="dgonimg Tile-image" src="/Content/extra-images/dgon/dgon-nix.png" alt="li-li">\
                                            </div>\
                                                    </div>\
        </div>\
        <div class="dgon-list">\
                                                    <span style="font-size:19px;color:#ffdc11;margin-bottom:10px;">請選擇巴龍+遠古龍總數</span>\
                                                </div>\
        <div class="dgon-list">\
                                                    <div class="dgon">\
                                                        <div class="dgon_big__portrait imageWrapper">\
                                                            <img class="dgonimg Tile-image" src="/Content/extra-images/dgon/dgon-bigdn.png" alt="li-li">\
                                                        </div>\
                                                        </div>\
                                                        <div class="dgon">\
                                                            <div class="dgon_number__portrait imageWrapper">\
                                                                <div class="dgon_big">\
                                                                    <span>0<input type="hidden" name="betnumber" value="0"></span>\
                                                                </div>\
                                                            </div>\
                                                        </div>\
                                                        <div class="dgon">\
                                                            <div class="dgon_number__portrait imageWrapper">\
                                                                <div class="dgon_big">\
                                                                    <span>1<input type="hidden" name="betnumber" value="1"></span>\
                                                                </div>\
                                                            </div>\
                                                        </div>\
                                                        <div class="dgon">\
                                                            <div class="dgon_number__portrait imageWrapper">\
                                                                <div class="dgon_big">\
                                                                    <span>2<input type="hidden" name="betnumber" value="2"></span>\
                                                                </div>\
                                                            </div>\
                                                        </div>\
                                                        <div class="dgon">\
                                                            <div class="dgon_number__portrait imageWrapper">\
                                                                <div class="dgon_big">\
                                                                    <span>3<input type="hidden" name="betnumber" value="3"></span>\
                                                                </div>\
                                                            </div>\
                                                        </div>\
                                                        <div class="dgon">\                                                      \
                                                            <div class="dgon_number__portrait imageWrapper">\
                                                                <div class="dgon_big">\
                                                                    <h7>4&nbsp;UP<input type="hidden" name="betnumber" value="4"></h7>\
                                                                </div>\
                                                            </div>\
                                                        </div>\
                                                        <div class="bet_dgon">\
        <div class="Container-gon">\
    <a id="betlimit" data-toggle="modal" data-target="" class="Btn Btn--primary Btn--purple Btn--shadow Btn--wide" href="#" ><span class="Btn-inner"><span class="Btn-innerContent">下注<img src="/Content/extra-images/fish_bone.png" style="width: 35%; margin: 0 0px 2px 10px;"></span></span></a>\
                                                            </div>\
                                                        </div>\
                                                    </div>\
         \
    <div>\
';  
   
    htmlstr +='                         </div>\
                        <p></p>\
                    </div>\
                 </div>\
            </div>\
    ';
   

    return htmlstr;
}




function submitbet(value) {
    var datego = value.edate;
    var newDate = datego.replace(/T/g, ' ');
    var htmlstr = "";
    htmlstr += '<form name="betForm" action="/bet/LottoCreate" method="post"> \
        <input type="hidden" name="edate" class="canbet" value="'+ newDate +'"> \
    <div class="loginmodal modal fade" id="betModal" tabindex="-1" role="dialog">\
    <div class="modal-dialog" id="modal-width" role="document" style="margin-top:15%;">\
        <div class="sportsmagazine-login-box">\
            <div style="border:5px  #8c8c8c ridge;">\
            <div class="bet_from">\
                <div class="bet_class" style="\
    text-align: left;\
    margin-left: 6em;\
    margin-bottom: 0px;\
">\
                    <span style="color: #d5d5d5;">注單確認框</span>\
                </div>\
        <div class="bet_margin">\
        <div class="bet_class">\ \
<span style="color:#e5b22a">競猜主題</span> ： '+ value.bigtitle + '</div>\
                    <div class="bet_class">\ \
<span style="color:#e5b22a">競猜說明</span> ： '+ value.title +'</div>\
                    <div class="bet_class"><span style="color:#e5b22a">預測選項</span> ： 勝隊將會擊殺 <span id="bet_dgonhtml" style="color:#ffdc11"></span></div>\
                    <div class="bet_class" style="\
    margin-bottom: 30px;\
">\
                        <span style="color:#e5b22a">下注金額</span> ： <input class="form-control canbet" id="dgonmoney"  type="tel" pattern="^+?[1-9][0-9]*0$|^$|^0$" inputmode="numeric" value="1" style="\
    width: 80px;text-align:center;\
"> * 100 = <span id="bet_m"></span> 魚骨幣\
                    </div>\
        </div>\
        <div id="bet_combination">\
                <div style="float:left" class="Container-gon"><a style="float:left" id="betButtonClose" class="Btn Btn--primary Btn--purple Btn--shadow Btn--wide" href="#"><span class="Btn-inner"><span class="Btn-innerContent">返回重選\
                            </span></span></a></div>\
                     <div style="float:left" class="Container-gon"><a style="float:left" id="betButton" class="Btn Btn--primary Btn--purple Btn--shadow Btn--wide" href="#"><span class="Btn-inner"><span class="Btn-innerContent">確認投注                  \
     </span></span></a></div>\
               </div>   \
               </div>\
    </div>\
</div>\
        </div>\
        <div class="clearfix"></div>\
    </div>\
</div>\
';

    var canbet = value.canbet;

    $.each(value.choiceList, function (ckey, cvalue) {
        if (canbet) {
            var OddsFunction = false;
            if (cvalue.Odds != null)
                OddsFunction = true;
            htmlstr += ' \
                                        <input id="EchoiceSn" name="betList['+ ckey + '].EchoiceSn" type="hidden" value="' + cvalue.EchoiceSn + '"> \
                                        <input name="Odds" type="hidden" value="' + cvalue.Odds + '"> \
                                        <input id="'+ cvalue.dragonshort + 'strsn" class="form-control canbet" name="betList[' + ckey + '].strsn" type="hidden"  value=""> \
                                        <input id="'+ cvalue.dragonshort + 'count" class="form-control canbet" name="betList[' + ckey + '].count" type="hidden" value="0">\
                                        <input class="form-control canbet" id="money" name="betList['+ ckey + '].money" type="hidden" pattern="^\+?[1-9][0-9]*0$|^$|^0$" \
                                            '+ (OddsFunction ? 'placeholder="賠率 ' + cvalue.Odds + '" onfocus="showOddsPanel(this)" onblur="hideOddsPanel(this)" onkeyup="showOddsDetail(this)"' : "")
                + ' inputmode="numeric" value=""> \
                ';
        }
        else {
            htmlstr += ' \
                ';
        }
    });


    htmlstr += '</form >'; 
    return htmlstr;

}


function createTopic(topicData) {
    var poolall = 0;
    $.each(JSON.parse(topicData), function (key, value) {
        var htmlstr = "";
        htmlstr = TopicHtmlGenerator(value);
        htmlbetstr = ball(value);
        htmlsubmit = submitbet(value);
        $("div[name=ddTopic]").append(htmlstr);
        $("div[name=bear_ball]").html(htmlbetstr);
        $("div[name=submit]").html(htmlsubmit);
        //$(".dropafter").css("height", value.choiceList[0].betball+"em");
        orderall = value.choiceList[0].betMoney;
        poolall = value.poolall;
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

    $("#bet_m").text($("#dgonmoney").val() * 100);   
    $("#dgonmoney").keyup(function () {
        var qtyx = $(this).val().indexOf(".") + 1;
        if (parseInt($(this).val()) <= 0) {
            alert("輸入數值有誤");
            $(this).val(1);
            $("#bet_m").text($(this).val() * 100);
            dgonRate($(this).val() * 100, poolall);
        } else if (qtyx > 0) {
            alert("輸入數值有誤");
            $(this).val(1);
            $("#bet_m").text($(this).val() * 100);
            dgonRate($(this).val() * 100,poolall);
        } else if (isNaN($(this).val())) {
            alert("輸入數值有誤");
            $(this).val(1);
            dgonRate($(this).val() * 100, poolall);
        } else {
            $("#bet_m").text($(this).val() * 100);
            $("input[name$=money]").val($(this).val() * 100);
            dgonRate($(this).val() * 100, poolall);

        }

       

    });
    

    $("#betlimit").on("click", function () {

        var dgnai = 0;
        for (nixj = 0; nixj < 5; nixj++) {
            dgnbet = $("#bet0" + nixj).find("img").attr("src");
            if (dgnbet == "/Content/extra-images/dgon/dgon_null.png") {
                dgnai += 1;
            }
        }
        if (dgnai != 0) {
            myAlert('注單錯誤', '注單空格請填滿，不再增加的選項請用無擊殺取代！'); 

           // alert("注單空格請填滿再點擊，如不想再增加選項請用無擊殺取代");
            $(this).attr("data-target", "");
        } else {                       
            dgonRate(100, poolall);
            $("#bet_m").text($("#dgonmoney").val() * 100);
            $("input[name$=money]").val($("#dgonmoney").val() * 100);
            $(this).attr("data-target", "#betModal");
            

        }

    });

    $("#fire,#ground,#wind,#water,#nix").on('click', function () {
        $(this).find(".dgon__portrait").animate({ padding: "5px 5px 5px 5px" },150);
        $(this).find(".dgon__portrait").animate({ padding: "10px 10px 10px 10px" }, 150);
    });

    $("#poolli").hide();
    
    $("#loginModal").on("hidden.bs.modal", function () {
        $("#poolli").show();
    });
    $("#betModal").on("hidden.bs.modal", function () {
        if (poolsubmit == 1) {
            $("#poolli").show();
        }
        poolsubmit = 0;
    });

    $("#betButtonClose").on('click', function () {
        $("#betModal").modal('toggle');


        });


    var $container = $(".masonry");
    $container.masonry({
        
    });
    choicedn(topicData);
}


function dgonRate(ratemoney,poolall) {
    var vm = 0;
    for (vmoney = 0; vmoney < 5; vmoney++) {
        vm += parseInt($("input[name='betList[" + vmoney + "].count']").val());

    }
    var bouns = 0;
    switch (vm) {
        case 100:
            bouns = 250;
            break;
        case 1:
            bouns = 150;
            break;
        case 2:
            bouns = 100;
            break;
        case 3:
            bouns = 60;
            break;
        case 4:
            if (parseInt($("input[name='betList[5].count']").val()) > 0) {
                bouns = 20;
            } else {
                bouns = 10;
            }
            break;
        case 5:
            if (parseInt($("input[name='betList[5].count']").val()) > 0) {
                bouns = 40;
            } else {
                bouns = 30;
            }
            break;

    }
    //預估
    /*if (vm < 4) {
        $("#dgonmoney").removeAttr('readonly');
        $("#bouns_estimate").text(ratemoney + " * " + bouns + " = " + parseInt(ratemoney) * bouns + " 魚骨幣");

    } else if (vm == 100) {
        $("#dgonmoney").removeAttr('readonly');
        $("#bouns_estimate").text(ratemoney + " * " + bouns + " = " + parseInt(ratemoney) * bouns + " 魚骨幣");

    } else {
        $("#dgonmoney").val(1);
        $("#dgonmoney").attr("readonly", "readonly");
        $("#bouns_estimate").text(poolall + " * " + bouns + "%" + " = " + poolall * bouns / 100 + " 魚骨幣");
        
    }*/

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
    choicedn(topicData);
    
}

function choicedn(topicData) {
    var dn = [];
    var betdn = [];
    var count = 0;
   
    $.each(JSON.parse(topicData), function (key, value) {
        $.each(value.choiceList, function (ckey, cvalue) {
            //小龍點擊區
            dn[count] = "#" + cvalue.dragonshort; 
            //注單
            betdn[count] = "#bet0" + count;
            $("#" + cvalue.dragonshort + "strsn").val(count);
            count += 1;           
        });
    });
    


    //刪除注單龍數
    for (var beti = 0; beti < betdn.length; beti++) {
        $(document).on('click', betdn[beti], function (event) {
            var betalt = $(this).find("img").attr("alt");
            //是小龍則刪除
            if (betalt != "nix" && $(this).find("img").attr("src") != "/Content/extra-images/dgon/dgon_null.png") {
                $(this).find("img").attr("src", "/Content/extra-images/dgon/dgon_null.png");
                
                var dnbetcount = parseInt($("#" + betalt + "count").val());
                if (dnbetcount > 0) {
                    $("#" + betalt + "count").val(dnbetcount - 1);
                    $("#" + betalt + "_dnnumber").text(dnbetcount - 1);
                    $(this).removeClass('hover_dgon__portrait').addClass('dgon__portrait');
                    $(this).parents(".dgon").find(".dgon-box").hide();
                    countbetall -= 1;
                    betfrom(dn);
                }
              //是無擊殺則刪除
            } else {
                if ($(this).find("img").attr("src") != "/Content/extra-images/dgon/dgon_null.png") {
                    $(this).find("img").attr("src", "/Content/extra-images/dgon/dgon_null.png");
                    $(this).removeClass('hover_dgon__portrait').addClass('dgon__portrait');
                    $(this).parents(".dgon").find(".dgon-box").hide();
                    var dgnai = 0;
                    for (nixj = 0; nixj < 5; nixj++) {
                        var dgnbet = $("#bet0" + nixj).find("img").attr("src");
                        if (dgnbet == "/Content/extra-images/dgon/dgon-nix.png") {
                            dgnai += 1;
                        }
                    }
                    if (dgnai < 5) {
                        $("#" + betalt + "count").val(0);
                        countbetall -= 1;
                    }
                    betfrom(dn);
                }
                

                
            }
            
        });
    }
    $(document).on('click', '.dgon_number__portrait', function () {
        var bignumber = $(this).find("input[name=betnumber]").val();
        if (parseInt(bignumber) === 4) {
           
            $("#big_dnnumber").text(bignumber + " up");  
        } else {          
            $("#big_dnnumber").text(bignumber); 
        }
        
        $("#bigdncount").val(bignumber);
        $(".hover_dgon_number__portrait").removeClass('hover_dgon_number__portrait').addClass('dgon_number__portrait');
        $(this).removeClass('dgon_number__portrait').addClass('hover_dgon_number__portrait');
        betfrom(dn);
    });


    
    for (var i = 0; i < dn.length; i++) {
        $(document).on('click', dn[i], function (event) {
            var nix = $("#nixcount").val();
            var thisid = $(this).attr("id");
            
            var countnix = parseInt($("#nixcount").val());
            if (countbetall < 5 && thisid != "nix") {
                /*if (countnix == 100) {
                    $("input[name$=count]").val("0"); 
                    for (var nixj = 0; nixj < 5; nixj++) {
                        $("#bet0" + nixj).removeClass('dgon__portrait').addClass('hover_dgon__portrait');
                        $("#bet0" + nixj).removeClass('hover_dgon__portrait').addClass('dgon__portrait');
                        $("#bet0" + nixj).find("img").attr("src", "/Content/extra-images/dgon/dgon_null.png");
                        $("#bet0" + nixj).find("img").attr("alt", "");
                        countbetall = 0;
                    }
                }*/
                for (var j = 0; j < 5; j++) {
                    var dgnbet = $("#bet0" + j).find("img").attr("src");
                    if (dgnbet == "/Content/extra-images/dgon/dgon_null.png") {
                        $("#bet0" + j).removeClass('dgon__portrait').addClass('hover_dgon__portrait');
                        $("#bet0" + j).find("img").attr("src", "/Content/extra-images/dgon/dgon-" + thisid + ".png");
                        $("#bet0" + j).find("img").attr("alt", thisid);
                        $("#bet0" + j).parents(".dgon").find(".dgon-box").show();
                        j = 5;
                    }
                }

                //取得小龍數量自動生成
                var count = parseInt($("#" + thisid + "count").val());
                count += 1;
                $("#" + thisid + "count").val(count);
                $("#" + thisid + "_dnnumber").text(count);
                betfrom(dn);


                //預設100 money
                $(this).parents(".row").find("input[name$='.money']").val(100);
                 //大龍不自動增加
                if (thisid != "bigdn") {
                    countbetall += 1;
                }

            } else if (countbetall < 5 && thisid == "nix") {               
                var dgnai = 0;
                for (nixj = 0; nixj < 5; nixj++) {
                    dgnbet = $("#bet0" + nixj).find("img").attr("src");
                    if (dgnbet == "/Content/extra-images/dgon/dgon_null.png") {
                        $("#bet0" + nixj).removeClass('dgon__portrait').addClass('hover_dgon__portrait');
                        $("#bet0" + nixj).find("img").attr("src", "/Content/extra-images/dgon/dgon-" + thisid + ".png"); 
                        $("#bet0" + nixj).find("img").attr("alt", thisid);
                        $("#bet0" + nixj).parents(".dgon").find(".dgon-box").show();
                        nixj = 5;
                    }
                    
                  
                }

                //大龍不自動增加
                if (thisid != "bigdn") {
                    countbetall += 1;
                }


                for (nixj = 0; nixj < 5; nixj++) {
                    dgnbet = $("#bet0" + nixj).find("img").attr("src");
                    if (dgnbet == "/Content/extra-images/dgon/dgon-nix.png") {
                        dgnai += 1;
                    }
                }
                if (dgnai == 5) {
                    $(this).parents(".row").find("input[name$='.money']").val(100);
                    countbetall = 0;
                    $("input[name$=count]").val("0");
                    $(".fromnumber span").text(0);
                    $("#nixcount").val(100);
                    betfrom(dn);
                    countbetall = 5;
                }
                    
            }
            else {
                myAlert('注單錯誤', '已超過格數，請清除後再點擊！');      
            }


            
           

        });
    }
    

}

function betfrom(dgon){
    var dgonname = "";
    var bethtml = "";
    var nixnumber = 5;
    for (var v = 0; v < 6; v++) {
        switch (dgon[v]) {
            case "#fire":
                dgonname = "火龍";
                break;
            case "#ground":
                dgonname = "地龍";
                break;
            case "#wind":
                dgonname = "風龍";
                break;
            case "#water":
                dgonname = "水龍";
                break;

            case "#bigdn":
                dgonname = "大龍";
                dgon[v] = "#big";
                break;
            case "#big":
                dgonname = "大龍";
                dgon[v] = "#big";
                break;           
            default:
                dgonname = "";
                break;
        }
        if (dgonname != "") {
            if ($(dgon[v] + "_dnnumber").text() != "0" && $(dgon[v] + "_dnnumber").text() != "" && dgon[v] != "#nix") {
                bethtml += dgonname + " * " + $(dgon[v] + "_dnnumber").text() + " ";
                if (dgonname != "大龍")
                nixnumber -= 1;
            }
        }

    }
    if (nixnumber == 5) {
        bethtml = "0 隻小龍 " + bethtml;
    }
    $("#bet_dgonhtml").html(bethtml);

}



function updateBetMoney(topicData) {

    $.each(JSON.parse(topicData), function (key, value) {
        var htmlstr = "";
        var synall = value.choiceList[0].betMoney - orderall;
        htmlbetstr = ball(value, "betanim", synall);
        $("div[name=bear_ball]").html(htmlbetstr);
        $(".dropafter").animate({ height: "10em" },1200);
        $(".dropafter").animate({ height: value.choiceList[0].betball+"em" },900);
        //$(".dropafter").css("height", value.choiceList[0].betball + "em");

        $.each(value.choiceList, function (ckey, cvalue) {
            $("div[name=p" + cvalue.md5ChoiceSn + "]").css("width", cvalue.betMoneyRate + "%");
            $("p[name=p" + cvalue.md5ChoiceSn + "]").html(cvalue.choiceStr + '： ' + cvalue.betMoney + ' (' + cvalue.betMoneyRate + '%)');            
        });
        orderall = value.choiceList[0].betMoney;
        
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
    $("input[name=edate]").each(function (i, obj) {
        
        if (Date.parse($(obj).val()) < Date.now())
        {
            $(obj).parents("form").find(".canbet").remove();
            $(".bet_dgon").find(".Btn-innerContent").html("結束預測");
            $(".card-body").css("opacity", "0.5");
            $("#mask_css").css("display", "block");
            $("#betlimit").removeAttr("data-toggle");
            
            
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


//用logo版本
/*function TopicHtmlGenerator(value) {
    var datego = value.edate;
    var newDate = datego.replace(/T/g, ' ');
    var htmlstr = "";
    htmlstr += '      \
            <div id='+ value.md5TopicSn + ' class="item col-xs-12 col-sm-12 col-md-12 menucolor"> \
       <div id="mask_css"> </div> \
        <div id="internal"> \
                    <h4 class="card-title" style="">\
                        ' + (value.comment == null ? '' : '題目說明：' + value.comment) + ' \
                    </h4>\
                    <div class="card-body"> \
                        <div class="donbetin"> \
                            \
    <div class="dgon_from">\
        <div class="dgon-list">\
    <span style="font-size:19px;color:#ffdc11;margin-bottom:10px;margin-top:20px">注單</span>\
    <div class="fromnumber"><div style="float:left;color:#fff;margin-right:10px;">勝隊擊殺：</div>火龍 * <span id="fire_dnnumber">0</span>地龍 * <span id="ground_dnnumber">0</span>風龍 * <span id="wind_dnnumber">0</span>水龍 * <span id="water_dnnumber">0</span>大龍 * <span id="big_dnnumber">0</span></div>\
    </div>\
        <div class="dgon-list">\
            <div class="dgon"> \
                <div class="dgon__portrait imageWrapper" id="bet00" name="betnumber">\
         <div class="dgon-box"><span>X</span></div >\
                    <img class="dgonimg Tile-image" src="/Content/extra-images/dgon/dgon_null.png" alt="li-li">\
                </div>\
            </div>\
            <div class="dgon">\
                <div class="dgon__portrait imageWrapper" id="bet01" name="betnumber">\
                            <div class="dgon-box"><span>X</span></div >\
                     <img class="dgonimg Tile-image" src="/Content/extra-images/dgon/dgon_null.png" alt="li-li">\
                </div>\
             </div>\
                  <div class="dgon">  \
                  <div class="dgon__portrait imageWrapper" id="bet02" name="betnumber">\
        <div class="dgon-box"><span>X</span></div >\
                      <img class="dgonimg Tile-image" src="/Content/extra-images/dgon/dgon_null.png" alt="li-li">\
                  </div>\
                        </div>\
                        <div class="dgon">\
                            <div class="dgon__portrait imageWrapper" id="bet03" name="betnumber">\
        <div class="dgon-box"><span>X</span></div >\
                                <img class="dgonimg Tile-image" src="/Content/extra-images/dgon/dgon_null.png" alt="li-li">\
                                            </div>\
                            </div>\
                            <div class="dgon">\
                                <div class="dgon__portrait imageWrapper" id="bet04" name="betnumber">\
                                    <div class="dgon-box"><span>X</span></div >\
                                    <img class="dgonimg Tile-image" src="/Content/extra-images/dgon/dgon_null.png" alt="li-li">\
                                            </div>\
                                </div>\
        </div>\
        <div class="dgon-list">\
             <span style="font-size:19px;color:#ffdc11;margin-bottom:10px;">請選擇小龍</span>\
        </div>\
        <div class="dgon-list">\
    <div class="dgon" id="fire">\
        <span class="dgon-text">火龍</span>\
                                    <div class="dgon__portrait imageWrapper" >\
                                        <img class="dgonimg Tile-image" src="/Content/extra-images/dgon/dgon-fire.png" alt="li-li">\
                                            </div>\
                                    </div>\
                                    <div class="dgon" id="ground">\
        <span class="dgon-text">地龍</span>\
                                        <div class="dgon__portrait imageWrapper">\
                                            <img class="dgonimg Tile-image" src="/Content/extra-images/dgon/dgon-ground.png" alt="li-li">\
                                            </div>\
                                        </div>\
                                        <div class="dgon" id="wind">\
        <span class="dgon-text">風龍</span>\
                                            <div class="dgon__portrait imageWrapper">\
                                                <img class="dgonimg Tile-image" src="/Content/extra-images/dgon/dgon-wind.png" alt="li-li">\
                                            </div>\
                                            </div>\
                                            <div class="dgon" id="water">\
        <span class="dgon-text">水龍</span>\
                                                <div class="dgon__portrait imageWrapper">\
                                                    <img class="dgonimg Tile-image" src="/Content/extra-images/dgon/dgon-water.png" alt="li-li">\
                                            </div>\
                                                </div>\
                                                <div class="dgon" id="nix">\
        <span class="dgon-text">無擊殺</span>\
                                                    <div class="dgon__portrait imageWrapper">\
                                                        <img class="dgonimg Tile-image" src="/Content/extra-images/dgon/dgon-nix.png" alt="li-li">\
                                            </div>\
                                                    </div>\
        </div>\
        <div class="dgon-list">\
                                                    <span style="font-size:19px;color:#ffdc11;margin-bottom:10px;">請選擇巴龍+遠古龍總數</span>\
                                                </div>\
        <div class="dgon-list">\
                                                    <div class="dgon">\
                                                        <div class="dgon_big__portrait imageWrapper">\
                                                            <img class="dgonimg Tile-image" src="/Content/extra-images/dgon/dgon-bigdn.png" alt="li-li">\
                                                        </div>\
                                                        </div>\
                                                        <div class="dgon">\
                                                            <div class="dgon_number__portrait imageWrapper">\
                                                                <div class="dgon_big">\
                                                                    <span>0<input type="hidden" name="betnumber" value="0"></span>\
                                                                </div>\
                                                            </div>\
                                                        </div>\
                                                        <div class="dgon">\
                                                            <div class="dgon_number__portrait imageWrapper">\
                                                                <div class="dgon_big">\
                                                                    <span>1<input type="hidden" name="betnumber" value="1"></span>\
                                                                </div>\
                                                            </div>\
                                                        </div>\
                                                        <div class="dgon">\
                                                            <div class="dgon_number__portrait imageWrapper">\
                                                                <div class="dgon_big">\
                                                                    <span>2<input type="hidden" name="betnumber" value="2"></span>\
                                                                </div>\
                                                            </div>\
                                                        </div>\
                                                        <div class="dgon">\
                                                            <div class="dgon_number__portrait imageWrapper">\
                                                                <div class="dgon_big">\
                                                                    <span>3<input type="hidden" name="betnumber" value="3"></span>\
                                                                </div>\
                                                            </div>\
                                                        </div>\
                                                        <div class="dgon">\                                                      \
                                                            <div class="dgon_number__portrait imageWrapper">\
                                                                <div class="dgon_big">\
                                                                    <h7>4&nbsp;UP<input type="hidden" name="betnumber" value="4"></h7>\
                                                                </div>\
                                                            </div>\
                                                        </div>\
                                                        <div class="bet_dgon">\
        <div class="Container-gon">\
    <a id="betlimit" data-toggle="modal" data-target="" class="Btn Btn--primary Btn--purple Btn--shadow Btn--wide" href="#" ><span class="Btn-inner"><span class="Btn-innerContent">下注<img src="/Content/extra-images/fish_bone.png" style="width: 35%; margin: 0 0px 2px 10px;"></span></span></a>\
                                                            </div>\
                                                        </div>\
                                                    </div>\
         \
    <div>\
';

    htmlstr += '                         </div>\
                        <p></p>\
                    </div>\
                 </div>\
            </div>\
    ';


    return htmlstr;
}*/