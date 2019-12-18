using RestSharp;
using RestSharp.Authenticators;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace SITW.Services
{
    public class DownloadService
    {
        private string processMsg = "";
        private string userID = System.Web.Configuration.WebConfigurationManager.AppSettings["Office365Account"];
        private string password = System.Web.Configuration.WebConfigurationManager.AppSettings["Office365PWD"];

        public DownloadService()
        { }

        /// <summary>
        /// 下載Email
        public string DownLoad(string url,string team)
        {

            string imageurl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host + "/DImage/" + team + ".png";
            string iurl = "";
            try
            {
               
                WebRequest request = WebRequest.Create(imageurl);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK)
                    iurl =  HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host + "/DImage/"  +team + ".png";
            }
            catch (Exception)
            {

                string path = url;//下載圖片的地址
                string newPath = HttpContext.Current.Server.MapPath("~/DImage/");//目標地址

                string ImsFileName = team + ".png"; //目標檔名

                System.Net.WebRequest imgRequest = System.Net.WebRequest.Create(path);
                HttpWebResponse res;
                try
                {
                    res = (HttpWebResponse)imgRequest.GetResponse();
                }
                catch (WebException ex)
                {
                    res = (HttpWebResponse)ex.Response;
                }
                if (res.StatusCode.ToString() == "OK")
                {
                    System.Drawing.Image dwonImage = System.Drawing.Image.FromStream(imgRequest.GetResponse().GetResponseStream());
                    if (!System.IO.Directory.Exists(newPath))//目標地址不存在自動建立
                    {
                        System.IO.Directory.CreateDirectory(newPath);
                    }
                    dwonImage.Save(newPath + ImsFileName);
                    dwonImage.Dispose();
                }
                //Uri uri = Request.Url;

                iurl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host + "/DImage/" + ImsFileName;

            }

            return iurl;
        }





    }
}