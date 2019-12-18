using SITW.Models.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Threading;
using System.Text;
using System.IO;
using System.Net;
using System.Web.SessionState;
using System.Web.Security;
using System.Collections;
using System.Configuration;
using System.Data;
using SITW.Models;
using SITW.Models.GameAPIModels;
using SITW.Controllers;

namespace SITW
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            RouteTable.Routes.MapMvcAttributeRoutes();
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //定時
            System.Threading.Thread LoadServiceData = new System.Threading.Thread(new System.Threading.ThreadStart(LoadFromWebservice));
            LoadServiceData.Start();

        }
       /* protected void Application_Start(object sender, EventArgs e)
        {

            //定义定时器
            System.Threading.Thread LoadServiceData = new System.Threading.Thread(new System.Threading.ThreadStart(LoadFromWebservice));
            LoadServiceData.Start();          

            /*System.Timers.Timer myTimer = new System.Timers.Timer(5000);

            myTimer.Elapsed += new ElapsedEventHandler(myTimer_Elapsed);

            myTimer.Enabled = true;

            myTimer.AutoReset = true;

        }*/

        private void LoadFromWebservice()
        {
            //定义一个定时器，并开启和配置相关属性
            System.Timers.Timer Wtimer = new System.Timers.Timer(1000);//执行任务的周期
            Wtimer.Elapsed += new System.Timers.ElapsedEventHandler(Wtimer_Elapsed);
            Wtimer.Enabled = true;
            Wtimer.AutoReset = true;
        }
        void Wtimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            
            // 得到 hour minute second　如果等于某个值就开始执行某个程序。
            int intHour = e.SignalTime.Hour;
            int intMinute = e.SignalTime.Minute;
            int intSecond = e.SignalTime.Second;
            //int intMillisecond = e.SignalTime.Millisecond;
            // 定制时间； 比如 在10：30 ：00 的时候执行某个函数
            int iHour = 00;
            int iMinute = 30;
            int iSecond = 00;
            //int iMillisecond = 50;
            // 设置　每天的１０：３０：００开始执行程序
            /*if (intSecond == iSecond)
            {
                YourTask();
            }*/

            if (intSecond == iSecond)
            {
                _ = autoCsgoPay();
            }


            float akm = (float)intMinute / 10;

            if (intHour == iHour && intMinute == 00 && intSecond == iSecond)
            {
                new H5GameRepository().AutoDate();
                //Thread.Sleep(3000);
            }


            if ((intMinute == iMinute && intSecond == iSecond) || (intMinute == 00 && intSecond == iSecond))
            {
                betBallTask();
                AKTask();
                Thread.Sleep(3000);
            }
            else if ((akm.ToString().IndexOf(".") == -1) && intSecond == iSecond)
            {              
                AKTask();
                Thread.Sleep(3000);

            }

            var h5game = new H5GameRepository().H5GetAll(2).Where(x => x.endTime < DateTime.Now && x.gameStatus == 1).FirstOrDefault();
            var akgame = new H5GameRepository().H5GetAll(1).Where(x => x.endTime < DateTime.Now && x.gameStatus == 1).FirstOrDefault();
            if(intSecond == iSecond)
            {
                if(h5game != null)
                {
                    new H5GameRepository().WaitballAutoPay(h5game);
                    Thread.Sleep(3000);

                }

                if (akgame != null)
                {
                    new H5GameRepository().WaitakAutoPay(akgame);
                    Thread.Sleep(3000);

                }

            }       




        }

        private async System.Threading.Tasks.Task autoCsgoPay()
        {
            var csgo = new GameAutoPayRepository().getCsgoList();
            foreach (var c in csgo)
            {
                CSGOMatches.RootObject getm = await new CSGORepository().GetCSGOMatches((int)c.AutoSn);
                var allgame = new List<CSGOGame.RootObject>();
                foreach (var m in getm.games)
                {

                    CSGOGame.RootObject getg = await new CSGORepository().GetCSGOGame(m.id);
                    allgame.Add(getg);
                }
                await new AutoPayController().CSGOSetAnswer(c.sn,allgame, getm);




            }

        }

        void betBallTask()
        {
            var ball = new H5GameRepository().H5GetAll(2);
            if (ball.Count != 0)
            {
                 new H5GameRepository().ballAutoPay(ball.Where(x => x.gameStatus == 1).FirstOrDefault());

              
            }
            else
            {
                H5Games h5g = new H5Games
                {
                    title = "樂透",
                    gameModel = 2,
                    gameStatus = 1,
                    valid = 1,
                    rake = 20,
                    createDate = DateTime.Now,
                    totallottery = 0,
                    endTime = DateTime.Now.AddMinutes(30)
                };
                new H5GameRepository().GameCreate(h5g);
            }
        }
     


        void AKTask()
        {
            var ak = new H5GameRepository().H5GetAll(1);
            if (ak.Count != 0 && (ak.Where(x => x.gameStatus == 1).FirstOrDefault() != null) )
            {
                new H5GameRepository().akAutoPay(ak.Where(x => x.gameStatus == 1).FirstOrDefault());                             
                
            }
            else {
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


        protected void Application_End(object sender, EventArgs e)
        {

            //Log.SaveNote(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":Application End!");

            //下面的代码是关键，可解决IIS应用程序池自动回收的问题

            Thread.Sleep(1000);

            //这里设置你的web地址，可以随便指向你的任意一个aspx页面甚至不存在的页面，目的是要激发Application_Start

            string url = "http://localhost:8040/WebMall";
 
　　        HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();

            Stream receiveStream = myHttpWebResponse.GetResponseStream();//得到回写的字节流

        }







    }
}
