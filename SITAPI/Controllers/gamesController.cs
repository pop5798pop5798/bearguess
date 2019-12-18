using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using SITAPI.Models;
using SITDto;
using System.Web.Configuration;
using Dapper;
using System.Data.SqlClient;
using SITAPI.Models.Repository;
using AutoMapper;
using SITDto.Request;
using SITAPI.Filter;
using SITDto.ViewModel;

namespace SITAPI.Controllers
{
    [LogParamFilter]
    public class gamesController : baseController
    {
        private sitdbEntities db = new sitdbEntities();

        // GET: api/games
        public List<gameDto> Getgames()
        {
            return new GameRepository().GetGameList();
        }

        [Route("api/admingames")]
        [HttpGet]
        public List<gameDto> admingames()
        {
            return new GameRepository().GetGameList();
            //return new GameRepository().GetGameAdminList();
        }

        [Route("api/gamesPay")]
        [HttpPost]
        public gameDto gamesPay(SetWinnerReq swq)
        {
            return new GameRepository().GetPayGameList(swq.gameSn).FirstOrDefault();
            //return new GameRepository().GetGameAdminList();
        }

        [Route("api/gamesLive")]
        [HttpPost]
        public gameDto gamesLive(SetWinnerReq swq)
        {
            return new GameRepository().GetLiveGameList(swq.gameSn).FirstOrDefault();
            //return new GameRepository().GetGameAdminList();
        }

        [Route("api/gamestopic")]
        [HttpGet]
        public SettingListDto gamestopic()
        {
            return new GameRepository().GettopicGameList();
            //return new GameRepository().GetGameAdminList();
        }

        [Route("api/gamesbonus")]
        [HttpGet]
        public SettingListDto gamesbonus()
        {
            return new GameRepository().GetBonusGameList();
            //return new GameRepository().GetGameAdminList();
        }

      
        // GET: api/games/5
        [ResponseType(typeof(gameDto))]
        public IHttpActionResult Getgame(int id)
        {
            gameDto gamedto = new GameRepository().GetGame(id);
            if (gamedto.sn == 0)
                return NotFound();

            return Ok(gamedto);
        }

        // PUT: api/games/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Putgame(int id, gameDto game)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != game.sn)
            {
                return BadRequest();
            }
            bool isOK = new GameRepository().SetGame(id, game);
            if (!isOK)
                return NotFound();

            return StatusCode(HttpStatusCode.NoContent);
        }


