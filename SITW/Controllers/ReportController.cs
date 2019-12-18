using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using SITDto;
using SITDto.Request;
using SITDto.ViewModel;
using SITW.Helper;
using SITW.Models;
using SITW.Models.Repository;
using SITW.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.Caching;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.IO;
using System.Net;
using System.Text;
using unirest_net.http;
using System.Threading.Tasks;
using System.Net.Http;
using System.Data;
using System.Collections;
using Newtonsoft.Json.Converters;
using static SITW.Models.ViewModel.GamePostViewModel;
using AutoMapper;
using System.Reflection;
using SITW;
using Microsoft.AspNet.Identity.Owin;

namespace SIT.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        //HttpClient client;
        string encryptedKey;
        private static MemoryCache _cache = MemoryCache.Default;
        List<gameDto> gList;
        private ApplicationUserManager _userManager;
        public ReportController()
        {
            encryptedKey = System.Web.Configuration.WebConfigurationManager.AppSettings["encryptedKey"];
            if (_cache.Contains("GameList"))
                gList = _cache.Get("GameList") as List<gameDto>;
            //client = new HttpClient();
            //client.BaseAddress = new Uri(System.Web.Configuration.WebConfigurationManager.AppSettings["apiurl"]);
            //client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<ActionResult> BetReportDetails(int id)
        {
            List<BetListDto> betlist = new List<BetListDto>();
            try
            {
                betlist = await new BetsRepository().GetBetsByGameAdmin(id);
                var groups = betlist.GroupBy(x => x.equalchoice).Select(grouping => grouping.FirstOrDefault()).ToList();
                //IEnumerable<BetListDto> bgy = groups.SelectMany(group => group);
                if (betlist.FirstOrDefault().betModel == 6)
                    betlist = groups;

                foreach (var b in betlist)
                {
                    //var user = new AssetsRepository().getAssetsListByUserID(b.userID);
                    b.username = UserManager.FindById(b.userID).UserName;
                }
                betlist = betlist.OrderByDescending(x => x.betDatetime).ToList();

                return View(betlist);
            }
            catch
            {
                return View(betlist);
            }
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }






    }

    
}

