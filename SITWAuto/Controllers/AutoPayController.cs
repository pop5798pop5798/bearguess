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
using System.Runtime.Caching;
using SITDto;
using Microsoft.AspNet.Identity.Owin;
using SITW.Models.ViewModel;
using SITDto.Request;
using SITW.Helper;
using SITW.Models.GameAPIModels;

namespace SITW.Controllers
{

    public class AutoPayController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        //HttpClient client;
        string encryptedKey;
        private static MemoryCache _cache = MemoryCache.Default;
        List<gameDto> gList;

        public AutoPayController()
        {
            encryptedKey = System.Web.Configuration.WebConfigurationManager.AppSettings["encryptedKey"];
            if (_cache.Contains("GameList"))
                gList = _cache.Get("GameList") as List<gameDto>;
            //client = new HttpClient();
            //client.BaseAddress = new Uri(System.Web.Configuration.WebConfigurationManager.AppSettings["apiurl"]);
            //client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public AutoPayController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

       /* public async System.Threading.Tasks.Task<ActionResult> pyatest()
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
                await this.CSGOSetAnswer(c.sn, allgame, getm);




            }

            return View();
        }*/


        public async System.Threading.Tasks.Task<string> CSGOSetAnswer(int gpid, List<CSGOGame.RootObject> cslist, CSGOMatches.RootObject csgetm)
        {
            try
            {
                GamePosts gamepost = new GamePostsRepository().get(gpid);
                gameDto game = null;
                game = await new GamesRepository().GetGameDetail(gamepost.GameSn);

                GamePostViewModel gpvm = new GamePostViewModel(gpid, encryptedKey, game);
                gpvm.gamepost = gamepost;

               // game = await new GamesRepository().GetGameDetail(gamepost.GameSn);


                gameDto gd = null;
                GamesRepository _games = new GamesRepository();
                gd = await _games.GetGameDetail(gpvm.game.sn);
                


                if (1 == 1)
                {
                    List<choiceDto> choice = new List<choiceDto>();
                    foreach (topicDto t in gpvm.game.topicList)
                    {
                        foreach(var c in t.choiceList)
                        {
                            //1:哪隊會獲得勝利 2:總地圖數奇/偶 3:第一個地圖勝利隊伍 4:第二個地圖勝利隊伍 5:總地圖數會高於還是低於x.5 6:第一個地圖總回合數奇/偶 7:第二個地圖總回合數奇/偶
                            switch (t.autotype)
                            {
                                case 1:
                                    if (csgetm.winner.name == c.choiceStr)
                                    {
                                        c.isTrue = 1;
                                    }
                                    else
                                    {
                                        c.isTrue = 0;
                                    }
                                    break;
                                case 2:
                                    if (cslist.Count() / 2 == 0 && c.cNumberType == 1)
                                    {
                                        c.isTrue = 1;
                                    }
                                    else if (cslist.Count() / 2 != 0 && c.cNumberType == 0)
                                    {

                                        c.isTrue = 1;
                                    }
                                    else
                                    {
                                        c.isTrue = 0;
                                    }
                                    break;
                                case 3:
                                    CSGOTeam.RootObject wteam = await new CSGORepository().GetCSGOTeam(csgetm.games[0].winner.id);
                                    if (wteam.name == c.choiceStr)
                                    {
                                        c.isTrue = 1;
                                    }
                                    else
                                    {
                                        c.isTrue = 0;
                                    }
                                    break;
                                case 4:
                                    wteam = await new CSGORepository().GetCSGOTeam(csgetm.games[1].winner.id);
                                    if (wteam.name == c.choiceStr)
                                    {
                                        c.isTrue = 1;
                                    }
                                    else
                                    {
                                        c.isTrue = 0;
                                    }
                                    break;
                                case 5:
                                    if (cslist.Count() < t.numberType && c.cNumberType == 1)
                                    {
                                        c.isTrue = 1;
                                    }
                                    else if (cslist.Count() > t.numberType && c.cNumberType == 0)
                                    {
                                        c.isTrue = 1;
                                    }
                                    else
                                    {
                                        c.isTrue = 0;
                                    }
                                    break;
                                case 6:
                                    if (cslist[0].rounds.Count() / 2 == 0 && c.cNumberType == 1)
                                    {
                                        c.isTrue = 1;
                                    }
                                    else if (cslist.Count() / 2 != 0 && c.cNumberType == 0)
                                    {

                                        c.isTrue = 1;
                                    }
                                    else
                                    {
                                        c.isTrue = 0;
                                    }
                                    break;
                                case 7:
                                    if (cslist[1].rounds.Count() / 2 == 0 && c.cNumberType == 1)
                                    {
                                        c.isTrue = 1;
                                    }
                                    else if (cslist.Count() / 2 != 0 && c.cNumberType == 0)
                                    {

                                        c.isTrue = 1;
                                    }
                                    else
                                    {
                                        c.isTrue = 0;
                                    }
                                    break;
                                default:
                                    Console.WriteLine("error");
                                    break;




                            }


                        }
                            
                        choice.AddRange(t.choiceList);
                    }
                        
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
                        swq.UserID = UserManager.FindByEmail("pop5798pop5798@gmail.com").Id;
                        swq.comSn = 1;
                        swq.choiceList = choice;
                        swq.gameSn = gpvm.game.sn;
                        bool issuccess = await _games.setWinner(swq);
                        if (!issuccess)
                            return "系統設定出錯";
                        new SignalRHelper().UpdateTopic(gpvm.game, encryptedKey, gpvm.game.md5GameSn);
                    }
                    else
                    {
                        return "未設定設定結果";
                    }
                }
                // Return the URI of the created resource.


                return "1";
            }
            catch
            {
                return "0";
            }


        }
        


    }
}