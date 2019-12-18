////////////////////////////////////////////////////////////
// MAIN
////////////////////////////////////////////////////////////
var stageW=1280;
var stageH=768;
var contentW = 1024;
var contentH = 576;
var pnumber = [];


/*!
 * 
 * START BUILD GAME - This is the function that runs build game
 * 
 */
function initMain(){
	if(!$.browser.mobile || !isTablet){
        $('#canvasHolder').show();	
       
	}
	
    initGameCanvas(stageW, stageH);   
	buildGameCanvas();
	buildGameButton();
	initPhysics();
	
	readyGame();
	//goPage('main');


    
    goPage('game');
    getdata();
	
	resizeCanvas();
}

var windowW=windowH=0;
var scalePercent=0;
var offset = {x:0, y:0, left:0, top:0};

/*!
 * 
 * GAME RESIZE - This is the function that runs to resize and centralize the game
 * 
 */
function resizeGameFunc(){
	setTimeout(function() {
		$('.mobileRotate').css('left', checkContentWidth($('.mobileRotate')));
		$('.mobileRotate').css('top', checkContentHeight($('.mobileRotate')));
		
		windowW = window.innerWidth;
		windowH = window.innerHeight;
		
		
		
		scalePercent = windowW/contentW;
		if((contentH*scalePercent)>windowH){
			scalePercent = windowH/contentH;
		}
		
		scalePercent = scalePercent > 1 ? 1 : scalePercent;
		
		if(windowW > stageW && windowH > stageH){
			if(windowW > stageW){
				scalePercent = windowW/stageW;
				if((stageH*scalePercent)>windowH){
					scalePercent = windowH/stageH;
				}	
			}
		}
		
		var newCanvasW = ((stageW)*scalePercent);
		var newCanvasH = ((stageH)*scalePercent);
		
		offset.left = 0;
		offset.top = 0;
		
		if(newCanvasW > windowW){
			offset.left = -((newCanvasW) - windowW);
		}else{
			offset.left = windowW - (newCanvasW);
		}
		
		if(newCanvasH > windowH){
			offset.top = -((newCanvasH) - windowH);
		}else{
			offset.top = windowH - (newCanvasH);	
		}
		
		offset.x = 0;
		offset.y = 0;
		
		if(offset.left < 0){
			offset.x = Math.abs((offset.left/scalePercent)/2);
		}
		if(offset.top < 0){
			offset.y = Math.abs((offset.top/scalePercent)/2);
		}
		
		$('canvas').css('width', newCanvasW);
		$('canvas').css('height', newCanvasH);
		
		$('canvas').css('left', (offset.left/2));
		$('canvas').css('top', (offset.top/2));
		
		
		
		$(window).scrollTop(0);
		
		resizeCanvas();
	}, 100);	
}

function getdata() {
   


    var url = location.protocol+"//" + location.host + "/Html5/LottoGameData";
    $.ajax({
        type: "Get",
        url: url,
        dataType: "json",
        success: function (msg) {
            
            var d = new Date(msg.h5game.endTime.match(/\d+/)[0] * 1);
            //var d = new Date(dateObject);
            var day = d.getDate();
            var month = d.getMonth() + 1;
            var year = d.getFullYear();
            var curr_hour = d.getHours();
            var curr_min = d.getMinutes();
            var curr_sec = d.getSeconds();
            if (day < 10) {
                day = "0" + day;
            }
            if (month < 10) {
                month = "0" + month;
            }
            var date = year + "/" + month + "/" + day + " " + curr_hour + ":" + curr_min + ":" + curr_sec;
            time(date); 
            pnumber = [];
            msg.gamenumberRecords.forEach(function (e) {
                pnumber.push(e.number);

            });
            
            recordsetting(msg);


           


            
           

            
        }, error: function () {
            location.href = location.protocol+"//" + location.host + "/account/Login";
        }

    });


}

function toThousands(num) {
    var result = [], counter = 0;
    num = (num || 0).toString().split('');
    for (var i = num.length - 1; i >= 0; i--) {
        counter++;
        result.unshift(num[i]);
        if (!(counter % 3) && i != 0) { result.unshift(','); }
    }

    var t = result.join('');
    console.log(t.substr(0, 1));
    if (t.substr(1, 1) == ',') {
        t = '$' + t.substr(2, t.length - 1);

    }


    return t;

}



