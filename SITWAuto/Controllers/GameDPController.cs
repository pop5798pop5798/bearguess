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

namespace SIT.Controllers
{
    [Authorize]
    public class gameDPController : Controller
    {
        //HttpClient client;
        string encryptedKey;
        private static MemoryCache _cache = MemoryCache.Default;
        List<gameDto> gList;
        public gameDPController()
        {
            encryptedKey = System.Web.Configuration.WebConfigurationManager.AppSettings["encryptedKey"];
            if (_cache.Contains("GameList"))
                gList = _cache.Get("GameList") as List<gameDto>;
            //client = new HttpClient();
            //client.BaseAddress = new Uri(System.Web.Configuration.WebConfigurationManager.AppSettings["apiurl"]);
            //client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        



        [Authorize(Roles = "Admin")]
        public ActionResult _bonusEdit(gameDto model, int? index)
        {
            ViewBag.Index = index;
            GamePostViewModel gpvm = new GamePostViewModel { game = model, vedio = new VedioRecord() };
            return View(gpvm);
        }


        [Authorize(Roles = "Admin")]
        public ActionResult _choiceStrEdit(gameDto model, int? index)
        {
            ViewBag.Index = index;           
            GamePostViewModel gpvm = new GamePostViewModel { game = model, vedio = new VedioRecord() };
            // choiceOddsViewModel cov = new choiceOddsViewModel();
            // cov.unitSn = model.topicList[topicIndex].choiceList[choiceIndex].betMoney[index].unitSn;
            //cov.Odds = model.topicList[topicIndex].choiceList[choiceIndex].betMoney[index].Odds;
            //ViewBag.choiceOddsViewModel = cov;          
           
            return View(gpvm);
        }




        [Authorize(Roles = "Admin")]
        public ActionResult _DPtopicEdit(gameDto model, int index)
        {
            //ViewBag.Index = index;

            //index = index ?? 0;
            ViewBag.Index = index;
            GamePostViewModel gpvm = new GamePostViewModel { game = model, vedio = new VedioRecord() };

            return View(gpvm);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult _DPchoiceEdit(gameDto model, int topicIndex, int index)
        {
            ViewBag.Index = index;
            ViewBag.topicIndex = topicIndex;          
            GamePostViewModel gpvm = new GamePostViewModel { game = model, vedio = new VedioRecord() };
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
        public async System.Threading.Tasks.Task<ActionResult> DPEdit(GamePostViewModel gpvm)
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
                return RedirectToAction("PrizePoolDetails", "game", new { id = gpvm.gamepost.sn });
            //}
            //catch (Exception ex)
            //{
            //    return View(gpvm);
            //}
        }

       




    }
}
