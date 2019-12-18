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
    public class CSGOGame
    {
        public class Map
        {
            public int id { get; set; }
            public string image_url { get; set; }
            public string name { get; set; }
        }

        public class Winner
        {
            public int id { get; set; }
            public string type { get; set; }
        }

        public class Game
        {
            public DateTime begin_at { get; set; }
            public bool detailed_stats { get; set; }
            public DateTime end_at { get; set; }
            public bool finished { get; set; }
            public bool forfeit { get; set; }
            public int id { get; set; }
            public int length { get; set; }
            public int match_id { get; set; }
            public int position { get; set; }
            public string status { get; set; }
            public object video_url { get; set; }
            public Winner winner { get; set; }
            public string winner_type { get; set; }
        }

        public class League
        {
            public int id { get; set; }
            public string image_url { get; set; }
            public bool live_supported { get; set; }
            public DateTime modified_at { get; set; }
            public string name { get; set; }
            public string slug { get; set; }
            public object url { get; set; }
        }

        public class Live
        {
            public object opens_at { get; set; }
            public bool supported { get; set; }
            public object url { get; set; }
        }

        public class Opponent2
        {
            public object acronym { get; set; }
            public int id { get; set; }
            public string image_url { get; set; }
            public string name { get; set; }
            public string slug { get; set; }
        }

        public class Opponent
        {
            public Opponent2 opponent { get; set; }
            public string type { get; set; }
        }

        public class Result
        {
            public int score { get; set; }
            public int team_id { get; set; }
        }

        public class Serie
        {
            public DateTime begin_at { get; set; }
            public object description { get; set; }
            public object end_at { get; set; }
            public string full_name { get; set; }
            public int id { get; set; }
            public int league_id { get; set; }
            public DateTime modified_at { get; set; }
            public string name { get; set; }
            public string prizepool { get; set; }
            public object season { get; set; }
            public string slug { get; set; }
            public object winner_id { get; set; }
            public object winner_type { get; set; }
            public int year { get; set; }
        }

        public class Tournament
        {
            public DateTime begin_at { get; set; }
            public object end_at { get; set; }
            public int id { get; set; }
            public int league_id { get; set; }
            public DateTime modified_at { get; set; }
            public string name { get; set; }
            public string prizepool { get; set; }
            public int serie_id { get; set; }
            public string slug { get; set; }
            public object winner_id { get; set; }
            public object winner_type { get; set; }
        }

        public class Videogame
        {
            public int id { get; set; }
            public string name { get; set; }
            public string slug { get; set; }
        }

        public class Winner2
        {
            public object acronym { get; set; }
            public int id { get; set; }
            public string image_url { get; set; }
            public string name { get; set; }
            public string slug { get; set; }
        }

        public class Match
        {
            public DateTime begin_at { get; set; }
            public bool detailed_stats { get; set; }
            public bool draw { get; set; }
            public DateTime end_at { get; set; }
            public bool forfeit { get; set; }
            public List<Game> games { get; set; }
            public int id { get; set; }
            public League league { get; set; }
            public int league_id { get; set; }
            public Live live { get; set; }
            public string live_url { get; set; }
            public string match_type { get; set; }
            public DateTime modified_at { get; set; }
            public string name { get; set; }
            public int number_of_games { get; set; }
            public List<Opponent> opponents { get; set; }
            public List<Result> results { get; set; }
            public DateTime scheduled_at { get; set; }
            public Serie serie { get; set; }
            public int serie_id { get; set; }
            public string slug { get; set; }
            public string status { get; set; }
            public Tournament tournament { get; set; }
            public int tournament_id { get; set; }
            public Videogame videogame { get; set; }
            public object videogame_version { get; set; }
            public Winner2 winner { get; set; }
            public int winner_id { get; set; }
        }

        public class Opponent3
        {
            public object acronym { get; set; }
            public int id { get; set; }
            public string image_url { get; set; }
            public string name { get; set; }
            public string slug { get; set; }
        }

        public class Player2
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

        public class Team
        {
            public object acronym { get; set; }
            public int id { get; set; }
            public string image_url { get; set; }
            public string name { get; set; }
            public string slug { get; set; }
        }

        public class Player
        {
            public double adr { get; set; }
            public int assists { get; set; }
            public int deaths { get; set; }
            public int first_kills_diff { get; set; }
            public int flash_assists { get; set; }
            public int game_id { get; set; }
            public int headshots { get; set; }
            public int k_d_diff { get; set; }
            public double kast { get; set; }
            public int kills { get; set; }
            public Opponent3 opponent { get; set; }
            public Player2 player { get; set; }
            public double rating { get; set; }
            public Team team { get; set; }
        }

        public class Round
        {
            public int ct { get; set; }
            public string outcome { get; set; }
            public int round { get; set; }
            public int terrorists { get; set; }
            public string winner_side { get; set; }
            public int winner_team { get; set; }
        }

        public class RoundsScore
        {
            public int score { get; set; }
            public int team_id { get; set; }
        }

        public class Team2
        {
            public object acronym { get; set; }
            public int id { get; set; }
            public string image_url { get; set; }
            public string name { get; set; }
            public string slug { get; set; }
        }

        public class Winner3
        {
            public int id { get; set; }
            public string type { get; set; }
        }

        public class RootObject
        {
            public DateTime begin_at { get; set; }
            public bool detailed_stats { get; set; }
            public DateTime end_at { get; set; }
            public bool finished { get; set; }
            public bool forfeit { get; set; }
            public int id { get; set; }
            public int length { get; set; }
            public Map map { get; set; }
            public Match match { get; set; }
            public int match_id { get; set; }
            public List<Player> players { get; set; }
            public int position { get; set; }
            public List<Round> rounds { get; set; }
            public List<RoundsScore> rounds_score { get; set; }
            public string status { get; set; }
            public List<Team2> teams { get; set; }
            public object video_url { get; set; }
            public Winner3 winner { get; set; }
            public string winner_type { get; set; }
        }

    }

}
