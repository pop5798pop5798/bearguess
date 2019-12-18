using SITDto;
using SITDto.Request;
using SITDto.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Caching;
using System.Web;
using System.Web.Script.Serialization;

namespace SITW.Models.Repository
{
    public class DgonHonusRepository
    {
        private static MemoryCache _cache = MemoryCache.Default;
        HttpClient client;

        public DgonHonusRepository()
        {
            client = new HttpClient();
            client.Timeout = TimeSpan.FromMilliseconds(200000);
            client.BaseAddress = new Uri(System.Web.Configuration.WebConfigurationManager.AppSettings["apiurl"]);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        }


        public async System.Threading.Tasks.Task<SettingListDto> GetBonusList()
        {

            //List<gameDto> gameList = null;

            /* lock (_cache)
             {
                 if (_cache.Contains("setSetting"))
                     return _cache.Get("setSetting") as SettingListDto;
             }*/



             return await bonusGameListAsync();
        }



        public async System.Threading.Tasks.Task<SettingListDto> bonusGameListAsync()
        {
            SettingListDto gameList = null;
            HttpResponseMessage response = await client.GetAsync("/api/gamesbonus");
            if (response.IsSuccessStatusCode)
            {
                gameList = await response.Content.ReadAsAsync<SettingListDto>();
            }

            return gameList;
        }
         
      

        public async System.Threading.Tasks.Task<SettingListDto> GetSetting(SettingListDto setting)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync("api/setSetting", setting);
            var result = await response.Content.ReadAsStringAsync();
            JavaScriptSerializer json_serializer = new JavaScriptSerializer();
            setting = json_serializer.Deserialize<SettingListDto>(result);
            response.EnsureSuccessStatusCode();

           // await reflashGameListAsync();

            return setting;
        }

        public async System.Threading.Tasks.Task<SettingListDto> topicSettingCreate(SettingListDto setting)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync("api/setSettingCreate", setting);
            var result = await response.Content.ReadAsStringAsync();
            JavaScriptSerializer json_serializer = new JavaScriptSerializer();
            setting = json_serializer.Deserialize<SettingListDto>(result);
            response.EnsureSuccessStatusCode();

            // await reflashGameListAsync();

            return setting;
        }





    }
}