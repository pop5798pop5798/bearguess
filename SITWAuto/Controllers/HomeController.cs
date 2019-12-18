﻿using SITW.Filter;
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

namespace SITW.Controllers
{
    
    public class HomeController : Controller
    {
        
        [MissionFilter]
        public ActionResult Index()
        {         
            return View();
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
                ViewBag.Banner = 1;

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
                int brickcount = 5;

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


    }
}