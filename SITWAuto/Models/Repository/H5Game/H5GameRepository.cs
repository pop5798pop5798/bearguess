using Dapper;
using Newtonsoft.Json;
using SITW.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using static SITW.Models.ViewModel.RandomModel;

namespace SITW.Models.Repository
{
    public class H5GameRepository
    {
        private sitwEntities Db = new sitwEntities();

        public List<Product> getAll()
        {
            return Db.Product.Where(x=>x.valid == 1).ToList();
        }
 

       //中獎號碼記錄
        public void GNCreate(GameNumberRecord instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                Db.GameNumberRecord.Add(instance);                 
                this.SaveChanges();
               

            }
        }

        //A-K下注
        public void AkBets(H5Bets instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                Db.GameBets.Add(instance.gameBets);
                this.SaveChanges();
                instance.playnumber.BetId = instance.gameBets.id;
                Db.PlayerNumber.Add(instance.playnumber);
                this.SaveChanges();

            }
        }

        //一般下注
        public void GameBets(H5Bets instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                Db.GameBets.Add(instance.gameBets);
                this.SaveChanges();

            }
        }

        //樂透下注
        public void LottoBets(H5LottoBets instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                Db.GameBets.Add(instance.gameBets);
                this.SaveChanges();
                foreach(var n in instance.playnumber)
                {
                    n.BetId = instance.gameBets.id;
                    Db.PlayerNumber.Add(n);
                    this.SaveChanges();
                }
                
                

            }
        }


        //全部遊戲開局
        public void GameCreate(H5Games instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                Db.H5Games.Add(instance);
                this.SaveChanges();                                        

            }
        }

        //全部中獎下注號碼
        public List<GameNumberRecord> GetNumberAll(int gamesn)
        {
            return Db.GameNumberRecord.Where(x=>x.gameSn == gamesn).ToList();
        }

        //抓取機台
        public cfgSlotCash GetSlotCash(int id)
        {
            return Db.cfgSlotCash.Where(x => x.id == id).FirstOrDefault();
        }

        //存入機台
        public void SlotCashCreate(cfgSlotCash instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                Db.Entry(instance).State = EntityState.Modified;
                this.SaveChanges();
            }
        }

        //此遊戲全部玩家下注
        public List<GameBets> PlayerGetAll(int gameSn)
        {
            return Db.GameBets.Where(x => x.GameSn == gameSn).ToList();
        }
        //玩家下注清單
        public List<GameBets> PlayerBetList(string user)
        {
          
            return Db.GameBets.Where(x => x.userId == user).ToList();
        }
        //玩家下注號碼
        public List<PlayerNumber> NumberGetAll(int betId)
        {
            return Db.PlayerNumber.Where(x => x.BetId == betId).ToList();
        }
        //玩家派彩資料
        public H5payouts GetPay(int betId)
        {
            return Db.H5payouts.Where(x => x.betSn == betId).FirstOrDefault();
        }


        //GET h5全部賽局
        public List<H5Games> H5GetAll(int m)
        {
            return Db.H5Games.Where(x=>x.gameModel == m).ToList();
        }

        //派彩記錄加入
        public void Payouts(H5payouts instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                Db.H5payouts.Add(instance);
                this.SaveChanges();

            }
        }

        //自動更新每日
        public void AutoDate()
        {

                //更新每日次數
                foreach(var user in this.GetUsercount())
                {
                    user.count = 5;
                    if(user.gameModel == 4)
                        user.count = 1;
                    //更新記錄
                    this.DailyUpdate(user);
                }
                




        }

        //取得全部玩家遊戲次數
        public List<DailyGameCount> GetUsercount()
        {

            return Db.DailyGameCount.ToList();

        }

        //每日更新Get
        public List<DailyGameCount> Usercount(string user)
        {

            return Db.DailyGameCount.Where(x=>x.userId == user).ToList();

        }

        //每日進扣取
        public void DaycountRemove(DailyGameCount instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                instance.count -= 1;
                Db.Entry(instance).State = EntityState.Modified;
                this.SaveChanges();
            }

        }

        //無每日新增新進資料
        public DailyGameCount DaycountCreate(DailyGameCount instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                Db.DailyGameCount.Add(instance);
                this.SaveChanges();
                return instance;

            }
        }


        //ak自動派彩
        public void akAutoPay(H5Games h5)
        {
            try {
                if (h5 == null)
                {
                    throw new Exception("error");
                }
                //亂數產生
                int[] randomArray = new int[5];
                Random rnd = new Random();  //產生亂數初始值
                for (int i = 0; i < 5; i++)
                {
                    randomArray[i] = rnd.Next(1, 14);   //亂數產生，亂數產生的範圍是1~9

                    for (int j = 0; j < i; j++)
                    {
                        while (randomArray[j] == randomArray[i])    //檢查是否與前面產生的數值發生重複，如果有就重新產生
                        {
                            j = 0;  //如有重複，將變數j設為0，再次檢查 (因為還是有重複的可能)
                            randomArray[i] = rnd.Next(1, 14);   //重新產生，存回陣列，亂數產生的範圍是1~9
                        }
                    }
                }


                //1:A-K 2:樂透
                GameNumberRecord gnr = new GameNumberRecord
                {
                    gameSn = h5.id,
                    number = randomArray[0],
                    inpdate = DateTime.Now,
                };
                //寫入牌記錄
                this.GNCreate(gnr);
                //派彩
                var player = this.PlayerGetAll(h5.id);
                foreach (var p in player)
                {
                    var number = this.NumberGetAll(p.id);
                    foreach (var n in number)
                    {
                        //確認正解
                        p.valid = 2;
                        this.betsUpdate(p);

                        if (gnr.number == n.Number)
                        {
                            //派彩記錄
                            H5payouts h5p = new H5payouts
                            {
                                gameSn = h5.id,
                                userId = p.userId,
                                betSn = p.id,
                                Odds = p.Odds,
                                money = p.money,
                                readlMoney = p.money * p.Odds * (100 - h5.rake) / 100,
                                createDate = DateTime.Now,
                                modiDate = DateTime.Now,
                                rake = h5.rake

                            };
                            this.Payouts(h5p);
                            //玩家加錢和記錄
                            AssetsRecord assr = new AssetsRecord
                            {
                                UserId = h5p.userId,
                                unitSn = 1,
                                gameSn = h5.id,
                                assets = (double)h5p.readlMoney,
                                type = 15,
                                h5forValue = h5.gameModel


                            };

                            new AssetsRepository().Addh5gameByAssets(assr);


                        }
                    }
                }

                
                //開盤資料更新
                h5.gameStatus = 0;
                h5.payDate = DateTime.Now;
                this.H5GameUpdate(h5);
                var ak = this.H5GetAll(1).Where(x => x.gameStatus == 1).ToList();

                if (ak.Count != 1)
                {
                    H5Games h5g = new H5Games
                    {
                        title = "A-K選牌",
                        gameModel = 1,
                        gameStatus = 1,
                        valid = 1,
                        rake = 5,
                        createDate = DateTime.Now,
                        endTime = DateTime.Now.AddMinutes(10)
                    };
                    new H5GameRepository().GameCreate(h5g);

                }
                else
                {
                    //開盤資料更新
                    h5.gameStatus = 0;
                    h5.payDate = DateTime.Now;
                    this.H5GameUpdate(h5);

                    H5Games h5g = new H5Games
                    {
                        title = "A-K選牌",
                        gameModel = 1,
                        gameStatus = 1,
                        valid = 1,
                        rake = 5,
                        createDate = DateTime.Now,
                        endTime = DateTime.Now.AddMinutes(10)
                    };
                    new H5GameRepository().GameCreate(h5g);

                }
                
        }catch
            {
                var ak = this.H5GetAll(1).Where(x => x.gameStatus == 1).ToList();
                if (ak.Count == 1)
                    akAutoPay(h5);
            }
            




            /* try
             {
                 string url = "https://api.random.org/json-rpc/2/invoke";
                 Random r = new Random();
                 int id = r.Next(0, 100);


                 HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                 request.ContentType = "application/json";
                 request.Method = "POST";
                 using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                 {
                     string json = "{\"jsonrpc\":\"2.0\"," +
                         "\"method\":\"generateIntegers\"," +
                         "\"params\":{" +
                         "\"apiKey\":\"3d80c333-78f2-4ed1-b4e1-80e07eb9f041\"," +
                         "\"n\":1," +
                         "\"min\":1," +
                         "\"max\":13," +
                         "\"replacement\":true}," +
                         "\"id\":" + id + "}"

                         ;

                     streamWriter.Write(json);
                     streamWriter.Flush();
                     streamWriter.Close();
                 }
                 var httpResponse = (HttpWebResponse)request.GetResponse();

                 using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                 {
                     var result = streamReader.ReadToEnd();
                     RandomObject ro = JsonConvert.DeserializeObject<RandomObject>(result);

                     //1:A-K 2:樂透
                     GameNumberRecord gnr = new GameNumberRecord
                     {
                         gameSn = h5.id,
                         number = ro.result.random.data[0],
                         inpdate = DateTime.Now,
                     };
                     //寫入牌記錄
                     this.GNCreate(gnr);
                     //派彩
                     var player = this.PlayerGetAll(h5.id);
                     foreach (var p in player)
                     {
                         var number = this.NumberGetAll(p.id);
                         foreach (var n in number)
                         {
                             //確認正解
                             p.valid = 2;
                             this.betsUpdate(p);

                             if (gnr.number == n.Number)
                             {
                                 //派彩記錄
                                 H5payouts h5p = new H5payouts
                                 {
                                     gameSn = h5.id,
                                     userId = p.userId,
                                     betSn = p.id,
                                     Odds = p.Odds,
                                     money = p.money,
                                     readlMoney = p.money * p.Odds * (100 - h5.rake) / 100,
                                     createDate = DateTime.Now,
                                     modiDate = DateTime.Now,
                                     rake = h5.rake

                                 };
                                 this.Payouts(h5p);
                                 //玩家加錢和記錄
                                 AssetsRecord assr = new AssetsRecord
                                 {
                                     UserId = h5p.userId,
                                     unitSn = 1,
                                     gameSn = h5.id,
                                     assets = (double)h5p.readlMoney,
                                     type = 15,
                                     h5forValue = h5.gameModel


                                 };

                                 new AssetsRepository().Addh5gameByAssets(assr);


                             }
                         }
                     }


                     //開盤資料更新
                     h5.gameStatus = 0;
                     h5.payDate = DateTime.Now;
                     this.H5GameUpdate(h5);

                     H5Games h5g = new H5Games
                     {
                         title = "A-K選牌",
                         gameModel = 1,
                         gameStatus = 1,
                         valid = 1,
                         rake = 5,
                         createDate = DateTime.Now,
                         endTime = DateTime.Now.AddMinutes(10)
                     };
                     new H5GameRepository().GameCreate(h5g);
                 }

             }
             catch {
                 var ak = this.H5GetAll(1).Where(x => x.gameStatus == 1).ToList();
                 if(ak.Count == 1)
                     akAutoPay(h5);
             }*/


        }


        //ak無派彩自動派彩
        public void WaitakAutoPay(H5Games h5)
        {
            try
            {
                if(h5 == null)
                {
                    throw new Exception("error");
                }


                //亂數產生
                int[] randomArray = new int[5];
                Random rnd = new Random();  //產生亂數初始值
                for (int i = 0; i < 5; i++)
                {
                    randomArray[i] = rnd.Next(1, 14);   //亂數產生，亂數產生的範圍是1~9

                    for (int j = 0; j < i; j++)
                    {
                        while (randomArray[j] == randomArray[i])    //檢查是否與前面產生的數值發生重複，如果有就重新產生
                        {
                            j = 0;  //如有重複，將變數j設為0，再次檢查 (因為還是有重複的可能)
                            randomArray[i] = rnd.Next(1, 14);   //重新產生，存回陣列，亂數產生的範圍是1~9
                        }
                    }
                }


                //1:A-K 2:樂透
                GameNumberRecord gnr = new GameNumberRecord
                {
                    gameSn = h5.id,
                    number = randomArray[0],
                    inpdate = DateTime.Now,
                };
                //寫入牌記錄
                this.GNCreate(gnr);
                //派彩
                var player = this.PlayerGetAll(h5.id);
                foreach (var p in player)
                {
                    var number = this.NumberGetAll(p.id);
                    foreach (var n in number)
                    {
                        //確認正解
                        p.valid = 2;
                        this.betsUpdate(p);

                        if (gnr.number == n.Number)
                        {
                            //派彩記錄
                            H5payouts h5p = new H5payouts
                            {
                                gameSn = h5.id,
                                userId = p.userId,
                                betSn = p.id,
                                Odds = p.Odds,
                                money = p.money,
                                readlMoney = p.money * p.Odds * (100 - h5.rake) / 100,
                                createDate = DateTime.Now,
                                modiDate = DateTime.Now,
                                rake = h5.rake

                            };
                            this.Payouts(h5p);
                            //玩家加錢和記錄
                            AssetsRecord assr = new AssetsRecord
                            {
                                UserId = h5p.userId,
                                unitSn = 1,
                                gameSn = h5.id,
                                assets = (double)h5p.readlMoney,
                                type = 15,
                                h5forValue = h5.gameModel


                            };

                            new AssetsRepository().Addh5gameByAssets(assr);


                        }
                    }
                }


                //開盤資料更新
                h5.gameStatus = 0;
                h5.payDate = DateTime.Now;
                this.H5GameUpdate(h5);
                var ak = this.H5GetAll(1).Where(x => x.gameStatus == 1).ToList();
                if(ak.Count == 0)
                {
                    double m = DateTime.Now.Minute;
                    DateTime dtn = DateTime.Now;
                    m += 10;
                    string md = "";
                    md = m.ToString().Substring(0,1);
                    md = md + "0";
                    string dt = dtn.Year +"-"+ dtn.Month +  "-" + dtn.Day + " "+ dtn.Hour +":"+ md + ":" + dtn.Second;


                    H5Games h5g = new H5Games
                    {
                        title = "A-K選牌",
                        gameModel = 1,
                        gameStatus = 1,
                        valid = 1,
                        rake = 5,
                        createDate = DateTime.Now,
                        endTime = Convert.ToDateTime(dt)
                    };
                    
                    new H5GameRepository().GameCreate(h5g);


                }
                


            }
            catch
            {

            }





          


        }


        //樂透自動派彩
        public double ballAutoPay(H5Games h5)
        {
           
            double b = 0;
            /*try
            {
                string url = "https://api.random.org/json-rpc/2/invoke";
                Random r = new Random();
                int id = r.Next(0, 100);


                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/json";
                request.Method = "POST";
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    string json = "{\"jsonrpc\":\"2.0\"," +
                        "\"method\":\"generateIntegers\"," +
                        "\"params\":{" +
                        "\"apiKey\":\"3d80c333-78f2-4ed1-b4e1-80e07eb9f041\"," +
                        "\"n\":5," +
                        "\"min\":0," +
                        "\"max\":35," +
                        "\"replacement\":false}," +
                        "\"id\":" + id + "}"

                        ;

                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                var httpResponse = (HttpWebResponse)request.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    RandomObject ro = JsonConvert.DeserializeObject<RandomObject>(result);

                    //1:A-K 2:樂透
                    foreach (var rd in ro.result.random.data)
                    {
                        GameNumberRecord gnr = new GameNumberRecord
                        {
                            gameSn = h5.id,
                            number = rd,
                            inpdate = DateTime.Now,
                        };
                        //寫入牌記錄
                        this.GNCreate(gnr);

                    }

                    //派彩
                    var player = this.PlayerGetAll(h5.id);
                    int[] ary = new int[] { 0, 0, 0, 0 };
                    var ucm = new List<BallGameModel>();
                    //確認多少中獎
                    foreach (var p in player)
                    {
                        var number = this.NumberGetAll(p.id);
                        int count = 0;
                        var uc = new BallGameModel();

                        foreach (var n in number)
                        {

                            foreach (var rd in ro.result.random.data)
                            {
                                if (rd == n.Number)
                                {
                                    count += 1;
                                }

                            }

                        }
                        if (count >= 2)
                        {
                            ary[count - 2] += 1;
                            uc.gamebets = p;
                            uc.count = count;
                            ucm.Add(uc);
                        }
                        //確認正解
                        p.valid = 2;
                        this.betsUpdate(p);




                    }
                    double total = (h5.totallottery != null) ? (double)h5.totallottery : 0;
                    double ball = 500000 + total;
                    double totalbets = this.PlayerGetAll(h5.id).Sum(x => (double)x.money);

                    double deduction = 0;

                    foreach (var c in ucm)
                    {
                        double? rm = (c.count != 5) ? (totalbets * 25 / 100) / ary[c.count - 2] : ball + (totalbets * 25 / 100) / ary[c.count - 2];


                        if (c.count == 5)
                        {
                            h5.bingo = 1;
                            deduction += total + (totalbets * 25 / 100) / ary[c.count - 2];
                        }
                        else
                        {
                            deduction += (double)rm;
                        }


                        //派彩記錄
                        H5payouts h5p = new H5payouts
                        {
                            gameSn = h5.id,
                            userId = c.gamebets.userId,
                            betSn = c.gamebets.id,
                            Odds = c.gamebets.Odds,
                            money = c.gamebets.money,
                            readlMoney = rm * (100 - h5.rake) / 100,
                            createDate = DateTime.Now,
                            modiDate = DateTime.Now,
                            rake = h5.rake

                        };

                        this.Payouts(h5p);
                        //玩家加錢和記錄
                        AssetsRecord assr = new AssetsRecord
                        {
                            UserId = h5p.userId,
                            unitSn = 1,
                            gameSn = h5.id,
                            assets = (double)h5p.readlMoney,
                            type = 15,
                            h5forValue = h5.gameModel


                        };

                        new AssetsRepository().Addh5gameByAssets(assr);

                    }






                    //開盤資料更新
                    h5.gameStatus = 0;
                    h5.payDate = DateTime.Now;
                    this.H5GameUpdate(h5);


                    b = totalbets + total - deduction;
                    H5Games h5g = new H5Games
                    {
                        title = "樂透",
                        gameModel = 2,
                        gameStatus = 1,
                        valid = 1,
                        rake = 20,
                        createDate = DateTime.Now,
                        totallottery = b,
                        endTime = DateTime.Now.AddMinutes(30)
                    };
                    //h5g.totallottery = 500;
                    new H5GameRepository().GameCreate(h5g);
                    //new H5GameRepository().akAutoPay(ak.LastOrDefault());



                }

            }
            catch {
                var ak = this.H5GetAll(2).Where(x => x.gameStatus == 1).ToList();
                if (ak.Count == 1)
                    b = ballAutoPay(h5);

            }*/
            try
            {
                if (h5 == null)
                {
                    throw new Exception("error");
                }
                //亂數產生
                int[] randomArray = new int[5];
                Random rnd = new Random();  //產生亂數初始值
                for (int i = 0; i < 5; i++)
                {
                    randomArray[i] = rnd.Next(1, 37);   //亂數產生，亂數產生的範圍是1~9

                    for (int j = 0; j < i; j++)
                    {
                        while (randomArray[j] == randomArray[i])    //檢查是否與前面產生的數值發生重複，如果有就重新產生
                        {
                            j = 0;  //如有重複，將變數j設為0，再次檢查 (因為還是有重複的可能)
                            randomArray[i] = rnd.Next(1, 37);   //重新產生，存回陣列，亂數產生的範圍是1~9
                        }
                    }
                }



                //1:A-K 2:樂透
                foreach (var rd in randomArray)
                {
                    GameNumberRecord gnr = new GameNumberRecord
                    {
                        gameSn = h5.id,
                        number = rd,
                        inpdate = DateTime.Now,
                    };
                    //寫入牌記錄
                    this.GNCreate(gnr);

                }

                //派彩
                var player = this.PlayerGetAll(h5.id);
                int[] ary = new int[] { 0, 0, 0, 0 };
                int[] bonus = new int[] { 50, 30, 20, 70 };
                var ucm = new List<BallGameModel>();
                //確認多少中獎
                foreach (var p in player)
                {
                    var number = this.NumberGetAll(p.id);
                    int count = 0;
                    var uc = new BallGameModel();

                    foreach (var n in number)
                    {

                        foreach (var rd in randomArray)
                        {
                            if (rd == n.Number)
                            {
                                count += 1;
                            }

                        }

                    }
                    if (count >= 2)
                    {
                        ary[count - 2] += 1;
                        uc.gamebets = p;
                        uc.count = count;
                        ucm.Add(uc);
                    }
                    //確認正解
                    p.valid = 2;
                    this.betsUpdate(p);




                }
                double total = (h5.totallottery != null) ? (double)h5.totallottery : 0;
                double ball = 18326000 + total;
                double totalbets = this.PlayerGetAll(h5.id).Sum(x => (double)x.money);

                double deduction = 0;

                foreach (var c in ucm)
                {

                                    
                    double? rm = (c.count != 5) ? (totalbets * bonus[c.count - 2] / 100) / ary[c.count - 2] : ball + (totalbets * bonus[c.count - 2] / 100) / ary[c.count - 2];

                    if (c.count == 5)
                    {
                        h5.bingo = 1;
                        deduction = totalbets + total;
                        //deduction += total + (totalbets * 25 / 100) / ary[c.count - 2];
                    }
                    else
                    {
                        deduction = totalbets;
                    }


                    //派彩記錄
                    H5payouts h5p = new H5payouts
                    {
                        gameSn = h5.id,
                        userId = c.gamebets.userId,
                        betSn = c.gamebets.id,
                        Odds = c.gamebets.Odds,
                        money = c.gamebets.money,
                        readlMoney = rm * (100 - h5.rake) / 100,
                        createDate = DateTime.Now,
                        modiDate = DateTime.Now,
                        rake = h5.rake

                    };

                    this.Payouts(h5p);
                    //玩家加錢和記錄
                    AssetsRecord assr = new AssetsRecord
                    {
                        UserId = h5p.userId,
                        unitSn = 1,
                        gameSn = h5.id,
                        assets = (double)h5p.readlMoney,
                        type = 15,
                        h5forValue = h5.gameModel


                    };

                    new AssetsRepository().Addh5gameByAssets(assr);

                }






                //開盤資料更新
                h5.gameStatus = 0;
                h5.payDate = DateTime.Now;
                this.H5GameUpdate(h5);


                b = totalbets + total - deduction;
                H5Games h5g = new H5Games
                {
                    title = "樂透",
                    gameModel = 2,
                    gameStatus = 1,
                    valid = 1,
                    rake = 20,
                    createDate = DateTime.Now,
                    totallottery = b,
                    endTime = DateTime.Now.AddMinutes(30)
                };
                //h5g.totallottery = 500;
                new H5GameRepository().GameCreate(h5g);
                //new H5GameRepository().akAutoPay(ak.LastOrDefault());

            }
            catch
            {
                var ak = this.H5GetAll(2).Where(x => x.gameStatus == 1).ToList();
                if (ak.Count == 1)
                    b = ballAutoPay(h5);

            }


            return b;
            


        }



        //樂透無派彩自動派彩
        public double WaitballAutoPay(H5Games h5)
        {
            double b = 0;
           
            try
            {
                if (h5 == null)
                {
                    throw new Exception("error");
                }
                //亂數產生
                int[] randomArray = new int[5];
                Random rnd = new Random();  //產生亂數初始值
                for (int i = 0; i < 5; i++)
                {
                    randomArray[i] = rnd.Next(1, 37);   //亂數產生，亂數產生的範圍是1~9

                    for (int j = 0; j < i; j++)
                    {
                        while (randomArray[j] == randomArray[i])    //檢查是否與前面產生的數值發生重複，如果有就重新產生
                        {
                            j = 0;  //如有重複，將變數j設為0，再次檢查 (因為還是有重複的可能)
                            randomArray[i] = rnd.Next(1, 37);   //重新產生，存回陣列，亂數產生的範圍是1~9
                        }
                    }
                }



                //1:A-K 2:樂透
                foreach (var rd in randomArray)
                {
                    GameNumberRecord gnr = new GameNumberRecord
                    {
                        gameSn = h5.id,
                        number = rd,
                        inpdate = DateTime.Now,
                    };
                    //寫入牌記錄
                    this.GNCreate(gnr);

                }

                //派彩
                var player = this.PlayerGetAll(h5.id);
                int[] ary = new int[] { 0, 0, 0, 0 };
                int[] bonus = new int[] { 50, 30, 20, 70 };
                var ucm = new List<BallGameModel>();
                //確認多少中獎
                foreach (var p in player)
                {
                    var number = this.NumberGetAll(p.id);
                    int count = 0;
                    var uc = new BallGameModel();

                    foreach (var n in number)
                    {

                        foreach (var rd in randomArray)
                        {
                            if (rd == n.Number)
                            {
                                count += 1;
                            }

                        }

                    }
                    if (count >= 2)
                    {
                        ary[count - 2] += 1;
                        uc.gamebets = p;
                        uc.count = count;
                        ucm.Add(uc);
                    }
                    //確認正解
                    p.valid = 2;
                    this.betsUpdate(p);




                }
                double total = (h5.totallottery != null) ? (double)h5.totallottery : 0;
                double ball = 18326000 + total;
                double totalbets = this.PlayerGetAll(h5.id).Sum(x => (double)x.money);

                double deduction = 0;

                foreach (var c in ucm)
                {
                    double? rm = (c.count != 5) ? (totalbets * bonus[c.count - 2] / 100) / ary[c.count - 2] : ball + (totalbets * bonus[c.count - 2] / 100) / ary[c.count - 2];


                    if (c.count == 5)
                    {
                        h5.bingo = 1;
                        deduction = totalbets + total;
                        //deduction += total + (totalbets * 25 / 100) / ary[c.count - 2];
                    }
                    else
                    {
                        deduction = totalbets;
                    }


                    //派彩記錄
                    H5payouts h5p = new H5payouts
                    {
                        gameSn = h5.id,
                        userId = c.gamebets.userId,
                        betSn = c.gamebets.id,
                        Odds = c.gamebets.Odds,
                        money = c.gamebets.money,
                        readlMoney = rm * (100 - h5.rake) / 100,
                        createDate = DateTime.Now,
                        modiDate = DateTime.Now,
                        rake = h5.rake

                    };

                    this.Payouts(h5p);
                    //玩家加錢和記錄
                    AssetsRecord assr = new AssetsRecord
                    {
                        UserId = h5p.userId,
                        unitSn = 1,
                        gameSn = h5.id,
                        assets = (double)h5p.readlMoney,
                        type = 15,
                        h5forValue = h5.gameModel


                    };

                    new AssetsRepository().Addh5gameByAssets(assr);

                }






                //開盤資料更新
                h5.gameStatus = 0;
                h5.payDate = DateTime.Now;
                this.H5GameUpdate(h5);

                var ak = this.H5GetAll(2).Where(x => x.gameStatus == 1).ToList();
                if (ak.Count == 0)
                {
                    double m = DateTime.Now.Minute;
                    double h = DateTime.Now.Hour;


                    DateTime dtn = DateTime.Now;
                    double md = (m < 30) ? 30 : 00;
                    double hd = (m < 30) ? h : h + 1;

                    int result = 0;
                    result = Convert.ToInt16(Math.Floor(m / 3)) + 10;
                    string dt = dtn.Year + "-" + dtn.Month + "-" + dtn.Day + " " + hd + ":" + md + ":" + dtn.Second;


                    b = totalbets + total - deduction;
                    H5Games h5g = new H5Games
                    {
                        title = "樂透",
                        gameModel = 2,
                        gameStatus = 1,
                        valid = 1,
                        rake = 20,
                        createDate = DateTime.Now,
                        totallottery = b,
                        endTime = Convert.ToDateTime(dt)
                    };
                    //h5g.totallottery = 500;
                    new H5GameRepository().GameCreate(h5g);


                }



            }
            catch
            {

            }


            return b;



        }



        //每日更新
        public void DailyUpdate(DailyGameCount instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                Db.Entry(instance).State = EntityState.Modified;
                this.SaveChanges();
            }
        }




        //開局更新
        public void H5GameUpdate(H5Games instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                Db.Entry(instance).State = EntityState.Modified;
                this.SaveChanges();
            }
        }

        //下注更新
        public void betsUpdate(GameBets instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                Db.Entry(instance).State = EntityState.Modified;
                this.SaveChanges();
            }
        }
        //商品記錄修改
        public void PRUpdate(ProductRecord instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                Db.Entry(instance).State = EntityState.Modified;
                this.SaveChanges();
            }
        }

        public void Delete(Product instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                Db.Entry(instance).State = EntityState.Deleted;
                this.SaveChanges();
            }
        }

        

        public Product Get(int id)
        {
            return Db.Product.FirstOrDefault(x => x.id == id);
        }
        public ProductRecord GetProductRecord(int id)
        {
            return Db.ProductRecord.FirstOrDefault(x => x.id == id);
        }

        public List<ProductMenu> PMenuGetAll()
        {
            return Db.ProductMenu.ToList();
        }
      

        public void SaveChanges()
        {
            this.Db.SaveChanges();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.Db != null)
                {
                    this.Db.Dispose();
                    this.Db = null;
                }
            }
        }

    }
}