using Dapper;
using Newtonsoft.Json;
using SITW.Models.GameAPIModels;
using SITW.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using unirest_net.http;




namespace SITW.Models.Repository
{
    public class CSGORepository
    {

        public async System.Threading.Tasks.Task<List<CSGOUpcoming.RootObject>> GetCSGOUpcoming()
        {
            //Dota2Model.RootObject dota2 = null;
            string url = "https://api.pandascore.co/csgo/matches/upcoming?token=lhqJrfIEI-Qsz1FX0AV1W4b8QJtOPTS78ykqU2p6zI2BWA1D9KY";

            JsonSerializerSettings microsoftDateFormatSettings = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Local
            };


            List<CSGOUpcoming.RootObject> data = null;
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = response.Content.ReadAsStringAsync().Result;
                    data = JsonConvert.DeserializeObject<List<CSGOUpcoming.RootObject>>(jsonResponse, microsoftDateFormatSettings);


                }
            }


            return data;
        }


        public async System.Threading.Tasks.Task<CSGOMatches.RootObject> GetCSGOMatches(int gameSn)
        {
            //Dota2Model.RootObject dota2 = null;
            string url = "https://api.pandascore.co/csgo/matches?filter[id]=" + gameSn + "&token=76QMCPlm65iZx8TgwGFLho-TUadJ7U1UeuogzSwltFzwAeksRqQ";

            JsonSerializerSettings microsoftDateFormatSettings = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Local
            };


            List<CSGOMatches.RootObject> data = null;
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = response.Content.ReadAsStringAsync().Result;
                    data = JsonConvert.DeserializeObject<List<CSGOMatches.RootObject>>(jsonResponse, microsoftDateFormatSettings);


                }
            }


            return data.FirstOrDefault();
        }

        public async System.Threading.Tasks.Task<CSGOGame.RootObject> GetCSGOGame(int gameSn)
        {
            //Dota2Model.RootObject dota2 = null;
            string url = "https://api.pandascore.co/csgo/games/" + gameSn + "?token=76QMCPlm65iZx8TgwGFLho-TUadJ7U1UeuogzSwltFzwAeksRqQ";

            JsonSerializerSettings microsoftDateFormatSettings = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Local
            };


           CSGOGame.RootObject data = null;
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = response.Content.ReadAsStringAsync().Result;
                    data = JsonConvert.DeserializeObject<CSGOGame.RootObject>(jsonResponse, microsoftDateFormatSettings);


                }
            }


            return data;
        }

        public async System.Threading.Tasks.Task<CSGOTeam.RootObject> GetCSGOTeam(int gameSn)
        {
            //Dota2Model.RootObject dota2 = null;
            string url = "https://api.pandascore.co/csgo/teams?filter[id]=" + gameSn + "&token=76QMCPlm65iZx8TgwGFLho-TUadJ7U1UeuogzSwltFzwAeksRqQ";

            JsonSerializerSettings microsoftDateFormatSettings = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Local
            };


            List<CSGOTeam.RootObject> data = null;
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = response.Content.ReadAsStringAsync().Result;
                    data = JsonConvert.DeserializeObject<List<CSGOTeam.RootObject>>(jsonResponse, microsoftDateFormatSettings);


                }
            }


            return data.FirstOrDefault();
        }






    }
}
