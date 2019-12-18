using Dapper;
using SITW.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace SITW.Models.Repository
{
    public class GamePostsRepository
    {
        private sitwEntities db = new sitwEntities();

        public void add(GamePosts gp)
        {
            gp.inpdate = DateTime.Now;
            gp.valid = 1;
            db.GamePosts.Add(gp);

            db.SaveChanges();


        }

        public void update(GamePosts gp)
        {
            GamePosts gpd = db.GamePosts.Where(p => p.sn == gp.sn).FirstOrDefault();
            gpd.VedioRecordSn = gp.VedioRecordSn;
            gpd.GameSn = gp.GameSn;
            gpd.TeamASn = gp.TeamASn;
            gpd.TeamBSn = gp.TeamBSn;
            gpd.sdate = gp.sdate;
            gpd.edate = gp.edate;
            gpd.PlayGameSn = gp.PlayGameSn;
            gpd.Visited = gp.Visited;
            gpd.LiveCount = gp.LiveCount;
            db.SaveChanges();


        }

        public GamePosts get(int sn)
        {
            return db.GamePosts.Where(p => p.sn == sn).FirstOrDefault();
        }

        public GamePosts getgame(int sn)
        {
            return db.GamePosts.Where(p => p.GameSn == sn).FirstOrDefault();
        }

        public List<GamePosts> getAll()
        {
            return db.GamePosts.Where(p => p.valid == 1 && p.UserLive == null).ToList();
        }

        public List<GamePosts> getValidAll()
        {
           // DateTime dt = DateTime.Now.AddHours(-2);
            return db.GamePosts.Where(p => p.valid == 1 && p.UserLive == null).ToList();
        }
        public List<GamePosts> getLiveValidAll()
        {
            // DateTime dt = DateTime.Now.AddHours(-2);
            return db.GamePosts.Where(p => p.valid == 1 && p.UserLive != null).ToList();
        }

        public WebPush  getWebPush()
        {           
            return db.WebPush.Where(p => p.id == 1).FirstOrDefault();
        }

        public GamePosts getLive(string user)
        {
            return db.GamePosts.Where(x=>x.UserLive == user).FirstOrDefault();
            //return null;
        }

        public void DeleteGamePost(int gp)
        {
            if (gp == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                GamePosts instance = db.GamePosts.Where(x => x.GameSn == gp).FirstOrDefault();
                db.Entry(instance).State = EntityState.Deleted;
                this.SaveChanges();
            }
        }

        public void SaveChanges()
        {
            this.db.SaveChanges();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.db != null)
                {
                    this.db.Dispose();
                    this.db = null;
                }
            }
        }

    }
}