        // PUT: api/games/5
        [Route("api/PutgameLive/")]
        [HttpPost]
        public IHttpActionResult PutgameLive(SetWinnerReq swq)
        {
           /* if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }*/

            if (swq.game.sn != swq.gameSn)
            {
                return BadRequest();
            }
            bool isOK = new GameRepository().SetGameLive(swq.gameSn, swq.game);
            if (!isOK)
                return NotFound();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // PUT: api/games/5
        [Route("api/DeleteLive/")]
        [HttpPost]
        public IHttpActionResult DeleteLive(SetWinnerReq swq)
        {
            
            bool isOK = new GameRepository().DeleteLtopic(swq.topicSn);
            if (!isOK)
                return NotFound();

            return StatusCode(HttpStatusCode.NoContent);
        }



        // POST: api/games
        [ResponseType(typeof(game))]
        public IHttpActionResult Postgame(gameDto gameD)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            user u = db.users.Where(p => p.userID == gameD.userId && p.comSn == gameD.comSn).FirstOrDefault();
            if (u == null)
            {
                return BadRequest("UserID不存在");
            }
            else
            {
                gameD.userSn = u.sn;
            }

            Mapper.Initialize(cfg => {
                cfg.CreateMap<gameDto, game>();
                cfg.CreateMap<topicDto, topic>();
                cfg.CreateMap<choiceDto, choice>();
                cfg.CreateMap<choiceStrDto, choiceStr>();
                cfg.CreateMap<bonusDto, bonu> ();
            });
            game game = Mapper.Map<game>(gameD);

            game.gameStatus = 1;      //遊戲建立的時候是關閉狀態
            game.valid = 1;
            //game.createDate = DateTime.Now;
            db.games.Add(game);
            db.SaveChanges();
                foreach (topicDto tD in gameD.topicList)
                {
                    if (tD.valid.HasValue && tD.valid.Value == 1)
                    {
                        topic t = Mapper.Map<topic>(tD);
                        t.gameSn = game.sn;
                        t.comSn = game.comSn;
                        if (!t.sdate.HasValue)
                            t.sdate = gameD.sdate;
                        if (!t.edate.HasValue)
                            t.edate = gameD.edate;
                        db.topics.Add(t);
                        db.SaveChanges();
                        foreach (choiceDto cD in tD.choiceList)
                        {
                            if (cD.valid.HasValue && cD.valid.Value == 1)
                            {
                                choice c = Mapper.Map<choice>(cD);
                                c.comSn = game.comSn;
                                c.topicSn = t.sn;
                                db.choices.Add(c);
                                db.SaveChanges();
                                //龍的傳人才會新增 
                                if (game.betModel == 5)
                                {
                                    foreach (choiceStrDto cb in cD.choiceString)
                                    {

                                        choiceStr cs = Mapper.Map<choiceStr>(cb);
                                        //只有在可下注單位的資料要存
                                        //if (game.betUnit.Contains(cs.unitSn.ToString()))
                                        //{
                                        cs.choiceSn = c.sn;
                                        cs.choiceStr1 = cb.choiceStr;
                                        cs.eChoiceStr1 = cb.eChoiceStr;
                                        cs.unitSn = 1;
                                        db.choiceStrs.Add(cs);
                                        db.SaveChanges();
                                        //cs = new choiceStrRepository().setData(cs);
                                        //}
                                    }
                                }
                            }
                        }
                    }
                }
                //獎金置入
                if (gameD.bonusList != null)
                {
                    foreach (bonusDto bod in gameD.bonusList)
                    {
                        bonu b = Mapper.Map<bonu>(bod);
                        b.gamesn = game.sn;
                        db.bonus.Add(b);
                        db.SaveChanges();

                    }


                }



            
            




            return CreatedAtRoute("DefaultApi", new { id = game.sn }, game);
        }

        // POST: api/games
        [Route("api/PostgameLive")]
        public game PostgameLive(gameDto gameD)
        {
          /*  if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }*/
            user u = db.users.Where(p => p.userID == gameD.userId && p.comSn == gameD.comSn).FirstOrDefault();
            if (u == null)
            {
                //return BadRequest("UserID不存在");
            }
            else
            {
                gameD.userSn = u.sn;
            }

            Mapper.Initialize(cfg => {
                cfg.CreateMap<gameDto, game>();
                cfg.CreateMap<topicDto, topic>();
                cfg.CreateMap<choiceDto, choice>();
                cfg.CreateMap<choiceStrDto, choiceStr>();
                cfg.CreateMap<bonusDto, bonu>();
            });
            game game = Mapper.Map<game>(gameD);

            game.gameStatus = 1;      //遊戲建立的時候是關閉狀態
            game.valid = 1;
            game.live = 1;
            
            //game.createDate = DateTime.Now;
            db.games.Add(game);
            db.SaveChanges();        

            return game;
        }

        [Route("api/setSetting")]
        [HttpPost]
        public SettingListDto setSetting(SettingListDto setting)
        {
            if(setting.gamesettingList != null)
            {
                foreach (var gsetting in setting.gamesettingList)
                {
                    new GameRepository().DgonsettingEdit(gsetting);
                }

            }
            
            if(setting.bonussettingList != null)
            {
                foreach (var bsetting in setting.bonussettingList)
                {
                    new GameRepository().bonussettingEdit(bsetting);
                }
            }
            if (setting.topicsettingList != null)
            {
                foreach (var tsetting in setting.topicsettingList)
                {
                    new GameRepository().topicsettingEdit(tsetting);
                }
            }
                return setting;
        }

        [Route("api/setSettingCreate")]
        [HttpPost]
        public SettingListDto setSettingCreate(SettingListDto setting)
        {           
            if (setting.topicsettingList != null)
            {
                foreach (var tsetting in setting.topicsettingList)
                {
                    new GameRepository().topicsettingCreate(tsetting);
                }
            }
            return setting;
        }


