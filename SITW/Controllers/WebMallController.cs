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
using SITW.Services;
using System.Threading.Tasks;
using System.Net.Http;

namespace SITW.Controllers
{
    [Authorize]
    public class WebMallController : Controller
    {
        private ApplicationUserManager _userManager;
        private ImageUploader _imageUploader;
        private readonly IAddressService service;

        public WebMallController()
        {
            this.service = new AddressService();
        }

        // GET: WebMall
        [AllowAnonymous]
        public ActionResult Index()
        {
            List<PProductViewModel> pam = new List<PProductViewModel>();
            ViewData["cfgUnit"] = new UnitsRepository().getAll();
            List<Product> pd = new MallRepository().getAll().ToList();
            List<Preferential> pft = new PreferentialRepository().getAllvaild();
            foreach(var p in pd)
            {
                PProductViewModel pm = new PProductViewModel();
                pm.id = p.id;
                pm.image = p.image;
                pm.inpdate = p.inpdate;
                pm.Price = p.Price;
                pm.ProductContent = p.ProductContent;
                pm.transform = p.transform;
                pm.type = p.type;
                pm.unitSn = p.unitSn;
                pm.valid = p.valid;
                pm.ProductName = p.ProductName;
                pm.createDate = p.createDate;

                
               
               if(p.type == 1 || p.type == 2)
                {
                    if(pft.Where(x => x.offerModel == 3 && x.productId == null).FirstOrDefault() != null)
                    {
                        pm.offer = pft.Where(x => x.offerModel == 3 && x.productId == null).FirstOrDefault().offer;
                        pm.Pname = pft.Where(x => x.offerModel == 3 && x.productId == null).FirstOrDefault().Pname;
                    }
                   
                    foreach (var pt in pft.Where(x => x.offerModel == 3))
                    {
                        if (pt.productId == p.id)
                        {
                            pm.offer = pt.offer;
                            pm.Pname = pt.Pname;
                        }
                    }
                }
                
                pam.Add(pm);

            }
            //new MailServiceMailgun().ChangeSend("熊i猜兌獎通知信", "test","pop5798pop5798@gmail.com", "");
            ViewData["ProductM"] = new MallRepository().PMenuGetAll();
            return View(pam);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Mall()
        {
            ViewData["cfgUnit"] = new UnitsRepository().getAll();
            List<Product> pd = new MallRepository().getAll();
            return View(pd);
        }

       /* [AllowAnonymous]
        public ActionResult EmailPost()
        {
            var user = new UserRepository().GetAll();
            var EmailContent = EmailTemplatesService.GetAllUserEmailHTML("");
            foreach (var u in user)
            {
                new MailServiceMailgun().AllUserSend("燈燈燈燈!!!! 熊i猜V2.0 隆重開幕啦!!!", EmailContent, u.Email);
            }
            
            
            return View();
        }*/


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
                cr.change_address = pd.address;
                cr.produc = new MallRepository().Get((int)pd.ProductId);
                cr.change_phone = pd.phone;
                cr.change_recipient = pd.recipient;
                cr.pay = 0;
                var record = new MallRepository().GetProductRecord(pd.UserID).Where(x=>x.type == 1);
                foreach(var r in record)
                {
                    cr.pay += (float)new MallRepository().Get((int)r.ProductId).Price;
                }

                if (cr.produc != null)
                crm.Add(cr);
            }

            var pdListDy = crm.OrderByDescending(x=>x.productRecord.inpdate);


            return View(pdListDy);
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
                    ajd.ErrorMsg = "登入後才可以購買";
                    ajd.Title = "未登入!";
                    ajd.isTrue = true;
                    throw new Exception("error");
                }
                var user = UserManager.FindById(User.Identity.GetUserId());
                if (!user.EmailConfirmed)
                {
                    ajd.ErrorMsg = "請先至會員中心進行驗證，謝謝";
                    ajd.Title = "Email尚未驗證!";
                    ajd.isTrue = false;
                    throw new Exception("error");
                }
                if (!user.PhoneNumberConfirmed)
                {
                    ajd.ErrorMsg = "請先至會員中心進行驗證，謝謝";
                    ajd.Title = "手機尚未驗證!";
                    ajd.isTrue = false;
                    throw new Exception("error");
                }

