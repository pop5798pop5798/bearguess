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
    public class MallRepository
    {
        private sitwEntities Db = new sitwEntities();

        public List<Product> getAll()
        {
            return Db.Product.Where(x=>x.valid == 1).ToList();
        }
 

       
        public void MallCreate(Product instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                Db.Product.Add(instance);
                this.SaveChanges();

            }


        }

        //加入返回購買記錄
        public void CreateReturnRecord(User_CashReturn instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                Db.User_CashReturn.Add(instance);
                this.SaveChanges();

            }
        }

        //商品記錄
        public List<ProductRecord> GetProductRecord(string userId)
        {
            return Db.ProductRecord.Where(x => x.UserID == userId).ToList();
        }

        //全部商品記錄
        public List<ProductRecord> GetAllProductRecord()
        {
            return Db.ProductRecord.ToList();
        }

        //單筆暫存訂單記錄
        public User_CashReturn GetUserPRecord(string no)
        {
            return Db.User_CashReturn.Where(x => x.MerchantID == no).FirstOrDefault();
        }


        public void Update(Product instance)
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
        //商品記錄修改
        public void PRUpdate(ProductRecord instance)
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

        public void Delete(Product instance)
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

        

        public Product Get(int id)
        {
            return Db.Product.FirstOrDefault(x => x.id == id);
        }
        public ProductRecord GetProductRecord(int id)
        {
            return Db.ProductRecord.FirstOrDefault(x => x.id == id);
        }

        public List<ProductMenu> PMenuGetAll()
        {
            return Db.ProductMenu.ToList();
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