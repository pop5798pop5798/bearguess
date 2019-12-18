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
using GoogleCloudSamples.Services;

namespace SIT.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SettingController : Controller
    {
        //HttpClient client;
        string encryptedKey;
        private static MemoryCache _cache = MemoryCache.Default;
        List<gameDto> gList;
        private ImageUploader _imageUploader;
        public SettingController()
        {
            encryptedKey = System.Web.Configuration.WebConfigurationManager.AppSettings["encryptedKey"];
            if (_cache.Contains("GameList"))
                gList = _cache.Get("GameList") as List<gameDto>;
            //client = new HttpClient();
            //client.BaseAddress = new Uri(System.Web.Configuration.WebConfigurationManager.AppSettings["apiurl"]);
            //client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        // GET: game
        public async System.Threading.Tasks.Task<ActionResult> Index()
        {


            SettingListDto gameb = await new DgonHonusRepository().GetBonusList();
            gameb.gamesettingList.Where(x => x.sn == 1).FirstOrDefault().outlay = (100 - gameb.gamesettingList.Where(x => x.sn == 1).FirstOrDefault().outlay);

            return View(gameb);



        }

        // GET: game
        public async System.Threading.Tasks.Task<ActionResult> pool()
        {


            SettingListDto gameb = await new DgonHonusRepository().GetBonusList();
            gameb.gamesettingList.Where(x => x.sn == 2).FirstOrDefault().outlay = (100 - gameb.gamesettingList.Where(x => x.sn == 2).FirstOrDefault().outlay);
            gameb.gamesettingList.Where(x => x.sn == 2).FirstOrDefault().allocation = (100 - gameb.gamesettingList.Where(x => x.sn == 2).FirstOrDefault().allocation);

            gameb.gamesettingList = gameb.gamesettingList.Where(x => x.sn == 2).ToList();
            return View(gameb);



        }

        // GET: game
        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> pool(SettingListDto stting)
        {


            try
            {

                stting.gamesettingList.Where(x => x.sn == 2).FirstOrDefault().outlay = (100 - stting.gamesettingList.Where(x => x.sn == 2).FirstOrDefault().outlay);
                stting.gamesettingList.Where(x => x.sn == 2).FirstOrDefault().allocation = (100 - stting.gamesettingList.Where(x => x.sn == 2).FirstOrDefault().allocation);
                stting = await new DgonHonusRepository().GetSetting(stting);



                return View(stting);

            }
            catch
            {
                return View(stting);
            }



        }

        // POST: game/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async System.Threading.Tasks.Task<ActionResult> Index(SettingListDto stting)
        {
            try
            {

                stting.gamesettingList.Where(x => x.sn == 1).FirstOrDefault().outlay = (100 - stting.gamesettingList.Where(x => x.sn == 1).FirstOrDefault().outlay);
                stting = await new DgonHonusRepository().GetSetting(stting);



                return View(stting);

            }
            catch
            {
                return View(stting);
            }
        }


        // GET: game

        public async System.Threading.Tasks.Task<ActionResult> nabobIndex()
        {
            SettingListDto gameb = await new SttingRepository().GetTopicSettingList();
            SettingListDto gameblol = gameb;
            gameblol.topicsettingList = gameblol.topicsettingList.Where(x => x.gametype == 1).ToList();
            ViewData["lolList"] = gameblol;
            return View();
        }


        public ActionResult _nabobLol(SettingListDto stting)
        {


            return View(stting);



        }

        // POST: game/Create

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> _nabobLolPost(SettingListDto stting)
        {
            try
            {

                stting = await new DgonHonusRepository().GetSetting(stting);
                return View("_nabobLol", stting);

                // return RedirectToAction("nabobindex","Setting");

            }
            catch
            {
                return View(stting);
            }
        }


        // GET: game
        [AllowAnonymous]
        public async System.Threading.Tasks.Task<ActionResult> _topicCreate(SettingListDto model, int? index)
        {


            //SettingListDto gameb = await new SttingRepository().GetTopicSettingList();
            model.topicsettingList = new List<topicSettingDto>();
            for (int i = 0; i <= index; i++)
            {
                model.topicsettingList.Add(new topicSettingDto { id = ((int)index + 1), valid = 1, gametype = 1, choicsettingList = new List<choiceSettingDto>() });
                for (int j = 0; j < 2; j++)
                {
                    model.topicsettingList[i].choicsettingList.Add(new choiceSettingDto { valid = 1, topiceSetting = (index + 1) });

                }
            }


            ViewBag.Index = index;

            return View(model);



        }



        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> _topicCreate(SettingListDto setting, HttpPostedFileBase image, HttpPostedFileBase hoverimage)
        {
            if (setting != null && ModelState.IsValid)
            {
                foreach (var s in setting.topicsettingList)
                {
                    if (string.IsNullOrEmpty(s.image) && image != null)
                    {
                        string filename = "";
                        filename = DateTime.Now.ToString("yyyyMMddTHHmmssfff");
                        Google.Apis.Auth.OAuth2.GoogleCredential credential = await Google.Apis.Auth.OAuth2.GoogleCredential.GetApplicationDefaultAsync();
                        _imageUploader = new ImageUploader(System.Web.Configuration.WebConfigurationManager.AppSettings["GoogleCloud:BucketName"]);
                        var imageUrl = await _imageUploader.UploadImage(image, filename, "Topicsimg");
                        s.image = imageUrl;
                    }

                    if (string.IsNullOrEmpty(s.hoverImage) && hoverimage != null)
                    {
                        string filename = "";
                        filename = DateTime.Now.ToString("yyyyMMddTHHmmssfff");
                        Google.Apis.Auth.OAuth2.GoogleCredential credential = await Google.Apis.Auth.OAuth2.GoogleCredential.GetApplicationDefaultAsync();
                        _imageUploader = new ImageUploader(System.Web.Configuration.WebConfigurationManager.AppSettings["GoogleCloud:BucketName"]);
                        var imageUrl = await _imageUploader.UploadImage(hoverimage, filename, "Topicsimg");
                        s.hoverImage = imageUrl;
                    }




                }
                ;
                await new DgonHonusRepository().topicSettingCreate(setting);
                //this.teamsRepository.Create(teams);
                return View(setting);
                //return RedirectToAction("nabobindex", "Setting");
            }
            else
            {
                return View(setting);
            }


        }
















    }
}
