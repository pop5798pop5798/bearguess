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
using SITW.Models.ViewModel;
using SITW.Models;
using SITDto;
using System.Runtime.Caching;
using System.Text.RegularExpressions;
using SITW.Helper;
using SITDto.Request;
using Newtonsoft.Json;
using Microsoft.AspNet.Identity.Owin;
using SITDto.ViewModel;
using GoogleCloudSamples.Services;
using System.Threading.Tasks;
using SITW.Services;

namespace SITW.Controllers
{
    [Authorize]
    public class LiveController : Controller
    {
        string encryptedKey;
        private static MemoryCache _cache = MemoryCache.Default;
        List<gameDto> gList;
        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;
        private ImageUploader _imageUploader;

        public LiveController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
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
        public LiveController()
        {
            encryptedKey = System.Web.Configuration.WebConfigurationManager.AppSettings["encryptedKey"];
            if (_cache.Contains("GameList"))
                gList = _cache.Get("GameList") as List<gameDto>;
            //client = new HttpClient();
            //client.BaseAddress = new Uri(System.Web.Configuration.WebConfigurationManager.AppSettings["apiurl"]);
            //client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        [AllowAnonymous]
        public async System.Threading.Tasks.Task<ActionResult> _Live()
        {
            string[,] gp = new string[30,4] {
                 { "(官方頻道分享) T1", "https://www.twitch.tv/team/sktt1", "sktt1","英雄聯盟" },
                  { "(官方頻道分享) Riot", "https://www.twitch.tv/riotgames", "riotgames","英雄聯盟" },
                  { "(官方頻道分享) Garena", "https://www.twitch.tv/garenatw", "garenatw","英雄聯盟" },
                  { "(官方頻道分享) LCK", "https://www.twitch.tv/lck_korea", "lck_korea","英雄聯盟" },
                  { "(官方頻道分享) Ahq", "https://www.twitch.tv/ahqesportsclub/", "ahqesportsclub","英雄聯盟" },
                   { "(官方頻道分享) Team Liquid", "https://www.twitch.tv/team/teamliquid", "teamliquid","英雄聯盟" },
                { "(頻道分享) T1 Faker", "https://www.twitch.tv/faker/", "faker","英雄聯盟" },
                { "(頻道分享) T1 Effort", "https://www.twitch.tv/effort", "effort","英雄聯盟" },
                { "(頻道分享) T1 Khan", "https://www.twitch.tv/lol_khan", "lol_khan","英雄聯盟" },
                { "(頻道分享) T1 Clid", "https://www.twitch.tv/clid", "clid","英雄聯盟" },
                { "(頻道分享) T1 kkOma", "https://www.twitch.tv/kkoma", "kkoma","英雄聯盟" },
                { "(頻道分享) T1 Haru", "https://www.twitch.tv/t1_haru/", "t1_harua","英雄聯盟" },
                { "(頻道分享) T1 Teddy", "https://www.twitch.tv/t1_teddy", "t1_teddy","英雄聯盟" },
                { "(頻道分享) T1 Leo", "https://www.twitch.tv/t1_leo", "t1_leo","英雄聯盟" },
                { "(頻道分享) T1 Mata", "https://www.twitch.tv/mata", "mata","英雄聯盟" },
                { "(頻道分享) AHQ Ziv", "https://www.twitch.tv/ziv_lol", "ziv_lol","英雄聯盟" },
                 { "(頻道分享) JT FOFO", "https://www.twitch.tv/fofo0108", "fofo0108","英雄聯盟" },
                 { "(頻道分享) TL Doublelift", "https://www.twitch.tv/doublelift/", "doublelift","英雄聯盟" },
                 { "(頻道分享) TL CoreJJ", "https://www.twitch.tv/corejj", "corejj","英雄聯盟" },
                 { "(頻道分享) TL Jensen", "https://www.twitch.tv/jensen/", "jensen","英雄聯盟" },
                 { "(頻道分享) TL Impact", "https://www.twitch.tv/tlimpact/", "tlimpact","英雄聯盟" },
                 { "(頻道分享) FNC Rekkles", "https://www.twitch.tv/rekkles", "rekkles","英雄聯盟" },
                 { "(頻道分享) FNC Broxah", "https://www.twitch.tv/broxah", "broxah","英雄聯盟" },
                 { "(頻道分享) G2 Jankos", "https://www.twitch.tv/jankos/", "jankos","英雄聯盟" },
                 { "(頻道分享) G2 Caps", "https://www.twitch.tv/caps/", "caps","英雄聯盟" },
                 { "(頻道分享) G2 Wunder", "https://www.twitch.tv/wunderlol", "wunderlol","英雄聯盟" },
                  { "(頻道分享) CG Huni", "https://www.twitch.tv/huni", "huni","英雄聯盟" },
                  { "(頻道分享) C9 Sneaky", "https://www.twitch.tv/c9sneaky", "c9sneaky","英雄聯盟" },
                  { "(頻道分享) SUP Wolf", "https://www.twitch.tv/lol_woolf/", "lol_woolf","英雄聯盟" },
                  { "(頻道分享) MaRin", "https://www.twitch.tv/marin000", "marin000","英雄聯盟" },


            };
            List<GamePostViewModel> gpvmList = new List<GamePostViewModel>();
            for(int i = 0;i < 30;i++)
            {
                GamePostViewModel gpvm = new GamePostViewModel();
                //gpvm.vedio = vrlist.Where(p => p.sn == gp.VedioRecordSn).FirstOrDefault();
                gpvm.shorttitle = gp[i, 0];
                gpvm.PlayGame.shortName = gp[i, 3];
                gpvm.vedio_url = "https://player.twitch.tv/?channel=" + gp[i, 2];
                gpvm.href = gp[i, 1];
                gpvmList.Add(gpvm);
            }




            return View(gpvmList);
        }


        [AllowAnonymous]
        public async System.Threading.Tasks.Task<ActionResult> Index()
        {
            GamePostsRepository _gpr = new GamePostsRepository();

            List<GamePosts> gpList = _gpr.getLiveValidAll();

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

                    var user =  await UserManager.FindByIdAsync(gpvm.gamepost.UserLive);
                    if(user != null)
                    gpvm.gamepost.UserLive = user.UserName;
                    gpvm.vedio = vrlist.Where(p => p.sn == gp.VedioRecordSn).FirstOrDefault();
                    gpvm.shorttitle = gpvm.game.title;

                    if (gpvm.vedio == null)
                    {
                        gpvm.vedio_url = "";
                    }
                    else
                    {

                        string movieclass_twitch = "";
                        string movieclass_youtube = "";
                        string movieclass_huya = "";
                        string movieclass_be = "";
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
                        if (gpvm.vedio.vediourl.Length >= 17)
                        {
                            movieclass_be = gpvm.vedio.vediourl.Substring(0, 17);
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
                        else if (movieclass_huya == "https://youtu.be/")
                        {
                            gpvm.vedio_url = Regex.Replace(gpvm.vedio.vediourl, movieclass_huya, String.Empty);
                            gpvm.vedio_url = "https://www.youtube.com/embed/" + movieclass_be;
                        }
                        else
                        {
                            gpvm.vedio_url = "";
                        }
                        


                    }

                    

                    if(gpvm.game.title.Length > 23)
                    {
                        gpvm.shorttitle = gpvm.game.title.Substring(0, 23);
                        gpvm.shorttitle = gpvm.shorttitle + "...";
                    }


                    gpvm.TeamA = teamlist.Where(p => p.sn == gp.TeamASn).FirstOrDefault();
                    gpvm.TeamB = teamlist.Where(p => p.sn == gp.TeamBSn).FirstOrDefault();
                    gpvm.gamesearch = gpvm.game.title + gpvm.game.comment + gpvm.gamepost.UserLive + gpvm.PlayGame.shortName;
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



                    if(gpvm.game.topicList.Where(x=>x.valid == 1).Count() >0)
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
        public ActionResult LiveApply()
        {

            if (string.IsNullOrEmpty(User.Identity.GetUserId()))
                return View();
            else
                return RedirectToAction("Index", "Home");
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LiveApply(ApplyLiveViewModel model)
        {

            if (ModelState.IsValid && new reCAPTCHAHelper().Validate(Request["g-recaptcha-response"]))
            {

                    //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // 傳送包含此連結的電子郵件
                    //string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    //await UserManager.SendEmailAsync(user.Id, "熊i猜Email驗證信", "請按此連結確認您的帳戶 <a href=\"" + callbackUrl + "\">驗證</a>");
                    var apply = new LiveApply
                    {
                        name = model.Name,
                        Email = model.Email,
                        livecount = model.livecount,
                        comment = model.comment,
                        valid = 0,
                        lievlink = model.lievlink,
                        inpdate = DateTime.Now,
                        createComment = model.createComment


                    };

                    new UserRepository().applylive(apply);


                var content = "<h2 style='text-align: center;'>申請通知信</h2>" +
                "<div style='text-align: center;padding:0 14%;'>" +
                        "<h4 style='color:#222'>親愛的直播主您好：<p>" +
                        "我們已經收到您的申請<p>" +
                       "<h3 style='color:#500050'>熊i猜會在5~7個工作日審核您的資格，審核通過將為您寄送相關文件及合約書至您的信箱，還請您耐心等候，如有任何問題，可至<a href='https://www.facebook.com/funbet.esport/'>熊i猜粉絲團</a>發訊詢問喔!<br>" +
                        "(本通知為自動發送，請勿直接回信詢問，以避免延誤問題解決時間。)</h3><br>";
                    var EmailContent = EmailTemplatesService.GetLiveEmailHTML(content);
                    //寄送通知信
                    new MailServiceMailgun().ChangeSend("競猜直播主申請通知信", EmailContent, model.Email, "");





                    return RedirectToAction("Index", "Home", new { message = "申請成功，已寄送通知信至你的信箱" });
             }

            // 如果執行到這裡，發生某項失敗，則重新顯示表單
            return View(model);
        }


        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            ControllerContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            aJaxDto ajd = new aJaxDto();
            ajd.isTrue = false;
            if (!ModelState.IsValid)
            {               
                return Json(ajd, JsonRequestBehavior.AllowGet);
            }
            var result = await UserManager.ChangePasswordAsync(model.userId, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(model.userId);
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                ajd.isTrue = true;
                return Json(ajd, JsonRequestBehavior.AllowGet);
            }
            //AddErrors(result);
            return Json(ajd, JsonRequestBehavior.AllowGet);
        }







        [AllowAnonymous]
        [HttpPost]
        public async System.Threading.Tasks.Task<JsonResult> ReLiveData(GamePostViewModel gpvm)
        {
            ControllerContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            gpvm.game = await new GamesRepository().reLiveGameListAsync(gpvm.gamepost.GameSn);


            return Json(gpvm, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        [AllowAnonymous]      
        public async System.Threading.Tasks.Task<JsonResult> LiveData(int id)
        {
            ControllerContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            GamePosts gamepost = new GamePostsRepository().get(id);
            gameDto game = null;
            game = await new GamesRepository().reLiveGameListAsync(gamepost.GameSn);

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
                }else if(gpvm.vedio.cfgVedioSn == 5)
                {
                    gpvm.vedio_url = gpvm.vedio_url;
                }
                else
                {
                    gpvm.vedio_url = "";
                }


            }

            gpvm.recommend = new UserRepository().getRecommendValid(gpvm.game.userId);
            gpvm.recommendstr = new UserRepository().getRecommendStartV((int)gpvm.recommend.ReId);
            var live = new UserRepository().getlive();
            gpvm.livevalid = (int)live.Where(x => x.username == gpvm.game.userId).FirstOrDefault().valid;

            return Json(gpvm, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpPost]
        public async System.Threading.Tasks.Task<JsonResult> LivePayData(int id)
        {
            ControllerContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            GamePosts gamepost = new GamePostsRepository().get(id);
            gameDto game = null;
            game = await new GamesRepository().GetGamePayDetail(gamepost.GameSn);

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
                else if (gpvm.vedio.cfgVedioSn == 5)
                {
                    gpvm.vedio_url = gpvm.vedio_url;
                }
                else
                {
                    gpvm.vedio_url = "";
                }


            }

            gpvm.recommend = new UserRepository().getRecommendValid(gpvm.game.userId);
            gpvm.recommendstr = new UserRepository().getRecommendStartV((int)gpvm.recommend.ReId);

            return Json(gpvm, JsonRequestBehavior.AllowGet);
        }


        [AllowAnonymous]
        [HttpPost]
        public async System.Threading.Tasks.Task<JsonResult> SetWinnerPay(GamePostViewModel gp,int topicSn)
        {
            ControllerContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            aJaxDto ajd = new aJaxDto();
            try
            {
                gameDto gd = null;
                GamesRepository _games = new GamesRepository();
                
                gameDto game = null;
                gd = await _games.GetGameDetail(gp.game.sn);
                gp.gamepost.edate = gd.edate;
                gp.gamepost.sdate = DateTime.Now;
                gp.gamepost.inpdate = DateTime.Now;
                gp.game.sdate = DateTime.Now;
                gp.game.edate = gd.gamedate;
                gp.game.gamedate = gd.gamedate;

                for (int i = 0; i < gd.topicList.Count; i++)
                {
                    gp.game.topicList[i].sdate = gd.topicList[i].sdate;
                    gp.game.topicList[i].edate = gd.topicList[i].edate;
                }




                    List<choiceDto> choice = new List<choiceDto>();
                    foreach (topicDto t in gp.game.topicList)
                        choice.AddRange(t.choiceList);
                    /*bool haveTrue = false;
                    bool allReturn = true;
                    foreach (choiceDto cho in choice)
                    {
                        if (cho.isTrue.HasValue && cho.isTrue == 1)
                            haveTrue = true;
                        if (cho.isTrue != 2)
                            allReturn = false;

                    }*/
                    
                        SetWinnerReq swq = new SetWinnerReq();
                        swq.UserID = gp.game.userId;
                        swq.comSn = 1;
                        swq.choiceList = choice;
                        swq.gameSn = gp.game.sn;
                        swq.topicSn = topicSn;
                        //ajd = await _games.pays(sbr, game.betModel);
                        ajd = await _games.setWinnerPay(swq);

                        new SignalRHelper().UpdateTopic(gp.game, encryptedKey, gp.game.md5GameSn);
                  
                // Return the URI of the created resource.


              
            }
            catch
            {
                ajd.isTrue = false;
                ajd.ErrorCode = 500;
            }

            return Json(ajd, JsonRequestBehavior.AllowGet);
        }




        // GET: game/Edit/5
        [AllowAnonymous]
        [HttpPost]
        public async System.Threading.Tasks.Task<JsonResult> GetEdit(int id)
        {
            ControllerContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            GamePosts gamepost = new GamePostsRepository().get(id);
            gameDto game = null;
            string UserID = User.Identity.GetUserId();
            game = await new GamesRepository().reLiveGameListAsync(gamepost.GameSn);
            //if (game.userId != UserID)
            //    return View("Details", game);

            GamePostViewModel gpvm = new GamePostViewModel(id, encryptedKey, game);



            return Json(gpvm, JsonRequestBehavior.AllowGet);
        }


        [AllowAnonymous]
        [HttpPost]
        public async System.Threading.Tasks.Task<JsonResult> Edit(GamePostViewModel gpvm)
        {
            //try
            //{
            ControllerContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            int iGamePostSn = gpvm.getGamePostSn(encryptedKey);
            GamePosts gp = new GamePostsRepository().get(iGamePostSn);
            var vedio = new VedioRecord();
            if(gpvm.vedio == null)
            {
                gpvm.vedio = vedio;
                
            }

            if (gpvm.livename == "1")
            {

                gpvm.vedio.vediourl = "https://www.twitch.tv/" + gpvm.live;
                gpvm.vedio.cfgVedioSn = 4;

            }else if (gpvm.livename == "2")
            {
                gpvm.vedio.vediourl = gpvm.live;
                gpvm.vedio.cfgVedioSn = 5;
            }
            else {

                gpvm.vedio = vedio;
            }
            if (gpvm.vedio != null)
            {
                gpvm.vedio.title = (string.IsNullOrEmpty(gpvm.vedio.title) ? "" : gpvm.vedio.title);
                gpvm.vedio.comment = (string.IsNullOrEmpty(gpvm.vedio.comment) ? "" : gpvm.vedio.comment);
                gpvm.vedio.valid = 1;
                gpvm.vedio.inpdate = DateTime.Now;

                
            }
            VedioRecordRepository vrr = new VedioRecordRepository();
            if (gp.VedioRecordSn.HasValue)
            {
                VedioRecord vr = new VedioRecordRepository().get(gp.VedioRecordSn.Value);
                vr.cfgVedioSn = gpvm.vedio.cfgVedioSn;
                vr.vediourl = gpvm.vedio.vediourl;
                vr.live = gpvm.live;
                vrr.update(vr);
                gpvm.vedio = vr;
            }
            else
            {
                if (gpvm.vedio != null)
                {
                    vrr.add(gpvm.vedio);
                }
            }

           


            gameDto game = gpvm.game;
            var regame = await new GamesRepository().reLiveGameListAsync(gpvm.gamepost.GameSn);
            for (int i = 0; i < regame.topicList.Count; i++)
            {
                game.topicList[i].sdate = regame.topicList[i].sdate;
                game.topicList[i].edate = regame.topicList[i].edate;
            }
            game.comSn = 1;
            game.sdate = DateTime.Now;
            game.edate = DateTime.Now.AddYears(100);
            game.gamedate = DateTime.Now.AddYears(100);
            game = await new GamesRepository().EditLive(game.sn, game);


            if (gpvm.vedio != null && gpvm.vedio.sn != 0)
            {
                gp.VedioRecordSn = gpvm.vedio.sn;
            }

            gp.TeamASn = gpvm.gamepost.TeamASn;
            gp.TeamBSn = gpvm.gamepost.TeamBSn;
            gp.sdate = DateTime.Now;
            gp.edate = DateTime.Now.AddYears(100);
            var cfpgame = new cfgPlayGameRepository().getAllLive();
            bool cf = false;
            foreach (var cg in cfpgame)
            {
                if (cg.shortName == gpvm.PlayGame.shortName)
                {
                    cf = true;
                    gp.PlayGameSn = cg.sn;
                }

            }
         
            if (!cf)
            {
                cfgPlayGame cfpg = new cfgPlayGame();

                cfpg.shortName = gpvm.PlayGame.shortName;
                cfpg.cName = gpvm.PlayGame.shortName;
                cfpg.eName = gpvm.PlayGame.shortName;
                cfpg.valid = 2;
                gp.PlayGameSn = new cfgPlayGameRepository().Create(cfpg);


            }

            new GamePostsRepository().update(gp);
            gpvm.gamepost = gp;
            gpvm.game = game;

            ///把新的topic推播到前端
            new SignalRHelper().UpdateTopic(game, encryptedKey, gpvm.md5GameSn);
            return Json(gpvm, JsonRequestBehavior.AllowGet);
           // return RedirectToAction(game.betDetails, new { id = gpvm.gamepost.sn });
            //}
            //catch (Exception ex)
            //{
            //    return View(gpvm);
            //}


        }

        [AllowAnonymous]
        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> EditTopic(GamePostViewModel gpvm)
        {
            ControllerContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
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
                gpvm.vedio.cfgVedioSn = 4;
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

            var regame = await new GamesRepository().reLiveGameListAsync(gpvm.gamepost.GameSn);
            for(int i = 0; i< regame.topicList.Count;i++)
            {
                game.topicList[i].sdate = regame.topicList[i].sdate;
                game.topicList[i].edate = regame.topicList[i].edate;
            }
            game.comSn = 1;
            game.sdate = DateTime.Now;
            game.edate = DateTime.Now.AddYears(100);
            game.gamedate = DateTime.Now.AddYears(100);
            game.gameStatus = 0;
            game = await new GamesRepository().EditLive(game.sn, game);


            if (gpvm.vedio != null && gpvm.vedio.sn != 0)
            {
                gp.VedioRecordSn = gpvm.vedio.sn;
            }

            gp.TeamASn = gpvm.gamepost.TeamASn;
            gp.TeamBSn = gpvm.gamepost.TeamBSn;
            gp.sdate = DateTime.Now;
            gp.edate = DateTime.Now.AddYears(100);
            gp.PlayGameSn = gpvm.gamepost.PlayGameSn;
            new GamePostsRepository().update(gp);

            gpvm.game = game;

            ///把新的topic推播到前端
            new SignalRHelper().UpdateTopic(game, encryptedKey, gpvm.md5GameSn);
            return Json(gpvm, JsonRequestBehavior.AllowGet);
            //}
            //catch (Exception ex)
            //{
            //    return View(gpvm);
            //}
        }


        [AllowAnonymous]
        [HttpPost]
        public JsonResult _topicCreate(gameDto model, int? index)
        {
            ControllerContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            index = index ?? 0;
            //ViewBag.Index = index;
            //ViewBag.walk = walk;
            //ViewBag.choice = choice;
            if(model.topicList == null)
                model.topicList = new List<topicDto>();
            for(int i=0;i<=index;i++)
            {
                model.topicList.Add(new topicDto { valid = 1, main = (index == 0), choiceList = new List<choiceDto>() });
            }
            ///GamePostViewModel gpvmm = new GamePostViewModel { game = model };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult _choiceCreate(gameDto model, int? topicIndex, int? index)
        {
            ControllerContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            index = index ?? 0;
            int tIndex = topicIndex ?? 0;

            if (model.topicList == null)
                model.topicList = new List<topicDto>();

            var c = new List<choiceDto>();
            for (int j = 0; j <= index; j++)
                c.Add(new choiceDto() { valid = 1, choiceString = new List<choiceStrDto>() });
            //model.topicList[tIndex].choiceList.Add(new choiceDto() { valid = 1, choiceString = new List<choiceStrDto>() });

            model.topicList[tIndex].choiceList = c;
            /*for (int i = 0; i <= topicIndex; i++)
            {
               
            }*/




            // GamePostViewModel gpvm = new GamePostViewModel { game = model };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpPost]
        public async System.Threading.Tasks.Task<JsonResult> ReportsAsync(string user)
        {
            ControllerContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            List<BetListDto> BetList = await new BetsRepository().BetsLiveByUserID(user);
            return Json(BetList, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        [HttpPost]
        public async System.Threading.Tasks.Task<string> AddBugCreate(ProblemViewModel pbvm)
        {
            ControllerContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            try
            {
                if (string.IsNullOrEmpty(pbvm.image_1) && pbvm.image1 != null)
                {
                    string filename = "";
                    filename = DateTime.Now.ToString("yyyyMMddTHHmmssfff");
                    Google.Apis.Auth.OAuth2.GoogleCredential credential = await Google.Apis.Auth.OAuth2.GoogleCredential.GetApplicationDefaultAsync();
                    _imageUploader = new ImageUploader(System.Web.Configuration.WebConfigurationManager.AppSettings["GoogleCloud:BucketName"]);
                    var imageUrl = await _imageUploader.UploadImage(pbvm.image1, filename, "Problem");
                    pbvm.image_1 = imageUrl;
                }

                if (string.IsNullOrEmpty(pbvm.image_2) && pbvm.image2 != null)
                {
                    string filename = "";
                    filename = DateTime.Now.ToString("yyyyMMddTHHmmssfff");
                    Google.Apis.Auth.OAuth2.GoogleCredential credential = await Google.Apis.Auth.OAuth2.GoogleCredential.GetApplicationDefaultAsync();
                    _imageUploader = new ImageUploader(System.Web.Configuration.WebConfigurationManager.AppSettings["GoogleCloud:BucketName"]);
                    var imageUrl = await _imageUploader.UploadImage(pbvm.image2, filename, "Problem");
                    pbvm.image_2 = imageUrl;
                }

                if (string.IsNullOrEmpty(pbvm.image_3) && pbvm.image3 != null)
                {
                    string filename = "";
                    filename = DateTime.Now.ToString("yyyyMMddTHHmmssfff");
                    Google.Apis.Auth.OAuth2.GoogleCredential credential = await Google.Apis.Auth.OAuth2.GoogleCredential.GetApplicationDefaultAsync();
                    _imageUploader = new ImageUploader(System.Web.Configuration.WebConfigurationManager.AppSettings["GoogleCloud:BucketName"]);
                    var imageUrl = await _imageUploader.UploadImage(pbvm.image3, filename, "Problem");
                    pbvm.image_3 = imageUrl;
                }


                pbvm.valid = 1;
                pbvm.inpdate = DateTime.Now;

                new UserRepository().addbug(pbvm);
                // TODO: Add insert logic here

                return "1";
            }
            catch
            {
                return "";
            }
            return "";
        }




        [AllowAnonymous]
        [HttpPost]
        public async System.Threading.Tasks.Task<string> Create(GamePostViewModel gpvm)
        {
            ControllerContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            try
            {
                var vedio = new VedioRecord();
                if (gpvm.vedio == null)
                {
                    gpvm.vedio = vedio;

                }


                if (gpvm.livename == "1")
                {
                    gpvm.vedio.vediourl = "https://www.twitch.tv/" + gpvm.live;
                    gpvm.vedio.cfgVedioSn = 4;

                }else if (gpvm.livename == "2")
                {
                    gpvm.vedio.vediourl =  gpvm.live;
                    gpvm.vedio.cfgVedioSn = 5;

                }
                else
                {
                    gpvm.vedio.cfgVedioSn = 0;


                }

               // gpvm.vedio = vedio;
                gpvm.vedio.title = (string.IsNullOrEmpty(gpvm.vedio.title) ? "" : gpvm.vedio.title);
                gpvm.vedio.comment = (string.IsNullOrEmpty(gpvm.vedio.comment) ? "" : gpvm.vedio.comment);
                gpvm.vedio.vediourl = (string.IsNullOrEmpty(gpvm.vedio.vediourl) ? "" : gpvm.vedio.vediourl);
                gpvm.vedio.valid = 1;
                gpvm.vedio.inpdate = DateTime.Now;
                gpvm.vedio.live = gpvm.live;
                List<cfgVedio> cvList = new cfgVedioRepository().getAll();


                VedioRecordRepository vrr = new VedioRecordRepository();
                vrr.add(gpvm.vedio);




                gameDto game = gpvm.game;
                //game.userId = User.Identity.GetUserId();
                game.comSn = 1;
                game.sdate = DateTime.Now;
                game.edate = DateTime.Now.AddYears(100);
                game.gamedate = DateTime.Now.AddYears(100);
                game.betModel = 2;
                game.rake = 10;
                game = await new GamesRepository().CreateLive(game);

                GamePosts gp = new GamePosts { GameSn = game.sn, valid = 1, inpdate = DateTime.Now, Synchronize = game.sn };
                if (gpvm.vedio != null && gpvm.vedio.sn != 0)
                {
                    gp.VedioRecordSn = gpvm.vedio.sn;
                }
                gp.TeamASn = gpvm.gamepost.TeamASn;
                gp.TeamBSn = gpvm.gamepost.TeamBSn;
                gp.sdate = gpvm.game.sdate;
                gp.edate = gpvm.game.edate;
                gp.PlayGameSn = gpvm.gamepost.PlayGameSn;
                gp.UserLive = gpvm.game.userId;

                var cfpgame = new cfgPlayGameRepository().getAllLive();
                bool cf = false;
                foreach (var cg in cfpgame)
                {
                    if (cg.shortName == gpvm.PlayGame.shortName)
                    {
                        cf = true;
                        gp.PlayGameSn = cg.sn;
                    }

                }

                if (!cf)
                {
                    cfgPlayGame cfpg = new cfgPlayGame();

                    cfpg.shortName = gpvm.PlayGame.shortName;
                    cfpg.cName = gpvm.PlayGame.shortName;
                    cfpg.eName = gpvm.PlayGame.shortName;
                    cfpg.valid = 2;
                    gp.PlayGameSn = new cfgPlayGameRepository().Create(cfpg);


                }

                new UserRepository().CreateRecommend(gpvm.game.userId, str(6));

                new GamePostsRepository().add(gp);

                return gp.sn.ToString();
            }
            catch
            {
                return "0";
            }

        }

        
        [AllowAnonymous]
        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> DeleteAsync(int t)
        {
            ControllerContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            await new GamesRepository().DeleteLtopic(t);
           // new SignalRHelper().UpdateTopic(game, encryptedKey, gpvm.md5GameSn);

            return null;
            
        }

        //停止下注
        [AllowAnonymous]
        [HttpPost]
        public async System.Threading.Tasks.Task<bool> setClose(int id,int t)
        {
            ControllerContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            try
            {
                gameDto game = null;
                GamesRepository _games = new GamesRepository();
                game = await _games.GetGameDetail(id);
                if (1 == 1)
                {
                    StartBetReq sbr = new StartBetReq();
                    sbr.UserID = User.Identity.GetUserId();
                    sbr.comSn = 1;
                    sbr.topicSn = t;
                    sbr.gameSn = id;
                    bool bresult = await _games.setLiveClose(sbr);
                    game = await _games.reLiveGameListAsync(sbr.gameSn);
                    new SignalRHelper().UpdateTopic(game, encryptedKey, game.md5GameSn);
                    return bresult;
                }
                else
                {
                    return false;
                }
                    
            }
            catch
            {
                return false;
            }
        }

        //重新開盤
        [AllowAnonymous]
        [HttpPost]
        public async System.Threading.Tasks.Task<bool> reopen(int id, int t)
        {
            ControllerContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            try
            {
                gameDto game = null;
                GamesRepository _games = new GamesRepository();
                game = await _games.GetGameDetail(id);
                if (1 == 1)
                {
                    StartBetReq sbr = new StartBetReq();
                    sbr.UserID = User.Identity.GetUserId();
                    sbr.comSn = 1;
                    sbr.topicSn = t;
                    sbr.gameSn = id;
                    bool bresult = await _games.reopenLive(sbr);
                    game = await _games.reLiveGameListAsync(sbr.gameSn);
                    new SignalRHelper().UpdateTopic(game, encryptedKey, game.md5GameSn);
                    return bresult;
                }
                else
                {
                    return false;
                }
                    
            }
            catch
            {
                return false;
            }
        }


        //無派彩返還
        [AllowAnonymous]
        [HttpPost]
        public async System.Threading.Tasks.Task<bool> deleteClose(int id,int t,string back)
        {
            ControllerContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            try
            {
                gameDto game = null;
                GamesRepository _games = new GamesRepository();
                game = await _games.GetGameDetail(id);
                if (1 == 1 || game.userId == User.Identity.GetUserId())
                {
                    StartBetReq sbr = new StartBetReq();
                    sbr.comSn = 1;
                    sbr.gameSn = id;
                    sbr.topicSn = t;
                    sbr.back = back;
                    bool bresult = await _games.deleteLiveClose(sbr);
                    game = await _games.reLiveGameListAsync(sbr.gameSn);
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

        //派彩後返還
        [AllowAnonymous]
        [HttpPost]
        public async System.Threading.Tasks.Task<bool> PayBackClose(int id, int t, string back)
        {
            ControllerContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            try
            {
                gameDto game = null;
                GamesRepository _games = new GamesRepository();
                game = await _games.GetGameDetail(id);
                if (1 == 1 || game.userId == User.Identity.GetUserId())
                {
                    StartBetReq sbr = new StartBetReq();
                    sbr.comSn = 1;
                    sbr.gameSn = id;
                    sbr.topicSn = t;
                    sbr.back = back;
                    bool bresult = await _games.deletePayLiveClose(sbr);
                    game = await _games.reLiveGameListAsync(sbr.gameSn);
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

        //生成字母和数字随机数
        public static string str(int length, bool sleep)
        {
            if (sleep)
            {
                System.Threading.Thread.Sleep(3);
            }
            char[] Pattern = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            string result = "";
            int n = Pattern.Length;
            System.Random random = new Random(~unchecked((int)DateTime.Now.Ticks));
            for (int i = 0; i < length; i++)
            {
                int rnd = random.Next(0, n);
                result += Pattern[rnd];
            }
            return result;
        }

        public static string str(int length)
        {
            return str(length, false);
        }



    }
}