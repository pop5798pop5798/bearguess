using Dapper;
using SITW.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Xml;

namespace SITW.Models.Repository
{
    [Authorize(Roles = "Admin")]
    public class PreferentialRepository
    {
        private sitwEntities Db = new sitwEntities();

        public List<Preferential> getAllvaild()
        {
            return Db.Preferential.Where(x=>x.vaild == 1 && x.sdate < DateTime.Now && x.edate > DateTime.Now).ToList();
        }
        public List<Preferential> getAll()
        {
            return Db.Preferential.Where(x => x.vaild == 1).ToList();
        }

        public PreferentialRecords getPRecords(string user)
        {
            return Db.PreferentialRecords.Where(x => x.UserId == user).FirstOrDefault();

        }
        public PreferentialRecords getPRecordsType(string user,int type)
        {
            return Db.PreferentialRecords.Where(x => x.UserId == user && x.PreferentialID == type).FirstOrDefault();

        }


        public List<PreferentialRecords> getPRecordsAll(string user)
        {
            return Db.PreferentialRecords.Where(x => x.UserId == user).ToList();

        }

        public Preferential getpreferential(int offer)
        {
            return Db.Preferential.Where(x => x.offerModel == offer).FirstOrDefault();
        }



        public void PRecordsCreate(PreferentialRecords instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                Db.PreferentialRecords.Add(instance);
                this.SaveChanges();
            }

        }



        public void Create(Preferential instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                Db.Preferential.Add(instance);
                this.SaveChanges();

            }


        }




        public void Update(Preferential instance)
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


        public void PRecordsUpdate(PreferentialRecords instance)
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
  

        public void Delete(Preferential instance)
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
       

      


        public Preferential Get(int id)
        {
            return Db.Preferential.FirstOrDefault(x => x.id == id);
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