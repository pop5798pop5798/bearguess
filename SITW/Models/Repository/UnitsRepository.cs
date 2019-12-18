using Dapper;
using SITW.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Configuration;

namespace SITW.Models.Repository
{
    public class UnitsRepository
    {
        private static MemoryCache _cache = MemoryCache.Default;
        private sitwEntities db = new sitwEntities();

        public List<cfgUnit> getAll()
        {
            return db.cfgUnit.ToList();
        }

        public List<cfgUnit> getAllValid()
        {
            return db.cfgUnit.Where(p => p.valid == 1).ToList();
        }

        public cfgUnit getValid(int? unitSn)
        {
            return db.cfgUnit.Where(p => p.sn == unitSn).FirstOrDefault();
        }

        /// <summary>
        /// 取得金錢的單位清單
        /// </summary>
        /// <returns></returns>
        public List<cfgUnit> getAllValidMoney()
        {
            List<cfgUnit> cuList = new List<cfgUnit>();
            lock (_cache)
            {
                if (_cache.Contains("cuvList"))
                    cuList = _cache.Get("cuvList") as List<cfgUnit>;
                else
                {
                    cuList = db.cfgUnit.Where(p => p.valid == 1 && p.type == 1).ToList();
                    CacheItemPolicy cacheItemPolicy = new CacheItemPolicy() { AbsoluteExpiration = DateTime.Now.AddDays(1) };
                    _cache.Remove("cuvList");
                    _cache.Add("cuvList", cuList, cacheItemPolicy);
                }
            }
            return cuList;
        }

    }
}