                //var pr = new PreferentialRepository().getPRecords(User.Identity.GetUserId());

                //普通首儲
                /*if (pr == null)
                {
                    var prm = new PreferentialRecords {
                        UserId = User.Identity.GetUserId(),
                        Count = 1,
                        inpdate = DateTime.Now,
                        PreferentialID = 1
                    };

                    new PreferentialRepository().PRecordsCreate(prm);
                    pr = new PreferentialRepository().getPRecords(User.Identity.GetUserId());
                }*/
                var pr = new PreferentialRepository().getPRecordsAll(User.Identity.GetUserId()).Where(x=>x.PreferentialID == 5);
                //2倍首儲
                if (pr.Count() == 0)
                {
                    var m = new MallRepository().getAll().Where(x=>x.unitSn == 3 && x.type == 3).ToList();
                    foreach(var md in m)
                    {
                        var prm = new PreferentialRecords
                        {
                            UserId = User.Identity.GetUserId(),
                            Count = 1,
                            inpdate = DateTime.Now,
                            PreferentialID = 5,
                            productID = md.id
                            
                            
                        };

                        new PreferentialRepository().PRecordsCreate(prm);
                        //pr = new PreferentialRepository().getPRecords(User.Identity.GetUserId());

                    }


                    
                }
                pr = new PreferentialRepository().getPRecordsAll(User.Identity.GetUserId()).Where(x => x.PreferentialID == 5);



                Product product = new MallRepository().Get(id);
                    List<Product> productAll = new MallRepository().getAll();
                    cfgUnit unit = new UnitsRepository().getValid(product.unitSn);
                    ProductApiModel pm = new ProductApiModel();
                    pm.unit = unit;
                    pm.product = product;
                    pm.producList = productAll.Where(x => x.unitSn == 3 && x.type == 3).ToList();
   
