using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using SITW.Models;
using SITW.Models.Repository;
using SITW.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static SITW.Models.ViewModel.RandomModel;
using MoreLinq;
using SITDto;

namespace SITW.Controllers
{
    [Authorize]
    public class Html5Controller : Controller
    {

        public JsonResult LottoGameData()
        {
            LottoGameVewModel akm = new LottoGameVewModel();
            akm.h5game = new H5GameRepository().H5GetAll(2).Where(x => x.gameStatus == 1).FirstOrDefault();
            var h5 = new H5GameRepository().H5GetAll(2).Where(x => x.gameStatus != 1).LastOrDefault();
            var precord = new H5GameRepository().PlayerGetAll(akm.h5game.id).Sum(x=>x.money);

            akm.h5game.totallottery += 500000;
            akm.gamenumberRecords = new H5GameRepository().GetNumberAll(h5.id);

            var bet = new H5GameRepository().PlayerBetList(User.Identity.GetUserId()).Where(x=>x.gameModel == 2).TakeLast(5);
            precord = (double)precord * 25 / 100;
            double[] r = new double[] { (double)akm.h5game.totallottery + (double)precord, (double)precord, (double)precord , (double)precord };
            List<double> tm = new List<double>();
           for(int j = 0; j< 4;j++)
            {
                tm.Add(r[j]);
            }
            

            akm.accumulation = tm;



            List<Lottobets> bslist = new List<Lottobets>();
            foreach (var b in bet)
            {
                var pay = new H5GameRepository().GetPay(b.id);
                Lottobets bs = new Lottobets();
                bs.gameBets = b;
                bs.readMoney = (pay != null) ? pay.readlMoney : 0;
                var bn = new List<int>();
                
                foreach(var n in new H5GameRepository().NumberGetAll(b.id))
                {
                    bn.Add((int)(n.Number+1));
                    
                }
                bs.bn = bn;

                var truebn = new List<int>();
                foreach (var gn in new H5GameRepository().GetNumberAll((int)b.GameSn))
                {
                    truebn.Add((int)gn.number);
                }

                
                 bs.tureNuber = truebn;
                


                //bs.tureNuber = ;
                //bs.tureNuber = (bs.tureNuber != null) ? bs.tureNuber : 0;
                bslist.Add(bs);


            }

            akm.Bets = bslist.OrderByDescending(x => x.gameBets.id).ToList();


            return Json(akm, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        [AllowAnonymous]
        [HttpPost]
        public void RatScoretoMoney(float money)
        {
            //H5Bets h5b = new H5Bets();
            var game = new H5GameRepository().H5GetAll(3).Where(x => x.gameStatus == 1).FirstOrDefault();
            /*H5payouts h5p = new H5payouts
            {
                gameSn = h5.id,
                userId = User.Identity.GetUserId(),
                betSn = p.id,
                Odds = p.Odds,
                money = p.money,
                readlMoney = p.money * p.Odds * (100 - h5.rake) / 100,
                createDate = DateTime.Now,
                modiDate = DateTime.Now,
                rake = 0

            };
            this.Payouts(h5p);*/
            //玩家加錢和記錄
            AssetsRecord assr = new AssetsRecord
            {
                UserId = User.Identity.GetUserId(),
                unitSn = 1,
                gameSn = game.id,
                assets = (double)money,
                type = 1,


            };

            new AssetsRepository().Addh5gameByAssets(assr);
            //int b = 1;
            List<AssetsViewModel> avList = new List<AssetsViewModel>();
           // avList = new AssetsRepository().getAssetsListByUserID(User.Identity.GetUserId());

            //玩家扣錢記錄
              /*  AssetsRecord assr = new AssetsRecord
                {
                    UserId = User.Identity.GetUserId(),
                    unitSn = 1,
                    gameSn = h5b.gameBets.GameSn,
                    assets = (double)money,
                    type = -1,


                };

                new AssetsRepository().Addh5gameByAssets(assr);*/
           


            //return b;
            //return Json(akm, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpGet]
        public void RattoUserData()
        {

            var u = new H5GameRepository().Usercount(User.Identity.GetUserId()).Where(x => x.gameModel == 3).FirstOrDefault();


            new H5GameRepository().DaycountRemove(u);
                H5Bets h5b = new H5Bets();
                var game = new H5GameRepository().H5GetAll(3).Where(x => x.gameStatus == 1).FirstOrDefault();
                h5b.gameBets = new GameBets
                {
                    userId = User.Identity.GetUserId(),
                    GameSn = game.id,
                    unitSn = 1,
                    money = 200,
                    Odds = 0,
                    valid = 1,
                    gameModel = game.gameModel,
                    createDate = DateTime.Now,
                    modiDate = DateTime.Now

                };
                new H5GameRepository().GameBets(h5b);
                AssetsRecord assr = new AssetsRecord
                {
                    UserId = h5b.gameBets.userId,
                    unitSn = 1,
                    gameSn = h5b.gameBets.GameSn,
                    assets = -200,
                    type = -1,


                };

                new AssetsRepository().Addh5gameByAssets(assr);
           


        }





        [AllowAnonymous]
        [HttpGet]
        public int RattoCount()
        {

            int b = 1;
            if (User.Identity.GetUserId() != null)
            {
                var u = new H5GameRepository().Usercount(User.Identity.GetUserId()).Where(x=>x.gameModel == 3).FirstOrDefault();
                if (u == null && User.Identity.GetUserId() != null)
                {
                    DailyGameCount d = new DailyGameCount
                    {
                        userId = User.Identity.GetUserId(),
                        gameModel = 3,
                        count = 5

                    };
                    u = new H5GameRepository().DaycountCreate(d);

                }

                List<AssetsViewModel> avList = new List<AssetsViewModel>();
                avList = new AssetsRepository().getAssetsListByUserID(User.Identity.GetUserId());

                if (avList.Where(x => x.unitSn == 1).FirstOrDefault().Asset < 200)
                {
                    b = 5;
                }



                if (u.count == 0)
                {
                    b = 0;
                }


            }
            else {
                b = 4;
            }
           




            return b;
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult SlottoUserData(float money,int id,int win)
        {
            SlotVewModel slotvm = new SlotVewModel();
            var u = new H5GameRepository().Usercount(User.Identity.GetUserId()).Where(x => x.gameModel == 5).FirstOrDefault();

            H5Bets h5b = new H5Bets();
            var game = new H5GameRepository().H5GetAll(5).Where(x => x.gameStatus == 1).FirstOrDefault();
            h5b.gameBets = new GameBets
            {
                userId = User.Identity.GetUserId(),
                GameSn = game.id,
                unitSn = 1,
                money = money,
                Odds = 0,
                valid = 1,
                gameModel = game.gameModel,
                createDate = DateTime.Now,
                modiDate = DateTime.Now

            };
            
            new H5GameRepository().GameBets(h5b);
            AssetsRecord assr = new AssetsRecord
            {
                UserId = h5b.gameBets.userId,
                unitSn = 1,
                gameSn = h5b.gameBets.GameSn,
                assets = -(money - win),
                type = -1,

            };
            if((money - win) != 0)
            {
                new AssetsRepository().Addh5gameByAssets(assr);
            }
            

            slotvm.slotm = new H5GameRepository().GetSlotCash(id);

            slotvm.slotm.slot_cash += (money - win);

            slotvm.slotm.baseCash = (slotvm.slotm.slot_cash >= 11000) ? slotvm.slotm.slot_cash : 11000;

            var slom = new cfgSlotCash
            {
                id = id,
                slot_cash = slotvm.slotm.slot_cash,
                baseCash = slotvm.slotm.baseCash
            };

            new H5GameRepository().SlotCashCreate(slom);

            

            return Json(slotvm, JsonRequestBehavior.AllowGet);

        }





        [AllowAnonymous]
        [HttpGet]
        public JsonResult SlottoLogin()
        {
            int b = 1;
            SlotVewModel slotvm = new SlotVewModel();                           

            if (User.Identity.GetUserId() == null)
            {
                b = 4;
            }
            else {
                List<AssetsViewModel> avList = new List<AssetsViewModel>();
                avList = new AssetsRepository().getAssetsListByUserID(User.Identity.GetUserId());
                slotvm.usermoney = avList.Where(x => x.unitSn == 1).FirstOrDefault().Asset;


                var u = new H5GameRepository().Usercount(User.Identity.GetUserId()).Where(x => x.gameModel == 5).FirstOrDefault();
                if (u == null)
                {
                    DailyGameCount d = new DailyGameCount
                    {
                        userId = User.Identity.GetUserId(),
                        gameModel = 5,
                        count = 0
                    };
                    u = new H5GameRepository().DaycountCreate(d);
                    
                }
                int r = new Random().Next(1, 14);
                slotvm.slotm = new H5GameRepository().GetSlotCash(r);

            }
            slotvm.loginUser = b;

            return Json(slotvm, JsonRequestBehavior.AllowGet);
        }




        [AllowAnonymous]
        [HttpGet]
        public void BrickUserData()
        {

            var u = new H5GameRepository().Usercount(User.Identity.GetUserId()).Where(x => x.gameModel == 4).FirstOrDefault();


            new H5GameRepository().DaycountRemove(u);
            H5Bets h5b = new H5Bets();
            var game = new H5GameRepository().H5GetAll(4).Where(x => x.gameStatus == 1).FirstOrDefault();
            h5b.gameBets = new GameBets
            {
                userId = User.Identity.GetUserId(),
                GameSn = game.id,
                unitSn = 1,
                money = 200,
                Odds = 0,
                valid = 1,
                gameModel = game.gameModel,
                createDate = DateTime.Now,
                modiDate = DateTime.Now

            };
            new H5GameRepository().GameBets(h5b);
            AssetsRecord assr = new AssetsRecord
            {
                UserId = h5b.gameBets.userId,
                unitSn = 1,
                gameSn = h5b.gameBets.GameSn,
                assets = -200,
                type = -1,


            };

            new AssetsRepository().Addh5gameByAssets(assr);



        }

        [AllowAnonymous]
        [HttpGet]
        public int BricktoCount()
        {
            //1:沒問題 4:沒登入 5:沒錢 0:沒每日
            int b = 1;
            if (User.Identity.GetUserId() != null)
            {
                var u = new H5GameRepository().Usercount(User.Identity.GetUserId()).Where(x => x.gameModel == 4).FirstOrDefault();
                if (u == null && User.Identity.GetUserId() != null)
                {
                    DailyGameCount d = new DailyGameCount
                    {
                        userId = User.Identity.GetUserId(),
                        gameModel = 4,
                        count = 5

                    };
                    u = new H5GameRepository().DaycountCreate(d);

                }

                List<AssetsViewModel> avList = new List<AssetsViewModel>();
                avList = new AssetsRepository().getAssetsListByUserID(User.Identity.GetUserId());

                if (avList.Where(x => x.unitSn == 1).FirstOrDefault().Asset < 200)
                {
                    b = 5;
                }



                if (u.count == 0)
                {
                    b = 0;
                }


            }
            else
            {
                b = 4;
            }





            return b;
        }


        [AllowAnonymous]
        [HttpPost]
        public void BrickScoretoMoney(float money)
        {
            var game = new H5GameRepository().H5GetAll(4).Where(x => x.gameStatus == 1).FirstOrDefault();
            AssetsRecord assr = new AssetsRecord
            {
                UserId = User.Identity.GetUserId(),
                unitSn = 1,
                gameSn = game.id,
                assets = (double)money,
                type = 1,


            };

            new AssetsRepository().Addh5gameByAssets(assr);
            
        }



        [HttpPost]
        public int LottoBets(int[] Number)
        {
            List<AssetsViewModel> avList = new List<AssetsViewModel>();
            avList = new AssetsRepository().getAssetsListByUserID(User.Identity.GetUserId());
            int b = 1;
            //玩家扣錢記錄
            if (avList.Where(x => x.unitSn == 1).FirstOrDefault().Asset >= 10000)
            {
                H5LottoBets h5b = new H5LottoBets();
            var game = new H5GameRepository().H5GetAll(2).Where(x => x.gameStatus == 1).FirstOrDefault();
            h5b.gameBets = new GameBets
            {
                userId = User.Identity.GetUserId(),
                GameSn = game.id,
                unitSn = 1,
                money = 10000,
                Odds = 12,
                valid = 1,
                gameModel = game.gameModel,
                createDate = DateTime.Now,
                modiDate = DateTime.Now

            };
            List<PlayerNumber> pnvm = new List<PlayerNumber>();
            foreach (var n in Number)
            {
                var pn = new PlayerNumber
                {
                    Number = n
                };
                pnvm.Add(pn);


            }
            h5b.playnumber = pnvm;
            
            new H5GameRepository().LottoBets(h5b);
           
            
                AssetsRecord assr = new AssetsRecord
                {
                    UserId = h5b.gameBets.userId,
                    unitSn = 1,
                    gameSn = h5b.gameBets.GameSn,
                    assets = -10000,
                    type = -1,


                };

                new AssetsRepository().Addh5gameByAssets(assr);
            }
            else
            {
                b = 0;
            }


            return b;
            //return Json(akm, JsonRequestBehavior.AllowGet);
        }

        



        // GET: Html5
        public ActionResult AKGame()
        {
            List<AssetsViewModel> avList = new List<AssetsViewModel>();
            avList = new AssetsRepository().getAssetsListByUserID(User.Identity.GetUserId());
            Session["Assets"] = avList;           
            ViewBag.Assets = String.Format("{0:N1}", avList.Where(x => x.unitSn == 1).FirstOrDefault().Asset);
            ViewBag.a = avList.Where(x => x.unitSn == 1).FirstOrDefault().Asset;
            var game = new H5GameRepository().H5GetAll(1).Where(x => x.gameStatus == 1).FirstOrDefault();
            ViewBag.EndTime = string.Format("{0:yyyy/MM/dd HH:mm:ss}", game.endTime);
           
            var h5 = new H5GameRepository().H5GetAll(1).Where(x=>x.gameStatus != 1).LastOrDefault();
            //var akm = new List<AkGameVewModel>();
            AkGameVewModel ak = new AkGameVewModel();
            ak.gamenumberRecords = new H5GameRepository().GetNumberAll(h5.id);
            ViewBag.p = ak.Brand;
            /* foreach (var h in h5.Where(x=>x.gameStatus!= 1))
             {
                 AkGameVewModel ak = new AkGameVewModel();
                 ak.gamenumberRecords = new H5GameRepository().GetNumberAll(h.id);


                 akm.Add(ak);
             }

             ViewBag.p = akm.LastOrDefault().Brand;*/


            return View(ak);
        }

        public ActionResult AkAssets()
        {
            List<AssetsViewModel> avList = new List<AssetsViewModel>();
            avList = new AssetsRepository().getAssetsListByUserID(User.Identity.GetUserId());
            Session["Assets"] = avList;
            ViewBag.Assets = String.Format("{0:N1}", avList.Where(x => x.unitSn == 1).FirstOrDefault().Asset);
            ViewBag.a = avList.Where(x => x.unitSn == 1).FirstOrDefault().Asset;
            return View();
        }

        [HttpGet]
        public JsonResult AKGameData()
        {
            AkGameVewModel akm = new AkGameVewModel();
            akm.h5game = new H5GameRepository().H5GetAll(1).Where(x => x.gameStatus == 1).FirstOrDefault();
            var h5 = new H5GameRepository().H5GetAll(1).Where(x => x.gameStatus != 1).LastOrDefault();
            
             AkGameVewModel ak = new AkGameVewModel();
             ak.gamenumberRecords = new H5GameRepository().GetNumberAll(h5.id);

            akm.gamenumberRecords = ak.gamenumberRecords;


            return Json(akm, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public int AKBets(float money,int Number)
        {
            H5Bets h5b = new H5Bets();
            var game = new H5GameRepository().H5GetAll(1).Where(x => x.gameStatus == 1).FirstOrDefault();
            h5b.gameBets = new GameBets {
                userId = User.Identity.GetUserId(),
                GameSn = game.id,
                unitSn = 1,
                money = money,
                Odds = 12,
                valid = 1,
                gameModel = game.gameModel,
                createDate = DateTime.Now,
                modiDate = DateTime.Now

            };
            h5b.playnumber = new PlayerNumber {
                Number = Number
            };
            new H5GameRepository().AkBets(h5b);
            int b = 1;
            List<AssetsViewModel> avList = new List<AssetsViewModel>();
            avList = new AssetsRepository().getAssetsListByUserID(User.Identity.GetUserId());

            //玩家扣錢記錄
            if (avList.Where(x => x.unitSn == 1).FirstOrDefault().Asset > money)
            {
                AssetsRecord assr = new AssetsRecord
                {
                    UserId = h5b.gameBets.userId,
                    unitSn = 1,
                    gameSn = h5b.gameBets.GameSn,
                    assets = -(double)money,
                    type = -1,


                };

                new AssetsRepository().Addh5gameByAssets(assr);
            }
            else {
                b = 0;
            }
            

            return b;
            //return Json(akm, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AkBetRecord() {
            AkGameVewModel akm = new AkGameVewModel();
           
            try
            {
                
                akm.h5game = new H5GameRepository().H5GetAll(1).Where(x => x.gameStatus == 1).FirstOrDefault();
                //var h5 = new H5GameRepository().H5GetAll(1).Where(x => x.gameStatus != 1).TakeLast(5);
                var bet = new H5GameRepository().PlayerBetList(User.Identity.GetUserId()).Where(x => x.gameModel == 1).TakeLast(5);


                            

            

                List<bets> bslist = new List<bets>();
                foreach (var b in bet)
                {
                    var pay = new H5GameRepository().GetPay(b.id);
                    bets bs = new bets();
                    bs.gameBets = b;
                    bs.readMoney = (pay != null) ? pay.readlMoney : 0;
                    bs.bn = new H5GameRepository().NumberGetAll(b.id).FirstOrDefault().Number;

                    bs.tureNuber = (new H5GameRepository().GetNumberAll((int)b.GameSn).Count() != 0) ? new H5GameRepository().GetNumberAll((int)b.GameSn).FirstOrDefault().number : 0;
                    //bs.tureNuber = (bs.tureNuber != null) ? bs.tureNuber : 0;
                    bslist.Add(bs);


                }

                akm.Bets = bslist.OrderByDescending(x=>x.gameBets.id).ToList();
                return View(akm);
            }
            catch
            {
                return View(akm);
            }
        }

        // GET: Html5/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Html5/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Html5/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Html5/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Html5/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Html5/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Html5/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
