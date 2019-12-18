using SITW.Filter;
using SITW.Models.Repository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Net.Mime;
using System.Web.Routing;
using System.Xml;
using System.Xml.Linq;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Text;
using Microsoft.AspNet.Identity;
using HtmlAgilityPack;
using SITW.Models;

namespace SITW.Controllers
{

    public class HomeController : Controller
    {

        [MissionFilter]
        public ActionResult Index(string message)
        {
            int start = 11625;
            placard getnews = new placard();
            var news = new NewsRepository().GetAll().Where(x => x.getId != null).ToList();
            if (news.Count != 0)
            {
                var n = news.Last();
                for(int i = 0;i< 5;i++)
                {
                    var h = Homenew((int)n.getId + i);
                    if (h)
                    {
                        i = 5;
                        
                    }
                    else {
                        Homenew((int)n.getId + i);
                    }
                }
             

                    

            }
            else
            {
                string url = "http://www.ggesports.com/zh-TW/news/lol/global/Detail/" + start.ToString();

                var webGet = new HtmlWeb();
                var doc = webGet.Load(url);
                HtmlNodeCollection body = doc.DocumentNode.SelectNodes("//*[@class='news-content-body']");
                string t = doc.DocumentNode.SelectNodes("//*[@class='subject']")[0].InnerText;
                string c = body[0].InnerHtml;
                string utctime = doc.DocumentNode.SelectNodes("//*[@class='subtitle']/span")[0].InnerText;
                utctime = utctime.Replace(" (UTC)", "Z");
                DateTime rt = DateTime.Parse(utctime);

                DateTime tme = DateTime.Parse(utctime);
                string img = doc.DocumentNode.SelectNodes("//*[@class='news-content-body']//img")[0].Attributes["src"].Value;
                getnews = new placard
                {
                    title = t,
                    content = c,
                    real_time = rt,
                    time = tme,
                    image = img,
                    p_class = 4,
                    getId = start

                };
                getnews.manager = "來源：GG電競王";

                new NewsRepository().Create(getnews);

            }

            ViewBag.StatusMessage =
                message != "" ? message
                : "";
            return View();
        }
        private bool Homenew(int i) {
            bool b = true;

            try
            {
                int di = i + 1;
                string url = "http://www.ggesports.com/zh-TW/news/lol/global/Detail/" + di.ToString();



                var webGet = new HtmlWeb();
                var doc = webGet.Load(url);
                if (doc != null)
                {
                    HtmlNodeCollection body = doc.DocumentNode.SelectNodes("//*[@class='news-content-body']");
                    string t = doc.DocumentNode.SelectNodes("//*[@class='subject']")[0].InnerText;
                    string c = body[0].InnerHtml;
                    string utctime = doc.DocumentNode.SelectNodes("//*[@class='subtitle']/span")[0].InnerText;
                    utctime = utctime.Replace(" (UTC)", "Z");
                    DateTime rt = DateTime.Parse(utctime);

                    DateTime tme = DateTime.Parse(utctime);
                    string img = doc.DocumentNode.SelectNodes("//*[@class='news-content-body']//img")[0].Attributes["src"].Value;
                    var getnews = new placard
                    {
                        title = t,
                        content = c,
                        real_time = rt,
                        time = tme,
                        image = img,
                        p_class = 4,
                        getId = di

                    };
                    getnews.manager = "來源：GG電競王";

                    new NewsRepository().Create(getnews);
                }
            }
            catch {
                b = false;

            }

            return b;
                    

        }




        public ActionResult Sitemap()
        {
            Response.ContentType = "application/xml";
            return View();
        }
        public ActionResult PrivacyPolicy()
        {
            return View();
        }

        public ActionResult _Banner()
        {
            DateTime date1 = DateTime.Parse("2019-01-11T00:00:00");
            DateTime dt = DateTime.Now;

            if (DateTime.Compare(date1, dt) < 0)
            {
                ViewBag.Banner = 3;

            }
            else
            {
                ViewBag.Banner = 0;
            }



            return View();
        }

        public ActionResult IndexPage()
        {
            return View();
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult News()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Repair()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult _GameMenu()
        {
            if (User.Identity.GetUserId() != null)
            {
                var rat = new H5GameRepository().Usercount(User.Identity.GetUserId()).Where(x => x.gameModel == 3).FirstOrDefault();
                var brick = new H5GameRepository().Usercount(User.Identity.GetUserId()).Where(x => x.gameModel == 4).FirstOrDefault();
                int ratcount = 5;
                int brickcount = 1;

                if (rat != null)
                {
                    ratcount = (int)rat.count;
                }
                if (brick != null)
                {
                    brickcount = (int)brick.count;
                }
                ViewBag.Rat = "/ " + ratcount + "次";
                ViewBag.brick = "/ " + brickcount + "次";

            }
            else {
                ViewBag.Rat = "";
                ViewBag.brick = "";

            }

            return View();
        }

        public ActionResult ContractPage() {

            return View();
        }

        public ActionResult ChangeAbout()
        {
            return View();
        }

        public ActionResult LiveAbout()
        {
            return View();
        }

        public ActionResult Load()
        {
            return View();
        }


    }
}