using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SITW.Models.Repository
{
    public class cfgPlayGameRepository
    {
        private sitwEntities Db = new sitwEntities();

        public List<cfgPlayGame> getAll()
        {
            return Db.cfgPlayGame.Where(p => p.valid == 1).ToList();
        }

        public cfgPlayGame get(int sn)
        {
            return Db.cfgPlayGame.Where(p => p.sn == sn).FirstOrDefault();
        }
        public List<cfgPlayGame> getAllLive()
        {
            return Db.cfgPlayGame.ToList();
        }

        public int Create(cfgPlayGame instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                Db.cfgPlayGame.Add(instance);
                this.SaveChanges();
                return instance.sn;
            }
        }



        public void Create(Teams instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                Db.Teams.Add(instance);
                this.SaveChanges();
            }
        }
        public void Update(Teams instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                Db.Entry(instance).State = EntityState.Modified;
                this.SaveChanges();
            }
        }

        public void Delete(Teams instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                Db.Entry(instance).State = EntityState.Deleted;
                this.SaveChanges();
            }
        }
        public Teams Get(int teamsID)
        {
            return Db.Teams.FirstOrDefault(x => x.sn == teamsID);
        }

        public void SaveChanges()
        {
            this.Db.SaveChanges();
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
                if (this.Db != null)
                {
                    this.Db.Dispose();
                    this.Db = null;
                }
            }
        }

    }
}