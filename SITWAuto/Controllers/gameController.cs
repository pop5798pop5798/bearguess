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

namespace SIT.Controllers
{
    [Authorize]
    public class gameController : Controller
    {
        //HttpClient client;
        string encryptedKey;
        private static MemoryCache _cache = MemoryCache.Default;
        List<gameDto> gList;
        public gameController()
        {
            encryptedKey = System.Web.Configuration.WebConfigurationManager.AppSettings["encryptedKey"];
            if (_cache.Contains("GameList"))
                gList = _cache.Get("GameList") as List<gameDto>;
            //client = new HttpClient();
            //client.BaseAddress = new Uri(System.Web.Configuration.WebConfigurationManager.AppSettings["apiurl"]);
            //client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        // GET: Home
        [AllowAnonymous]
        public ActionResult test()
        {
            return View();
        }

        // GET: game
        [AllowAnonymous]
        public async System.Threading.Tasks.Task<ActionResult> Index()
        {
            GamePostsRepository _gpr = new GamePostsRepository();

            List<GamePosts> gpList = _gpr.getValidAll();

            List<gameDto> gameList = await new GamesRepository().GetMainGameList();
            List<VedioRecord> vrlist = new List<VedioRecord>();
            vrlist = new VedioRecordRepository().getAll();
            List<Teams> teamlist = new TeamsRepository().getAll();


            List<GamePostViewModel> gpvmList = new List<GamePostViewModel>();
            gpList = gpList.OrderByDescending(p => p.inpdate).ToList();
            DateTime dt = DateTime.Now.AddHours(-2);
            foreach (var gp in gpList)
            {
                
                    GamePostViewModel gpvm = new GamePostViewModel();
                    gpvm.gamepost = gp;
                    gpvm.game = gameList.Where(p => p.sn == gp.GameSn).FirstOrDefault();
                    if (gpvm.game == null)
                       continue;
                    if ((gp.sdate <= DateTime.Now && gp.edate >= dt) || gpvm.game.betModel == 7)
                    {
                        

                        gpvm.vedio = vrlist.Where(p => p.sn == gp.VedioRecordSn).FirstOrDefault();

                        if (gpvm.vedio == null)
                        {
                            gpvm.vedio_url = "";
                        }
                        else
                        {

                            string movieclass_twitch = "";
                            string movieclass_youtube = "";
                            string movieclass_huya = "";
                            if (gpvm.vedio.vediourl.Length >= 22)
                            {
                                movieclass_twitch = gpvm.vedio.vediourl.Substring(0, 22);
                            }
                            if (gpvm.vedio.vediourl.Length >= 24)
                            {
                                movieclass_youtube = gpvm.vedio.vediourl.Substring(0, 24);
                            }
                            if (gpvm.vedio.vediourl.Length >= 21)
                            {
                                movieclass_huya = gpvm.vedio.vediourl.Substring(0, 21);
                            }

                            if (movieclass_twitch == "https://www.twitch.tv/")
                            {
                                gpvm.vedio_url = Regex.Replace(gpvm.vedio.vediourl, movieclass_twitch, String.Empty);
                                gpvm.vedio_url = "https://player.twitch.tv/?channel=" + gpvm.vedio_url;
                            }
                            else if (movieclass_youtube == "https://www.youtube.com/")
                            {
                                int vediolength = gpvm.vedio.vediourl.Length - 32;
                                string regex_youtube = gpvm.vedio.vediourl.Substring(32, vediolength);
                                //regex_youtube = Regex.Replace(gpvm.vedio.vediourl, regex_youtube, String.Empty);
                                gpvm.vedio_url = "https://www.youtube.com/embed/" + regex_youtube;
                            } else if (movieclass_huya == "https://www.huya.com/")
                            {
                                gpvm.vedio_url = Regex.Replace(gpvm.vedio.vediourl, movieclass_huya, String.Empty);
                                gpvm.vedio_url = "http://liveshare.huya.com/iframe/" + gpvm.vedio_url;
                            }
                            else
                            {
                                gpvm.vedio_url = "";
                            }


                        }


                        gpvm.TeamA = teamlist.Where(p => p.sn == gp.TeamASn).FirstOrDefault();
                        gpvm.TeamB = teamlist.Where(p => p.sn == gp.TeamBSn).FirstOrDefault();
                        gpvm.gamesearch = gpvm.game.title + gpvm.game.comment + gpvm.TeamA.shortName + gpvm.TeamB.shortName + gpvm.game.betModelString;
                        gpvm.gamesearch = gpvm.gamesearch.ToLower();
                        gpvm.endguess = 1;
              


                        if (gpvm.game.edate <= DateTime.Now)
                        {
                            gpvm.endguess = 0;
                        }
                        else if (gpvm.game.edate <= DateTime.Now.AddMinutes(30))
                        {
                            gpvm.endguess = 2;
                        }
                        if (gp.PlayGameSn.HasValue)
                        {
                            gpvm.PlayGame = new cfgPlayGameRepository().get(gp.PlayGameSn.Value);
                        }



                
                        gpvmList.Add(gpvm);


                    }




            }
            


            var playgamelist = new cfgPlayGameRepository().getAll();
            ViewData["playlist"] = playgamelist;


            var gpvmListDy = gpvmList.OrderByDescending(x => x.game.betModel == 5).ThenBy(x => x.game.gamedate);
          

            ViewData["UserID"] = User.Identity.GetUserId();
            return View(gpvmListDy);



        }




        [AllowAnonymous]
        public async System.Threading.Tasks.Task<ActionResult> _GetGameList(int playgamsn)
        {
            GamePostsRepository _gpr = new GamePostsRepository();

            List<GamePosts> gpList = _gpr.getValidAll();
            List<gameDto> gameList = await new GamesRepository().GetMainGameList();

            List<VedioRecord> vrlist = new List<VedioRecord>();
            vrlist = new VedioRecordRepository().getAll();
            List<Teams> teamlist = new TeamsRepository().getAll();


            List<GamePostViewModel> gpvmList = new List<GamePostViewModel>();
            gpList = gpList.OrderByDescending(p => p.inpdate).ToList();
            DateTime dt = DateTime.Now.AddHours(-2);
            foreach (var gp in gpList)
            {
                GamePostViewModel gpvm = new GamePostViewModel();
                gpvm.gamepost = gp;
                gpvm.game = gameList.Where(p => p.sn == gp.GameSn).FirstOrDefault();
                if (gpvm.game == null)
                    continue;
                if ((gp.sdate <= DateTime.Now && gp.edate >= dt) || gpvm.game.betModel == 7)
                {
                   

                    gpvm.vedio = vrlist.Where(p => p.sn == gp.VedioRecordSn).FirstOrDefault();


                    //VedioRecord movieclass_twitch_string = new VedioRecordRepository().getnew(gp.VedioRecordSn);
                    //var movieclass_twitch = gpvm.vedio.vediourl.Substring(0, 21);
                    //string movieclass_youtube = gpvm.vedio.vediourl.Substring(0, 23);
                    if (gpvm.vedio == null)
                    {
                        gpvm.vedio_url = "";
                    }
                    else
                    {

                        string movieclass_twitch = "";
                        string movieclass_youtube = "";
                        string movieclass_huya = "";
                        if (gpvm.vedio.vediourl.Length >= 22)
                        {
                            movieclass_twitch = gpvm.vedio.vediourl.Substring(0, 22);
                        }
                        if (gpvm.vedio.vediourl.Length >= 24)
                        {
                            movieclass_youtube = gpvm.vedio.vediourl.Substring(0, 24);
                        }
                        if (gpvm.vedio.vediourl.Length >= 21)
                        {
                            movieclass_huya = gpvm.vedio.vediourl.Substring(0, 21);
                        }

                        if (movieclass_twitch == "https://www.twitch.tv/")
                        {
                            gpvm.vedio_url = Regex.Replace(gpvm.vedio.vediourl, movieclass_twitch, String.Empty);
                            gpvm.vedio_url = "https://player.twitch.tv/?channel=" + gpvm.vedio_url;
                        }
                        else if (movieclass_youtube == "https://www.youtube.com/")
                        {
                            int vediolength = gpvm.vedio.vediourl.Length - 32;
                            string regex_youtube = gpvm.vedio.vediourl.Substring(32, vediolength);
                            //regex_youtube = Regex.Replace(gpvm.vedio.vediourl, regex_youtube, String.Empty);
                            gpvm.vedio_url = "https://www.youtube.com/embed/" + regex_youtube;
                        }
                        else if (movieclass_huya == "https://www.huya.com/")
                        {
                            gpvm.vedio_url = Regex.Replace(gpvm.vedio.vediourl, movieclass_huya, String.Empty);
                            gpvm.vedio_url = "http://liveshare.huya.com/iframe/" + gpvm.vedio_url;
                        }
                        else
                        {
                            gpvm.vedio_url = "";
                        }


                    }


                    gpvm.TeamA = teamlist.Where(p => p.sn == gp.TeamASn).FirstOrDefault();
                    gpvm.TeamB = teamlist.Where(p => p.sn == gp.TeamBSn).FirstOrDefault();
                    gpvm.gamesearch = gpvm.game.title + gpvm.game.comment + gpvm.TeamA.shortName + gpvm.TeamB.shortName + gpvm.game.betModelString;
                    gpvm.gamesearch = gpvm.gamesearch.ToLower();
                    gpvm.endguess = 1;
                    string poolallstr = string.Format("{0:0,0}", gpvm.game.topicList[0].poolall);
                    // string[] arrStringB = new string[poolallstr.Length];
                    gpvm.arrStringB = new string[poolallstr.Length];


                    for (int k = 0; k < poolallstr.Length; k++)
                    {
                        gpvm.arrStringB[k] = poolallstr.Substring(k, 1);
                    }



                    if (gpvm.game.edate <= DateTime.Now)
                    {
                        gpvm.endguess = 0;
                    }
                    else if (gpvm.game.edate <= DateTime.Now.AddMinutes(30))
                    {
                        gpvm.endguess = 2;
                    }
                    if (gp.PlayGameSn.HasValue)
                    {
                        gpvm.PlayGame = new cfgPlayGameRepository().get(gp.PlayGameSn.Value);
                    }


                    if (playgamsn == gpvm.PlayGame.sn || playgamsn == 0)
                    {

                        gpvmList.Add(gpvm);
                    }

                }

            }


            if (playgamsn != 0)
            {
                ViewBag.pgame = new cfgPlayGameRepository().get(playgamsn).cName;
                ViewBag.pgamesn = new cfgPlayGameRepository().get(playgamsn).sn;
            }
            else
            {
                ViewBag.pgame = "全部賽局";
                ViewBag.pgamesn = 0;

            }

            var gpvmListDy = gpvmList.OrderByDescending(x => x.game.betModel == 5).ThenBy(x => x.game.gamedate);
            ViewData["UserID"] = User.Identity.GetUserId();
            return View(gpvmListDy);



        }




        [Authorize(Roles = "Admin")]
        public async System.Threading.Tasks.Task<ActionResult> AdminIndex()
        {

            GamePostsRepository _gpr = new GamePostsRepository();

            List<GamePosts> gpList = _gpr.getAll();
            List<gameDto> gameList = await new GamesRepository().GetGameList();

            List<VedioRecord> vrlist = new List<VedioRecord>();
            vrlist = new VedioRecordRepository().getAll();
            List<Teams> teamlist = new TeamsRepository().getAll();


            List<GamePostViewModel> gpvmList = new List<GamePostViewModel>();
            gpList = gpList.OrderByDescending(p => p.inpdate).ToList();
            foreach (var gp in gpList)
            {
                GamePostViewModel gpvm = new GamePostViewModel();
                gpvm.gamepost = gp;
                if (gameList.Where(p => p.sn == gp.GameSn).Count() > 0)
                {
                    gpvm.game = gameList.Where(p => p.sn == gp.GameSn).FirstOrDefault();
                }
                else
                    continue;
                gpvm.vedio = vrlist.Where(p => p.sn == gp.VedioRecordSn).FirstOrDefault();
                gpvm.TeamA = teamlist.Where(p => p.sn == gp.TeamASn).FirstOrDefault();
                gpvm.TeamB = teamlist.Where(p => p.sn == gp.TeamBSn).FirstOrDefault();
                gpvm.endguess = 1;
                if (gpvm.game.Status != "已派彩" && gpvm.game.Status != "")
                {
                    gpvmList.Add(gpvm);
                }

            }
            var gpvmListDy = gpvmList.OrderBy(x => x.game.gamedate);
            ViewData["UserID"] = User.Identity.GetUserId();
            return View(gpvmListDy);
        }

        [Authorize(Roles = "Admin")]
        public async System.Threading.Tasks.Task<ActionResult> Payout()
        {

            GamePostsRepository _gpr = new GamePostsRepository();

            List<GamePosts> gpList = _gpr.getAll();
            List<gameDto> gameList = await new GamesRepository().GetGameList();

            List<VedioRecord> vrlist = new List<VedioRecord>();
            vrlist = new VedioRecordRepository().getAll();
            List<Teams> teamlist = new TeamsRepository().getAll();


            List<GamePostViewModel> gpvmList = new List<GamePostViewModel>();
            gpList = gpList.OrderByDescending(p => p.inpdate).ToList();
            foreach (var gp in gpList)
            {
                GamePostViewModel gpvm = new GamePostViewModel();
                gpvm.gamepost = gp;
                if (gameList.Where(p => p.sn == gp.GameSn).Count() > 0)
                {
                    gpvm.game = gameList.Where(p => p.sn == gp.GameSn).FirstOrDefault();
                }
                else
                    continue;
                gpvm.vedio = vrlist.Where(p => p.sn == gp.VedioRecordSn).FirstOrDefault();
                gpvm.TeamA = teamlist.Where(p => p.sn == gp.TeamASn).FirstOrDefault();
                gpvm.TeamB = teamlist.Where(p => p.sn == gp.TeamBSn).FirstOrDefault();
                gpvm.endguess = 1;
                if (gpvm.game.Status == "已派彩")
                {
                    gpvmList.Add(gpvm);
                }

            }
            var gpvmListDy = gpvmList.OrderByDescending(x => x.game.gamedate);

            ViewData["UserID"] = User.Identity.GetUserId();
            return View("AdminIndex", gpvmListDy);
        }

        //[ChildActionOnly]
        [AllowAnonymous]
        public async System.Threading.Tasks.Task<ActionResult> _popularGame()
        {
            GamePostsRepository _gpr = new GamePostsRepository();
            List<GamePosts> gpList = _gpr.getAll();
            List<gameDto> gameList = await new GamesRepository().GetValidGameList();
            // List<VedioRecord> vrlist = new List<VedioRecord>();
            /*vrlist = new VedioRecordRepository().getAll();*/
            List<Teams> teamlist = new TeamsRepository().getAll();

            //gameList = gameList.OrderBy(p => p.gamedate).Take(3).ToList();

            List<GamePostViewModel> gpvmList = new List<GamePostViewModel>();
            gpList = gpList.OrderByDescending(p => p.inpdate).ToList();
            foreach (var gp in gpList)
            {
                GamePostViewModel gpvm = new GamePostViewModel();
                gpvm.gamepost = gp;
                if (gameList.Where(p => p.sn == gp.GameSn).Count() > 0)
                {
                    gpvm.game = gameList.Where(p => p.sn == gp.GameSn).FirstOrDefault();
                }
                else
                    continue;
                /*gpvm.vedio = vrlist.Where(p => p.sn == gp.VedioRecordSn).FirstOrDefault();*/
                gpvm.TeamA = teamlist.Where(p => p.sn == gp.TeamASn).FirstOrDefault();
                if (gpvm.TeamA == null)
                    gpvm.TeamA = new Teams();
                gpvm.TeamB = teamlist.Where(p => p.sn == gp.TeamBSn).FirstOrDefault();
                if (gpvm.TeamB == null)
                    gpvm.TeamB = new Teams();
                gpvmList.Add(gpvm);
            }

            var gpvmListDy = gpvmList.OrderBy(x => x.game.gamedate).Take(3);
            return PartialView(gpvmListDy);
        }

        // GET: game/Details/5
        [AllowAnonymous]
        public async System.Threading.Tasks.Task<ActionResult> Details(int id)
        {
            string UserID = User.Identity.GetUserId();
            GamePosts gamepost = new GamePostsRepository().get(id);
            gameDto game = null;
            game = await new GamesRepository().GetGameDetail(gamepost.GameSn);
            if (game.betModel == 1 || game.betModel == 2 || game.betModel == 7)
            {
                List<VedioRecord> vrlist = new List<VedioRecord>();
                vrlist = new VedioRecordRepository().getAll();


                GamePostViewModel gpvm = new GamePostViewModel(id, encryptedKey, game);
                gpvm.gamepost = gamepost;
                List<Teams> teamlist = new TeamsRepository().getAll();
                gpvm.TeamA = teamlist.Where(p => p.sn == gamepost.TeamASn).FirstOrDefault();
                gpvm.TeamB = teamlist.Where(p => p.sn == gamepost.TeamBSn).FirstOrDefault();
                if (gpvm.gamepost.PlayGameSn.HasValue)
                {
                    gpvm.PlayGame = new cfgPlayGameRepository().get(gpvm.gamepost.PlayGameSn.Value);
                }
                gpvm.vedio = vrlist.Where(p => p.sn == gpvm.gamepost.VedioRecordSn).FirstOrDefault();


                //VedioRecord movieclass_twitch_string = new VedioRecordRepository().getnew(gp.VedioRecordSn);
                //var movieclass_twitch = gpvm.vedio.vediourl.Substring(0, 21);
                //string movieclass_youtube = gpvm.vedio.vediourl.Substring(0, 23);
                if (gpvm.vedio == null)
                {
                    gpvm.vedio_url = "";
                }
                else
                {
                    string movieclass_twitch = "";
                    string movieclass_youtube = "";
                    string movieclass_huya = "";
                    if (gpvm.vedio.vediourl.Length >= 22)
                    {
                        movieclass_twitch = gpvm.vedio.vediourl.Substring(0, 22);
                    }
                    if (gpvm.vedio.vediourl.Length >= 24)
                    {
                        movieclass_youtube = gpvm.vedio.vediourl.Substring(0, 24);
                    }
                    if (gpvm.vedio.vediourl.Length >= 21)
                    {
                        movieclass_huya = gpvm.vedio.vediourl.Substring(0, 21);
                    }

                    if (movieclass_twitch == "https://www.twitch.tv/")
                    {
                        gpvm.vedio_url = Regex.Replace(gpvm.vedio.vediourl, movieclass_twitch, String.Empty);
                        gpvm.vedio_url = "https://player.twitch.tv/?channel=" + gpvm.vedio_url;
                    }
                    else if (movieclass_youtube == "https://www.youtube.com/")
                    {
                        int vediolength = gpvm.vedio.vediourl.Length - 32;
                        string regex_youtube = gpvm.vedio.vediourl.Substring(32, vediolength);
                        //regex_youtube = Regex.Replace(gpvm.vedio.vediourl, regex_youtube, String.Empty);
                        gpvm.vedio_url = "https://www.youtube.com/embed/" + regex_youtube;
                    }
                    else if (movieclass_huya == "https://www.huya.com/")
                    {
                        gpvm.vedio_url = Regex.Replace(gpvm.vedio.vediourl, movieclass_huya, String.Empty);
                        gpvm.vedio_url = "http://liveshare.huya.com/iframe/" + gpvm.vedio_url;
                    }
                    else
                    {
                        gpvm.vedio_url = "";
                    }


                }


                //ViewData["BetList"] = await new GamesRepository().GetBetsByGame(gamepost.GameSn);

                ViewData["NewUserAssets"] = SITW.Helper.CacheHelper.GlobalSettingData.Where(p => p.Key == "NewUserAssets").FirstOrDefault().Value;

                return View(gpvm);
            }
            return RedirectToAction("index");
        }


        // GET: game/Details/5
        [AllowAnonymous]
        public async System.Threading.Tasks.Task<ActionResult> NabobDetails(int id)
        {
            string UserID = User.Identity.GetUserId();
            GamePosts gamepost = new GamePostsRepository().get(id);
            gameDto game = null;
            game = await new GamesRepository().GetGameDetail(gamepost.GameSn);
            if (game.betModel == 6)
            {
                List<VedioRecord> vrlist = new List<VedioRecord>();
                vrlist = new VedioRecordRepository().getAll();


                GamePostViewModel gpvm = new GamePostViewModel(id, encryptedKey, game);
                gpvm.gamepost = gamepost;
                List<Teams> teamlist = new TeamsRepository().getAll();
                gpvm.TeamA = teamlist.Where(p => p.sn == gamepost.TeamASn).FirstOrDefault();
                gpvm.TeamB = teamlist.Where(p => p.sn == gamepost.TeamBSn).FirstOrDefault();
                if (gpvm.gamepost.PlayGameSn.HasValue)
                {
                    gpvm.PlayGame = new cfgPlayGameRepository().get(gpvm.gamepost.PlayGameSn.Value);
                }
                gpvm.vedio = vrlist.Where(p => p.sn == gpvm.gamepost.VedioRecordSn).FirstOrDefault();


                //VedioRecord movieclass_twitch_string = new VedioRecordRepository().getnew(gp.VedioRecordSn);
                //var movieclass_twitch = gpvm.vedio.vediourl.Substring(0, 21);
                //string movieclass_youtube = gpvm.vedio.vediourl.Substring(0, 23);
                if (gpvm.vedio == null)
                {
                    gpvm.vedio_url = "";
                }
                else
                {
                    string movieclass_twitch = "";
                    string movieclass_youtube = "";
                    string movieclass_huya = "";
                    if (gpvm.vedio.vediourl.Length >= 22)
                    {
                        movieclass_twitch = gpvm.vedio.vediourl.Substring(0, 22);
                    }
                    if (gpvm.vedio.vediourl.Length >= 24)
                    {
                        movieclass_youtube = gpvm.vedio.vediourl.Substring(0, 24);
                    }
                    if (gpvm.vedio.vediourl.Length >= 21)
                    {
                        movieclass_huya = gpvm.vedio.vediourl.Substring(0, 21);
                    }

                    if (movieclass_twitch == "https://www.twitch.tv/")
                    {
                        gpvm.vedio_url = Regex.Replace(gpvm.vedio.vediourl, movieclass_twitch, String.Empty);
                        gpvm.vedio_url = "https://player.twitch.tv/?channel=" + gpvm.vedio_url;
                    }
                    else if (movieclass_youtube == "https://www.youtube.com/")
                    {
                        int vediolength = gpvm.vedio.vediourl.Length - 32;
                        string regex_youtube = gpvm.vedio.vediourl.Substring(32, vediolength);
                        //regex_youtube = Regex.Replace(gpvm.vedio.vediourl, regex_youtube, String.Empty);
                        gpvm.vedio_url = "https://www.youtube.com/embed/" + regex_youtube;
                    }
                    else if (movieclass_huya == "https://www.huya.com/")
                    {
                        gpvm.vedio_url = Regex.Replace(gpvm.vedio.vediourl, movieclass_huya, String.Empty);
                        gpvm.vedio_url = "http://liveshare.huya.com/iframe/" + gpvm.vedio_url;
                    }
                    else
                    {
                        gpvm.vedio_url = "";
                    }


                }


                //ViewData["BetList"] = await new GamesRepository().GetBetsByGame(gamepost.GameSn);

                ViewData["NewUserAssets"] = SITW.Helper.CacheHelper.GlobalSettingData.Where(p => p.Key == "NewUserAssets").FirstOrDefault().Value;

                return View(gpvm);
            }
            return RedirectToAction("index");
        }


        // GET: game/PrizePoolDetails/5
        [AllowAnonymous]
        public async System.Threading.Tasks.Task<ActionResult> PrizePoolDetails(int id)
        {
            string UserID = User.Identity.GetUserId();
            GamePosts gamepost = new GamePostsRepository().get(id);
            gameDto game = null;
            game = await new GamesRepository().GetGameDetail(gamepost.GameSn);
            if(game.betModel == 5)
            {
                List<VedioRecord> vrlist = new List<VedioRecord>();
                vrlist = new VedioRecordRepository().getAll();


                GamePostViewModel gpvm = new GamePostViewModel(id, encryptedKey, game);
                gpvm.gamepost = gamepost;
                List<Teams> teamlist = new TeamsRepository().getAll();
                gpvm.TeamA = teamlist.Where(p => p.sn == gamepost.TeamASn).FirstOrDefault();
                gpvm.TeamB = teamlist.Where(p => p.sn == gamepost.TeamBSn).FirstOrDefault();
               
                string poolallstr = string.Format("{0:0,0}", gpvm.game.topicList[0].poolall);
                string[] arrStringB = new string[poolallstr.Length];



                for (int k = 0; k < poolallstr.Length; k++)
                {
                    arrStringB[k] = poolallstr.Substring(k, 1);
                }
                ViewData["poolall"] = arrStringB;


                ViewData["NewUserAssets"] = SITW.Helper.CacheHelper.GlobalSettingData.Where(p => p.Key == "NewUserAssets").FirstOrDefault().Value;

                return View(gpvm);

            }
            return RedirectToAction("index");

        }

        public async System.Threading.Tasks.Task<ActionResult> WBALL(int id)
        {
            GamePosts gamepost = new GamePostsRepository().get(id);
            gameDto game = null;
            game = await new GamesRepository().GetGameDetail(gamepost.GameSn);

            return View(game);
        }





        // GET: game/Details/5
        [Authorize(Roles = "Admin")]
        public async System.Threading.Tasks.Task<ActionResult> DetailsAdmin(int id)
        {
           
            string UserID = User.Identity.GetUserId();
            GamePosts gamepost = new GamePostsRepository().get(id);
            gameDto game = null;
            game = await new GamesRepository().GetGameDetail(gamepost.GameSn);

            GamePostViewModel gpvm = new GamePostViewModel(id, encryptedKey, game);
            gpvm.gamepost = gamepost;

            game = await new GamesRepository().GetGameDetail(gamepost.GameSn);
            //if (game.userId != UserID)
            //    return View("Details", gpvm);


            ViewData["BetList"] = await new BetsRepository().GetBetsByGame(gamepost.GameSn);          

            return View(gpvm);
        }


        // GET: game/Details/5
        [Authorize(Roles = "Admin")]
        public async System.Threading.Tasks.Task<ActionResult> DPDetailsAdmin(int id)
        {

            string UserID = User.Identity.GetUserId();
            GamePosts gamepost = new GamePostsRepository().get(id);
            gameDto game = null;
            game = await new GamesRepository().GetGameDetail(gamepost.GameSn);

            GamePostViewModel gpvm = new GamePostViewModel(id, encryptedKey, game);
            gpvm.gamepost = gamepost;

            game = await new GamesRepository().GetGameDetail(gamepost.GameSn);
            //if (game.userId != UserID)
            //    return View("Details", gpvm);


            ViewData["BetList"] = await new BetsRepository().GetBetsByGame(gamepost.GameSn);


            return View(gpvm);
        }




        // GET: game/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {           
            gameDto game = new gameDto { topicList = new List<topicDto>(),bonusList = new List<bonusDto>() };

            GamePostViewModel gpvm = new GamePostViewModel { game = game, vedio = new VedioRecord(),TeamList = new List<Teams>()};
            ///Leagues leagues = new LeaguesRepository().getAll().Where(x => x.playGamesn == 0).FirstOrDefault();
            for (int i = 0; i <= 2; i++)
            {

                gpvm.TeamList.Add(new Teams { valid = 1});
            }

            return View(gpvm);
        }

