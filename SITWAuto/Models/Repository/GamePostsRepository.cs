using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
            return db.GamePosts.Where(p => p.valid == 1).ToList();
        }

        public List<GamePosts> getValidAll()
        {
           // DateTime dt = DateTime.Now.AddHours(-2);
            return db.GamePosts.Where(p => p.valid == 1).ToList();
        }

        public WebPush  getWebPush()
        {           
            return db.WebPush.Where(p => p.id == 1).FirstOrDefault();
        }
    }
}