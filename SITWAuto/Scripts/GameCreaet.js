var currentIndex = 0;
var stardtime;
var enddtime;
var timeindex;

var model = 0;
var topicC = 0;


function indexIterate(m) {
    if (m == 1) {
        $("input[name='game.topicList[0].comment']").val("預測正確的選項，愈早預測獎金越高");
        
    }
        


    var newHref = $("#addTopic").attr("href");
    currentIndex = $("div[name=topicDiv]").length;
    timeindex = currentIndex;
    var newerHref = newHref.replace(/(?:index=)[0-9]+/i, "index=" + currentIndex);
    
    $("#addTopic").attr("href", newerHref);
    $(".form_datetime").datetimepicker(
        {
            sideBySide: true,
            format: 'YYYY/M/D HH:mm'
        }
    );
    
    var bM = $("input[name$=betModel]:checked").val();
    $("#topname").hide();
    if (currentIndex == 1 && bM == 1) {
        $("#addChoice" + 0).attr("href", "/game/_choiceCreate?index=" + 0 + "&topicIndex=" + 0 + "&allnumber=" + 2);
        $("#addChoice" + 0).click();
        $("#gamepost_TeamASn,#gamepost_TeamBSn").change(function () { syncTeamChoice(); });
        



    } else if (currentIndex == 1 && bM == 2) {
        $("#addChoice" + 0).attr("href", "/game/_choiceCreate?index=" + 0 + "&topicIndex=" + 0 + "&allnumber=" + 2);
        $("#addChoice" + 0).click();
        $("#gamepost_TeamASn,#gamepost_TeamBSn").change(function () { syncTeamChoice(); });
        



    } else if (currentIndex == 1 && bM == 7) {
        $("#addChoice" + 0).attr("href", "/game/_choiceCreate?index=" + 0 + "&topicIndex=" + 0 + "&allnumber=" + 2);
        $("#addChoice" + 0).click();
        $("#gamepost_TeamASn,#gamepost_TeamBSn").change(function () { syncTeamChoice(); });
       

    } else if (bM==5) {
        $("#addChoice" + 0).attr("href", "/game/_choiceCreate?index=" + 0 + "&topicIndex=" + 0 + "&allnumber=" + 1);
        

        $("#addChoice" + 0).click();
        $("#game_topicList_0__title").val("勝隊擊殺小龍的屬性及數量及大龍數量(巴龍、遠古龍)");
        $("div[name=choiceListGroup]").hide();
        $("#addTopic").hide();
        $("#setting").css("display", "inline-block");       

    }
   // var model = $('#walkm').val();
    if (model == 1) {
        $("#addChoice" + 1).attr("href", "/game/_choiceCreate?index=" + 0 + "&topicIndex=" + 1 + "&allnumber=" + 2);
        $("#addChoice" + 1).click();
        syncTeamChoice(); 
        model = 0;
       // $('#walkm').val(0);

    }


    $("#game_topicList_" + (currentIndex - 1) + "__sdate").val($("#game_sdate").val());
    $("#game_topicList_" + (currentIndex - 1) + "__edate").val($("#game_edate").val());

    var cuindex = currentIndex - 1;

    choiceCreate(cuindex, 0);
    if (m == 1) {
        $("#addTopic").click();
        $("#walkm").val(1);
        model = 1;
        topicC = 1;
       // $("#walkm").val(1);
        //$("#addChoice" + 1).attr("href", "/game/_choiceCreate?index=" + 1 + "&topicIndex=" + 1 + "&allnumber=" + 2);
        //$("#addChoice" + 1).click();
    }

}

function indexbouns() {
    var newHref = $("#addBonus").attr("href");
    currentIndex = $("div[name=bonusDiv]").length;
    timeindex = currentIndex;
    var newerHref = newHref.replace(/(?:index=)[0-9]+/i, "index=" + currentIndex);
    newerHref = newerHref.replace(/(?:count=)[0-9]+/i, "count=" + 1);
    $("#addBonus").attr("href", newerHref);
    //href = "/game/_bonusCreate?index=0&count=7"
    syncBonusChoice();

}

