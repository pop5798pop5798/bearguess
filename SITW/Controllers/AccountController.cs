using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SITW.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using SITDto;
using SITW.Models.Repository;
using SITW.Helper;
using System.Security.Claims;
using SITW.Services;
using System.Collections.Generic;
using SITW.Models.ViewModel;
using System.Text.RegularExpressions;
using System.Runtime.Caching;

namespace SITW.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        //HttpClient client;
        string encryptedKey;
        private static MemoryCache _cache = MemoryCache.Default;
        List<gameDto> gList;

        public AccountController()
        {
            encryptedKey = System.Web.Configuration.WebConfigurationManager.AppSettings["encryptedKey"];
            if (_cache.Contains("GameList"))
                gList = _cache.Get("GameList") as List<gameDto>;
            //client = new HttpClient();
            //client.BaseAddress = new Uri(System.Web.Configuration.WebConfigurationManager.AppSettings["apiurl"]);
            //client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
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

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl,string message,int? log)
        {
            if (string.IsNullOrEmpty(User.Identity.GetUserId()))
            {
                ViewBag.ReturnUrl = returnUrl;
                if (message != "")
                    ViewData["Message"] = message;

                if (log == 1)
                    ViewBag.log = log;
                return View();
            }
            else
            {
                PreferentialRecords pr = new PreferentialRepository().getPRecordsType(User.Identity.GetUserId(),100);
                if (log == 1)
                    ViewBag.log = log;

                if (pr == null && log == 1)
                {
                    var prm = new PreferentialRecords
                    {
                        UserId = User.Identity.GetUserId(),
                        Count = 0,
                        inpdate = DateTime.Now,
                        PreferentialID = 100
                    };

                    new PreferentialRepository().PRecordsCreate(prm);
                    var ar = new AssetsRecord
                    {
                        UserId = User.Identity.GetUserId(),
                        unitSn = 1,
                        assets = 5000,
                        type = 2,
                        inpdate = DateTime.Now
                    };
                    new AssetsRepository().AddAssetsByAssets(ar);
                    return RedirectToAction("Index", "Manage", new { Log = "已贈送5000魚骨幣至您的帳戶" });
                }
                return RedirectToAction("Index", "Manage");
            }
               

            
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl,int? log)
        {
            if (!ModelState.IsValid || !new reCAPTCHAHelper().Validate(Request["g-recaptcha-response"]))
            {
                ViewData["Message"] = "驗證有誤，請重新登入";
                return View(model);
            }

            // 這不會計算為帳戶鎖定的登入失敗
            // 若要啟用密碼失敗來觸發帳戶鎖定，請變更為 shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Name, model.Password, true, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    var live = new UserRepository().getlive();
                    var user = UserManager.FindByName(model.Name);

                    foreach (var l in live)
                    {
                        if (l.username == user.Id)
                        {
                            if (l.valid != 0)
                            {
                                ViewData["Message"] = "此為直播主帳號，無法登入。";
                                return View(model);
                            }
                            else {
                                ViewData["Message"] = "此帳號還在申請審核中。";
                                return View(model);
                            }
                            
                        }
                    }
                    string vClientIP = GetIPAddress();

                    new UserRepository().CreateIP(user.Id, vClientIP);

                    PreferentialRecords pr = new PreferentialRepository().getPRecordsType(user.Id,100);

                 



                    Session["Assets"] = new AssetsRepository().getAssetsListByUserID(User.Identity.GetUserId());
                    //Session["levelExp"] = new AssetsRepository().getExpByUserID(User.Identity.GetUserId());
                    if (returnUrl != null)
                        Response.Redirect(returnUrl);

                    if (pr == null && log == 1)
                    {
                        var prm = new PreferentialRecords
                        {
                            UserId = user.Id,
                            Count = 0,
                            inpdate = DateTime.Now,
                            PreferentialID = 100
                        };

                        new PreferentialRepository().PRecordsCreate(prm);
                        var ar = new AssetsRecord
                        {
                            UserId = user.Id,
                            unitSn = 1,
                            assets = 5000,
                            type = 2,
                            inpdate = DateTime.Now
                        };
                        new AssetsRepository().AddAssetsByAssets(ar);
                        return RedirectToAction("Index", "Manage", new { Log = "已贈送5000魚骨幣至您的帳戶" });

                    }
                    /*HttpCookie MyGreatCookie = new HttpCookie("ASP.NET_SessionId");
                    MyGreatCookie.Expires = DateTime.Now.AddDays(1);
                    MyGreatCookie.Secure = true;
                    Response.Cookies.Set(MyGreatCookie);

                    Response.Cookies[".AspNet.ApplicationCookie"].Expires = DateTime.Now.AddDays(1);
                    Response.Cookies[".AspNet.ApplicationCookie"].Secure = true;
                    Response.Cookies.Add(Response.Cookies[".AspNet.ApplicationCookie"]);

                    Response.Cookies["__RequestVerificationToken"].Expires = DateTime.Now.AddDays(1);
                    Response.Cookies["__RequestVerificationToken"].Secure = true;
                    Response.Cookies.Add(Response.Cookies["__RequestVerificationToken"]);

                    Response.Cookies[".AspNet.Correlation.Google"].Expires = DateTime.Now.AddDays(1);
                    Response.Cookies[".AspNet.Correlation.Google"].Secure = true;
                    Response.Cookies.Add(Response.Cookies[".AspNet.Correlation.Google"]);

                    Response.Cookies[".AspNet.ExternalCookie"].Expires = DateTime.Now.AddDays(1);
                    Response.Cookies[".AspNet.ExternalCookie"].Secure = true;
                    Response.Cookies.Add(Response.Cookies[".AspNet.ExternalCookie"]);

                    Response.Cookies[".AspNet.TwoFactorCookie"].Expires = DateTime.Now.AddDays(1);
                    Response.Cookies[".AspNet.TwoFactorCookie"].Secure = true;
                    Response.Cookies.Add(Response.Cookies[".AspNet.TwoFactorCookie"]);*/

                    return RedirectToAction("Index", "Home");


                //return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    //ModelState.AddModelError("", "登入嘗試失試。");
                    ViewData["Message"] = "登入嘗試失試。";
                    return View(model);
            }
        }


        //Get Ip Address
        protected string GetIPAddress()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }
            return context.Request.ServerVariables["REMOTE_ADDR"];
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //直播擴充程式
        public async Task<JsonResult> LoginExtensions(LoginViewModel model, string returnUrl)
        {
            ControllerContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            GameAJaxDto ajd = new GameAJaxDto();
            // 這不會計算為帳戶鎖定的登入失敗
            // 若要啟用密碼失敗來觸發帳戶鎖定，請變更為 shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Name, model.Password, model.RememberMe, shouldLockout: false);
           // string[] array = new string[2];
            switch (result)
            {
                case SignInStatus.Success:
                    //Session["Assets"] = new AssetsRepository().getAssetsListByUserID(User.Identity.GetUserId());
                    var live = new UserRepository().getlive();
                   

                    var user = UserManager.FindByName(model.Name);
                    if (live.Count == 0)
                    {
                        //ModelState.AddModelError("", "登入嘗試失試。");
                        ajd.isTrue = false;
                        ajd.ErrorMsg = "Liveerror";
                        return Json(ajd, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {

                        if(live.Where(x=>x.username == user.Id).FirstOrDefault() == null)
                        {
                            ajd.isTrue = false;
                            ajd.ErrorMsg = "Liveerror";
                            return Json(ajd, JsonRequestBehavior.AllowGet);
                        }
                    }



                    GamePosts gamepost = new GamePostsRepository().getLive(user.Id);
                    GamePostViewModel gpvm = new GamePostViewModel();
                    gpvm.gamepost = gamepost;

                    ajd.gpvm = gpvm;

                    //Session["levelExp"] = new AssetsRepository().getExpByUserID(User.Identity.GetUserId());

                    ajd.userId = user.Id;
                    ajd.username = user.UserName;
                    ajd.isTrue = true;
                    ajd.live = (int)live.Where(x=>x.username == user.Id).FirstOrDefault().valid;
                    return Json(ajd, JsonRequestBehavior.AllowGet);
                //return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    //return View("Lockout");
                case SignInStatus.RequiresVerification:
                    //return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "登入嘗試失試。");
                    ajd.ErrorMsg = "error";
                    ajd.isTrue = false;
                   // array[0] = "error";
                    return Json(ajd, JsonRequestBehavior.AllowGet);
            }
        }


        [Authorize(Roles = "UserAdmin")]
        public async Task<ActionResult> ImpersonateUserAsync(string userName)
        {
            
            var context = System.Web.HttpContext.Current;

            string originalUsername = context.User.Identity.Name;

            var impersonatedUser = await UserManager.FindByNameAsync(userName);

            var impersonatedIdentity = await UserManager.CreateIdentityAsync(impersonatedUser, DefaultAuthenticationTypes.ApplicationCookie);
            impersonatedIdentity.AddClaim(new Claim("UserImpersonation", "true"));
            impersonatedIdentity.AddClaim(new Claim("OriginalUsername", originalUsername));

            var authenticationManager = context.GetOwinContext().Authentication;
            authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, impersonatedIdentity);

            return Redirect("/");
        }


        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // 需要使用者已透過使用者名稱/密碼或外部登入進行登入
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 下列程式碼保護兩個因素碼不受暴力密碼破解攻擊。 
            // 如果使用者輸入不正確的代碼來表示一段指定的時間，則使用者帳戶 
            // 會有一段指定的時間遭到鎖定。 
            // 您可以在 IdentityConfig 中設定帳戶鎖定設定
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "代碼無效。");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            if (string.IsNullOrEmpty(User.Identity.GetUserId()))
                return View();
            else
                return RedirectToAction("Index", "Home");
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid && new reCAPTCHAHelper().Validate(Request["g-recaptcha-response"]))
            {
                var user = new ApplicationUser { UserName = model.Name, Email = model.Email, Name = model.Name, RegistrationDate = DateTime.Now };
                var result = await UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                   
                    await SignInManager.SignInAsync(user, isPersistent:true, rememberBrowser:false);

                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // 傳送包含此連結的電子郵件
                    //string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    //await UserManager.SendEmailAsync(user.Id, "熊i猜Email驗證信", "請按此連結確認您的帳戶 <a href=\"" + callbackUrl + "\">驗證</a>");
                    await new AccountModel().RegisterFinsh(user);

                    //寄送驗證信件
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    string emailBody = EmailTemplatesService.GetVerifyEmailHTML(callbackUrl);
                    await UserManager.SendEmailAsync(user.Id, "熊i猜Email驗證信", emailBody);
                    if (model.Code != "")
                    {
                        var r = new UserRepository().getRecommend();
                        foreach (var re in r)
                        {
                            var v = new UserRepository().getRecommendStart((int)re.ReId);
                            if (v.edate > DateTime.Now)
                            {
                                if (model.Code == re.code)
                                {
                                    AssetsRecord ar = new AssetsRecord();
                                    ar.assets = (double)v.money;
                                    ar.unitSn = 1;
                                    ar.UserId = user.Id;
                                    new AssetsRepository().AddAssetsByAssets(ar);
                                }
                            }


                        }


                    }
                    if(model.Assets > 5000)
                    {
                        model.Assets = 5000;
                    }

                    if(model.Assets != 0)
                    {
                        AssetsRecord ar2 = new AssetsRecord();
                        ar2.assets = (double)model.Assets;
                        ar2.unitSn = 1;
                        ar2.UserId = user.Id;

                        new AssetsRepository().AddAssetsByAssets(ar2);
                    }
                    var prm = new PreferentialRecords
                    {
                        UserId = user.Id,
                        Count = 0,
                        inpdate = DateTime.Now,
                        PreferentialID = 100
                    };

                    new PreferentialRepository().PRecordsCreate(prm);






                    return RedirectToAction("Index", "Manage", new { RM = "溫馨提醒：Email以及手機驗證，才可以參與所有競猜活動!!" });
                }
               
                AddErrors(result);
            }

            // 如果執行到這裡，發生某項失敗，則重新顯示表單
            return View(model);
        }
        [AllowAnonymous]
        public ActionResult LiveRegister()
        {
          
            if (string.IsNullOrEmpty(User.Identity.GetUserId()))
                return View();
            else
                return RedirectToAction("Index", "Home");
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LiveRegister(RegisterLiveViewModel model)
        {

            if (ModelState.IsValid && new reCAPTCHAHelper().Validate(Request["g-recaptcha-response"]))
            {
                var user = new ApplicationUser { UserName = model.Name, Email = model.Email, Name = model.Name, RegistrationDate = DateTime.Now };
                var result = await UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {

                    //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // 傳送包含此連結的電子郵件
                    //string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    //await UserManager.SendEmailAsync(user.Id, "熊i猜Email驗證信", "請按此連結確認您的帳戶 <a href=\"" + callbackUrl + "\">驗證</a>");
                    await new AccountModel().RegisterFinsh(user);
                   /* var content = "<h2 style='text-align: center;'>申請通知信</h2>" +
                "<div style='text-align: center;padding:0 14%;'>" +
                        "<h4 style='color:#222'>親愛的直播主您好：<p>" +
                        "我們已經收到您的申請，可自行至此<a href='https://chrome.google.com/webstore/detail/%E7%86%8Ai%E7%8C%9C%E7%9B%B4%E6%92%AD%E7%AB%B6%E7%8C%9C%E5%B7%A5%E5%85%B7/pbkojdjpoepfcnnbfhonnemjjaeidaoa?hl=zh-TW'>連結</a>下載競猜工具(僅支援google chome)<p>" +
                       "<h3 style='color:#500050'>熊i猜會在1-2個工作日寄送合約書至您的信箱，還請您耐心等候，如有任何問題，可至<a href='https://www.facebook.com/funbet.esport/'>熊i猜粉絲團</a>發訊詢問喔!<br>" +
                        "(本通知為自動發送，請勿直接回信詢問，以避免延誤問題解決時間。)</h3><br>";
                    var EmailContent = EmailTemplatesService.GetLiveEmailHTML(content);
                    //寄送通知信
                    new MailServiceMailgun().ChangeSend("競猜直播主申請通知信", EmailContent, user.Email, "");*/
                    new UserRepository().addlive(user.Id);




                    return RedirectToAction("Index", "Home", new { message = "申請成功，已寄送通知信至你的信箱" });
                }

                AddErrors(result);
            }

            // 如果執行到這裡，發生某項失敗，則重新顯示表單
            return View(model);
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Recommend(string recommend)
        {
            var r = new UserRepository().getRecommend();
            aJaxDto ajd = new aJaxDto();
            foreach(var re in r)
            {
                var v = new UserRepository().getRecommendStart((int)re.ReId);
                if(v.edate > DateTime.Now)
                {
                    if (recommend == re.code)
                    {
                        ajd.isTrue = true;
                    }
                    else
                    {
                        ajd.isTrue = false;
                        ajd.ErrorMsg = "推薦碼錯誤";
                    }
                }
                

            }

            return Json(ajd, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterLIVE(RegisterLiveViewModel model)
        {
            ControllerContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            GameAJaxDto ajd = new GameAJaxDto();
           
                var user = new ApplicationUser { UserName = model.Name, Email = model.Email, Name = model.Name, RegistrationDate = DateTime.Now };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // 傳送包含此連結的電子郵件
                    //string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    //await UserManager.SendEmailAsync(user.Id, "熊i猜Email驗證信", "請按此連結確認您的帳戶 <a href=\"" + callbackUrl + "\">驗證</a>");
                    await new AccountModel().RegisterFinsh(user);
                    new UserRepository().addlive(user.Id);

                    //寄送驗證信件
                    /*string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    string emailBody = EmailTemplatesService.GetVerifyEmailHTML(callbackUrl);
                    await UserManager.SendEmailAsync(user.Id, "熊i猜Email驗證信", emailBody);*/




                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                //AddErrors(result);

            // 如果執行到這裡，發生某項失敗，則重新顯示表單
            //ajd.isTrue = false;
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult RegisterFinish(string returnUrl)
        {

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // GET: /Account/ConfirmEmail
        //[AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("EmailConfirmError");
            }
            if (userId != User.Identity.GetUserId())
            {
                return View("EmailConfirmError");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, User.Identity.GetEmail(), code);
            
            //return View(result.Succeeded ? "ConfirmEmail" : "EmailConfirmError");
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }

                return RedirectToAction("Index", "Manage", new { Message = ManageController.ManageMessageId.EmailConfirmSuccess });
            }
            else
                return View("EmailConfirmError");

        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // 不顯示使用者不存在或未受確認
                    //return View("ForgotPasswordConfirmation");
                    //TempData["testmsg"] = "<script>alert('已發送至您電子郵件，請檢查以重設密碼');</script>";
                    return RedirectToAction("Login", "Account" ,new { message = "本站查無您的信箱，請重新輸入" });
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                // 傳送包含此連結的電子郵件
                 string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                 var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                 await UserManager.SendEmailAsync(user.Id, "重設密碼", "請按 <a href=\"" + callbackUrl + "\">這裏</a> 重設密碼");
                //return RedirectToAction("ForgotPasswordConfirmation", "Account");
                return RedirectToAction("Login", "Account", new { message = "已發送至您電子郵件，請檢查以重設密碼" });
            }

            // 如果執行到這裡，發生某項失敗，則重新顯示表單
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // 不顯示使用者不存在
                // return RedirectToAction("ResetPasswordConfirmation", "Account");
                return RedirectToAction("Login", "Account", new { message = "本站查無您的信箱，請返回信箱重新輸入" });
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("Login", "Account", new { message = "您的密碼已重設" });
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // 要求重新導向至外部登入提供者
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // 產生並傳送 Token
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // 若使用者已經有登入資料，請使用此外部登入提供者登入使用者
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: true);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // 若使用者沒有帳戶，請提示使用者建立帳戶
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // 從外部登入提供者處取得使用者資訊
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Name, Email = model.Email, Name = model.Name, RegistrationDate = DateTime.Now };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: true, rememberBrowser: false);
                        await new AccountModel().RegisterFinsh(user);
                        //return RedirectToLocal(returnUrl);
                        return RedirectToAction("RegisterFinish", "Account", new { returnUrl = returnUrl });

                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }


        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helper
        // 新增外部登入時用來當做 XSRF 保護
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                context.RequestContext.HttpContext.Response.SuppressFormsAuthenticationRedirect = true;

                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }


        #endregion
    }
}