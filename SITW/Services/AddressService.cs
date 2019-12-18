﻿using Microsoft.Owin.BuilderProperties;
using SITW.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace SITW.Services
{
    public class AddressService : IAddressService
    {
        private sitwEntities db = new sitwEntities();

        /// <summary>
        /// Determines whether the specified predicate is exists.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public bool IsExists(Expression<Func<AddressRecord, bool>> predicate)
        {
            return this.db.AddressRecord.Any(predicate);
        }

        /// <summary>
        /// Totals the count.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public int TotalCount(Expression<Func<AddressRecord, bool>> predicate)
        {
            return this.db.AddressRecord.Count(predicate);
        }


        /// <summary>
        /// Finds the one.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public AddressRecord FindOne(int id)
        {
            if (!this.IsExists(x => x.ID == id))
            {
                return null;
            }
            if (!this.IsExists(x => x.ID == id))
            {
                return null;
            }
            return this.db.AddressRecord.FirstOrDefault(x => x.ID == id);
        }

        /// <summary>
        /// Finds the one by postal code.
        /// </summary>
        /// <param name="postalCode">The postal code.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">沒有輸入 PostalCode</exception>
        public AddressRecord FindOneByPostalCode(int postalCode)
        {
            if (!this.IsExists(x => x.PostalCode == postalCode))
            {
                return null;
            }
            if (!this.IsExists(x => x.PostalCode == postalCode))
            {
                return null;
            }
            return this.db.AddressRecord.FirstOrDefault(x => x.PostalCode == postalCode);
        }

        /// <summary>
        /// Finds all.
        /// </summary>
        /// <returns></returns>
        public IQueryable<AddressRecord> FindAll()
        {
            return this.db.AddressRecord.AsQueryable();
        }

        /// <summary>
        /// Finds the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IQueryable<AddressRecord> Find(Expression<Func<AddressRecord, bool>> predicate)
        {
            return this.db.AddressRecord.Where(predicate);
        }


        /// <summary>
        /// Gets all cities.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetAllCities()
        {
            var query = (from c in this.FindAll()
                         where c.IsEnabled
                         select new
                         {
                             CityCode = c.CitySort.ToString(),
                             CityName = c.City
                         })
                        .Distinct().OrderBy(x => x.CityCode);

            return query.ToDictionary(x => x.CityCode, x => x.CityName);
        }

        /// <summary>
        /// Gets all city dictinoary.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Dictionary<string, string> GetAllCityDictinoary()
        {
            var query = (from c in this.FindAll()
                         where c.IsEnabled
                         select new
                         {
                             CityCode = c.CitySort,
                             CityName = c.City
                         })
                        .Distinct().OrderBy(x => x.CityCode);

            Dictionary<string, string> dict = new Dictionary<string, string>();

            foreach (var item in query)
            {
                if (dict.Keys.Count(x => x.Equals(item.CityName)).Equals(0))
                {
                    dict.Add(item.CityName, item.CityName);
                }
            }
            return dict;
        }

        /// <summary>
        /// Gets the name of the county by city.
        /// </summary>
        /// <param name="cityName">Name of the city.</param>
        /// <returns></returns>
        public Dictionary<string, string> GetCountyByCityName(string cityName)
        {
            var query = (from c in this.FindAll()
                         where c.IsEnabled && c.City == cityName
                         select new
                         {
                             PostalCode = c.PostalCode,
                             CountyName = c.County,
                             Sort = c.PostalCode
                         })
                        .Distinct().OrderBy(x => x.Sort);

            return query.ToDictionary(x => x.PostalCode.ToString(), x => x.CountyName);
        }


        public void Dispose()
        {
            this.db.Dispose();
        }
    }
}