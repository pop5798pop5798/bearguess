using GoogleCloudSamples.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using SITDto;
using SITW.Models;
using SITW.Models.Repository;
using SITW.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpGatewayHelper;
using SpGatewayHelper.Models;
using SpGatewayHelper.Helper;

namespace SITW.Controllers
{
    [Authorize]
    public class WebMallController : Controller
    {
        private ApplicationUserManager _userManager;
        private ImageUploader _imageUploader;
        // GET: WebMall
        [AllowAnonymous]
        public ActionResult Index()
        {
            ViewData["cfgUnit"] = new UnitsRepository().getAll();
            List<Product> pd = new MallRepository().getAll().ToList();
            ViewData["ProductM"] = new MallRepository().PMenuGetAll();
            return View(pd);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Mall()
        {
            ViewData["cfgUnit"] = new UnitsRepository().getAll();
            List<Product> pd = new MallRepository().getAll();
            return View(pd);
        }


        /*public ActionResult Pay()
        {
            return View();
        }*/

        public ActionResult Pay(int id)
        {           
            Product product = new MallRepository().Get(id);
            cfgUnit unit = new UnitsRepository().getValid(product.unitSn);
            ProductApiModel pm = new ProductApiModel();
            pm.unit = unit;
            pm.product = product;


            return View(pm);
        }

        public ActionResult ChangeRecord()
        {
            var pdrlist = new MallRepository().GetAllProductRecord();
            var crm = new List<ProductRecordModel>();
            pdrlist = pdrlist.Where(x => x.unitSn == 2 && x.type != -3).ToList();
            

            foreach (var pd in pdrlist)
            {
                var cr = new ProductRecordModel();
                var user = UserManager.FindById(pd.UserID);
                cr.change_name = user.UserName;
                cr.change_email = user.Email;
                cr.productRecord = pd;
                cr.produc = new MallRepository().Get((int)pd.ProductId);

                crm.Add(cr);
            }


            return View(crm);
        }
        [HttpPost]
        public void Change_Confirm(int id)
        {
            var pr = new MallRepository().GetProductRecord(id);
            pr.p_confirm = 1;
            new MallRepository().PRUpdate(pr);
            //return View(pr);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult _FishPay(int id)
        {
            aJaxDto ajd = new aJaxDto();
            try
            {
                if (User.Identity.GetUserId() == null)
                {
                    //ajd.ErrorMsg = "下注前須先登入\n請先至會員登入中心進行登入註冊，謝謝";
                    ajd.isTrue = false;
                    throw new Exception("error");
                }

                    Product product = new MallRepository().Get(id);
                    List<Product> productAll = new MallRepository().getAll();
                    cfgUnit unit = new UnitsRepository().getValid(product.unitSn);
                    ProductApiModel pm = new ProductApiModel();
                    pm.unit = unit;
                    pm.product = product;
                    pm.producList = productAll.Where(x => x.unitSn == 3 && x.type == 3).ToList();
                    return View(pm);

                
            }
            catch
            {
                return Json(ajd);
            }
           
        }

        [HttpGet]
        public ActionResult _ProductRecord()
        {
            
            var pdrlist = new MallRepository().GetProductRecord(User.Identity.GetUserId());
            List<ProductApiModel> pml = new List<ProductApiModel>();
            foreach (var p in pdrlist.Where(x=>x.type != -3))
            {
                
                    ProductApiModel pm = new ProductApiModel();
                    pm.productRecord = p;
                    pm.product = new MallRepository().Get((int)p.ProductId);                             
                    pm.unit = new UnitsRepository().getValid(pm.product.unitSn);
                    if (pm.product.unitSn == 2 && pm.product.type == 3)
                    {
                        pm.product.ProductName = "兌換魚骨幣 "+ p.assets;
                        pm.product.Price = p.assets;
                    }
                    pml.Add(pm);


            }

           

            return View(pml.OrderByDescending(x => x.productRecord.inpdate));
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult _MallChange(int id)
        {
             aJaxDto ajd = new aJaxDto();
            try {
                if (User.Identity.GetUserId() == null)
                {
                    //ajd.ErrorMsg = "下注前須先登入\n請先至會員登入中心進行登入註冊，謝謝";
                    ajd.isTrue = false;
                    throw new Exception("error");
                }
                ProductApiModel pm = new ProductApiModel();

                Product product = new MallRepository().Get(id);
                // List<Product> productAll = new MallRepository().getAll();
                cfgUnit unit = new UnitsRepository().getValid(product.unitSn);

                pm.unit = unit;
                pm.product = product;
                //pm.producList = productAll.Where(x => x.id < 5).ToList();
                var gold = new AssetsRepository().getAssetsListByUserID(User.Identity.GetUserId()).Where(x => x.unitSn == product.unitSn).FirstOrDefault();
                ViewBag.gold = (gold != null) ? gold.Asset : 0;
                ViewBag.change = (product.Price > ViewBag.gold) ? 0 : 1;
                return View(pm);

            }
            catch
            {
                //ajd.isTrue = false;
                return Json(ajd);
            }







        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult _MoneyChange(int id)
        {
            aJaxDto ajd = new aJaxDto();
            try
            {
                if (User.Identity.GetUserId() == null)
                {
                    //ajd.ErrorMsg = "下注前須先登入\n請先至會員登入中心進行登入註冊，謝謝";
                    ajd.isTrue = false;
                    throw new Exception("error");
                }
                ProductApiModel pm = new ProductApiModel();

                Product product = new MallRepository().Get(id);
                // List<Product> productAll = new MallRepository().getAll();
                cfgUnit unit = new UnitsRepository().getValid(product.unitSn);

                pm.unit = unit;
                pm.product = product;
                //pm.producList = productAll.Where(x => x.id < 5).ToList();
                var gold = new AssetsRepository().getAssetsListByUserID(User.Identity.GetUserId()).Where(x => x.unitSn == product.unitSn).FirstOrDefault();
                ViewBag.gold = (gold != null) ? gold.Asset : 0;
                ViewBag.change = (product.Price > ViewBag.gold) ? 0 : 1;
                return View(pm);

            }
            catch
            {
                //ajd.isTrue = false;
                return Json(ajd);
            }







        }

        [HttpPost]
        public int MallChange(int id,float? money)
        {
            var userId = User.Identity.GetUserId();
           // var malldata = new MangerRepository().GetTransferRecords(userId);
            Product product = new MallRepository().Get(id);
            // 1 現金換魚骨 -2 兌獎 -3 鮭魚換魚骨
            
            ProductRecord assr = new ProductRecord
            {
                UserID = User.Identity.GetUserId(),
                unitSn = product.unitSn,
                assets = (money != null) ? -money : -product.Price,
                inpdate = DateTime.Now,
                type = (money != null)?-3:-2,
                ProductId = product.id
            };
            bool aset = new AssetsRepository().AddBearByAssets(assr);
            if(product.unitSn == 2 && product.type == 3)
            {
               assr = new ProductRecord
                {
                    UserID = User.Identity.GetUserId(),
                    unitSn = 1,
                    assets = money,
                    inpdate = DateTime.Now,
                    type = 1,
                    ProductId = product.id
                };
                aset = new AssetsRepository().AddBearByAssets(assr);
            }


            return aset?1:0;
        }

        [HttpGet]
        public ActionResult _Invoice(int id)
        {
            Product product = new MallRepository().Get(id);
            cfgUnit unit = new UnitsRepository().getValid(product.unitSn);
            ProductApiModel pm = new ProductApiModel();
            pm.unit = unit;
            pm.product = product;

            
            
            string _Vi = "ugZbqRhI6x5LGI94";
            string _key = "CON3KthrvPulsAWQQiQ3jsswLIzxxgQK";
            string MerchantOrderNo = DateTime.Now.Ticks.ToString() + "0" + pm.product.id.ToString();
            var user = UserManager.FindById(User.Identity.GetUserId());
            var tradeInfo = new TradeInfo()
            {
                MerchantID = "MS15822085",
                RespondType = "JSON",
                TimeStamp = DateTime.Now.ToUnixTimeStamp(),
                Version = "1.5",
                Amt = (int)pm.product.Price,
                ItemDesc = pm.product.ProductName,
                //InstFlag="3,6",
                //CreditRed = 0,
                Email = user.Email,
                EmailModify = 1,
                LoginType = 0,
                MerchantOrderNo = MerchantOrderNo,
                TradeLimit = 180
            };
            var postData = tradeInfo.ToDictionary();
            var cryptoHelper = new CryptoHelper(_key, _Vi);
            var aesString = cryptoHelper.GetAesString(postData);

            ViewData["TradeInfo"] = aesString;
            ViewData["TradeSha"] = cryptoHelper.GetSha256String(aesString);
            ViewData["Email"] = tradeInfo.Email;
            ViewBag.TimeStamp = tradeInfo.TimeStamp;
            ViewBag.MerchantOrderNo = tradeInfo.MerchantOrderNo;
            User_CashReturn re = new User_CashReturn
            {
                userId = User.Identity.GetUserId(),
                MerchantID = MerchantOrderNo,
                productId = pm.product.id,
                inpdate= DateTime.Now,
            };

            new MallRepository().CreateReturnRecord(re);


            //ViewBag.sha256 = getHashSha256("HashKey=CON3KthrvPulsAWQQiQ3jsswLIzxxgQK&Amt="+pm.product.Price+"&MerchantID=MS15822085&MerchantOrderNo="+ time + "&TimeStamp="+ time + "&Version=1.5&HashIV=ugZbqRhI6x5LGI94");





            return View(pm);
        }


       


        [HttpPost]
        public ActionResult paymentRequest(int pm, string prime)
        {
            Product productModel = new MallRepository().Get(pm);
            //User.Identity.GetUserId()
            var user = UserManager.FindById(User.Identity.GetUserId());
            string url = "https://sandbox.tappaysdk.com/tpc/payment/pay-by-prime";
            string partner_key = "partner_FAzkt3BlZbSejs0ZVxzJgulcU8vHW8LvBx7wcX2VHxmbgZsyyRZH564q";
            string product = productModel.ProductName;
            string money = productModel.Price.ToString();
            string phone_number = user.PhoneNumber;
            string name = User.Identity.GetUserName();
            string email = user.Email;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json";
            request.Method = "POST";
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string json = "{\"partner_key\":\""+ partner_key + "\"," +
                    "\"prime\":\""+ prime +"\"," +
                    "\"amount\":\""+ money +"\"," +
                    "\"merchant_id\":\"pop5798pop5798_CTBC\"," +
                    "\"details\":\""+ product + "\"," +
                    "\"cardholder\":{" +
                        "\"phone_number\":\""+ phone_number + "\"," +
                        "\"name\":\""+ name + "\"," +
                        "\"email\":\""+ email + "\"" +                     
                    "}}";

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
            var httpResponse = (HttpWebResponse)request.GetResponse();

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                ProductRecord assr = new ProductRecord
                {
                    UserID = User.Identity.GetUserId(),
                    unitSn = 1,
                    assets = (double)productModel.transform,
                    inpdate = DateTime.Now,
                    type = 1,
                    ProductId = pm
                };
                new AssetsRepository().AddBearByAssets(assr);
                //new AssetsRepository().AddAssetsByAssets(AssetsRecord ar)
                return Json(result, "application/json", JsonRequestBehavior.AllowGet);
            }

            
        }


        [HttpPost]
        [AllowAnonymous]
        public ActionResult bluenewRequest(SpGatewayResponse response)
        {

            // Product productModel = new MallRepository().Get(pm);
            //User.Identity.GetUserId()
            /* var user = UserManager.FindById(User.Identity.GetUserId());

                 ProductRecord assr = new ProductRecord
                 {
                     UserID = User.Identity.GetUserId(),
                     unitSn = 1,
                     assets = (double)productModel.transform,
                     inpdate = DateTime.Now,
                     type = 1,
                     ProductId = pm
                 };
                 new AssetsRepository().AddBearByAssets(assr);*/
            //new AssetsRepository().AddAssetsByAssets(AssetsRecord ar)
            ///return Json(result, "application/json", JsonRequestBehavior.AllowGet);
            ///
            //var json = DecryptAES256(TradeInfo.ToString());
          
            response.Key = "CON3KthrvPulsAWQQiQ3jsswLIzxxgQK";
            response.Vi = "ugZbqRhI6x5LGI94";
            var success = response.Validate("MS15822085");
            
            if (success)
            {
                var tradInfoModel = response.GetResponseModel<TradeInfoModel>();
                //var OrderNoToProuctId = tradInfoModel.Result.MerchantOrderNo.OrderNoToProuctId();
                
                User_CashReturn user = new MallRepository().GetUserPRecord(tradInfoModel.Result.MerchantOrderNo);
                Product productModel = new MallRepository().Get((int)user.productId);
                

                ProductRecord assr = new ProductRecord
                 {
                     UserID = user.userId,
                     unitSn = 1,
                     assets = productModel.transform,
                     inpdate = DateTime.Now,
                     type = 1,
                     ProductId = productModel.id
                 };
                 new AssetsRepository().AddBearByAssets(assr);


               /* User_CashReturn re = new User_CashReturn
                {
                    Status = response.Status,
                    MerchantID = OrderNoToProuctId,
                    TradeInfo = response.TradeInfo,
                    TradeSha = response.TradeSha,
                    Version = "1.5"
                };

                new MallRepository().CreateReturnRecord(re);*/
                //return Json(re, "application/json", JsonRequestBehavior.AllowGet);
            }
            return View();

            // var json = DecryptAES256(Tradeinfo);
            //string jsonString = JsonConvert.SerializeObject(json, Formatting.Indented, new JsonSerializerSettings { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii });

            //var json = new string[]{ "123" };


            //return Json();



        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: WebMall/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: WebMall/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {          
            //Product mall = new MallRepository().getAll();
            Product pt = new Product();
            return View(pt);
        }

        // POST: WebMall/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async System.Threading.Tasks.Task<ActionResult> Create(Product pt, HttpPostedFileBase image)
        {
            try
            {
                if (string.IsNullOrEmpty(pt.image) && image != null)
                {
                    string filename = "";
                    filename = DateTime.Now.ToString("yyyyMMddTHHmmssfff");
                    Google.Apis.Auth.OAuth2.GoogleCredential credential = await Google.Apis.Auth.OAuth2.GoogleCredential.GetApplicationDefaultAsync();
                    _imageUploader = new ImageUploader(System.Web.Configuration.WebConfigurationManager.AppSettings["GoogleCloud:BucketName"]);
                    var imageUrl = await _imageUploader.UploadImage(image, filename, "Mall");
                    pt.image = imageUrl;
                }
                pt.valid = 1;

                if(pt.unitSn == 1 || pt.unitSn == 2)
                {
                    pt.Price = pt.Price * 1.2 * 100000 / 30;

                }
                if(pt.createDate == null)
                {
                    pt.createDate = DateTime.Now;
                }
                
                new MallRepository().MallCreate(pt);
                // TODO: Add insert logic here

                return RedirectToAction("Mall");
            }
            catch
            {
                return View();
            }
        }

        // GET: WebMall/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            var mall = new MallRepository().Get((int)id);
            if ((mall.unitSn == 1 || mall.unitSn == 2) && mall.type != 3)
            {
                mall.Price = mall.Price * 30  / 100000 / 1.2;
            }
            return View(mall);
        }

        // POST: WebMall/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async System.Threading.Tasks.Task<ActionResult> Edit(Product pd, HttpPostedFileBase image)
        {
            try
            {
                if (string.IsNullOrEmpty(pd.image) && image != null)
                {
                    string filename = "";
                    filename = DateTime.Now.ToString("yyyyMMddTHHmmssfff");
                    Google.Apis.Auth.OAuth2.GoogleCredential credential = await Google.Apis.Auth.OAuth2.GoogleCredential.GetApplicationDefaultAsync();
                    _imageUploader = new ImageUploader(System.Web.Configuration.WebConfigurationManager.AppSettings["GoogleCloud:BucketName"]);
                    var imageUrl = await _imageUploader.UploadImage(image, filename, "Mall");
                    pd.image = imageUrl;
                }
                if ((pd.unitSn == 1 || pd.unitSn == 2) && pd.type != 3)
                {
                    pd.Price = pd.Price * 1.2 * 100000 / 30;

                }
                // TODO: Add update logic here
                new MallRepository().Update(pd);
                return RedirectToAction("Mall");
            }
            catch
            {
                return View();
            }
        }

        // GET: WebMall/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("Mall");
            }
            else
            {
                //var category = this.newsRepository.Get(id.Value);
                var mall = new MallRepository().Get((int)id);
                return View(mall);
            }
        }

        // POST: WebMall/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int? id)
        {
            try
            {
                // TODO: Add delete logic here
                var mall = new MallRepository().Get((int)id);
                new MallRepository().Delete(mall);
                return RedirectToAction("Mall");
            }
            catch (DataException)
            {
                return RedirectToAction("Delete", new { id = id });
            }
            //return RedirectToAction("Mall");
        }
    }
}
