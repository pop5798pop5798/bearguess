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
using AutoMapper;
using SITDto.ViewModel;
using SITAPI.Models.Repository;
using SITDto.Request;

namespace SITAPI.Controllers
{
    public class betsController : baseController
    {
        private sitdbEntities db = new sitdbEntities();

        // GET: api/bets
        public IQueryable<bet> Getbets()
        {
            return db.bets;
        }

        // GET: api/bets/5
        [ResponseType(typeof(bet))]
        public IHttpActionResult Getbet(int id)
        {
            bet bet = db.bets.Find(id);
            if (bet == null)
            {
                return NotFound();
            }

            return Ok(bet);
        }

        // PUT: api/bets/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Putbet(int id, bet bet)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != bet.sn)
            {
                return BadRequest();
            }

            db.Entry(bet).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!betExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/bets
        [ResponseType(typeof(bet))]
        public IHttpActionResult Postbet(betDto bet)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            user u = db.users.Where(p => p.userID == bet.userId && p.comSn == bet.comSn).FirstOrDefault();
            game g = db.games.Where(x => x.sn == bet.gameSn).FirstOrDefault();
            topic dgontc = db.topics.Where(x => x.gameSn == bet.gameSn).FirstOrDefault();
            
            topic tc = db.topics.Where(x => x.sn == bet.topicSn).FirstOrDefault();
            List<bet> bettotal = db.bets.Where(p => p.topicSn == tc.sn).ToList();
            //龍的傳人才新增
            if (g.betModel == 5)
            {
                prizepoolRecord ppr = new prizepoolRecord();
                ppr.unitSn = 1;
                ppr.userSn = u.sn;
                ppr.gameSn = bet.gameSn;
                ppr.topicSn = bet.topicSn;
                ppr.choiceSn = bet.choiceSn;
                ppr.assets = (double)bet.money * (double)dgontc.outlay /100;
                ppr.type = 2;
                ppr.inpdate = DateTime.Now;
                db.prizepoolRecords.Add(ppr);
                db.SaveChanges();

            }


            if (u == null)
            {
                return BadRequest("UserID不存在");
            }
            bet.userSn = u.sn;
            bet.valid = 1;
            Mapper.Initialize(cfg => {
                cfg.CreateMap<betDto, bet>();
            });
            bet b = Mapper.Map<bet>(bet);

            choice cho = db.choices.Where(p => p.sn == b.choiceSn).FirstOrDefault();
            b.Odds = cho.Odds;
            b.topicSn = cho.topicSn;
            if(tc.walk == 1)
            {
                b.totalmoney = bettotal.Sum(x => x.money);
                b.totalmoney += bet.money;
            }
            
            db.bets.Add(b);
            db.SaveChanges();
            //只有新模式才會新增          


            if (bet.betCount != null)
            {
                foreach (betCountDto bcd in bet.betCount)
                {
                //樂透模式才會新增
                
                    betCount bc = new betCount();
                    //只有在可下注單位的資料要存
                    //if (game.betUnit.Contains(cs.unitSn.ToString()))
                    //{
                    bc.betSn = b.sn;
                    bc.choiceStr = bcd.choiceStr;
                    bc.choiceCount = bcd.choiceCount;
                    bc.unitSn = 1;

                    db.betCounts.Add(bc);
                    db.SaveChanges();
                    //cs = new choiceStrRepository().setData(cs);
                    //}
                }
            }



            return CreatedAtRoute("DefaultApi", new { id = bet.sn }, bet);
        }


        // POST: api/bets
        [Route("api/NabobbetsCreate")]
        [HttpPost]
        public IHttpActionResult NabobbetsCreate(List<betDto> betlist)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var count = 0;
            var eqchoice = 0;
            foreach(var bet in betlist)
            {
                user u = db.users.Where(p => p.userID == bet.userId && p.comSn == bet.comSn).FirstOrDefault();
                game g = db.games.Where(x => x.sn == bet.gameSn).FirstOrDefault();
                topic tc = db.topics.Where(x => x.gameSn == bet.gameSn).FirstOrDefault();
                
                if (u == null)
                {
                    return BadRequest("UserID不存在");
                }
                bet.userSn = u.sn;
                bet.valid = 1;
                Mapper.Initialize(cfg => {
                    cfg.CreateMap<betDto, bet>();
                });
                bet b = Mapper.Map<bet>(bet);

                choice cho = db.choices.Where(p => p.sn == b.choiceSn).FirstOrDefault();
                b.Odds = cho.Odds;
                b.topicSn = cho.topicSn;

                db.bets.Add(b);
                db.SaveChanges();
                //只有新模式才會新增                         
                if(count == 0)
                {
                    eqchoice = b.sn;
                }
                   b.equalchoice = eqchoice;
                   db.Entry(b).State = EntityState.Modified;

                    db.SaveChanges();
                count++;

                }



            return Ok(betlist);

     
        }



        // DELETE: api/bets/5
        [ResponseType(typeof(bet))]
        public IHttpActionResult Deletebet(int id)
        {
            bet bet = db.bets.Find(id);
            if (bet == null)
            {
                return NotFound();
            }

            db.bets.Remove(bet);
            db.SaveChanges();

            return Ok(bet);
        }

        [HttpGet]
        [Route("api/BetsByUserID")]
        [ResponseType(typeof(List<BetListDto>))]
        public IHttpActionResult BetsByUserID(string userID)
        {
            return Ok(new BetRepository().getBetsByUserID(userID));
        }

        [HttpGet]
        [Route("api/LiveBetsByUserID")]
        [ResponseType(typeof(List<BetListDto>))]
        public IHttpActionResult LiveBetsByUserID(string userID)
        {
            
            return Ok(new BetRepository().getLiveBetsByUserID(userID));
        }

        [HttpGet]
        [Route("api/NabobBetsByUserID")]
        [ResponseType(typeof(List<NabobBetListDto>))]
        public IHttpActionResult NabobBetsByUserID(string userID)
        {
            return Ok(new BetRepository().getNabobBetsByUserID(userID));
        }

        [HttpPost]
        [Route("api/BetsByGame")]
        [ResponseType(typeof(List<BetListDto>))]
        public IHttpActionResult BetsByGame(BetsByGameReq bbgr)
        {
            return Ok(new BetRepository().getBetsByGame(bbgr));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool betExists(int id)
        {
            return db.bets.Count(e => e.sn == id) > 0;
        }
    }
}