function syncBonusChoice() {
    var ibonus = [0, 1, 2, 3, 4, 4, 5, 5];
    var bonus = [250, 150, 100, 60, 10, 20, 30, 40];
    for (var i = 0; i < 8; i++) {
        $("input[name='game.bonusList[" + i + "].Quantity']").val(ibonus[i]);
        $("input[name='game.bonusList[" + i + "].BonusRatio']").val(bonus[i]);
        if (i > 3) {
            $("input[name='game.bonusList[" + i + "].pool']").val(true);

        }
        if (i === 5 || i === 7) {
            $("input[name='game.bonusList[" + i + "].bonus']").attr("checked", true);
        }
    }
}


function syncChoice() {
    $("input[name='game.topicList[0].choiceList[0].choiceStr']").val(0);
    for (var i = 0; i < 6; i++) {
        $("input[name='game.topicList[0].choiceList[0].choiceString[" + i + "].choiceStr']").val(i);
        //$("input[name='game.topicList[0].choiceList[1].choiceStr']").val($("#gamepost_TeamBSn option:selected").text());
    }

}



function indexIterateChoice(i, walk) {
    var newHref = $("#addChoice" + i).attr("href");
    var choiceIndex = newHref.match('(?:index=)([0-9]+)')[1];
    choiceIndex = $("input[name^='game.topicList[" + i + "].choiceList'][name$='hashSn']").length - 1;
    var newerHref = newHref.replace(/(?:index=)[0-9]+/i, "index=" + ++choiceIndex);
    $("#addChoice" + i).attr("href", newerHref);

    var bM = $("input[name$=betModel]:checked").val();
    if (walk != 0)
        bM = "7";
    switch (bM) {
        case "1":
            $("div[id$=Odds-"+ i +"]").show();
            //$("#Odds-"+ i).show();
            $("#addTopic").show();
            $("#addWTopic").hide();
            break;
        case "2":
            $("div[id$=Odds-" + i + "]").hide();
            //$("#Odds-" + i).hide();
            $("#addTopic").show();
            $("#addWTopic").hide();
            break;
        case "7":
            $("div[id$=Odds-" + i + "]").hide();
            //$("#Odds-" + i).hide();
            $("#addTopic").show();
            $("#addWTopic").hide();
            break;
        case "5":
            $("div[id$=Odds-" + i + "]").hide();
            //$("#Odds-"+ i).hide();
            $("div[name=choiceListGroup]").hide();
            $("#addTopic").hide();
            $("#addWTopic").hide();
            break;

    }

    changeUnit();
    choiceCreate(i, choiceIndex);
    if (i == 0 && bM != 5) {
        syncTeamChoice();
        //$("div[name=topicDiv]").eq(i).find("[name=delChoice],[name=delTopic]").hide();
        // $("#addChoice" + i).hide();
    } else {
        //syncChoice();
    }
    if (topicC == 1) {
        $("input[name='game.topicList[1].choiceList[0].choiceStr']").val($("#gamepost_TeamASn option:selected").text());
        $("input[name='game.topicList[1].choiceList[1].choiceStr']").val($("#gamepost_TeamBSn option:selected").text());
    }
   
    for (var j = 0; j < 30; j++) {
        $('#game_topicList_' + i + '__choiceList_' + j + '__Odds').on('change', function () {
            if ($(this).val() >= 10) {
                myoddscheck($(this).val(), $(this));
            }

        });

        $('#game_topicList_' + i + '__choiceList_' + j + '__bearSn').val(String(i) + String(j));

    }

}

function myoddscheck(odds, topicname) {
    var oddscheck = prompt("賠率異常,是否繼續:", odds);
    if (oddscheck == null || oddscheck == "") {
        topicname.val(0);
    } else {
        topicname.val(oddscheck);
    }

}

