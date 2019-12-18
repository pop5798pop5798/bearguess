using Dapper;
using Newtonsoft.Json;
using SITW.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Xml;
using System.Xml.Serialization;

namespace SITW.Models.Repository
{
    public class InvoiceRepository
    {
        private sitwEntities Db = new sitwEntities();
        //發票待發
        public void WaitCreate(InvoiceWait instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                Db.InvoiceWait.Add(instance);
                this.SaveChanges();

            }


        }

        //發票記錄
        public void InvoiceCreate(Invoice instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                Db.Invoice.Add(instance);
                this.SaveChanges();

            }


        }

        //發票發送
        public SmilePayEinvoice invoiceSend(string Merchant, Product p)
        {
            try
            {
                //電子發票API的Url
                string url = "https://ssl.smse.com.tw/api/SPEinvoice_Storage.asp";
                //string url = "https://ssl.smse.com.tw/api_test/SPEinvoice_Storage.asp";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                string invoiceDate = DateTime.Now.ToString("yyyy/MM/dd");
                string invoiceTime = DateTime.Now.ToString("hh:mm:ss");

                var wait = this.GetWait(Merchant);

                string DonateMark = "0";
                string CarrierType = "";
                string Buyer_id = "";
                string CarrierID = "";
                string LoveKey = "";
                string Address = "";
                string Name = "";
                string CompanName = "";

                if (!string.IsNullOrEmpty(wait.LoveKey))
                {
                    DonateMark = "1";
                    LoveKey = wait.LoveKey;

                }
                else
                {
                    if (!string.IsNullOrEmpty(wait.Buyer_id))
                    {
                        Buyer_id = wait.Buyer_id;
                        CompanName = wait.Name;
                        Address = wait.County.Substring(0, 3) + wait.City + wait.County.Substring(wait.County.Length - 3, 3) + wait.Address;
                    }
                    else if (wait.CarrierType == "1")
                    {
                        Name = wait.Name;
                        Address = wait.County.Substring(0, 3) + wait.City + wait.County.Substring(wait.County.Length - 3, 3) + wait.Address;
                    }
                    else
                    {
                        CarrierType = wait.CarrierType;
                        CarrierID = wait.CarrierID;

                    }

                }







                //必須透過ParseQueryString()來建立NameValueCollection物件，之後.ToString()才能轉換成queryString
                NameValueCollection postParams = System.Web.HttpUtility.ParseQueryString(string.Empty);
                postParams.Add("Grvc", "SEI1000165");
                postParams.Add("Verify_key", "5965AE54773AA37E28120541E69EE0C2");
                postParams.Add("InvoiceDate", invoiceDate);
                postParams.Add("InvoiceTime", invoiceTime);
                postParams.Add("intype", "07"); // 07:一般 08: 特種
                postParams.Add("TaxType", "1"); //1:應稅 2:零稅率 3:免稅 4:應稅(特種) 9:混稅 
                postParams.Add("DonateMark", DonateMark); //1:捐贈 20:不捐贈
                postParams.Add("LoveKey", LoveKey); //愛心碼 捐贈為1時需要有值
                                                    //商品
                postParams.Add("Description", p.ProductName);
                postParams.Add("Quantity", "1");//數量
                postParams.Add("UnitPrice", p.Price.ToString());//單價
                postParams.Add("Amount", p.Price.ToString());//總額
                postParams.Add("AllAmount", p.Price.ToString());//總額(含稅)
                                                                //購買人資訊
                postParams.Add("Buyer_id", Buyer_id);//統編
                postParams.Add("CarrierType", CarrierType);//載具類型
                postParams.Add("CarrierID", CarrierID);//載具明碼
                postParams.Add("CompanyName", CompanName);//公司名稱
                postParams.Add("Name", Name);//購買人姓名
                postParams.Add("Address", Address);//購買人地址


                postParams.Add("Email", wait.Email); //信箱


                //Console.WriteLine(postParams.ToString());// 將取得"version=1.0&action=preserveCodeCheck&pCode=pCode&TxID=guid&appId=appId", key和value會自動UrlEncode
                //要發送的字串轉為byte[] 
                byte[] byteArray = Encoding.UTF8.GetBytes(postParams.ToString());
                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(byteArray, 0, byteArray.Length);
                }//end using

                JsonSerializerSettings microsoftDateFormatSettings = new JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                    DateTimeZoneHandling = DateTimeZoneHandling.Local
                };

                //API回傳的字串
                string responseStr = "";
                SmilePayEinvoice data = null;
                //發出Request
                using (WebResponse response = request.GetResponse())
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        //data = (InvoiceModel)serializer.Deserialize(sr.ReadToEnd());
                        //responseStr = sr.ReadToEnd();
                        responseStr = sr.ReadToEnd();
                    }
                    //end using  
                }
                XmlSerializer serializer = new XmlSerializer(typeof(SmilePayEinvoice));
                using (TextReader reader = new StringReader(responseStr))
                {
                    data = (SmilePayEinvoice)serializer.Deserialize(reader);
                }




                return data;



            }
            catch
            {
                return null;

            }




        }

        //發票欄位驗證
        public string InvoiceIDSend(string Buyer_id,string DonateMark,string LoveKey,string CarrierType,string CarrierID)
        {
            string status = "success";
            try
            {
                //電子發票API的Url
                string url = "https://ssl.smse.com.tw/api_test/SPEinvoice_Storage.asp";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                string invoiceDate = DateTime.Now.ToString("yyyy/MM/dd");
                string invoiceTime = DateTime.Now.ToString("hh:mm:ss");


                string Address = "";
                string Name = "";
                string CompanName = "test";


                //必須透過ParseQueryString()來建立NameValueCollection物件，之後.ToString()才能轉換成queryString
                NameValueCollection postParams = System.Web.HttpUtility.ParseQueryString(string.Empty);
                postParams.Add("Grvc", "SEI1000034");
                postParams.Add("Verify_key", "9D73935693EE0237FABA6AB744E48661");
                postParams.Add("InvoiceDate", invoiceDate);
                postParams.Add("InvoiceTime", invoiceTime);
                postParams.Add("intype", "07"); // 07:一般 08: 特種
                postParams.Add("TaxType", "1"); //1:應稅 2:零稅率 3:免稅 4:應稅(特種) 9:混稅 
                postParams.Add("DonateMark", DonateMark); //1:捐贈 20:不捐贈
                postParams.Add("LoveKey", LoveKey); //愛心碼 捐贈為1時需要有值

                postParams.Add("Description", "test");//商品
                postParams.Add("Quantity", "1");//數量
                postParams.Add("UnitPrice", "1");//單價
                postParams.Add("Amount", "1");//總額
                postParams.Add("AllAmount", "1");//總額(含稅)
                                                 //購買人資訊
                postParams.Add("Buyer_id", Buyer_id);//統編
                postParams.Add("CarrierType", CarrierType);//載具類型
                postParams.Add("CarrierID", CarrierID);//載具明碼
                postParams.Add("CompanyName", CompanName);//公司名稱
                postParams.Add("Name", Name);//購買人姓名
                postParams.Add("Address", Address);//購買人地址


                postParams.Add("Email", "test01@gmail.com"); //信箱


                //Console.WriteLine(postParams.ToString());// 將取得"version=1.0&action=preserveCodeCheck&pCode=pCode&TxID=guid&appId=appId", key和value會自動UrlEncode
                //要發送的字串轉為byte[] 
                byte[] byteArray = Encoding.UTF8.GetBytes(postParams.ToString());
                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(byteArray, 0, byteArray.Length);
                }//end using

                //API回傳的字串
                string responseStr = "";
                //發出Request
                using (WebResponse response = request.GetResponse())
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        responseStr = sr.ReadToEnd();
                    }//end using  
                }
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(responseStr);
                XmlNode Node_Status = xmlDoc.SelectSingleNode("/SmilePayEinvoice/Status");
                if (Node_Status.InnerText != "0")
                {
                    status = "error";
                }

                return status;


            }
            catch
            {

                return "error";
            }




        }


        public InvoiceWait GetWait(string Merchant)
        {

            return Db.InvoiceWait.Where(x => x.MerchantOrder == Merchant).FirstOrDefault();

        }

        public Invoice GetInvoice(int w)
        {

            return Db.Invoice.Where(x=>x.IwaitId == w).FirstOrDefault();

        }


        public void SaveChanges()
        {
            this.Db.SaveChanges();
        }




    }
}