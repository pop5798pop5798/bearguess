﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SITW.Models;
using SITW.Models.ViewModel;
using SITW.Filter;
using SITW.Models.Repository;
using System.Collections.Generic;
using SITW.Helper;
using SITW.Services;
using System.Text;

namespace SITW.Controllers
{
    [Authorize]
    [UserDataFilter]
    [MissionFilter]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "已變更您的密碼。"
                : message == ManageMessageId.SetPasswordSuccess ? "已設定您的密碼。"
                : message == ManageMessageId.SetTwoFactorSuccess ? "已設定您的雙因素驗證。"
                : message == ManageMessageId.Error ? "發生錯誤。"
                : message == ManageMessageId.AddPhoneSuccess ? "手機號碼新增並驗證成功。"
                : message == ManageMessageId.RemovePhoneSuccess ? "已移除您的電話號碼。"
                : message == ManageMessageId.SendEmailConfirmSuccess ? "已送出驗證信件，請檢察您的信箱。"
                : message == ManageMessageId.EmailConfirmSuccess ? "信箱驗證成功。"
                : "";

            var userId = User.Identity.GetUserId();
            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId),
                levelExp = (levelExpViewModel)Session["levelExp"],
                Assets = (List<AssetsViewModel>)Session["Assets"],
                BetList = await new BetsRepository().BetsByUserID(userId),
                Email = User.Identity.GetEmail(),
                EmailConfirm=User.Identity.GetEmailConfirmed(),
                PhoneNumberConfirmed=User.Identity.GetPhoneNumberConfirmed()
                
            };
            return View(model);
        }

        public ActionResult _GameMenu()
        {
            if (User.Identity.GetUserId() != null)
            {
                var rat = new H5GameRepository().Usercount(User.Identity.GetUserId()).Where(x => x.gameModel == 3).FirstOrDefault();
                var brick = new H5GameRepository().Usercount(User.Identity.GetUserId()).Where(x => x.gameModel == 4).FirstOrDefault();
                int ratcount = 5;
                int brickcount = 5;

                if (rat != null)
                {
                    ratcount = (int)rat.count;
                }
                if (brick != null)
                {
                    brickcount = (int)brick.count;
                }
                ViewBag.Rat = "/ " + ratcount + "次";
                ViewBag.brick = "/ " + brickcount + "次";

            }
            else
            {
                ViewBag.Rat = "";
                ViewBag.brick = "";

            }
            return View();
        }

       

        public async Task<ActionResult> ProductRecord(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "已變更您的密碼。"
                : message == ManageMessageId.SetPasswordSuccess ? "已設定您的密碼。"
                : message == ManageMessageId.SetTwoFactorSuccess ? "已設定您的雙因素驗證。"
                : message == ManageMessageId.Error ? "發生錯誤。"
                : message == ManageMessageId.AddPhoneSuccess ? "手機號碼新增並驗證成功。"
                : message == ManageMessageId.RemovePhoneSuccess ? "已移除您的電話號碼。"
                : message == ManageMessageId.SendEmailConfirmSuccess ? "已送出驗證信件，請檢察您的信箱。"
                : message == ManageMessageId.EmailConfirmSuccess ? "信箱驗證成功。"
                : "";

            var userId = User.Identity.GetUserId();
            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId),
                levelExp = (levelExpViewModel)Session["levelExp"],
                Assets = (List<AssetsViewModel>)Session["Assets"],
                BetList = await new BetsRepository().BetsByUserID(userId),
                Email = User.Identity.GetEmail(),
                EmailConfirm = User.Identity.GetEmailConfirmed(),
                PhoneNumberConfirmed = User.Identity.GetPhoneNumberConfirmed()

            };
            return View(model);
        }

        public async Task<ActionResult> Setting()
        {
            var userId = User.Identity.GetUserId();
            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(userId),
                Logins = await UserManager.GetLoginsAsync(userId),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId),
                levelExp = (levelExpViewModel)Session["levelExp"],
                Assets = (List<AssetsViewModel>)Session["Assets"],
                BetList = await new BetsRepository().BetsByUserID(userId),
                Email = User.Identity.GetEmail(),
                EmailConfirm = User.Identity.GetEmailConfirmed(),
                PhoneNumberConfirmed = User.Identity.GetPhoneNumberConfirmed()

            };
            return View(model);
        }

        public ActionResult Transform()
        {
            var userId = User.Identity.GetUserId();
            ViewBag.Safety = new MangerRepository().GetValid(userId);
            return View();
        }

        public async Task<ActionResult> _TransformRecords()
        {
            var userId = User.Identity.GetUserId();
            var trm = new MangerRepository().GetTransferRecords(userId);
            List<TransferViewModel> tfvmlist = new List<TransferViewModel>();
            foreach (var t in trm)
            {
                TransferViewModel tfvm = new TransferViewModel();
                tfvm.transferRecords = t;
                var transfer_out = await UserManager.FindByIdAsync(t.UserId);
                var transfer_in = await UserManager.FindByIdAsync(t.Transfer);
                tfvm.transferRecords = t;
                if (userId == t.UserId)
                {
                    tfvm.Transfer_records = "轉給 " + transfer_in.UserName;
                    tfvm.type = -1;
                }
                else if(userId == t.Transfer)
                {
                    tfvm.Transfer_records = transfer_out.UserName + " 轉進";
                    tfvm.type = 1;
                }

               
                tfvmlist.Add(tfvm);

            }
            //ViewBag.Safety = new MangerRepository().GetValid(useriId);
            return View(tfvmlist.OrderByDescending(x => x.transferRecords.createDate));
        }

        [HttpPost]
        public async Task<int> TransformAsync(string email,float money,string safety)
        {
            var user = User.Identity.GetUserId();

            try
            {
               Safety sf = new MangerRepository().GetValid(user);
               var tfuser = await UserManager.FindByEmailAsync(email);

                if (tfuser != null && money >= 100000 && safety.ToMD5Comparison(sf.SafetyHash))
                {
                    TransferRecords tf = new TransferRecords
                    {
                        UserId = user,
                        Transfer = tfuser.Id,
                        type = 0,
                        credit = money,
                        createDate = DateTime.Now,
                        rake = 5,
                        unitSn = 1

                    };
                    //new MangerRepository().TransferRecordsCreate(tf);
                    new AssetsRepository().AddAssetsByTransfer(tf);
                    return 1;

                }
                else {
                    return 0;
                }
              
            }
            catch
            {
                return 0;
            }


        }

        [HttpPost]
        public int Safety(string pd)
        {
            var user = User.Identity.GetUserId();
            try
            {
                Safety sf = new Safety
                {
                    userId = user,
                    SafetyHash = pd.ToMD5()
                };
                new MangerRepository().Create(sf);
                return 1;
            }
            catch
            {
                return 0;
            }


        }

       




        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }

        //
        // GET: /Manage/AddPhoneNumber
        public async Task<ActionResult> AddPhoneNumber()
        {
            string phoneNumber = await UserManager.GetPhoneNumberAsync(User.Identity.GetUserId());
            AddPhoneNumberViewModel apnv = new AddPhoneNumberViewModel();
            apnv.Number = phoneNumber;
            return View(apnv);
        }

        //
        // POST: /Manage/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if(new SMSRecordRepository().checkPhoneNumSend(model.Number,60,1))
            {
                ViewBag.StatusMessage = "60秒內已發送過簡訊，請稍後再試";
            }
            if (new SMSRecordRepository().checkPhoneNumSend(model.Number, 3600, 3))
            {
                ViewBag.StatusMessage = "已發送太多次，請稍後再試";
            }
            if (new SMSRecordRepository().checkUserIdSend(User.Identity.GetUserId(), 60, 1))
            {
                ViewBag.StatusMessage = "60秒內已發送過簡訊，請稍後再試";
            }
            if (new SMSRecordRepository().checkUserIdSend(User.Identity.GetUserId(), 3600, 3))
            {
                ViewBag.StatusMessage = "已發送太多次，請稍後再試";
            }
            if (new UserRepository().checkPhoneNumber(model.Number))
            {
                ViewBag.StatusMessage = "此手機號碼已經綁定";
            }
            if (new UserRepository().checkUserHavePhone(User.Identity.GetUserId()))
            {
                ViewBag.StatusMessage = "此帳號已綁定手機，不可重複綁定";
            }
            if (ViewBag.StatusMessage!=null)
            {
                return View(model);
            }

            
            // 產生並傳送 Token
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
            if (UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = "熊i猜感謝您進行手機驗證，您的驗證碼為: " + code + "，請於30分鐘內輸入系統，謝謝。"
                };
                await UserManager.SmsService.SendAsync(message);
            }
            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        }

        //
        // POST: /Manage/EnableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // POST: /Manage/DisableTwoFactorAuthentication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DisableTwoFactorAuthentication()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // GET: /Manage/VerifyPhoneNumber
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
            // 透過 SMS 提供者傳送 SMS，以驗證電話號碼
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        //
        // POST: /Manage/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (new UserRepository().checkPhoneNumber(model.PhoneNumber))
            {
                ViewBag.StatusMessage = "此手機號碼已經綁定";
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
            }
            // 如果執行到這裡，發生某項失敗，則重新顯示表單
            ModelState.AddModelError("", "無法驗證號碼");
            return View(model);
        }

        //
        // POST: /Manage/RemovePhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // 如果執行到這裡，發生某項失敗，則重新顯示表單
            return View(model);
        }

        //
        // GET: /Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "已移除外部登入。"
                : message == ManageMessageId.Error ? "發生錯誤。"
                : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // 要求重新導向至外部登入提供者，以連結目前使用者的登入
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        //
        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

        public async Task<ActionResult> SendEmailConfirm()
        {
            try
            {
                string code = await UserManager.GenerateEmailConfirmationTokenAsync(User.Identity.GetUserId());
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = User.Identity.GetUserId(), code = code }, protocol: Request.Url.Scheme);
                string emailBody = EmailTemplatesService.GetVerifyEmailHTML(callbackUrl);
                await UserManager.SendEmailAsync(User.Identity.GetUserId(), "熊i猜Email驗證信", emailBody);
            }
            catch(Exception e)
            {

            }
            return RedirectToAction("Index", new { Message = ManageMessageId.SendEmailConfirmSuccess });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
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

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            SendEmailConfirmSuccess,
            EmailConfirmSuccess,
            Error
        }

#endregion
    }
}
//MD5擴充
public static class MD5Extensions
{
    public static string ToMD5(this string str)
    {
        using (var cryptoMD5 = System.Security.Cryptography.MD5.Create())
        {
            //將字串編碼成 UTF8 位元組陣列
            var bytes = Encoding.UTF8.GetBytes(str);

            //取得雜湊值位元組陣列
            var hash = cryptoMD5.ComputeHash(bytes);

            //取得 MD5
            var md5 = BitConverter.ToString(hash)
              .Replace("-", String.Empty)
              .ToUpper();

            return md5;
        }
    }

    public static bool ToMD5Comparison(this string str,string password)
    {
        using (var cryptoMD5 = System.Security.Cryptography.MD5.Create())
        {
            //將字串編碼成 UTF8 位元組陣列
            var bytes = Encoding.UTF8.GetBytes(str);

            //取得雜湊值位元組陣列
            var hash = cryptoMD5.ComputeHash(bytes);

            //取得 MD5
            var md5 = BitConverter.ToString(hash)
              .Replace("-", String.Empty)
              .ToUpper();

            if (md5 == password)
            {
                return true;

            }
            else {
                return false;
            }

            
        }
    }
}