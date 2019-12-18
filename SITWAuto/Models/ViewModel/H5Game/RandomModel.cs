using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SITW.Models.ViewModel;

namespace SITW.Models.ViewModel
{
    public class RandomModel
    {

        /*public class Data
        {
            public string number { set; get; }
        }*/

        public class RandomData
        {
            public List<int> data { get; set; }
            public string completionTime { get; set; }
        }

        public class Result
        {
            public RandomData random { get; set; }
            public string bitsUsed { get; set; }
            public string bitsLeft { get; set; }
            public string requestsLeft { get; set; }
            public string advisoryDelay { get; set; }
        }

        public class RandomObject
        {
            public string jsonrpc { get; set; }
            public Result result { get; set; }
            public string id { get; set; }
        }

    }

}