        [Route("api/StartBet")]
        [HttpPost]
        public IHttpActionResult StartBet(StartBetReq sbr)
        {

            GameRepository gr = new GameRepository();
            byte? gameclosestatus = gr.getGameCloseStatus(sbr.gameSn);
            if (gameclosestatus != 1)
                return BadRequest("此遊戲無法開始");
            gr.setGameCloseStatus(sbr.gameSn, 0);
            return Ok();
        }
        [Route("api/setClose")]
        [HttpPost]
        public IHttpActionResult setClose(StartBetReq sbr)
        {
            GameRepository gr = new GameRepository();
            byte? gameclosestatus = gr.getGameCloseStatus(sbr.gameSn);
            if (gameclosestatus != 0)
                return BadRequest("此遊戲無法關閉");
            gr.setGameCloseStatus(sbr.gameSn, 2);
            return Ok();
        }

        [Route("api/setLiveClose")]
        [HttpPost]
        public IHttpActionResult setLiveClose(StartBetReq sbr)
        {
            GameRepository gr = new GameRepository();
            /*byte? gameclosestatus = gr.getGameCloseStatus(sbr.gameSn);
            if (gameclosestatus != 0)
                return BadRequest("此遊戲無法關閉");*/
            gr.setTopicCloseStatus(sbr.topicSn, 3, sbr.back);
            return Ok();
        }

        [Route("api/deleteClose")]
        [HttpPost]
        public IHttpActionResult deleteClose(StartBetReq sbr)
        {
            GameRepository gr = new GameRepository();
            byte? gameclosestatus = gr.getGameCloseStatus(sbr.gameSn);
            gr.setGameCloseStatus(sbr.gameSn, 5);
            return Ok();
        }

        [Route("api/deleteLiveClose")]
        [HttpPost]
        public IHttpActionResult deleteLiveClose(StartBetReq sbr)
        {
            GameRepository gr = new GameRepository();
            //byte? gameclosestatus = gr.getGameCloseStatus(sbr.gameSn);
            //bool isOK = new GameRepository().DeleteLtopic(swq.topicSn);
            gr.setTopicCloseStatus(sbr.topicSn, 5,sbr.back);
            return Ok();
        }

        [Route("api/PayLiveClose")]
        [HttpPost]
        public IHttpActionResult PayLiveClose(StartBetReq sbr)
        {
            GameRepository gr = new GameRepository();
            //byte? gameclosestatus = gr.getGameCloseStatus(sbr.gameSn);
            //bool isOK = new GameRepository().DeleteLtopic(swq.topicSn);
            gr.setTopicCloseStatus(sbr.topicSn, 4, sbr.back);
            return Ok();
        }


        [Route("api/reopen")]
        [HttpPost]
        public IHttpActionResult reopen(StartBetReq sbr)
        {
            GameRepository gr = new GameRepository();
            byte? gameclosestatus = gr.getGameCloseStatus(sbr.gameSn);
            if (gameclosestatus == 3)
                return BadRequest("已完成派彩，無法重新開啟");
            new GameRepository().setGameCloseStatus(sbr.gameSn, 1);
            return Ok();
        }

        [Route("api/reopenLive")]
        [HttpPost]
        public IHttpActionResult reopenLive(StartBetReq sbr)
        {
            GameRepository gr = new GameRepository();
            gr.setTopicCloseStatus(sbr.topicSn, 1, sbr.back);
            return Ok();
        }

        [Route("api/setDone")]
        [HttpPost]
        public IHttpActionResult setDone(StartBetReq sbr)
        {
            GameRepository gr = new GameRepository();
            byte? gameclosestatus = gr.getGameCloseStatus(sbr.gameSn);
            if (gameclosestatus == 3)
            {
                return Content(HttpStatusCode.BadRequest, "已完成派彩，無法重新派彩");
                //return BadRequest("已完成派彩，無法重新派彩");
            }
            new GameRepository().setGameCloseStatus(sbr.gameSn, 3);
            return Ok();
        }

        [Route("api/setWinner")]
        [HttpPost]
        public IHttpActionResult setWinner(SetWinnerReq swq)
        {
            user u = db.users.Where(p => p.userID == swq.UserID && p.comSn == swq.comSn).FirstOrDefault();
            if (u == null)
            {
                return BadRequest("UserID不存在");
            }
            GameRepository gr = new GameRepository();
            gr.setWinner(swq.choiceList);
            gr.setGameCloseStatus(swq.gameSn, 4);
            return Ok();
        }

