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
    public class Dota2Python
    {
        public int id { get; set; }
        public string team1 { get; set; }
        public string team1_image { get; set; }
        public string team2 { get; set; }
        public string team2_image { get; set; }

        public string Model { get; set; }
        public string time { get; set; }
        public string leagues { get; set; }

    }

}
