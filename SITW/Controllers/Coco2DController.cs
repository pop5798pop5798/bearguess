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
using SITW.Models.ViewModel;
using Microsoft.AspNet.Identity;
using System.Configuration;
using HtmlAgilityPack;

namespace SITW.Controllers
{
    
    public class Coco2DController : Controller
    {
        
        [MissionFilter]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Game()
        {
            return View();
        }
        
        public ActionResult FishJoy()
        {           
            return View();
        }    
        [AllowAnonymous]
        [HttpPost]
        public string UserA(string user,int score)
        {
            return user;
        }

        [AllowAnonymous]
        [HttpPost]
        public string Usertest(string user, int score)
        {
            AssetsViewModel avList = new AssetsRepository().getAssetsListByUserID(User.Identity.GetUserId()).Where(x=>x.unitSn == 1).FirstOrDefault();
            avList.Asset -= score;
            return avList.Asset.ToString();
        }

        [AllowAnonymous]
        [HttpGet]
        public string UserGet()
        {
            //return "5555555555555555555555555555555555555555555555";
            return User.Identity.GetUserId();
        }




        
    }
}