function syncTeamChoice() {
    //var model = $('#walkm').val();
    $("input[name='game.topicList[0].choiceList[0].choiceStr']").val($("#gamepost_TeamASn option:selected").text());
    $("input[name='game.topicList[0].choiceList[1].choiceStr']").val($("#gamepost_TeamBSn option:selected").text());
   // alert(topicC);
    if (topicC == 1) {
        $("input[name='game.topicList[1].choiceList[0].choiceStr']").val($("#gamepost_TeamASn option:selected").text());
        $("input[name='game.topicList[1].choiceList[1].choiceStr']").val($("#gamepost_TeamBSn option:selected").text());
    }
}



function syncTeamTopic() {
    $("input[name='game.title']").val($("#gamepost_TeamASn option:selected").text() + " " + "Vs" + " " + $("#gamepost_TeamBSn option:selected").text());

}

function changeUnit() {
    $("input[name$=betUnitArray]:checked").each(function (i, e) {
        $("div[name=choiceUnitDiv][unit=" + $(e).val() + "]").show();
    });
    $("input[name$=betUnitArray]:not(:checked)").each(function (i, e) {
        $("div[name=choiceUnitDiv][unit=" + $(e).val() + "]").hide();
    });

}

function betModelChange() {
    var bM = $("input[name$=betModel]:checked").val();


    switch(bM) {
        case "1":
            //$("#rakegroup").hide();
            //$("div[name=Oddsgroup]").show();
            $("#addTopic").show();
            $("#addWTopic").show();
            break;
        case "5":
           // $("#rakegroup").hide();
           // $("div[name=Oddsgroup]").hide();
            $("div[name=choiceListGroup]").hide();
            $("#addTopic").hide();
            $("#addWTopic").hide();
            break;
        case "2":
            //$("#rakegroup").show();
           // $("div[name=Oddsgroup]").hide();
            $("#addTopic").show();
            $("#addWTopic").show();

            break;
        case "7":
           // $("#rakegroup").hide();
            //$("div[name=Oddsgroup]").hide();
            $("#addTopic").show();
            $("#addWTopic").show();

            break;

    }


}

function choiceCreate(i, index) {
    if ($("#allnumber-" + i).val() == 1) {
        $("#addChoice" + i).attr("href", "/game/_choiceCreate?index=" + index + "&topicIndex=" + i + "&allnumber=" + 1);
    }
    $("#allnumber-" + i).change(function () {
        var allch = $("#allnumber-" + i).val();
        if (allch == 0) {
            alert("error");
            $("#allnumber-" + i).val(1);
        }
        $("#addChoice" + i).attr("href", "/game/_choiceCreate?index=" + index + "&topicIndex=" + i + "&allnumber=" + allch);


    });


}

function timeEdit() {
    $("#game_sdate").datetimepicker().on('dp.change', function (e) {
        stardtime = $(this).val();
        $("div[name=topicDiv] ").find("input[name$='.sdate']").val(stardtime);


    });
    $("#game_edate").datetimepicker().on('dp.change', function (e) {
        enddtime = $(this).val();
        $("#game_gamedate").val(enddtime);
        $("div[name=topicDiv] ").find("input[name$='.edate']").val(enddtime);


    });

}