                    List<int> fta = new List<int>();
                    foreach (var p in pr)
                    {
                        
                        if (p.Count != 0)
                        {
                            fta.Add(1);
                        }
                        else {
                            fta.Add(0);
                        }

                        
                    }
                    pm.firstTypeArray = fta;
                    if (pr.Where(x => x.Count != 0).Count() != 0)
                    {
                        pm.firstType = 1;
                    }
                    else {
                        pm.firstType = 0;
                    }
                    //pm.firstType = (pr.Count != 0) ? 1 : 0;
                    pm.preferential = new PreferentialRepository().getpreferential(1);
                    return View(pm);

                
            }
            catch
            {
                return Json(ajd, JsonRequestBehavior.AllowGet);
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
                    if(pm.product != null)
                    {
                        pm.unit = new UnitsRepository().getValid(pm.product.unitSn);
                        if (pm.product.unitSn == 2 && pm.product.type == 3)
                        {
                            DateTime start = Convert.ToDateTime("2019/10/16 12:00:00");
                            DateTime end = Convert.ToDateTime(pm.productRecord.inpdate);
                            DateTime dtNow = DateTime.Today;
                            pm.product.ProductName = "兌換魚骨幣 " + p.assets;
                            if((DateTime.Compare(end, dtNow) >= 0) && (DateTime.Compare(start, dtNow) >= 0))
                                pm.product.Price = p.assets / pm.product.Price;
                            else 
                                 pm.product.Price = p.assets;
                        }
                        else if (pm.productRecord.unitSn == 1 && pm.productRecord.type == 5)
                        {
                            pm.product.ProductName = "首儲贈送";
                            pm.product.Price = p.assets;
                            pm.unit = new UnitsRepository().getValid(1);
                        }

                        pml.Add(pm);
                    }
                    


            }

           

            return View(pml.OrderByDescending(x => x.productRecord.inpdate));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> _MallChange(int id)
        {
             aJaxDto ajd = new aJaxDto();
            try {
                if (User.Identity.GetUserId() == null)
                {
                    ajd.ErrorMsg = "登入後才可以購買";
                    ajd.Title = "未登入!";
                    ajd.isTrue = true;
                    throw new Exception("error");
                }
                var user = UserManager.FindById(User.Identity.GetUserId());
                if (!user.EmailConfirmed)
                {
                    ajd.ErrorMsg = "請先至會員中心進行驗證，謝謝";
                    ajd.Title = "Email尚未驗證!";
                    ajd.isTrue = false;
                    throw new Exception("error");
                }
                if (!user.PhoneNumberConfirmed)
                {
                    ajd.ErrorMsg = "請先至會員中心進行驗證，謝謝";
                    ajd.Title = "手機尚未驗證!";
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
                int offer = 100;

                List<Preferential> pft = new PreferentialRepository().getAllvaild();
                ViewBag.offer = 100;
                if (pft != null)
                {
                 
                        if (pft.Where(x => x.offerModel == 3 && x.productId == null).FirstOrDefault() != null)
                        {
                            ViewBag.offer = pft.Where(x => x.offerModel == 3 && x.productId == null).FirstOrDefault().offer;
                             offer = (int)pft.Where(x => x.offerModel == 3 && x.productId == null).FirstOrDefault().offer;
                        }

                        foreach (var pt in pft.Where(x => x.offerModel == 3))
                        {
                            if (pt.productId == id)
                            {
                                ViewBag.offer = pt.offer;
                                offer = (int)pft.Where(x => x.offerModel == 3 && x.productId == id).FirstOrDefault().offer;
                            }
                        }

                }

                var gold = new AssetsRepository().getAssetsListByUserID(User.Identity.GetUserId()).Where(x => x.unitSn == product.unitSn).FirstOrDefault();
                ViewBag.gold = (gold != null) ? gold.Asset : 0;
                ViewBag.change = (product.Price * offer / 100 > ViewBag.gold) ? 0 : 1;
           
               

            return View(pm);

         }
         catch
         {
             //ajd.isTrue = false;
             return Json(ajd, JsonRequestBehavior.AllowGet);
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
        public int MallChange(int id,float? money,string name,string phone,string address)
        {
            var userId = User.Identity.GetUserId();
           // var malldata = new MangerRepository().GetTransferRecords(userId);
            Product product = new MallRepository().Get(id);
            // 1 現金換魚骨 -2 兌獎 -3 鮭魚換魚骨

            int offer = 100;

            List<Preferential> pft = new PreferentialRepository().getAllvaild();
            if (pft != null)
            {

                if (pft.Where(x => x.offerModel == 3 && x.productId == null).FirstOrDefault() != null)
                {                   
                    offer = (int)pft.Where(x => x.offerModel == 3 && x.productId == null).FirstOrDefault().offer;
                }

                foreach (var pt in pft.Where(x => x.offerModel == 3))
                {
                    if (pt.productId == id)
                    {                     
                        offer = (int)pft.Where(x => x.offerModel == 3 && x.productId == id).FirstOrDefault().offer;
                    }
                }

            }

            ProductRecord assr = new ProductRecord
            {
                UserID = User.Identity.GetUserId(),
                unitSn = product.unitSn,
                assets = (money != null) ? -money : -product.Price * offer / 100,
                inpdate = DateTime.Now,
                type = (money != null)?-3:-2,
                ProductId = product.id,
                recipient = name,
                phone = phone,
                address = address
            };
            bool aset = new AssetsRepository().AddBearByAssets(assr);
            if(product.unitSn == 2 && product.type == 3)
            {
               assr = new ProductRecord
                {
                    UserID = User.Identity.GetUserId(),
                    unitSn = 1,
                    assets = money * product.Price,
                    inpdate = DateTime.Now,
                    type = 3,
                    ProductId = product.id,
                    recipient = name,
                    phone = phone,
                    address = address
                };
                aset = new AssetsRepository().AddBearByAssets(assr);
            }
            if(money == null)
            {
                var m = Math.Ceiling((double)product.Price / 1.2 / 30000 * 30);
                var d = 0;
                if(m < 1000)
                {
                    d = 1;
                }else if(999 < m && m < 20000)
                {
                    d = 2;
                }else if(m> 19999)
                {
                    d = 3;
                }
                var user = UserManager.FindById(User.Identity.GetUserId());
                var content = ConfirmChange(d, assr,product);
                var EmailContent = EmailTemplatesService.GetChangeEmailHTML(content);
                var h = Server.MapPath("\\Content\\Pdf\\Change\\");
                string url = (product.pdf_file != null) ?  product.pdf_file : "";
                new MailServiceMailgun().ChangeSend("熊i猜兌獎通知信", EmailContent, user.Email, url);
            }
           


            return aset?1:0;
        }


        private string ConfirmChange(int m,ProductRecord pr,Product p)
        {
            string s = "";
            if (m == 1)
            {
                s = "<h2 style='text-align: center;'>兌獎通知信</h2>" +
"<div style='text-align: left;padding:0 14%;'>" +
        "<h4 style='color:#222'>親愛的熊粉您好：<p>" +
        "我們已經收到您的兌獎申請，您本次申請的兌換資料如下：<p>" +
        "兌換獎項：" + p.ProductName + "<p>" +
        "獎項收件人：" + pr.recipient + "<p>" +
        "連絡電話：" + pr.phone + "<p>" +
        "寄送地址：" + pr.address + "<p></h4>" +
       "<h3 style='color:#500050'>我們預計將於申請完成後十五個工作天內進行獎品寄送，還請您耐心等候，如有任何問題，可至<a href='https://www.facebook.com/funbet.esport/'>熊i猜粉絲團</a>發訊詢問喔!<br>" +
        "(本兌獎通知為自動發送，請勿直接回信詢問，以避免延誤問題解決時間。)</h3><br>" +
"</div>";


            }
            else if(m == 2){
                s = "<h2 style='text-align: center;text-indent:2em;'>兌獎通知信</h2>" +
"<div style='text-align: left;padding:0 14%;'>" +
        "<h4 style='color:#222'>親愛的熊粉您好：<p>" +
        "我們已經收到您的兌獎申請，您本次申請的兌換資料如下：<p>" +
        "兌換獎項：" + p.ProductName + "<p>" +
        "獎項收件人：" + pr.recipient + "<p>" +
        "連絡電話：" + pr.phone + "<p>" +
        "寄送地址：" + pr.address + "<p></h4>" +
       "<h3 style='color:#500050;text-indent:2em;'>依所得稅法各類所得扣繳標準第十一條規定，凡獎金獎品價值超過NT$1,000元整，需申報得獎人個人所得，並繳交身分證影本報稅使用，故還請您填寫好本通知附件內的活動領獎單，並與附件以紙本掛號郵寄的方式回傳至本公司(407台中市西屯區市政路500號6樓之3競猜數位股份有限公司 收)。</h3>" +
"<h3 style='color:#500050;text-indent:2em;'>另兌獎人若為未滿20歲之未成年人，須檢附戶口名簿影本、法定代理人身分證正反面影本，並提出法定代理人所簽署之活動領獎單，方可辦理獎項領取。</h3>" +
"<h3 style='color:#500050;text-indent:2em;'>我們預計將於收到完整且正確的相關文件後十五個工作天內進行獎品寄送，還請您耐心等候，如有任何問題，可至<a href='https://www.facebook.com/funbet.esport/'>熊i猜粉絲團</a>發訊詢問喔!</h3>" +
"(本兌獎通知為自動發送，請勿直接回信詢問，以避免延誤問題解決時間。)" +
"</div>";

            }else if(m == 3)
            {
                s = "<h2 style='text-align: center;'>兌獎通知信</h2>" +
"<div style='text-align: left;padding:0 14%;'>" +
        "<h4 style='color:#222'>親愛的熊粉您好：<p>" +
        "我們已經收到您的兌獎申請，您本次申請的兌換資料如下：<p>" +
        "兌換獎項：" + p.ProductName + "</h4>" +
        "<h4 style='color:#222'>獎項收件人：" + pr.recipient + "<p>" +
        "連絡電話：" + pr.phone + "<p>" +
        "寄送地址：" + pr.address + "<p></h4>" +
       "<h3 style='color:#500050;text-indent:2em;'>依所得稅法各類所得扣繳標準第十一條規定，凡獎金獎品價值超過NT$20,000元以上，除需申報得獎人個人所得，並繳交身分證影本報稅使用之外，依法須自行給付10 % 中獎稅金，故還請您完成以下兩件事情方能兌獎：</h3>" +
"<h3 style='color:#500050;'>1. 填寫好本通知附件內的活動領獎單，並與附件以紙本掛號郵寄的方式回傳至本公司(407台中市西屯區市政路500號6樓之3競猜數位股份有限公司 收)。</h3>" +
"<h3 style='color:#500050;'>2. 繳納10 % 中獎稅金(<div style='color:red;display: unset;'>" + Math.Ceiling((double)p.Price / 1.2 / 30000 * 30 * 10 / 100) + "</div>元)於以下帳戶：<br>" +
"銀行：元大銀行 北台中分行<br>" +
"戶名：競猜數位科技股份有限公司<br>" +
"帳號：00108273291218<br>" +
"繳納後將匯款證明以拍照、截圖的方式，連同匯款帳號後五碼告知<a href='https://www.facebook.com/funbet.esport/'>熊i猜粉絲團</a>小編(發訊告知即可)。</h3>" +
"<h3 style='text-indent:2em;'>另兌獎人若為未滿20歲之未成年人，須檢附戶口名簿影本、法定代理人身分證正反面影本，並提出法定代理人所簽署之活動領獎單，方可辦理獎項領取。</h3>" +
"<h3 style='text-indent:2em;'>我們預計將於收到完整且正確的相關文件與稅金後十五個工作天內進行獎品寄送，還請您耐心等候，如有任何問題，可至<a href='https://www.facebook.com/funbet.esport/'>熊i猜粉絲團</a>發訊詢問喔!</h3><br>" +
"(本兌獎通知為自動發送，請勿直接回信詢問，以避免延誤問題解決時間。)" +
"</div>";

            }
           
            return s;

        }

        [HttpGet]
        public ActionResult _Invoice(int id)
        {
            Product product = new MallRepository().Get(id);
            cfgUnit unit = new UnitsRepository().getValid(product.unitSn);
            ProductApiModel pm = new ProductApiModel();
            pm.unit = unit;
            pm.product = product;

            
            //test
            //string _Vi = "ugZbqRhI6x5LGI94";
            //string _key = "CON3KthrvPulsAWQQiQ3jsswLIzxxgQK";

            string _Vi = "PmNER6HP23jikkcC";
            string _key = "382go6Z9UrDy3XBuJnCHzNusEYFnBfls";
            string MerchantOrderNo = DateTime.Now.Ticks.ToString() + "0" + pm.product.id.ToString();
            var user = UserManager.FindById(User.Identity.GetUserId());
            var tradeInfo = new TradeInfo()
            {
                //test//
                //MerchantID = "MS15822085",
                MerchantID = "MS3276146654",
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
                TradeLimit = 180,
                WEBATM = 1,
                VACC = 1,
                CVS = 1,
                BARCODE = 1
            };

            if (pm.product.Price != 1800)
            {
                tradeInfo.WEBATM = 1;
                tradeInfo.VACC = 1;
                tradeInfo.CVS = 0;
                tradeInfo.BARCODE = 0;

            }


            var postData = tradeInfo.ToDictionary();
            var cryptoHelper = new CryptoHelper(_key, _Vi);
            var aesString = cryptoHelper.GetAesString(postData);

            ViewData["TradeInfo"] = aesString;
            ViewData["TradeSha"] = cryptoHelper.GetSha256String(aesString);
            ViewData["Email"] = tradeInfo.Email;
            ViewBag.TimeStamp = tradeInfo.TimeStamp;
            ViewBag.MerchantOrderNo = tradeInfo.MerchantOrderNo;
            ViewBag.MerchantID = tradeInfo.MerchantID;
            ViewBag.WEBATM = 1;
            ViewBag.VACC = 1;
            ViewBag.CVS = 0;
            ViewBag.BARCODE = 0;


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

        public ActionResult GetCityDropDownlist(string selectedCity)
        {
            StringBuilder sb = new StringBuilder();

            var cities = this.service.GetAllCityDictinoary();

            if (string.IsNullOrWhiteSpace(selectedCity))
            {
                foreach (var item in cities)
                {
                    sb.AppendFormat("<option value=\"{0}\">{1}</option>", item.Key, item.Value);
                }
            }
            else
            {
                foreach (var item in cities)
                {
                    sb.AppendFormat("<option value=\"{0}\" {2}>{1}</option>",
                        item.Key,
                        item.Value,
                        item.Key.Equals(selectedCity) ? "selected=\"selected\"" : "");
                }
            }
            return Content(sb.ToString());
        }

        public ActionResult GetCountyDropDownlist(string cityName, string selectedCounty)
        {
            if (!string.IsNullOrWhiteSpace(cityName))
            {
                StringBuilder sb = new StringBuilder();

                var counties = this.service.GetCountyByCityName(cityName);

                if (string.IsNullOrWhiteSpace(selectedCounty))
                {
                    foreach (var item in counties)
                    {
                        sb.AppendFormat("<option value=\"{1}\">{1}</option>",
                            item.Key,
                            string.Concat(item.Key, " ", item.Value)
                        );
                    }
                }
                else
                {
                    foreach (var item in counties)
                    {
                        sb.AppendFormat("<option value=\"{0}\" {2}>{1}</option>",
                            item.Key,
                            string.Concat(item.Key, " ", item.Value),
                            item.Key.Equals(selectedCounty) ? "selected=\"selected\"" : "");
                    }
                }

                return Content(sb.ToString());
            }
            return Content(string.Empty);
        }


        protected override void Dispose(bool disposing)
        {
            this.service.Dispose();
            base.Dispose(disposing);
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

            //正式
             response.Vi = "PmNER6HP23jikkcC";
             response.Key = "382go6Z9UrDy3XBuJnCHzNusEYFnBfls";

            //test
            //response.Key = "CON3KthrvPulsAWQQiQ3jsswLIzxxgQK";
            //response.Vi = "ugZbqRhI6x5LGI94";

            //test
            //var success = response.Validate("MS15822085");

            //正式
            var success = response.Validate("MS3276146654");

            if (success)
            {
                var tradInfoModel = response.GetResponseModel<TradeInfoModel>();
                var resuccess = true;
                //var OrderNoToProuctId = tradInfoModel.Result.MerchantOrderNo.OrderNoToProuctId();
                var wait = new InvoiceRepository().GetWait(tradInfoModel.Result.MerchantOrderNo);
                var inv = new InvoiceRepository().GetInvoice(wait.id);
                if(inv != null)
                {
                    resuccess = false;
                }

                if (resuccess)
                {
                    User_CashReturn user = new MallRepository().GetUserPRecord(tradInfoModel.Result.MerchantOrderNo);
                    Product productModel = new MallRepository().Get((int)user.productId);
                    Order order = new Order
                    {
                        UserId = user.userId,
                        ProductId = productModel.id,
                        Pay = tradInfoModel.Result.PaymentType,
                        Order_No = tradInfoModel.Result.MerchantOrderNo,
                        PayStore = tradInfoModel.Result.PayStore,
                        Barcode_1 = tradInfoModel.Result.Barcode_1,
                        Barcode_2 = tradInfoModel.Result.Barcode_2,
                        Barcode_3 = tradInfoModel.Result.Barcode_3,
                        PayBankCode = tradInfoModel.Result.PayBankCode,
                        CodeNo = tradInfoModel.Result.CodeNo,
                        PayerAccount5Code = tradInfoModel.Result.PayerAccount5Code,
                        inpdate = tradInfoModel.Result.PayTime
                    };

                    //加入購物記錄
                    var reutrnorder = new MallRepository().OrderCreate(order);




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

                    var pr = new PreferentialRepository().getPRecordsAll(user.userId).Where(x => x.PreferentialID == 5 && x.productID == productModel.id).FirstOrDefault();
                    //2倍
                    /*
                    if (pr.Count != 0)
                    {
                        assr = new ProductRecord
                        {
                            UserID = user.userId,
                            unitSn = 1,
                            assets = productModel.transform,
                            inpdate = DateTime.Now,
                            type = 5,
                            ProductId = productModel.id
                        };
                        new AssetsRepository().AddBearByAssets(assr);
                        var prds = new PreferentialRecords
                        {
                            id = pr.id,
                            UserId = user.userId,
                            Count = 0,
                            inpdate = DateTime.Now,
                            PreferentialID = pr.PreferentialID,
                            productID = pr.productID
                        };
                        new PreferentialRepository().PRecordsUpdate(prds);
                    }*/



                    //普通首儲
                    /*if(pr.Count != 0)
                    {
                        assr = new ProductRecord
                        {
                            UserID = user.userId,
                            unitSn = 1,
                            assets = new PreferentialRepository().getpreferential(1).assets,
                            inpdate = DateTime.Now,
                            type = 5,
                            ProductId = productModel.id
                        };
                        new AssetsRepository().AddBearByAssets(assr);
                        var prds = new PreferentialRecords {
                            id = pr.id,
                            UserId = user.userId,
                            Count = 0,
                            inpdate = DateTime.Now,
                            PreferentialID = 1
                        };
                        new PreferentialRepository().PRecordsUpdate(prds);
                    }*/
                    //InvoiceModel invoice = new InvoiceModel();
                    //發送發票
                    var smilePayEinvoice = new InvoiceRepository().invoiceSend(tradInfoModel.Result.MerchantOrderNo, productModel);

                    Invoice invoice = new Invoice
                    {
                        invoiceNumber = smilePayEinvoice.InvoiceNumber,
                        RandomNumber = smilePayEinvoice.RandomNumber,
                        inpdate = DateTime.Parse(smilePayEinvoice.InvoiceDate + " " + smilePayEinvoice.InvoiceTime),
                        IwaitId = wait.id,
                        CarrierID = smilePayEinvoice.CarrierID,
                        orderId = reutrnorder.id



                    };


                    new InvoiceRepository().InvoiceCreate(invoice);


                }
               


            }
            return View();





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
                    pt.Price = Math.Ceiling((double)pt.Price * 1.2 * 30000 / 30);

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
                mall.Price = Math.Ceiling((double)mall.Price * 30  / 30000 / 1.2);
            }
            return View(mall);
        }

        // POST: WebMall/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateInput(false)]
        public async System.Threading.Tasks.Task<ActionResult> Edit(Product pd, HttpPostedFileBase image)
        {
            try
            {
                if (image != null)
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
                    pd.Price = Math.Ceiling((double)pd.Price * 1.2 * 30000 / 30);

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