        public ActionResult gameSelect(int? playgame)
        {

            gameDto game = new gameDto { topicList = new List<topicDto>() };
            GamePostViewModel gpvm = new GamePostViewModel
            {
                game = game,
                vedio = new VedioRecord()
            };

            ViewBag.item = playgame;


            return View(gpvm);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult _topicCreate(gameDto model, int? index,int? walk, int? choice)
        {
            index = index ?? 0;
            ViewBag.Index = index;
            ViewBag.walk = walk;
            ViewBag.choice = choice;
            model.topicList = new List<topicDto>();
            for(int i=0;i<=index;i++)
            {
                model.topicList.Add(new topicDto { valid = 1, main = (index == 0), choiceList = new List<choiceDto>() });
            }
            GamePostViewModel gpvm = new GamePostViewModel { game = model, vedio = new VedioRecord() };
            return PartialView(gpvm);
        }


        [Authorize(Roles = "Admin")]
        public ActionResult _bonusCreate(gameDto model, int? index,int? count)
        {
            index = index ?? 0;
            //count = count ?? 0;
            ViewBag.Index = index;
            ViewBag.count = count;
            //ViewBag.choice = choice;
            model.bonusList = new List<bonusDto>();
            for (int? i = index; i < index + count; i++)
            {
                model.bonusList.Add(new bonusDto { bonus = false });
            }
            GamePostViewModel gpvm = new GamePostViewModel { game = model, vedio = new VedioRecord() };
            return PartialView(gpvm);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult _bonusEdit(gameDto model, int? index)
        {
            ViewBag.Index = index;
            GamePostViewModel gpvm = new GamePostViewModel { game = model, vedio = new VedioRecord() };

            return View(gpvm);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult _choiceCreate(gameDto model,int? topicIndex, int? index,int? allnumber,int? dnnumber)
        {
            
            index = index ?? 0;
            int tIndex = topicIndex ?? 0;
            ViewBag.Index = index;
            ViewBag.topicIndex = tIndex;
            ViewBag.allindex = allnumber;
            ViewBag.dnnumber = dnnumber;
            model.topicList = new List<topicDto>();
            /*for (int i = 0; i <= topicIndex; i++)
            {
                model.topicList.Add(new topicDto { choiceList = new List<choiceDto>() });
                for (int j = 0; j <= index+ allnumber && i == topicIndex; j++)
                    model.topicList[tIndex].choiceList.Add(new choiceDto() { valid = 1 });
            }*/
            for (int i = 0; i <= topicIndex; i++)
            {
                model.topicList.Add(new topicDto { choiceList = new List<choiceDto>() });
                for (int j = 0; j <= index + allnumber && i == topicIndex; j++)
                    model.topicList[tIndex].choiceList.Add(new choiceDto() { valid = 1 ,choiceString = new List<choiceStrDto>() });
            }

           



            GamePostViewModel gpvm = new GamePostViewModel { game = model, vedio = new VedioRecord() };
            return PartialView(gpvm);
        }


        [Authorize(Roles = "Admin")]
        public ActionResult _choiceStrCreate(gameDto model, int? topicIndex, int? choiceIndex, int? index)
        {
            int tIndex = topicIndex ?? 0;
            ViewBag.Index = index;
            ViewBag.choiceIndex = choiceIndex;
            ViewBag.topicIndex = topicIndex;
            GamePostViewModel gpvm = new GamePostViewModel { game = model, vedio = new VedioRecord() };
            // choiceOddsViewModel cov = new choiceOddsViewModel();
            // cov.unitSn = model.topicList[topicIndex].choiceList[choiceIndex].betMoney[index].unitSn;
            //cov.Odds = model.topicList[topicIndex].choiceList[choiceIndex].betMoney[index].Odds;
            //ViewBag.choiceOddsViewModel = cov;
            for (int k = 0; k <= topicIndex; k++)
            {
                choiceDto c = model.topicList[tIndex].choiceList[k];
                //補上所有幣別，取回的資料只會有這個競猜可下注的幣別
                model.topicList[tIndex].choiceList[k].choiceString.Add(new choiceStrDto { choiceSn = c.sn, choiceStr = c.choiceStr });
            }


            return PartialView(gpvm);
        }



        [Authorize(Roles = "Admin")]
        public ActionResult _topicEdit(gameDto model, int index)
        {
            //ViewBag.Index = index;

            //index = index ?? 0;
            ViewBag.Index = index;
            GamePostViewModel gpvm = new GamePostViewModel { game = model, vedio = new VedioRecord() };

            return PartialView("_topicCreate", gpvm);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult _choiceEdit(gameDto model, int topicIndex, int index)
        {
            ViewBag.Index = index;
            ViewBag.topicIndex = topicIndex;          
            GamePostViewModel gpvm = new GamePostViewModel { game = model, vedio = new VedioRecord() };
            return View(gpvm);
        }
        // POST: game/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async System.Threading.Tasks.Task<ActionResult> Create(GamePostViewModel gpvm)
        {
            try
            {

                if (!string.IsNullOrEmpty(gpvm.vedio.vediourl))
                {
                    gpvm.vedio.title = (string.IsNullOrEmpty(gpvm.vedio.title) ? "" : gpvm.vedio.title);
                    gpvm.vedio.comment = (string.IsNullOrEmpty(gpvm.vedio.comment) ? "" : gpvm.vedio.comment);
                    gpvm.vedio.valid = 1;
                    gpvm.vedio.inpdate = DateTime.Now;
                    List<cfgVedio> cvList = new cfgVedioRepository().getAll();
                    foreach (cfgVedio cv in cvList)
                    {
                        //Regex defaultRegex = new Regex(cv.RegularStr);
                        //if(defaultRegex.Match(gpvm.vedio.vediourl).Success)
                        gpvm.vedio.cfgVedioSn = cv.sn;

                        /*
                        List<string> includestr = cv.includeURL.Split(',').ToList();
                        foreach (string str in includestr)
                        {
                            if (includestr.Contains(str))
                            {
                                gpvm.vedio.cfgVedioSn = cv.sn;
                            }
                        }
                        */
                    }
                    if (gpvm.vedio.cfgVedioSn != 0)
                    {
                        VedioRecordRepository vrr = new VedioRecordRepository();
                        vrr.add(gpvm.vedio);
                    }
                }

                var leaguejudge = (gpvm.gamepost.PlayGameSn == 1) ? "LOL未分類" : (gpvm.PlayGame.sn == 2) ? "CS:GO未分類" : (gpvm.PlayGame.sn == 4) ? "DOTA2未分類":"";

                if (gpvm.TeamList[0].name != null)
                {

                    gpvm.TeamList[0].leagueSn = new LeaguesRepository().getAll().Where(x => x.name == leaguejudge).FirstOrDefault().sn;
                    int teamsn = new TeamsRepository().AutoCreate(gpvm.TeamList[0]);
                    gpvm.gamepost.TeamASn = teamsn;

                }

                if (gpvm.TeamList[1].name != null)
                {
                    gpvm.TeamList[1].leagueSn = new LeaguesRepository().getAll().Where(x => x.name == leaguejudge).FirstOrDefault().sn;
                    int teamsn = new TeamsRepository().AutoCreate(gpvm.TeamList[1]);
                    gpvm.gamepost.TeamBSn = teamsn;
                }
                if(gpvm.walkm == 1)
                {
                    SettingListDto gameb = await new GamesRepository().GetBonusList();
                    gpvm.game.allocation = (int)gameb.gamesettingList.Where(x => x.sn == 2).FirstOrDefault().allocation;
                    gpvm.game.rake = 100 - (int)gameb.gamesettingList.Where(x => x.sn == 2).FirstOrDefault().outlay;
                    gpvm.game.topicList[0].walk = 1;
                }


                switch (gpvm.game.betModel)
                {
                    case 6://百倍大串燒才新增
                        gpvm.game.topicList = await nabobcreate(gpvm);
                        break;
                    case 5://龍的傳人才新增
                        gpvm.game.topicList = await dgoncreate(gpvm);
                        break;
                    
                }               
               


                gameDto game = gpvm.game;
                game.userId = User.Identity.GetUserId();
                game.comSn = 1;
                game = await new GamesRepository().Create(game);




                GamePosts gp = new GamePosts { GameSn = game.sn, valid = 1, inpdate = DateTime.Now, Synchronize=game.sn };
                if (gpvm.vedio != null && gpvm.vedio.sn != 0)
                {
                    gp.VedioRecordSn = gpvm.vedio.sn;
                }
                gp.TeamASn = gpvm.gamepost.TeamASn;
                gp.TeamBSn = gpvm.gamepost.TeamBSn;
                gp.sdate = gpvm.game.sdate;
                gp.edate = gpvm.game.edate;
                gp.PlayGameSn = gpvm.gamepost.PlayGameSn;
                new GamePostsRepository().add(gp);
                //推播
                WebPush push = new GamePostsRepository().getWebPush();
                if(push.webpush1 != 0)
                {
                    bear_mail(gp, Convert.ToDateTime(gp.sdate), " 競猜開始囉!! 趕快點擊前往預測 ");
                    if(game.betModel != 7)
                    bear_mail(gp, Convert.ToDateTime(gp.edate).AddMinutes(-30), " 競猜快要結束囉‼️ 趕快點擊預測 ");
                }
                //bear_mail(gp, Convert.ToDateTime(gp.sdate), " 競猜開始囉!! 趕快點擊前往預測 ");
            
               // bear_mail(gp,Convert.ToDateTime(gp.edate).AddMinutes(-30), " 競猜快要結束囉‼️ 趕快點擊預測 ");


                GamePosts gamepost = new GamePostsRepository().getgame(game.sn);
                if(game.betModel == 5)
                {
                    return RedirectToAction("DPDetailsAdmin", new { id = gamepost.sn });
                }
                
                return RedirectToAction("DetailsAdmin", new { id = gamepost.sn });

            }
            catch
            {
                return View(gpvm);
            }
        }



        //百倍大串燒才新增
        public async System.Threading.Tasks.Task<List<topicDto>> nabobcreate(GamePostViewModel gpvm) {            
                List<topicDto> topicsdto = new List<topicDto>();
                SettingListDto gameb = await new GamesRepository().Gettopic();
                List<Teams> teamlist = new TeamsRepository().getAll();
                List<choiceDto> choicelist = new List<choiceDto>();

                


                //加入題目
                var config = new MapperConfiguration(cfg => cfg.CreateMap<topicSettingDto, topicDto>()
                   .IgnoreAllNonExisting().
                   ForMember(up => up.sdate, opt => opt.MapFrom(u => gpvm.game.sdate)).
                   ForMember(up => up.edate, opt => opt.MapFrom(u => gpvm.game.edate)).
                   ForMember(up => up.valid, opt => opt.MapFrom(u => 1)).
                   ForMember(up => up.title, opt => opt.MapFrom(u => u.topicsName)).
                   ForMember(up => up.choiceList, opt => opt.MapFrom(u => choicelist))
                   );
                 config.AssertConfigurationIsValid();
                 var mapper = config.CreateMapper();
                 topicsdto = mapper.Map<List<topicDto>>(gameb.topicsettingList);

            //增加及分辨選項
            int ti = 0;
            foreach (var topic in gameb.topicsettingList)
            {
                
                if (topic.choiceType == false)
                {
                    var teamsA = teamlist.Where(p => p.sn == gpvm.gamepost.TeamASn).FirstOrDefault();
                    var teamsB = teamlist.Where(p => p.sn == gpvm.gamepost.TeamBSn).FirstOrDefault();
                    for (int i = 0; i < 2; i++)
                    {
                        choiceDto choice = new choiceDto();
                        if (i == 0)
                            choice.choiceStr = teamsA.name;
                        if (i == 1)
                            choice.choiceStr = teamsB.name;
                        choice.valid = 1;
                        topicsdto[ti].choiceList.Add(choice);
                    }

                }
                else
                {
                    foreach (var choiceall in topic.choicsettingList)
                    {
                        choiceDto choice = new choiceDto();
                        choice.choiceStr = choiceall.choiceName;
                        choice.valid = 1;
                        topicsdto[ti].choiceList.Add(choice);
                    }

                }
                ti++;

            }
            return topicsdto;

        }
        



        //龍的傳人才新增
        public async System.Threading.Tasks.Task<List<topicDto>> dgoncreate(GamePostViewModel gpvm)
        {
            List<topicDto> topicsdto = new List<topicDto>();
            List<choiceDto> choicelist = new List<choiceDto>();
            SettingListDto gameb = await new GamesRepository().GetBonusList();
            
            //新增獎金
            var config = new MapperConfiguration(
                cfg => cfg.CreateMap<
                    bonusSettingDto ,bonusDto>().ForMember(up => up.gamesn,opt =>opt.MapFrom(u => u.settingsn)));
            config.AssertConfigurationIsValid();
            var mapper = config.CreateMapper();
            List<bonusSettingDto> bonuDto = gameb.bonussettingList;
            gpvm.game.bonusList = mapper.Map<List<bonusDto>>(bonuDto);


            //新增龍的傳人的選項
            choiceDto choice = new choiceDto();
            List<choiceStrDto> csdtolist = new List<choiceStrDto>();
            choice.choiceStr = "0";
            choice.valid = 1;
            for (int j = 0; j < 6; j++)
            {
                choiceStrDto cstdto = new choiceStrDto();
                cstdto.choiceStr = j.ToString();
                csdtolist.Add(cstdto);
            }
            choice.choiceString = csdtolist;
            choicelist.Add(choice);
            //加入進topics
            config = new MapperConfiguration(cfg => cfg.CreateMap<gameSettingDto, topicDto>()
            .IgnoreAllNonExisting().
            ForMember(up => up.sdate, opt => opt.MapFrom(u => gpvm.game.sdate)).
            ForMember(up => up.edate, opt => opt.MapFrom(u => gpvm.game.edate)).
            ForMember(up => up.valid, opt => opt.MapFrom(u => 1)).
            ForMember(up => up.choiceList, opt => opt.MapFrom(u => choicelist))
            );
            config.AssertConfigurationIsValid();
            mapper = config.CreateMapper();
            List<gameSettingDto> gd = gameb.gamesettingList.Where(x => x.sn == 1).ToList();
            topicsdto = mapper.Map<List<topicDto>>(gd);
          
            return topicsdto;           


        }

        




        public void bear_mail(GamePosts gp,DateTime gsdate,string content) {
            //Send to all subscribers

            List<Teams> teamlist = new TeamsRepository().getAll();
            var teamsA = teamlist.Where(p => p.sn == gp.TeamASn).FirstOrDefault();
            var teamsB = teamlist.Where(p => p.sn == gp.TeamBSn).FirstOrDefault();

            string timemessenger = gsdate.ToString("yyyy-MM-dd HH:mm:ss");
            timemessenger += " GMT+0800";

            var request = WebRequest.Create("https://onesignal.com/api/v1/notifications") as HttpWebRequest;

            request.KeepAlive = true;
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";

            request.Headers.Add("authorization", "Basic YmJkNzJjYmMtNzcwNi00NDhjLWE5N2QtNWQxZWM2YzEwMGI4");
            var serializer = new JavaScriptSerializer();
            var obj = new
            {
                app_id = "d7213a53-69e2-4845-b4d3-b487528a2483",
                contents = new { en = teamsA.shortName + " Vs " + teamsB.shortName + content },
                headings = new { en = "熊i猜-熊報信" },
                included_segments = new string[] { "All" },
                chrome_web_icon = "https://img.onesignal.com/t/bdacf604-f528-4aec-8618-1531f0c4a49e.png",
                send_after = timemessenger,
                ttl = 1800
            };
            var param = serializer.Serialize(obj);
            byte[] byteArray = Encoding.UTF8.GetBytes(param);

            string responseContent = null;

            try
            {
                using (var writer = request.GetRequestStream())
                {
                    writer.Write(byteArray, 0, byteArray.Length);
                }

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseContent = reader.ReadToEnd();
                    }
                }
            }
            catch (WebException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());
            }

            System.Diagnostics.Debug.WriteLine(responseContent);
            //Send to all subscribers END



        }






        // GET: game/Edit/5
        [Authorize(Roles = "Admin")]
        public async System.Threading.Tasks.Task<ActionResult> Edit(int id)
        {
            GamePosts gamepost = new GamePostsRepository().get(id);
            gameDto game = null;
            string UserID = User.Identity.GetUserId();
            game = await new GamesRepository().GetGameDetail(gamepost.GameSn);
            //if (game.userId != UserID)
            //    return View("Details", game);

            GamePostViewModel gpvm = new GamePostViewModel(id, encryptedKey, game);



            return View(gpvm);
        }

        // GET: game/Edit/5
        [Authorize(Roles = "Admin")]
        public async System.Threading.Tasks.Task<ActionResult> DPEdit(int id)
        {
            GamePosts gamepost = new GamePostsRepository().get(id);
            gameDto game = null;
            string UserID = User.Identity.GetUserId();
            game = await new GamesRepository().GetGameDetail(gamepost.GameSn);
            //if (game.userId != UserID)
            //    return View("Details", game);

            GamePostViewModel gpvm = new GamePostViewModel(id, encryptedKey, game);



            return View(gpvm);
        }


        // POST: game/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> Edit(GamePostViewModel gpvm)
        {
            //try
            //{
                int iGamePostSn = gpvm.getGamePostSn(encryptedKey);
                GamePosts gp = new GamePostsRepository().get(iGamePostSn);

            if (!string.IsNullOrEmpty(gpvm.vedio.vediourl))
            {
                gpvm.vedio.title = (string.IsNullOrEmpty(gpvm.vedio.title) ? "" : gpvm.vedio.title);
                gpvm.vedio.comment = (string.IsNullOrEmpty(gpvm.vedio.comment) ? "" : gpvm.vedio.comment);
                gpvm.vedio.valid = 1;
                gpvm.vedio.inpdate = DateTime.Now;
                List<cfgVedio> cvList = new cfgVedioRepository().getAll();
                gpvm.vedio.cfgVedioSn = 0;
                foreach (cfgVedio cv in cvList)
                {
                    Regex defaultRegex = new Regex(cv.RegularStr);
                    if (defaultRegex.Match(gpvm.vedio.vediourl).Success)
                        gpvm.vedio.cfgVedioSn = cv.sn;

                    /*
                    List<string> includestr = cv.includeURL.Split(',').ToList();
                    foreach (string str in includestr)
                    {
                        if (includestr.Contains(str))
                        {
                            gpvm.vedio.cfgVedioSn = cv.sn;
                        }
                    }
                    */
                }
            }
            VedioRecordRepository vrr = new VedioRecordRepository();
            if (gp.VedioRecordSn.HasValue)
            {
                VedioRecord vr = new VedioRecordRepository().get(gp.VedioRecordSn.Value);
                vr.cfgVedioSn = gpvm.vedio.cfgVedioSn;
                vr.vediourl = gpvm.vedio.vediourl;
                vrr.update(vr);
                gpvm.vedio = vr;
            }
            else
            {
                if (!string.IsNullOrEmpty(gpvm.vedio.vediourl))
                {
                    vrr.add(gpvm.vedio);
                }
            }



                gameDto game = gpvm.game;
                game.userId = User.Identity.GetUserId();
                game.comSn = 1;
                game = await new GamesRepository().Edit(game.sn, game);


                if (gpvm.vedio != null && gpvm.vedio.sn != 0)
                {
                    gp.VedioRecordSn = gpvm.vedio.sn;
                }

                gp.TeamASn = gpvm.gamepost.TeamASn;
                gp.TeamBSn = gpvm.gamepost.TeamBSn;
                gp.sdate = gpvm.game.sdate;
                gp.edate = gpvm.game.edate;
                gp.PlayGameSn = gpvm.gamepost.PlayGameSn;
                new GamePostsRepository().update(gp);

                ///把新的topic推播到前端
                new SignalRHelper().UpdateTopic(game, encryptedKey, gpvm.md5GameSn);
                return RedirectToAction(game.betDetails, new { id = gpvm.gamepost.sn });
            //}
            //catch (Exception ex)
            //{
            //    return View(gpvm);
            //}
        }

        // GET: game/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: game/Delete/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
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

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async System.Threading.Tasks.Task<ActionResult> SetWinner(GamePostViewModel gp)
        {
            try
            {
                gameDto gd = null;
                GamesRepository _games = new GamesRepository();
                gd = await _games.GetGameDetail(gp.game.sn);


                if (1 == 1 || gd.userId == User.Identity.GetUserId())
                {
                    List<choiceDto> choice = new List<choiceDto>();
                    foreach (topicDto t in gp.game.topicList)
                        choice.AddRange(t.choiceList);
                    bool haveTrue = false;
                    bool allReturn = true;
                    foreach (choiceDto cho in choice)
                    {
                        if (cho.isTrue.HasValue && cho.isTrue == 1)
                            haveTrue = true;
                        if (cho.isTrue != 2)
                            allReturn = false;

                    }
                    if (haveTrue || allReturn)
                    {
                        SetWinnerReq swq = new SetWinnerReq();
                        swq.UserID = User.Identity.GetUserId();
                        swq.comSn = 1;
                        swq.choiceList = choice;
                        swq.gameSn = gp.game.sn;
                        bool issuccess = await _games.setWinner(swq);
                        if (!issuccess)
                            return Content("系統設定出錯");
                        new SignalRHelper().UpdateTopic(gp.game, encryptedKey, gp.game.md5GameSn);
                    }
                    else
                    {
                        return Content("未設定設定結果");
                    }
                }
                // Return the URI of the created resource.


                return RedirectToAction("DetailsAdmin", new { id = gp.gamepost.sn });
            }
            catch
            {
                return View();
            }
        }

       


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async System.Threading.Tasks.Task<ActionResult> gdSetWinner(GamePostViewModel gp)
        {
            try
            {
                gameDto gd = null;
                GamesRepository _games = new GamesRepository();
                gd = await _games.GetGameDetail(gp.game.sn);


                if (1 == 1 || gd.userId == User.Identity.GetUserId())
                {
                    List<choiceDto> choice = new List<choiceDto>();
                    foreach (topicDto t in gp.game.topicList)
                        choice.AddRange(t.choiceList);
                    SetWinnerReq swq = new SetWinnerReq();
                    swq.UserID = User.Identity.GetUserId();
                    swq.comSn = 1;
                    swq.choiceList = choice;
                    foreach(var c in swq.choiceList)
                    {
                        c.betModel = 5;
                    }
                    
                    swq.gameSn = gp.game.sn;                    
                    bool issuccess = await _games.setWinner(swq);
                }
                // Return the URI of the created resource.


                return RedirectToAction("DPDetailsAdmin", new { id = gp.gamepost.sn });
            }
            catch
            {
                return View();
            }
        }




        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async System.Threading.Tasks.Task<bool> StartBet(int id)
        {
            try
            {
                gameDto game = null;
                GamesRepository _games = new GamesRepository();
                game = await _games.GetGameDetail(id);
                if (1 == 1 || game.userId == User.Identity.GetUserId())
                {
                    StartBetReq sbr = new StartBetReq();
                    sbr.UserID = User.Identity.GetUserId();
                    sbr.comSn = 1;
                    sbr.gameSn = id;
                    bool bresult = await _games.StartBet(sbr);
                    game = await _games.GetGameDetail(sbr.gameSn);
                    new SignalRHelper().UpdateTopic(game, encryptedKey, game.md5GameSn);
                    return bresult;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async System.Threading.Tasks.Task<bool> reopen(int id)
        {
            try
            {
                gameDto game = null;
                GamesRepository _games = new GamesRepository();
                game = await _games.GetGameDetail(id);
                if (1 == 1 || game.userId == User.Identity.GetUserId())
                {
                    StartBetReq sbr = new StartBetReq();
                    sbr.UserID = User.Identity.GetUserId();
                    sbr.comSn = 1;
                    sbr.gameSn = id;
                    bool bresult = await _games.reopen(sbr);
                    game = await _games.GetGameDetail(sbr.gameSn);
                    new SignalRHelper().UpdateTopic(game, encryptedKey, game.md5GameSn);
                    return bresult;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async System.Threading.Tasks.Task<bool> setClose(int id)
        {
            try
            {
                gameDto game = null;
                GamesRepository _games = new GamesRepository();
                game = await _games.GetGameDetail(id);
                if (1 == 1 || game.userId == User.Identity.GetUserId())
                {
                    StartBetReq sbr = new StartBetReq();
                    sbr.UserID = User.Identity.GetUserId();
                    sbr.comSn = 1;
                    sbr.gameSn = id;
                    bool bresult = await _games.setClose(sbr);
                    game = await _games.GetGameDetail(sbr.gameSn);
                    new SignalRHelper().UpdateTopic(game, encryptedKey, game.md5GameSn);
                    return bresult;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async System.Threading.Tasks.Task<bool> deleteClose(int id)
        {
            try
            {
                gameDto game = null;
                GamesRepository _games = new GamesRepository();
                game = await _games.GetGameDetail(id);
                if (1 == 1 || game.userId == User.Identity.GetUserId())
                {
                    StartBetReq sbr = new StartBetReq();
                    sbr.UserID = User.Identity.GetUserId();
                    sbr.comSn = 1;
                    sbr.gameSn = id;
                    bool bresult = await _games.deleteClose(sbr);
                    game = await _games.GetGameDetail(sbr.gameSn);
                    new SignalRHelper().UpdateTopic(game, encryptedKey, game.md5GameSn);
                    return bresult;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 無派彩歸還
        /// </summary>
        /// <param name="id">遊戲id(GameSn)</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async System.Threading.Tasks.Task<string> Rollback(int id)
        {
            aJaxDto ajd = new aJaxDto();
            try
            {
                gameDto game = null;
                GamesRepository _games = new GamesRepository();
                game = await _games.GetGameDetail(id);
                if (1 == 1 || game.userId == User.Identity.GetUserId())
                {
                    List<payoutDto> payoutList = new List<payoutDto>();
                    StartBetReq sbr = new StartBetReq();
                    sbr.UserID = User.Identity.GetUserId();
                    sbr.comSn = 1;
                    sbr.gameSn = id;
                    ajd = await _games.paysRollback(sbr);
                    game = await _games.GetGameDetail(sbr.gameSn);
                    new SignalRHelper().UpdateTopic(game, encryptedKey, game.md5GameSn);
                }
                else
                {
                    ajd.isTrue = false;
                    ajd.ErrorCode = 100;
                }
            }
            catch
            {
                ajd.isTrue = false;
                ajd.ErrorCode = 500;
            }
            return JsonConvert.SerializeObject(ajd);

        }



        /// <summary>
        /// 遊戲派彩
        /// </summary>
        /// <param name="id">遊戲id(GameSn)</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async System.Threading.Tasks.Task<string> pays(int id)
        {
            aJaxDto ajd = new aJaxDto();
            try
            {
                gameDto game = null;
                GamesRepository _games = new GamesRepository();
                game = await _games.GetGameDetail(id);
                if (1 == 1 || game.userId == User.Identity.GetUserId())
                {
                    List<payoutDto> payoutList = new List<payoutDto>();
                    StartBetReq sbr = new StartBetReq();
                    sbr.UserID = User.Identity.GetUserId();
                    sbr.comSn = 1;
                    sbr.gameSn = id;
                    ajd = await _games.pays(sbr,game.betModel);
                    game = await _games.GetGameDetail(sbr.gameSn);
                    new SignalRHelper().UpdateTopic(game, encryptedKey, game.md5GameSn);
                }
                else
                {
                    ajd.isTrue = false;
                    ajd.ErrorCode = 100;
                }
            }
            catch
            {
                ajd.isTrue = false;
                ajd.ErrorCode = 500;
            }
            return JsonConvert.SerializeObject(ajd);

        }

        /// <summary>
        /// 遊戲派彩
        /// </summary>
        /// <param name="id">遊戲id(GameSn)</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async System.Threading.Tasks.Task<string> paysRollback(int id)
        {
            aJaxDto ajd = new aJaxDto();
            try
            {
                gameDto game = null;
                GamesRepository _games = new GamesRepository();
                game = await _games.GetGameDetail(id);
                if (1 == 1 || game.userId == User.Identity.GetUserId())
                {
                    List<payoutDto> payoutList = new List<payoutDto>();
                    StartBetReq sbr = new StartBetReq();
                    sbr.UserID = User.Identity.GetUserId();
                    sbr.comSn = 1;
                    sbr.gameSn = id;
                    ajd = await _games.paysRollback(sbr);
                    game = await _games.GetGameDetail(sbr.gameSn);
                    new SignalRHelper().UpdateTopic(game, encryptedKey, game.md5GameSn);
                }
                else
                {
                    ajd.isTrue = false;
                    ajd.ErrorCode = 100;
                }
            }
            catch
            {
                ajd.isTrue = false;
                ajd.ErrorCode = 500;
            }
            return JsonConvert.SerializeObject(ajd);

        }
     

        /// <summary>
        /// 重設答案
        /// </summary>
        /// <param name="id">遊戲id(GameSn)</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async System.Threading.Tasks.Task<bool> gamereturn(int id)
        {
            try
            {
                gameDto game = null;
                GamesRepository _games = new GamesRepository();
                game = await _games.GetGameDetail(id);
                if (1 == 1 || game.userId == User.Identity.GetUserId())
                {
                    StartBetReq sbr = new StartBetReq();
                    sbr.UserID = User.Identity.GetUserId();
                    sbr.comSn = 1;
                    sbr.gameSn = id;
                    bool bresult = await _games.gamereturn(sbr);
                    game = await _games.GetGameDetail(sbr.gameSn);
                    new SignalRHelper().UpdateTopic(game, encryptedKey, game.md5GameSn);
                    return bresult;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }



        /// <summary>
        /// 遊戲派彩
        /// </summary>
        /// <param name="id">遊戲id(GameSn)</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async System.Threading.Tasks.Task<string> stopTopic(int id)
        {
            aJaxDto ajd = new aJaxDto();
            try
            {
                gameDto game = null;
                GamesRepository _games = new GamesRepository();
                game = await _games.GetGameDetailByTopicSn(id);
                if (1 == 1 || game.userId == User.Identity.GetUserId())
                {
                    List<payoutDto> payoutList = new List<payoutDto>();
                    StopTopicReq sbr = new StopTopicReq();
                    sbr.UserID = User.Identity.GetUserId();
                    sbr.comSn = 1;
                    sbr.topicSn = id;
                    ajd = await _games.stopTopic(sbr);
                    game = await _games.GetGameDetail(game.sn);
                    new SignalRHelper().UpdateTopic(game, encryptedKey, game.md5GameSn);
                }
                else
                {
                    ajd.isTrue = false;
                    ajd.ErrorCode = 100;
                }
            }
            catch
            {
                ajd.isTrue = false;
                ajd.ErrorCode = 500;
            }
            return JsonConvert.SerializeObject(ajd);

        }


        /// <summary>
        /// 遊戲派彩
        /// </summary>
        /// <param name="id">遊戲id(GameSn)</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async System.Threading.Tasks.Task<string> reopenTopic(int id)
        {
            aJaxDto ajd = new aJaxDto();
            try
            {
                gameDto game = null;
                GamesRepository _games = new GamesRepository();
                game = await _games.GetGameDetailByTopicSn(id);
                if (1 == 1 || game.userId == User.Identity.GetUserId())
                {
                    List<payoutDto> payoutList = new List<payoutDto>();
                    StopTopicReq sbr = new StopTopicReq();
                    sbr.UserID = User.Identity.GetUserId();
                    sbr.comSn = 1;
                    sbr.topicSn = id;
                    ajd = await _games.reopenTopic(sbr);
                    game = await _games.GetGameDetail(game.sn);
                    new SignalRHelper().UpdateTopic(game, encryptedKey, game.md5GameSn);
                }
                else
                {
                    ajd.isTrue = false;
                    ajd.ErrorCode = 100;
                }
            }
            catch
            {
                ajd.isTrue = false;
                ajd.ErrorCode = 500;
            }
            return JsonConvert.SerializeObject(ajd);

        }

        /// 熊i猜&& GTI Details
        //[AllowCrossSiteJson]
        [AllowAnonymous]
        public async System.Threading.Tasks.Task<JsonResult> GetGameJson(int id)
        {
            GamePosts gamepost = new GamePostsRepository().get(id);
            gameDto game = null;
            //string UserID = User.Identity.GetUserId();
            game = await new GamesRepository().GetGameDetail(gamepost.GameSn);
            game.topicList = game.topicList.Where(x => x.walk == null).ToList();
            //if (game.userId != UserID)
            //    return View("Details", game);

            GamePostViewModel gpvm = new GamePostViewModel(id, encryptedKey, game);
            foreach(var gp in gpvm.game.topicList)
            {
               foreach(var chgp in gp.choiceList)
                {
                    /*foreach(var gpmo in chgp.betMoney)
                    {
                        gpmo.betMoney = chgp.betMoneygti;
                        gpmo.Odds = chgp.Odds.Value;
                        gpmo.choiceSn = chgp.sn;
                        gpmo.unitSn = gp.unitSn.Value;

                    }*/
                }


            }



            return Json(gpvm, JsonRequestBehavior.AllowGet);
        }

        /// 熊i猜&& GTI List
        [AllowAnonymous]
        public async System.Threading.Tasks.Task<JsonResult> GetListJson()
        {
            GamePostsRepository _gpr = new GamePostsRepository();

            List<GamePosts> gpList = _gpr.getAll();
            List<gameDto> gameList = await new GamesRepository().GetGameList();
            

            List<VedioRecord> vrlist = new List<VedioRecord>();
            vrlist = new VedioRecordRepository().getAll();
            List<Teams> teamlist = new TeamsRepository().getAll();


            List<GamePostViewModel> gpvmList = new List<GamePostViewModel>();
            gpList = gpList.OrderByDescending(p => p.inpdate).ToList();
            foreach (var gp in gpList)
            {
                GamePostViewModel gpvm = new GamePostViewModel();
                gpvm.gamepost = gp;
                if (gameList.Where(p => p.sn == gp.GameSn).Count() > 0)
                {
                    gpvm.game = gameList.Where(p => p.sn == gp.GameSn).FirstOrDefault();
                }
                else
                    continue;
                gpvm.vedio = vrlist.Where(p => p.sn == gp.VedioRecordSn).FirstOrDefault();
                gpvm.TeamA = teamlist.Where(p => p.sn == gp.TeamASn).FirstOrDefault();
                gpvm.TeamB = teamlist.Where(p => p.sn == gp.TeamBSn).FirstOrDefault();
                gpvm.endguess = 1;
                if (gpvm.game.Status != "已派彩" && gpvm.game.Status != "" && gpvm.game.gamedate > DateTime.Now)
                {
                    gpvmList.Add(gpvm);
                }

            }

           // ViewData["UserID"] = User.Identity.GetUserId();

            return Json(gpvmList, JsonRequestBehavior.AllowGet);
        }

        /// 熊i猜&& GTI Pay
        [AllowAnonymous]
        public async System.Threading.Tasks.Task<JsonResult> GetPayJson(int id)
        {

            string UserID = User.Identity.GetUserId();
            GamePosts gamepost = new GamePostsRepository().get(id);
            gameDto game = null;
            game = await new GamesRepository().GetGameDetail(gamepost.GameSn);
            game.topicList = game.topicList.Where(x => x.walk == null).ToList();

            GamePostViewModel gpvm = new GamePostViewModel(id, encryptedKey, game);
            gpvm.gamepost = gamepost;
            if(gpvm.gamepost.Synchronize == null)
            {
                gpvm.gamepost.Synchronize = gpvm.game.sn;
            }
            

            game = await new GamesRepository().GetGameDetail(gamepost.GameSn);
            //if (game.userId != UserID)
            //    return View("Details", gpvm);
            


            ViewData["BetList"] = await new BetsRepository().GetBetsByGame(gamepost.GameSn);
            // ViewData["JsonList"] = test;

            return Json(gpvm, JsonRequestBehavior.AllowGet);
        }

        /// 熊i猜&& GTI Post
        [AllowAnonymous]
        [HttpPost]
        public void PostVisited(int id)
        {
         
            GamePosts gamepost = new GamePostsRepository().getgame(id);
            gamepost.Visited = 1;
            new GamePostsRepository().update(gamepost);
        }





    }

    
}