$(document).ready(function () {
    $("#rakegroup").hide();
    $("#topname").hide(); 
    $("#fixedsetting").hide();
    $("#addTopic").hide();
    $("#addWTopic").hide();

   /* $("#betModels label").on('click',function () {
       
    });*/
    $(".form_datetime").datetimepicker(
        {
            sideBySide: true,
            format: 'YYYY/M/D HH:mm'
        }
    );
    $(document).on('click', '#betModels', function () {
        // $("#addTopic").show();
       // betModelChange();
        betModelChange();
        $("#walkm").val(0);
        topicC = 0;
        model = 0;
        $("#topic-group").html('');
        $("#addTopic").attr("href", "/game/_topicCreate?index=0");
        var bM = $("input[name$=betModel]:checked").val();
        if (bM != 1 && bM != 2) {
            
            $("[data-valmsg-for='game.topicList']").html("");
            if (bM == 6) {
                $("#fixedsetting a").attr("href", "/Setting/nabobindex");
            } else if (bM == 5) {
                $("#fixedsetting a").attr("href", "/Setting/index");
            }
            $("#fixedsetting").show();
            $("#addTopic").hide();
            if (bM == 7) {
                $("#fixedsetting a").attr("href", "/Setting/pool");
                $("#fixedsetting").show();
                $("#addTopic").show();
            }
            //$("#addChick").remove();
        } else {
            $("#fixedsetting a").attr("href", "/Setting/pool");
            $("#fixedsetting").show();

            $("#addTopic").show();
            //$("#addChick").show();

        }
    });
           /* $("input:submit").click(function () {
                var boolAllTrue = true;
                var bM = $("input[name$=betModel]:checked").val();
                if (bM == 2 || bM == 1) {
                    //$("[data-valmsg-for]").html("");*/
                    

        $("input:submit").click(function () {
            var boolAllTrue = true;
            var bM = $("input[name$=betModel]:checked").val();
            if (bM == 2 || bM == 1) {
                //$("[data-valmsg-for]").html("");


                if ($("input[name^='game.topicList'][name$='title']").length == 0) {
                    $("[data-valmsg-for='game.topicList']").html("需新增題目");
                    return false;
                }
                if ($("input[name^='game.topicList'][name$='choiceStr']").length == 0) {
                    $("[data-valmsg-for='game.topicList']").html("需新增選項");
                    return false;
                }

            }
            return boolAllTrue;
        });


   



    var bM = $("input[name$=betModel]:checked").val();

    if (bM != 5)
        $("#gamepost_TeamASn,#gamepost_TeamBSn").change(function () { syncTeamChoice(); });



    /*$("#game_sdate").datetimepicker().on('dp.change', function (e) {
        stardtime = $(this).val();
        for (var i = 0; i < timeindex; i++) {
            if ($("#game_topicList_" + i + "__sdate").val()) {
                $("#game_topicList_" + i + "__sdate").val(stardtime);
            }
        }

    });

    $("#game_edate").datetimepicker().on('dp.change', function (e) {
        enddtime = $(this).val();
        $("#game_gamedate").val(enddtime);
        for (var i = 0; i < timeindex; i++) {
            if ($("#game_topicList_" + i + "__edate").val()) {
                $("#game_topicList_" + i + "__edate").val(enddtime);
            }
        }
    });*/
    timeEdit();





    $(document).on("click", "span[name=delTopic]", function () {
        if ($(this).parents("div[name=topicDiv]").find("#game_topicList_0__hashSn").attr("name") == "game.topicList[0].hashSn")
            $("#walkm").val(0);

        $(this).parents("div[name=topicDiv]").hide();
        $(this).parents("div[name=topicDiv]").find("input[name$='.valid']").attr("value", 0);

        $(this).parents("div[name=topicDiv]").find("input[name$='.title']").attr("value", "刪除題目");
        $(this).parents("div[name=topicDiv]").find("input[name$='.choiceStr']").attr("value", "刪除選項");
        $(this).parents("div[name=topicDiv]").find("input[name$='.Odds']").attr("value", 0);
       
        
        
    });

    $(document).on("click", "span[name=delChoice]", function () {
        $(this).parents("div[name=choiceDiv]").hide();
        $(this).parents("div[name=choiceDiv]").find("input[name$='.valid']").attr("value", 0);
        $(this).parents("div[name=choiceDiv]").find("input[name$='.choiceStr']").attr("value", "刪除選項");
        $(this).parents("div[name=choiceDiv]").find("input[name$='.Odds']").attr("value", 0);
    });


    changeUnit();
    betModelChange();
    $("#dota2dialog").dialog(

        { autoOpen: false }

    );
    $("#settingdialog").dialog(

        { autoOpen: false }

    );

    $("select[name$='.PlayGameSn']").change(function () {
        if ($(this).val() == 4) {
            $("#dota2dialog").dialog({ position: { my: 'right top', at: 'right top', of: window }, width: "40%", maxHeight: 800 });
            $("#dota2dialog").dialog("open");

        } else if ($(this).val() == 1) {
            $("#loldialog").dialog({ position: { my: 'right top', at: 'right top', of: window }, width: "40%", maxHeight: 800 });
            $("#loldialog").dialog("open");

        } else if ($(this).val() == 2) {
            $("#csgodialog").dialog({ position: { my: 'right top', at: 'right top', of: window }, width: "40%", maxHeight: 800 });
            $("#csgodialog").dialog("open");

        }

    });


    $(document).on("click", "tr #dota2data", function () {
        //alert($("input[name$ ='game.edate']']").val());
        $("#gamepost_TeamASn optgroup option").prop('selected', false);
        $("#gamepost_TeamBSn optgroup option").prop('selected', false);
        $("input[name$='TeamList[1].name']").val("");
        $("input[name$='TeamList[1].shortName").val("");
        $("input[name$='TeamList[1].imageURL").val("");
        $("input[name$='TeamList[0].name']").val("");
        $("input[name$='TeamList[0].shortName").val("");
        $("input[name$='TeamList[0].imageURL").val("");

        var dota2begin_at = $(this).find("input[name$='dota2begin_at']").val();
        var team1 = $(this).find("input[name$='team1']").val();
        var team2 = $(this).find("input[name$='team2']").val();
        var team1_shortName = $(this).find("input[name$='team1_shortName']").val();
        //var team2_shortName = $(this).find("input[name$='team2_shortName]").val();
        var team1_imageURL = $(this).find("input[name$='team1_imageURL']").val();
        var team2_imageURL = $(this).find("input[name$='team2_imageURL']").val();
        var team2_shortName = $(this).find("input[name$='team2_shortName']").val();

        var league = $(this).find("input[name$='league']").val();
        $("#game_edate,#game_gamedate").val(dota2begin_at);

        $("#game_title").val(team1 + " VS " + team2);
        $("#game_comment").val(league);
        //$("#game_edate").val(dota2begin_at);
        $("#gamepost_TeamASn optgroup option").filter(function () {
            //may want to use $.trim in here
            return $(this).text() == team1;
        }).prop('selected', true);
        $("#gamepost_TeamBSn optgroup option").filter(function () {
            //may want to use $.trim in here
            return $(this).text() == team2;
        }).prop('selected', true);

        
        if ($("#gamepost_TeamASn").val() == "") {

            var $optgroup = $("<optgroup label='未分類'>");
            var op = "<option value=''>" + team1 + "</option>";
            $optgroup.append(op);

            $("#gamepost_TeamASn").append($optgroup);
            $("#gamepost_TeamASn optgroup option").filter(function () {
                //may want to use $.trim in here
                return $(this).text() == team1;
            }).prop('selected', true);

            $("input[name$='TeamList[0].name']").val(team1);
            $("input[name$='TeamList[0].shortName").val(team1_shortName);
            $("input[name$='TeamList[0].imageURL").val(team1_imageURL);


        }
        if ($("#gamepost_TeamBSn").val() == "") {

            $optgroup = $("<optgroup label='未分類'>");
            op = "<option value=''>" + team2 + "</option>";
            $optgroup.append(op);

            $("#gamepost_TeamBSn").append($optgroup);
            $("#gamepost_TeamBSn optgroup option").filter(function () {
                //may want to use $.trim in here
                return $(this).text() == team2;
            }).prop('selected', true);

            $("input[name$='TeamList[1].name']").val(team2);
            $("input[name$='TeamList[1].shortName").val(team2_shortName);
            $("input[name$='TeamList[1].imageURL").val(team2_imageURL);


        } 

    });


});