function recordsetting(msg) {
    //記錄
    $.record = {};
    var i = 0;
    var j = 1;

    var st = ['70% 總獎池', '20% 本期下注彩金', '30% 本期下注彩金', '50% 本期下注彩金'];
    msg.accumulation.forEach(function (e) {
        //score_arr.push(e);
        /*if (e != 0) {
            $.prize['score' + j].text = e + '魚骨幣';
        } else {
            $.prize['score' + j].text = '暫無';

        }*/
        $.prize['score' + j].text = st[j - 1];
        //console.log($.prize['score' + 4].text);
        j++;

    });

    payTitleTxt.text = '目前獎池： ' + toThousands(msg.accumulation[0]) + '魚骨幣';

    

    msg.Bets.forEach(function (e) {

        $.record['Betrecord' + i] = new createjs.Bitmap(loader.getResult('Betrecord'));
        centerReg($.record['Betrecord' + i]);
        $.record['Betrecord' + i].x = itemCard.x;
        $.record['Betrecord' + i].y = canvasH / 100 * 35 + 70 * i;
        $.record['Betrecord' + i].scaleY = 1.5;


        $.record['Number' + i] = new createjs.Text();
        $.record['Number' + i].font = "16px quantifybold";
        $.record['Number' + i].color = "#ddb867";
        $.record['Number' + i].textAlign = "center";
        $.record['Number' + i].textBaseline = 'alphabetic';
        $.record['Number' + i].text = '投注號碼： ' + e.bn;
        $.record['Number' + i].lineHeight = 40;
        $.record['Number' + i].x = $.record['Betrecord' + i].x - 100;
        $.record['Number' + i].y = $.record['Betrecord' + i].y - 10;

        var tNumber;


        if (e.tureNuber.length != 0) {
            tNumber = e.tureNuber;
        } else {
            tNumber = '暫無';

        }


        $.record['GNumber' + i] = new createjs.Text();
        $.record['GNumber' + i].font = "16px quantifybold";
        $.record['GNumber' + i].color = "#4CAF50";
        $.record['GNumber' + i].textAlign = "center";
        $.record['GNumber' + i].textBaseline = 'alphabetic';
        $.record['GNumber' + i].text = '頭獎號碼： ' + tNumber;
        $.record['GNumber' + i].lineHeight = 40;
        $.record['GNumber' + i].x = $.record['Betrecord' + i].x - 100;
        $.record['GNumber' + i].y = $.record['Betrecord' + i].y + 20;

        var d = new Date(e.gameBets.createDate.match(/\d+/)[0] * 1);
        //var d = new Date(dateObject);
        var day = d.getDate();
        var month = d.getMonth() + 1;
        var year = d.getFullYear();
        var curr_hour = d.getHours();
        var curr_min = d.getMinutes();
        var curr_sec = d.getSeconds();
        if (day < 10) {
            day = "0" + day;
        }
        if (month < 10) {
            month = "0" + month;
        }
        var date = year + "/" + month + "/" + day + " " + curr_hour + ":" + curr_min + ":" + curr_sec;

        $.record['Time' + i] = new createjs.Text();
        $.record['Time' + i].font = "16px quantifybold";
        $.record['Time' + i].color = "#ddb867";
        $.record['Time' + i].textAlign = "center";
        $.record['Time' + i].textBaseline = 'alphabetic';
        $.record['Time' + i].text = date;
        $.record['Time' + i].lineHeight = 40;
        $.record['Time' + i].x = $.record['Betrecord' + i].x + 100;
        $.record['Time' + i].y = $.record['Betrecord' + i].y + 30;

        var endText;
        if (e.gameBets.valid != 2) {
            endText = '未開獎';
        } else {
            endText = e.readMoney;
        }


        $.record['End' + i] = new createjs.Text();
        $.record['End' + i].font = "16px quantifybold";
        $.record['End' + i].color = "#ddb867";
        $.record['End' + i].textAlign = "center";
        $.record['End' + i].textBaseline = 'alphabetic';
        $.record['End' + i].text = '結果： ' + endText;
        $.record['End' + i].lineHeight = 40;
        $.record['End' + i].x = $.record['Betrecord' + i].x + 100;
        $.record['End' + i].y = $.record['Betrecord' + i].y;
        betRecordContainer.addChild($.record['Betrecord' + i], $.record['Number' + i], $.record['GNumber' + i], $.record['Time' + i], $.record['End' + i]);
        i++;


    });


}


function time(t) {
    $("#time").countdown(t).on('update.countdown', function (event) {
        $(this).html(
             event.strftime('剩 %M:%S 後開獎') 
        );
    }).on('finish.countdown', function (event) {
        var m = 0;
        $(this).html("派彩中");
       // $('.prompt span').html("正在派彩中");
        if ($(this).text() == "派彩中" && m == 0) {
            
            m = 1;
        }

        if (m == 1) {
            var url = location.protocol+"//" + location.host + "/Html5/LottoGameData";
            $.ajax({
                type: "Get",
                url: url,
                dataType: "json",
                success: function (msg) {

                    var d = new Date(msg.h5game.endTime.match(/\d+/)[0] * 1);
                    //var d = new Date(dateObject);
                    var day = d.getDate();
                    var month = d.getMonth() + 1;
                    var year = d.getFullYear();
                    var curr_hour = d.getHours();
                    var curr_min = d.getMinutes();
                    var curr_sec = d.getSeconds();
                    if (day < 10) {
                        day = "0" + day;
                    }
                    if (month < 10) {
                        month = "0" + month;
                    }
                    var date = year + "/" + month + "/" + day + " " + curr_hour + ":" + curr_min + ":" + curr_sec;
                     
                    var number = [];
                    pnumber = [];
                    msg.gamenumberRecords.forEach(function (e) {
                        number.push(e.number);
                        pnumber.push(e.number);


                    });
                    startSpin(number);

                    time(date); 
                    recordsetting(msg);
                    m = 0;



                }, error: function () {
                    console.log("登出");
                }

            });
        }


    });

}