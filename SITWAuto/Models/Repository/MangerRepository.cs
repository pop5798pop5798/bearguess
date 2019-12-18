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
    public class MangerRepository
    {
        private sitwEntities Db = new sitwEntities();


        /*public List<TransferRecords> GetTransfers() {
            return Db.TransferRecords.ToList();
            
        }*/


        public void Create(Safety instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                Db.Safety.Add(instance);
                this.SaveChanges();               

            }
        }
        public void Update(Safety instance)
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

        public void Delete(Safety instance)
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

      

        public Safety GetValid(string userId)
        {
            return Db.Safety.FirstOrDefault(x => x.userId == userId);
        }

        

        //轉帳記錄
        public List<TransferRecords> GetTransferRecords(string userId)
        {
            return Db.TransferRecords.Where(x=>x.UserId == userId || x.Transfer == userId).ToList();
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