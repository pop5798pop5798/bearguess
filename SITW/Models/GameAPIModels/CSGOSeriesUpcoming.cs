using SITDto;
using SITW.Helper;
using SITW.Models.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace SITW.Models.GameAPIModels
{
    public class CSGOSeriesUpcoming
    {
        public class League
        {
            public int? id { get; set; }
            public string image_url { get; set; }
            public bool live_supported { get; set; }
            public DateTime modified_at { get; set; }
            public string name { get; set; }
            public string slug { get; set; }
            public string url { get; set; }
        }

        public class Tournament
        {
            public DateTime? begin_at { get; set; }
            public DateTime? end_at { get; set; }
            public int? id { get; set; }
            public int? league_id { get; set; }
            public bool live_supported { get; set; }
            public DateTime? modified_at { get; set; }
            public string name { get; set; }
            public string prizepool { get; set; }
            public int? serie_id { get; set; }
            public string slug { get; set; }
            public int? winner_id { get; set; }
            public string winner_type { get; set; }
        }

        public class Videogame
        {
            public int? id { get; set; }
            public string name { get; set; }
            public string slug { get; set; }
        }

        public class RootObject
        {
            public DateTime begin_at { get; set; }
            public object description { get; set; }
            public object end_at { get; set; }
            public string full_name { get; set; }
            public int? id { get; set; }
            public League league { get; set; }
            public int? league_id { get; set; }
            public DateTime modified_at { get; set; }
            public string name { get; set; }
            public string season { get; set; }
            public string slug { get; set; }
            public List<Tournament> tournaments { get; set; }
            public Videogame videogame { get; set; }
            public object winner_id { get; set; }
            public string winner_type { get; set; }
            public int? year { get; set; }
        }


    }

}
