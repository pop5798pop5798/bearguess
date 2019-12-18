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
    public class CSGOTeam
    {
        public class CurrentVideogame
        {
            public int id { get; set; }
            public string name { get; set; }
            public string slug { get; set; }
        }

        public class Player
        {
            public string first_name { get; set; }
            public string hometown { get; set; }
            public int id { get; set; }
            public string image_url { get; set; }
            public string last_name { get; set; }
            public string name { get; set; }
            public object role { get; set; }
            public string slug { get; set; }
        }

        public class RootObject
        {
            public object acronym { get; set; }
            public CurrentVideogame current_videogame { get; set; }
            public int id { get; set; }
            public string image_url { get; set; }
            public string name { get; set; }
            public List<Player> players { get; set; }
            public string slug { get; set; }
        }

    }

}
