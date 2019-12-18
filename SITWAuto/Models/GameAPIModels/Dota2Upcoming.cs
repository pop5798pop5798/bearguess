﻿using SITDto;
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
    public class Dota2Upcoming
    {
        public class Serie
        {
            public int id { get; set; }
            public string name { get; set; }
            public string slug { get; set; }
            public string season { get; set; }
            public DateTime begin_at { get; set; }
            public int year { get; set; }
            public int league_id { get; set; }
            public object winner_id { get; set; }
            public string winner_type { get; set; }
            public DateTime end_at { get; set; }
            public string full_name { get; set; }
        }

        public class League
        {
            public int id { get; set; }
            public object url { get; set; }
            public string name { get; set; }
            public string slug { get; set; }
            public string image_url { get; set; }
            public bool live_supported { get; set; }
        }

        public class Winner
        {
            public int id { get; set; }
            public string name { get; set; }
            public string acronym { get; set; }
            public string slug { get; set; }
            public string image_url { get; set; }
        }

        public class Live
        {
            public bool supported { get; set; }
            public DateTime? opens_at { get; set; }
            public string url { get; set; }
        }

        public class Videogame
        {
            public int id { get; set; }
            public string name { get; set; }
            public string slug { get; set; }
        }

        public class Tournament
        {
            public int id { get; set; }
            public string name { get; set; }
            public string slug { get; set; }
            public int? winner_id { get; set; }
            public DateTime begin_at { get; set; }
            public DateTime end_at { get; set; }
            public int serie_id { get; set; }
            public int league_id { get; set; }
        }

        public class RootObject
        {
            public int id { get; set; }
            public int tournament_id { get; set; }
            public string name { get; set; }
            public DateTime begin_at { get; set; }
            public int? number_of_games { get; set; }
            public string slug { get; set; }
            public bool draw { get; set; }
            public string status { get; set; }
            public DateTime modified_at { get; set; }
            public string match_type { get; set; }
            public int serie_id { get; set; }
            public int league_id { get; set; }
            public List<object> games { get; set; }
            public List<Opponents> opponents { get; set; }
            public Serie serie { get; set; }
            public League league { get; set; }
            public List<object> results { get; set; }
            public Winner winner { get; set; }
            public Live live { get; set; }
            public Videogame videogame { get; set; }
            public object videogame_version { get; set; }
            public Tournament tournament { get; set; }
            public string dtstring { get; set; }
        }

        
        public class Opponents
        {
            public string type { get; set; }
            public Opponentslist opponent { get; set; }

        }


        public class Opponentslist
        {
            public int id { get; set; }
            public string name { get; set; }
            public string acronym { get; set; }
            public string image_url { get; set; }

        }

    }

}
