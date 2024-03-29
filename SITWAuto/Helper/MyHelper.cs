﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using SpGatewayHelper.Models;

namespace SpGatewayHelper.Helper
{
    public static class MyHelper
    {
        /// <summary>
        /// To the unix time stamp.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        public static string ToUnixTimeStamp(this DateTime x)
        {
            return ((int)x.Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString();
        }

        /// <summary>
        /// To the unix time stamp.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        public static string OrderNoToProuctId(this string orderNo)
        {
            string pmid = "";
            bool end = true;
            for(int i = 1;i<3;i++)
            {
                if(end)
                {
                    pmid = orderNo.Substring(orderNo.Length - i,1) + pmid;
                    int j = i + 1;
                    if(orderNo.Substring(orderNo.Length - j,1) == "0")
                    {
                        end = false;
                    }
                }
            }

            return pmid;
        }

        /// <summary>
        /// To the dictionary.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static Dictionary<string, object> ToDictionary(this TradeInfo obj)
        {
            return obj.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(d => new {Name = d.Name, Value = d.GetValue(obj, null)})
                .Where(d => d.Value != null && string.IsNullOrWhiteSpace(d.Value.ToString()) == false)
                .ToDictionary(d => d.Name, d => d.Value);
        }
    }
}