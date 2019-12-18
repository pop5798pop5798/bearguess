﻿using Microsoft.AspNet.Identity;
using SITDto;
using SITDto.Request;
using SITDto.ViewModel;
using SITW.Filter;
using SITW.Helper;
using SITW.Models;
using SITW.Models.Repository;
using SITW.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SIT.Controllers
{
    [Authorize]
    [MissionFilter]
    public class betController : Controller
    {
        HttpClient client = new HttpClient();
        string encryptedKey;
        public betController()
        {
            //client = new HttpClient();
            client.BaseAddress = new Uri(System.Web.Configuration.WebConfigurationManager.AppSettings["apiurl"]);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            encryptedKey = System.Web.Configuration.WebConfigurationManager.AppSettings["encryptedKey"];
        }

        /*
        // GET: bet
        public async System.Threading.Tasks.Task<ActionResult> Index()
        {
            List<betDto> betList = null;
            HttpResponseMessage response = await client.GetAsync("/api/bets");
            if (response.IsSuccessStatusCode)
            {
                betList = await response.Content.ReadAsAsync<List<betDto>>();
            }
            return View(betList);
        }

        // GET: bet/Details/5
        public async System.Threading.Tasks.Task<ActionResult> Details(int id)
        {
            betDto bet = null;
            HttpResponseMessage response = await client.GetAsync("/api/bets/" + id);
            if (response.IsSuccessStatusCode)
            {
                bet = await response.Content.ReadAsAsync<betDto>();
            }
            return View(bet);
        }

        // GET: bet/Create
        public ActionResult Create()
        {
            return View();
        }
        */

        // POST: bet/Create
        [HttpPost]
        [AllowAnonymous]
        public async System.Threading.Tasks.Task<ActionResult> Create(betViewModel bv)
        {           
            aJaxDto ajd = new aJaxDto();
            try
            {
                if (User.Identity.GetUserId() == null)
                {
                    ajd.ErrorMsg = "下注前須先登入\n請先至會員登入中心進行登入註冊，謝謝";
                    ajd.isTrue = false;
                    throw new Exception("error");
                }

                if (!ModelState.IsValid)
                {
                    ajd.ErrorMsg = "";
                    foreach (var errs in ModelState.Values.Where(p => p.Errors.Count > 0).Select(p=>p.Errors).Distinct())
                    {
                        foreach(var e in errs)
                        {
                            ajd.ErrorMsg += e.ErrorMessage + "\n";
                        }
                        //ajd.ErrorMsg += "";
                    }

                    ajd.isTrue = false;
                    throw new Exception("error");
                }

                if(!User.Identity.GetEmailConfirmed())
                {
                    ajd.ErrorMsg = "下注前須先完成Email驗證\n請先至會員中心進行Email驗證，謝謝";
                    ajd.isTrue = false;
                    throw new Exception("error");
                }
                

                List<betDto> bet_list = new List<betDto>();
                GamesRepository _game = new GamesRepository();
                int firstChoiceSn = bv.betList.First().getChoiceSn(System.Web.Configuration.WebConfigurationManager.AppSettings["encryptedKey"]);
                gameDto gd = await _game.GetGameDetailByChoiceSn(firstChoiceSn);
                double Assets = new AssetsRepository().getAssetsByUserID(User.Identity.GetUserId(), 1);
                Session["Assets"] = new AssetsRepository().getAssetsListByUserID(User.Identity.GetUserId());
                if (gd.betModel == 6)
                {
                    int bj = 0;
                    double moneyclcik = 0;
                    for (var bi = 0;bi < (bv.betList.Count / 2); bi++)
                    {
                        
                        if(bv.betList[bj].money != null)
                        {
                            Regex regex = new Regex(@"^\+?[0-9]{1,10}[0][0]$|^$");
                            Match match = regex.Match(bv.betList[bj].money.ToString());
                            if (!match.Success)
                            {
                                ajd.ErrorMsg = "請勿竄改前端資料";
                                ajd.isTrue = false;
                                throw new Exception("error");
                            }
                            if (bj == 0)
                            {
                                moneyclcik = (double)bv.betList[bj].money;
                            }
                            else
                            {
                                if(bv.betList[bj].money != moneyclcik)
                                {
                                    ajd.ErrorMsg = "請勿竄改前端資料";
                                    ajd.isTrue = false;
                                    throw new Exception("error");
                                }
                            }
                            
                            
                            if(bv.betList[bj + 1].money != null)
                            {
                                ajd.ErrorMsg = "請勿竄改前端資料";
                                ajd.isTrue = false;
                                throw new Exception("error");
                            }
                        }
                        else
                        {
                            Regex regex = new Regex(@"^\+?[0-9]{1,10}[0][0]$|^$");
                            Match match = regex.Match(bv.betList[bj].money.ToString());
                            if (!match.Success)
                            {
                                ajd.ErrorMsg = "請勿竄改前端資料";
                                ajd.isTrue = false;
                                throw new Exception("error");
                            }
                            if (bj == 0)
                            {
                                moneyclcik = (double)bv.betList[bj + 1].money;
                            }
                            else
                            {
                                if (bv.betList[bj + 1].money != moneyclcik)
                                {
                                    ajd.ErrorMsg = "請勿竄改前端資料";
                                    ajd.isTrue = false;
                                    throw new Exception("error");
                                }
                            }
                            if (bv.betList[bj+ 1].money == null)
                            {
                                ajd.ErrorMsg = "請勿竄改前端資料";
                                ajd.isTrue = false;
                                throw new Exception("error");
                            }
                        }

                        bj += 2;
                    }
                    
                }


                foreach (var b in bv.betList)
                {
                    if (!b.money.HasValue || b.money <= 0)
                        continue;
                    //bv.encryptedKey = System.Web.Configuration.WebConfigurationManager.AppSettings["encryptedKey"];
                    betDto bet = new betDto(); ;
                    bet.userId = User.Identity.GetUserId();
                    bet.unitSn = 1;
                    bet.comSn = 1;
                    bet.money = b.money;
                    bet.choiceSn = b.getChoiceSn(System.Web.Configuration.WebConfigurationManager.AppSettings["encryptedKey"]);

                    if (bet.choiceSn.HasValue)
                    {
                        topicDto t = await _game.GetTopicByChoiceSn(bet.choiceSn.Value);
                        if (t != null)
                        {
                            bet.topicSn = t.sn;
                        }
                    }
                    bet.gameSn = gd.sn;

                    if (Assets < bet.money)
                    {
                        ajd.ErrorMsg = "剩餘彩金不足";
                        ajd.isTrue = false;
                        throw new Exception("error");
                    }
                    else
                    {
                        Assets -= bet.money.Value;
                    }

                    /*if (!gd.canbet)
                    {
                        ajd.ErrorMsg = "不可下注";
                        ajd.isTrue = false;
                        throw new Exception("error");
                    }*/

                    foreach (topicDto t in gd.topicList)
                    {
                        if (t.choiceList.Where(p => p.sn == bet.choiceSn).Count() > 0)
                        {
                            bool canbet = (t.walk == 1) ? true : t.canbet;
                            //t.canbet = (t.walk == 1) ? true : t.canbet;
                            if (!canbet)
                            {
                                ajd.ErrorMsg = "不可下注";
                                ajd.isTrue = false;
                                throw new Exception("error");
                            }
                        }
                    }
                    bet_list.Add(bet);
                }


                if (gd.betModel != 6)
                {
                    foreach (var bet in bet_list)
                    {
                        bool isTrue = await _game.CreateBet(bet);
                        if (isTrue)
                            ajd.isTrue = isTrue;
                        else
                        {
                            ajd.ErrorMsg = "系統錯誤，請重整後再重新下注";
                            throw new Exception("error");
                        }
                    }
                }
                else {
                    var betm = bet_list.Where(x => x.money != null).ToList();
                 
                    bool isTrue = await _game.NabobCreateBet(betm);
                    if (isTrue)
                        ajd.isTrue = isTrue;
                    else
                    {
                        ajd.ErrorMsg = "系統錯誤，請重整後再重新下注";
                        throw new Exception("error");
                    }

                }
                

                if (ajd.isTrue)
                {
                    Session["Assets"] = new AssetsRepository().getAssetsListByUserID(User.Identity.GetUserId());
                    new SignalRHelper().UpdateChoiceMoney(gd, encryptedKey, gd.md5GameSn);
                }
                else
                {
                    ajd.ErrorMsg = "魚骨幣發生問題，請重整後再重新下注";
                    throw new Exception("error");
                }

                return Json(ajd);
            }
            catch
            {
                return Json(ajd);
            }
        }

        // POST: bet/Create
        [HttpPost]
        [AllowAnonymous]
        public async System.Threading.Tasks.Task<ActionResult> LottoCreate(betViewModel bv)
        {
            aJaxDto ajd = new aJaxDto();
            try
            {
                if (!ModelState.IsValid)
                {
                    ajd.ErrorMsg = "";
                    foreach (var errs in ModelState.Values.Where(p => p.Errors.Count > 0).Select(p => p.Errors).Distinct())
                    {
                        foreach (var e in errs)
                        {
                            ajd.ErrorMsg += e.ErrorMessage + "\n";
                        }
                        //ajd.ErrorMsg += "";
                    }

                    ajd.isTrue = false;
                    throw new Exception("error");
                }

                if (User.Identity.GetUserId() == null)
                {
                    ajd.ErrorMsg = "下注前須先登入\n請先至會員登入中心進行登入註冊，謝謝";
                    ajd.isTrue = false;
                    throw new Exception("error");
                }

                if (!User.Identity.GetEmailConfirmed())
                {
                    ajd.ErrorMsg = "下注前須先完成Email驗證\n請先至會員中心進行Email驗證，謝謝";
                    ajd.isTrue = false;
                    throw new Exception("error");
                }

               



                int? bvcount = 0;
                foreach(var bvlt in bv.betList)
                {
                    if(bvlt.strsn != "5")
                    {
                        bvcount += bvlt.count;
                    }
                    
                }
                if(bvcount == 0)
                {
                    ajd.ErrorMsg = "請在注單選擇一個以上的選項";
                    ajd.isTrue = false;
                    throw new Exception("error");

                }

                double moneyclcik = 0;
                for (var bi = 0; bi < bv.betList.Count; bi++)
                {
                        if (bi == 0)
                        {
                            moneyclcik = (double)bv.betList[bi].money;
                        }
                        else
                        {
                            if (bv.betList[bi].money != moneyclcik)
                            {
                                ajd.ErrorMsg = "請勿竄改前端資料";
                                ajd.isTrue = false;
                                throw new Exception("error");
                            }
                        }

                        Regex regex = new Regex(@"^\+?[0-9]{1,10}[0][0]$|^$");
                        Match match = regex.Match(bv.betList[bi].money.ToString());
                        if (!match.Success)
                        {
                            ajd.ErrorMsg = "請勿竄改前端資料";
                            ajd.isTrue = false;
                            throw new Exception("error");
                        }

                }





                List<betDto> bet_list = new List<betDto>();
                GamesRepository _game = new GamesRepository();
                int firstChoiceSn = bv.betList.First().getChoiceSn(System.Web.Configuration.WebConfigurationManager.AppSettings["encryptedKey"]);
                gameDto gd = await _game.GetGameDetailByChoiceSn(firstChoiceSn);
                double Assets = new AssetsRepository().getAssetsByUserID(User.Identity.GetUserId(), 1);
                Session["Assets"] = new AssetsRepository().getAssetsListByUserID(User.Identity.GetUserId());
                
                //bv.encryptedKey = System.Web.Configuration.WebConfigurationManager.AppSettings["encryptedKey"];
                betDto bet = new betDto(); 
                bet.userId = User.Identity.GetUserId();
                bet.unitSn = 1;
                bet.comSn = 1;
                bet.money = bv.betList[0].money;
                bet.choiceSn = bv.betList[0].getChoiceSn(System.Web.Configuration.WebConfigurationManager.AppSettings["encryptedKey"]);
                if (bet.choiceSn.HasValue)
                {
                    topicDto t = await _game.GetTopicByChoiceSn(bet.choiceSn.Value);
                    if (t != null)
                    {
                        bet.topicSn = t.sn;
                    }
                }
                bet.gameSn = gd.sn;
                List<betCountDto> betlistmodel = new List<betCountDto>();
                foreach (var b in bv.betList)
                {
                    
                    betCountDto betcount = new betCountDto();
                    betcount.betSn = bet.choiceSn;
                    betcount.choiceStr = b.strsn;
                    betcount.unitSn = 1;
                    betcount.choiceCount = b.count;
                    betlistmodel.Add(betcount);
                }

                bet.betCount = betlistmodel;
                   



                if (Assets < bet.money)
                {
                    ajd.ErrorMsg = "剩餘彩金不足";
                    ajd.isTrue = false;
                    throw new Exception("error");
                }
                else
                {
                    Assets -= bet.money.Value;
                }

                if (!gd.canbet)
                {
                    ajd.ErrorMsg = "不可下注";
                    ajd.isTrue = false;
                    throw new Exception("error");
                }

                foreach (topicDto t in gd.topicList)
                {
                    if (t.choiceList.Where(p => p.sn == bet.choiceSn).Count() > 0)
                    {
                        if (!t.canbet)
                        {
                            ajd.ErrorMsg = "不可下注";
                            ajd.isTrue = false;
                            throw new Exception("error");
                        }
                    }
                }
                
               


                bet_list.Add(bet);
               /* foreach (var b in bv.betList)
                {
                    if (!b.money.HasValue || b.money <= 0)
                        continue;
                    //bv.encryptedKey = System.Web.Configuration.WebConfigurationManager.AppSettings["encryptedKey"];
                    betDto bet = new betDto(); ;
                    bet.userId = User.Identity.GetUserId();
                    bet.unitSn = 1;
                    bet.comSn = 1;
                    bet.money = b.money;
                    bet.choiceSn = b.getChoiceSn(System.Web.Configuration.WebConfigurationManager.AppSettings["encryptedKey"]);
                    if (bet.choiceSn.HasValue)
                    {
                        topicDto t = await _game.GetTopicByChoiceSn(bet.choiceSn.Value);
                        if (t != null)
                        {
                            bet.topicSn = t.sn;
                        }
                    }
                    bet.gameSn = gd.sn;

                    if (Assets < bet.money)
                    {
                        ajd.ErrorMsg = "剩餘彩金不足";
                        ajd.isTrue = false;
                        throw new Exception("error");
                    }
                    else
                    {
                        Assets -= bet.money.Value;
                    }

                    if (!gd.canbet)
                    {
                        ajd.ErrorMsg = "不可下注";
                        ajd.isTrue = false;
                        throw new Exception("error");
                    }

                    foreach (topicDto t in gd.topicList)
                    {
                        if (t.choiceList.Where(p => p.sn == bet.choiceSn).Count() > 0)
                        {
                            if (!t.canbet)
                            {
                                ajd.ErrorMsg = "不可下注";
                                ajd.isTrue = false;
                                throw new Exception("error");
                            }
                        }
                    }
                    bet_list.Add(bet);
                }*/

                foreach (var betlist in bet_list)
                {
                    bool isTrue = await _game.CreateBet(betlist);
                    if (isTrue)
                        ajd.isTrue = isTrue;
                    else
                    {
                        ajd.ErrorMsg = "系統錯誤，請重整後再重新下注";
                        throw new Exception("error");
                    }
                }

                if (ajd.isTrue)
                {
                    Session["Assets"] = new AssetsRepository().getAssetsListByUserID(User.Identity.GetUserId());
                    new SignalRHelper().UpdateChoiceMoney(gd, encryptedKey, gd.md5GameSn);
                }
                else
                {
                    ajd.ErrorMsg = "魚骨幣發生問題，請重整後再重新下注";
                    throw new Exception("error");
                }

                return Json(ajd);
            }
            catch
            {
                return Json(ajd);
            }
        }



        /*public async System.Threading.Tasks.Task<ActionResult> BetsByUserID()
        {
            string UserID = User.Identity.GetUserId();
            BetsByUserID bbui = new BetsByUserID();
            List<BetListDto> BetList = new List<BetListDto>();

            BetList = await new BetsRepository().BetsByUserID(UserID);

            bbui.BetList = BetList.OrderByDescending(p=>p.betDatetime).ToList();
            bbui.assets = new AssetsRepository().getAssetsByUserID(UserID, 1);

            return View(bbui);
        }*/

        public async System.Threading.Tasks.Task<ActionResult> BetsByUserIDList()
        {
            string UserID = User.Identity.GetUserId();
            BetsByUserID bbui = new BetsByUserID();
            List<BetListDto> BetList = new List<BetListDto>();

            BetList = await new BetsRepository().BetsByUserID(UserID);
            List<BetListDto> blist = new List<BetListDto>();
            blist = BetList.OrderByDescending(p => p.betDatetime).ToList();             
            bbui.BetList = blist.Where(x => x.betModel == 1 || x.betModel == 2 || x.betModel == 7).ToList();

            List<NabobBetListDto> NabobBetList = new List<NabobBetListDto>();
            NabobBetList = await new BetsRepository().NabobBetsByUserID(UserID);

            bbui.NabobBetList = NabobBetList;
            bbui.assets = new AssetsRepository().getAssetsByUserID(UserID, 1);

            ViewData["DgonList"] = blist.Where(x => x.betModel == 5).ToList();
           

            return View(bbui);
        }



        public async Task<ActionResult> _BetsByUserIDPhone()
        {
            string UserID = User.Identity.GetUserId();
            List<BetListDto> BetList = await new BetsRepository().BetsByUserID(UserID);
            GamePostsRepository _gpr = new GamePostsRepository();
            List<GamePosts> gpList = _gpr.getValidAll();
            double assets = new AssetsRepository().getAssetsByUserID(UserID, 1);
            foreach (var b in BetList)
            {
                if (b.isTrueValue == "true")
                {
                    if (b.betModel == 2)
                    {
                        b.showTitle = "題目總彩金 " + b.topicMoney + " × 下注金 " + b.money + " ÷ 正解總彩金 " + b.choiceMoney;
                        if (b.rake > 0)
                        {
                            b.showTitle += " × (100-" + b.rake + ")%";
                        }
                        b.showTitle += "=" + b.realmoney;
                    }
                    else if (b.betModel == 1)
                    {
                        b.showTitle = "下注金 " + b.money + " × 賠率 " + b.Odds + " = " + b.realmoney;
                    }
                }
            }
            BetsByUserID bbui = new BetsByUserID();
           
            /*foreach (var bl in BetList)
            {
                BetListDto blvm = new BetListDto();
                blvm.gamepostsn  = gpList.Where(p => p.GameSn == bl.gameSn).FirstOrDefault().sn;
                //bbui.gamepostsn = gpList.Where(p => p.GameSn == bl.gameSn).FirstOrDefault().sn;  
                BetList.Add(blvm);
            }*/



           // BetsByUserID bbui = new BetsByUserID();
            bbui.BetList = BetList.OrderByDescending(p => p.betDatetime).ToList();
            bbui.assets = new AssetsRepository().getAssetsByUserID(UserID, 1);

            return View(bbui);
        }


        public ActionResult _BetsByUserID(List<BetListDto> BetList)
        {
            string UserID = User.Identity.GetUserId();

            foreach(var b in BetList)
            {
                if (b.isTrueValue == "true")
                {
                    if (b.betModel == 2 && b.walk != 1)
                    {
                        b.showTitle = "題目總彩金 " + b.topicMoney + " × 下注金 " + b.money + " ÷ 正解總彩金 " + b.choiceMoney;
                        if (b.rake > 0)
                        {
                            b.showTitle += " × (100-" + b.rake + ")%";
                        }
                        b.showTitle += "=" + b.realmoney;
                    }
                    else if(b.betModel==1 && b.walk != 1)
                    {
                        b.showTitle= "下注金 " + b.money + " × 賠率 "+ b.Odds+" = " + b.realmoney;
                    }
                    else
                    {
                        b.showTitle = "先知獎金 " + b.totalmoney + " + "+" 中獎彩金 " + Math.Round((Decimal)(b.realmoney - b.totalmoney),2, MidpointRounding.AwayFromZero);
                        /*if (b.rake > 0)
                        {
                            b.showTitle += " × (100-" + b.rake + ")%";
                        }*/
                        b.showTitle += " = " + b.realmoney;
                    }
                }
            }


            BetsByUserID bbui = new BetsByUserID();
            bbui.BetList = BetList.OrderByDescending(p => p.betDatetime).ToList();
            bbui.assets = new AssetsRepository().getAssetsByUserID(UserID, 1);

            return View(bbui);
        }

        public ActionResult _DgonBetsByUserID(List<BetListDto> BetList)
        {
            string UserID = User.Identity.GetUserId();

            foreach (var b in BetList)
            {
                if (b.isTrueValue == "true")
                {
                    if (b.betModel == 2)
                    {
                        b.showTitle = "題目總彩金 " + b.topicMoney + " × 下注金 " + b.money + " ÷ 正解總彩金 " + b.choiceMoney;
                        if (b.rake > 0)
                        {
                            b.showTitle += " × (100-" + b.rake + ")%";
                        }
                        b.showTitle += "=" + b.realmoney;
                    }
                    else if (b.betModel == 1)
                    {
                        b.showTitle = "下注金 " + b.money + " × 賠率 " + b.Odds + " = " + b.realmoney;
                    }
                }
            }


            BetsByUserID bbui = new BetsByUserID();
            bbui.BetList = BetList.OrderByDescending(p => p.betDatetime).ToList();
            bbui.assets = new AssetsRepository().getAssetsByUserID(UserID, 1);

            return View(bbui);
        }

        public ActionResult _NabobBetsByUserID(List<NabobBetListDto> BetList)
        {
            string UserID = User.Identity.GetUserId();

           /* foreach (var b in BetList)
            {
                if (b.isTrueValue == "true")
                {
                    if (b.betModel == 2)
                    {
                        b.showTitle = "題目總彩金 " + b.topicMoney + " × 下注金 " + b.money + " ÷ 正解總彩金 " + b.choiceMoney;
                        if (b.rake > 0)
                        {
                            b.showTitle += " × (100-" + b.rake + ")%";
                        }
                        b.showTitle += "=" + b.realmoney;
                    }
                    else if (b.betModel == 1)
                    {
                        b.showTitle = "下注金 " + b.money + " × 賠率 " + b.Odds + " = " + b.realmoney;
                    }
                }
            }*/


            BetsByUserID bbui = new BetsByUserID();
            //bbui.BetList = BetList.OrderByDescending(p => p.betDatetime).ToList();
            bbui.NabobBetList = BetList;
            bbui.assets = new AssetsRepository().getAssetsByUserID(UserID, 1);

            return View(bbui);
        }


        [Authorize(Roles = "Admin")]
        public async System.Threading.Tasks.Task<ActionResult> BetsByGame(int id)
        {
            string UserID = User.Identity.GetUserId();
            BetsByGameReq bbgr = new BetsByGameReq
            {
                GameSn = id
                //,
                //UserID = UserID
            };
            List<BetListDto> BetList = new List<BetListDto>();

            HttpResponseMessage response = await client.PostAsJsonAsync("api/BetsByGame", bbgr);
            if (response.IsSuccessStatusCode)
            {
                BetList = await response.Content.ReadAsAsync<List<BetListDto>>();
            }


            return View(BetList);
        }


    }
}