        [Route("api/setLiveWinnerPay")]
        [HttpPost]
        public IHttpActionResult setLiveWinnerPay(SetWinnerReq swq)
        {
            user u = db.users.Where(p => p.userID == swq.UserID && p.comSn == swq.comSn).FirstOrDefault();
            if (u == null)
            {
                return BadRequest("UserID不存在");
            }
            GameRepository gr = new GameRepository();
            gr.setLiveWinner(swq.choiceList);

            byte? gameclosestatus = gr.getGameCloseStatus(swq.gameSn);
            gr.ChangeGameValid(swq.topicSn);
            List<payoutDto> payoutList = gr.livepays(swq.gameSn);

            return Ok(payoutList);

            //gr.setGameCloseStatus(swq.gameSn, 4);
           // return Ok();
        }



        [Route("api/pays")]
        [HttpPost]
        public IHttpActionResult pays(StartBetReq sbr)
        {
            GameRepository gr = new GameRepository();
            byte? gameclosestatus = gr.getGameCloseStatus(sbr.gameSn);
            if (gameclosestatus != 4)
                return BadRequest("此遊戲未設定結果無法派彩");          
            List<payoutDto> payoutList = gr.pays(sbr.gameSn);
            return Ok(payoutList);
        }

        [Route("api/dgnpays")]
        [HttpPost]
        public IHttpActionResult dgnpays(StartBetReq sbr)
        {
            GameRepository gr = new GameRepository();
            byte? gameclosestatus = gr.getGameCloseStatus(sbr.gameSn);
            if (gameclosestatus != 4)
                return BadRequest("此遊戲未設定結果無法派彩");
            List<payoutDto> payoutList = gr.dgnpays(sbr.gameSn);
            return Ok(payoutList);
        }

        [Route("api/nabobpays")]
        [HttpPost]
        public IHttpActionResult nabobpays(StartBetReq sbr)
        {
            GameRepository gr = new GameRepository();
            byte? gameclosestatus = gr.getGameCloseStatus(sbr.gameSn);
            if (gameclosestatus != 4)
                return BadRequest("此遊戲未設定結果無法派彩");
            List<payoutDto> payoutList = gr.nabobpays(sbr.gameSn);
            return Ok(payoutList);
        }

       /* [Route("api/Runpays")]
        [HttpPost]
        public IHttpActionResult Runpays(StartBetReq sbr)
        {
            GameRepository gr = new GameRepository();
            byte? gameclosestatus = gr.getGameCloseStatus(sbr.gameSn);
            if (gameclosestatus != 4)
                return BadRequest("此遊戲未設定結果無法派彩");
            List<payoutDto> payoutList = gr.Runpays(sbr.gameSn);
            return Ok(payoutList);
        }*/



        [Route("api/paysRollback")]
        [HttpPost]
        public IHttpActionResult paysRollback(StartBetReq sbr)
        {
            GameRepository gr = new GameRepository();
            byte? gameclosestatus = gr.getGameCloseStatus(sbr.gameSn);
            if (gameclosestatus != 3)
                return BadRequest("此遊戲尚未派彩無法退回");
            List<payoutDto> payoutList = gr.paysRollback(sbr.gameSn);
            gr.setGameCloseStatus(sbr.gameSn, 2);
            return Ok(payoutList);
        }

        [Route("api/gamereturn")]
        [HttpPost]
        public IHttpActionResult gamereturn(StartBetReq sbr)
        {
            GameRepository gr = new GameRepository();
            byte? gameclosestatus = gr.getGameCloseStatus(sbr.gameSn);
            if (gameclosestatus != 4)
                return BadRequest("此遊戲無法重設");
            gr.setGameCloseStatus(sbr.gameSn, 2);
            return Ok();
        }

        // DELETE: api/games/5
        //[ResponseType(typeof(game))]
        //public IHttpActionResult Deletegame(int id)
        //{
        //    game game = db.games.Find(id);
        //    if (game == null)
        //    {
        //        return NotFound();
        //    }

        //    db.games.Remove(game);
        //    db.SaveChanges();

        //    return Ok(game);
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool gameExists(int id)
        {
            return db.games.Count(e => e.sn == id) > 0;
        }
    }
}