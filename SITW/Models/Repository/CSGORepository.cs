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
            string url = "https://api.pandascore.co/csgo/matches/upcoming?token=76QMCPlm65iZx8TgwGFLho-TUadJ7U1UeuogzSwltFzwAeksRqQ&per_page=150";

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

        public async System.Threading.Tasks.Task<List<CSGOUpcoming.RootObject>> GetCSGOSUpcoming(int id)
        {
            //Dota2Model.RootObject dota2 = null;
            string url = "https://api.pandascore.co/csgo/matches/upcoming?token=76QMCPlm65iZx8TgwGFLho-TUadJ7U1UeuogzSwltFzwAeksRqQ&per_page=150&filter[serie_id]=" + id;

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

        public async System.Threading.Tasks.Task<List<CSGOLeaguesUpcoming.RootObject>> GetCSGOLeaguesUpcoming()
        {
            //Dota2Model.RootObject dota2 = null;
            string url = "https://api.pandascore.co/csgo/leagues?token=76QMCPlm65iZx8TgwGFLho-TUadJ7U1UeuogzSwltFzwAeksRqQ";

            JsonSerializerSettings microsoftDateFormatSettings = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Local
            };


            List<CSGOLeaguesUpcoming.RootObject> data = null;
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = response.Content.ReadAsStringAsync().Result;
                    data = JsonConvert.DeserializeObject<List<CSGOLeaguesUpcoming.RootObject>>(jsonResponse, microsoftDateFormatSettings);


                }
            }


            return data;
        }

        public async System.Threading.Tasks.Task<List<CSGOSeriesUpcoming.RootObject>> GetCSGOSeriessRunning()
        {
            //Dota2Model.RootObject dota2 = null;
            string url = "https://api.pandascore.co/csgo/series/running?token=76QMCPlm65iZx8TgwGFLho-TUadJ7U1UeuogzSwltFzwAeksRqQ";

            JsonSerializerSettings microsoftDateFormatSettings = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Local
            };


            List<CSGOSeriesUpcoming.RootObject> data = null;
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = response.Content.ReadAsStringAsync().Result;
                    data = JsonConvert.DeserializeObject<List<CSGOSeriesUpcoming.RootObject>>(jsonResponse, microsoftDateFormatSettings);


                }
            }


            return data;
        }
 

        public async System.Threading.Tasks.Task<List<CSGOSeriesUpcoming.RootObject>> GetCSGOSeriessUpcoming()
        {
            //Dota2Model.RootObject dota2 = null;
            string url = "https://api.pandascore.co/csgo/series/upcoming?token=76QMCPlm65iZx8TgwGFLho-TUadJ7U1UeuogzSwltFzwAeksRqQ";

            JsonSerializerSettings microsoftDateFormatSettings = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Local
            };


            List<CSGOSeriesUpcoming.RootObject> data = null;
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = response.Content.ReadAsStringAsync().Result;
                    data = JsonConvert.DeserializeObject<List<CSGOSeriesUpcoming.RootObject>>(jsonResponse, microsoftDateFormatSettings);


                }
            }


            return data;
        }


        public async System.Threading.Tasks.Task<List<LOLALL.RootObject>> GetLOLAll()
        {
            //Dota2Model.RootObject dota2 = null;
            string url = "https://api.pandascore.co/lol/matches/?token=lhqJrfIEI-Qsz1FX0AV1W4b8QJtOPTS78ykqU2p6zI2BWA1D9KY";

            JsonSerializerSettings microsoftDateFormatSettings = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Local
            };


            List<LOLALL.RootObject> data = null;
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = response.Content.ReadAsStringAsync().Result;
                    data = JsonConvert.DeserializeObject<List<LOLALL.RootObject>>(jsonResponse, microsoftDateFormatSettings);


                }
            }


            return data;
        }




    }
}