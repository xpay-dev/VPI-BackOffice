using SDGDAL.Entities;
using SDGDAL.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mail;
using System.Web.Mvc;
using System.Web.Security;
using System.Net;
using System.Net.Mail;
using System.Configuration;
using System.Net.Configuration;
using SDGUtil;
using SDGDAL;
using CTPaymentBackOffice.Models;

namespace CTPaymentBackOffice.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        AccountRepository _accountRepo;
        UserRepository _userRepo;

        public HomeController()
        {
            _accountRepo = new AccountRepository();
            _userRepo = new UserRepository();
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();

            if (Request["ReturnUrl"] == null)
            {
                ViewBag.ReturnUrl = Request["ReturnUrl"];
            }
            else
            {
                ViewBag.ReturnUrl = Request["ReturnUrl"];
                Session["SessionExpired"] = "Session has expired. Please relogin.";
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [CustomAttributes.SessionExpireFilter]
        public ActionResult Index(string Username, string Password, bool RememberMe, string ReturnUrl, FormCollection form)
        {

            try
            {
                var acc = _userRepo.GetUserByCredentials(Username, Password, Request.ServerVariables["REMOTE_ADDR"]);

                DateTime pe = acc.PasswordExpirationDate;
                TimeSpan d = pe.Subtract(DateTime.Today);

                if (d.Days <= 3)
                {
                    Session["Expiry"] = true;
                    Session["Message"] = "Your password will expire in " + d.Days + " days. Please contact support for assistance.";
                }

                if (acc != null)
                {
                    Boolean isNeedsUpdate;

                    var actRefNumber = ApplicationLog.UsersLoginInfo("SDGBackOffice", acc.AccountId, acc.UserId, acc.IsDeleted, acc.IPAddress, "");

                    string msg = "Restricted Access. Authorized users only.";

                    CurrentUser.ParentId = acc.ParentId;
                    CurrentUser.ParentType = (Enums.ParentType)acc.ParentTypeId;
                    CurrentUser.DisplayName = acc.User.FirstName + " " + acc.User.LastName;
                    CurrentUser.Avatar = "/UserPictures/" + Username + "/" + acc.User.PhotoUrl;
                    CurrentUser.Role = ((Enums.PermissionType)acc.RoleId).ToString();
                    CurrentUser.UserId = acc.UserId;

                    if ((acc.RoleId).Equals(1) || (acc.RoleId).Equals(2) || (acc.RoleId).Equals(3))
                    {
                        string userData = string.Concat(acc.UserId, "|", acc.User.FirstName, "|", acc.User.LastName);

                        HttpCookie authCookie = FormsAuthentication.GetAuthCookie(Username, false);

                        FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);

                        FormsAuthenticationTicket newTicket = new FormsAuthenticationTicket(ticket.Version, ticket.Name, ticket.IssueDate, ticket.Expiration, ticket.IsPersistent, userData);

                        authCookie.Value = FormsAuthentication.Encrypt(newTicket);

                        Response.Cookies.Add(authCookie);

                        if (!string.IsNullOrEmpty(ReturnUrl))
                            return Redirect(ReturnUrl);

                        //check if user needs to change password
                        isNeedsUpdate = acc.NeedsUpdate;

                        if (isNeedsUpdate)
                            return RedirectToAction("ChangePassword", "Home", new { id = acc.UserId });

                        if (CurrentUser.ParentType == Enums.ParentType.Partner)
                        {
                            PartnerRepository partnerRepo = new PartnerRepository();

                            var info = partnerRepo.GetDetailsbyPartnerId(acc.ParentId);

                            if (info != null)
                            {
                                isNeedsUpdate = info.ContactInformation.NeedsUpdate;

                                if (isNeedsUpdate == true)
                                {
                                    return RedirectToAction("UpdateInfo", "Partners");
                                }
                                else
                                {
                                    return RedirectToAction("Dashboard");
                                }
                            }
                        }
                        else if (CurrentUser.ParentType == Enums.ParentType.Reseller)
                        {
                            ResellerRepository resellerRepo = new ResellerRepository();

                            var info = resellerRepo.GetDetailsbyResellerId(acc.ParentId);

                            if (info != null)
                            {
                                isNeedsUpdate = info.ContactInformation.NeedsUpdate;

                                if (isNeedsUpdate == true)
                                {
                                    return RedirectToAction("UpdateInfo", "Resellers");
                                }
                                else
                                {
                                    return RedirectToAction("Dashboard");
                                }
                            }
                        }
                        else if (CurrentUser.ParentType == Enums.ParentType.Merchant)
                        {
                            MerchantRepository merchantRepo = new MerchantRepository();

                            var info = merchantRepo.GetDetailsbyMerchantId(acc.ParentId);

                            if (info != null)
                            {
                                isNeedsUpdate = info.ContactInformation.NeedsUpdate;

                                if (isNeedsUpdate == true)
                                {
                                    return RedirectToAction("UpdateInfo", "Merchants");
                                }
                                else
                                {
                                    return RedirectToAction("Dashboard");
                                }
                            }
                        }
                        else if (CurrentUser.ParentType == Enums.ParentType.MerchantLocation)
                        {
                            MerchantBranchRepository merchantRepo = new MerchantBranchRepository();

                            var info = merchantRepo.GetDetailsbyMerchantBranchId(acc.ParentId);

                            if (info != null)
                            {
                                isNeedsUpdate = info.ContactInformation.NeedsUpdate;

                                if (isNeedsUpdate == true)
                                {
                                    return RedirectToAction("UpdateInfo", "MerchantBranches");
                                }
                                else
                                {
                                    return RedirectToAction("Dashboard");
                                }
                            }
                        }
                    }
                    else
                    {
                        ViewBag.Alert = msg;
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Alert = "Unknown error occured." + ex.Message + "Please contact support.";
                Session["Alert"] = ex.Message;
                if (ex.Message == "Password expired. Please contact support for assistance.")
                {
                    Session["PasswordExpired"] = "Your password is expired, please change now.";
                    return RedirectToAction("ChangePassword");
                }
            }

            return View();
        }

        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        [CustomAttributes.SessionExpireFilter]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [CustomAttributes.SessionExpireFilter]
        public ActionResult ForgotPassword(string Username, string Email)
        {
            ReferenceRepository _refRepo = new ReferenceRepository();
            UserRepository userRepo = new UserRepository();
            try
            {
                if (!userRepo.IsValidUserCredentials(Username, Email))
                {
                    var passwordGenerator = new PasswordGenerator();

                    string password = passwordGenerator.GeneratePassword(10);

                    Boolean isSuccess = SendEmailPassword(Email, password);
                    if (isSuccess)
                    {
                        Account acc = new Account();
                        acc.Username = Username;
                        acc.Password = password;
                        acc.NeedsUpdate = true;

                        var result = userRepo.UpdateUserPasswordByUsername(acc);

                        if (result != null)
                        {
                            ViewBag.Success = "Forgot Password Confirmation: Please check your email for your new password.";
                        }
                    }
                    else
                    {
                        ViewBag.Alert = "Error sending of password confirmation. Please Contact Support.";
                    }
                }
                else
                {
                    ViewBag.Alert = "Invalid credentials.";
                }
            }
            catch (Exception ex)
            {
                ErrorLog err = new ErrorLog();
                err.Action = "Change Password";
                err.Method = "Submit";
                err.ErrText = ex.Message;
                err.StackTrace = ex.StackTrace;
                err.DateCreated = DateTime.Now;

                if (ex.InnerException != null)
                {
                    err.InnerException = ex.InnerException.Message;
                    err.InnerExceptionStackTrace = ex.InnerException.StackTrace;
                }

                err = _refRepo.LogError(err);
            }
            return View();
        }

        [AllowAnonymous]
        [CustomAttributes.SessionExpireFilter]
        public ActionResult ChangePassword(int? id)
        {
            if (Session["PasswordExpired"] != null)
            {
                ViewBag.Expired = Session["PasswordExpired"];
            }

            Session["PasswordExpired"] = null;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [CustomAttributes.SessionExpireFilter]
        public ActionResult ChangePassword(ChangePasswordModel password)
        {
            try
            {
                if (password.OldPassword == password.NewPassword)
                {
                    ModelState.AddModelError(string.Empty, "New password should not match the current password.");
                    return View();
                }

                ModelState.Remove("UserId");

                var user = _userRepo.GetUserByUserAndPassword(password.UserName, password.OldPassword);

                if (ModelState.IsValid && password != null)
                {
                    UserRepository userRepo = new UserRepository();

                    Account account = new Account();
                    account.UserId = user.AccountId;
                    account.Password = password.NewPassword;
                    account.NeedsUpdate = false;
                    account.PasswordExpirationDate = user.PasswordExpirationDate;

                    var result = userRepo.UpdatePasswordByUserId(account);

                    if (result != null)
                    {
                        string Username = result.Username;
                        var acc = userRepo.GetUserByCredentials(Username, password.NewPassword, Request.ServerVariables["REMOTE_ADDR"]);

                        if (acc != null)
                        {
                            Boolean isNeedsUpdate;
                            string msg = "Restricted Access. Authorized users only.";

                            CurrentUser.ParentId = acc.ParentId;
                            CurrentUser.ParentType = (Enums.ParentType)acc.ParentTypeId;
                            CurrentUser.DisplayName = acc.User.FirstName + " " + acc.User.LastName;
                            CurrentUser.Avatar = "/UserPictures/" + Username + "/" + acc.User.PhotoUrl;
                            CurrentUser.Role = ((Enums.PermissionType)acc.RoleId).ToString();
                            CurrentUser.UserId = acc.UserId;

                            if ((acc.RoleId).Equals(1) || (acc.RoleId).Equals(2) || (acc.RoleId).Equals(3))
                            {
                                string userData = string.Concat(acc.UserId, "|", acc.User.FirstName, "|", acc.User.LastName);

                                HttpCookie authCookie = FormsAuthentication.GetAuthCookie(Username, false);

                                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);

                                FormsAuthenticationTicket newTicket = new FormsAuthenticationTicket(ticket.Version, ticket.Name, ticket.IssueDate, ticket.Expiration, ticket.IsPersistent, userData);

                                authCookie.Value = FormsAuthentication.Encrypt(newTicket);

                                Response.Cookies.Add(authCookie);

                                //check if user needs to change password
                                isNeedsUpdate = acc.NeedsUpdate;

                                if (isNeedsUpdate)
                                    return RedirectToAction("ChangePassword", "Home", new { id = acc.UserId });

                                if (CurrentUser.ParentType == Enums.ParentType.Partner)
                                {
                                    PartnerRepository partnerRepo = new PartnerRepository();

                                    var info = partnerRepo.GetDetailsbyPartnerId(acc.ParentId);

                                    if (info != null)
                                    {
                                        isNeedsUpdate = info.ContactInformation.NeedsUpdate;

                                        if (isNeedsUpdate == true)
                                        {
                                            return RedirectToAction("UpdateInfo", "Partners");
                                        }
                                        else
                                        {
                                            return RedirectToAction("Dashboard");
                                        }
                                    }
                                }
                                else if (CurrentUser.ParentType == Enums.ParentType.Reseller)
                                {
                                    ResellerRepository resellerRepo = new ResellerRepository();

                                    var info = resellerRepo.GetDetailsbyResellerId(acc.ParentId);

                                    if (info != null)
                                    {
                                        isNeedsUpdate = info.ContactInformation.NeedsUpdate;

                                        if (isNeedsUpdate == true)
                                        {
                                            return RedirectToAction("UpdateInfo", "Resellers");
                                        }
                                        else
                                        {
                                            return RedirectToAction("Dashboard");
                                        }
                                    }
                                }
                                else if (CurrentUser.ParentType == Enums.ParentType.Merchant)
                                {
                                    MerchantRepository merchantRepo = new MerchantRepository();

                                    var info = merchantRepo.GetDetailsbyMerchantId(acc.ParentId);

                                    if (info != null)
                                    {
                                        isNeedsUpdate = info.ContactInformation.NeedsUpdate;

                                        if (isNeedsUpdate == true)
                                        {
                                            return RedirectToAction("UpdateInfo", "Merchants");
                                        }
                                        else
                                        {
                                            return RedirectToAction("Dashboard");
                                        }
                                    }
                                }
                                else if (CurrentUser.ParentType == Enums.ParentType.MerchantLocation)
                                {
                                    MerchantBranchRepository merchantRepo = new MerchantBranchRepository();

                                    var info = merchantRepo.GetDetailsbyMerchantBranchId(acc.ParentId);

                                    if (info != null)
                                    {
                                        isNeedsUpdate = info.ContactInformation.NeedsUpdate;

                                        if (isNeedsUpdate == true)
                                        {
                                            return RedirectToAction("UpdateInfo", "MerchantBranches");
                                        }
                                        else
                                        {
                                            return RedirectToAction("Dashboard");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                ViewBag.Alert = msg;
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                //Error Logs
            }
            return View();
        }

        public ActionResult Dashboard()
        {
            var merchant = _accountRepo.GetTypeIdByAccountId(CurrentUser.UserId);

            bool e = Convert.ToBoolean(Session["Expiry"]);

            if (e == true)
            {
                ViewBag.PasswordExpiration = Session["Message"];
            }

            Session["Message"] = null;

            var id = merchant.ParentId;
            var x = CurrentUser.ParentId;
            ViewBag.User = id;

            if (CurrentUser.ParentType == Enums.ParentType.Partner)
            {
                ViewBag.ParentType = "Partner";
            }
            else if (CurrentUser.ParentType == Enums.ParentType.Reseller)
            {
                ViewBag.ParentType = "Reseller";
            }
            else
            {
                ViewBag.ParentType = "Merchant";
            }

            return View();
        }

        [HttpPost]
        public JsonResult UploadUserPhoto(HttpPostedFileBase imageFile)
        {
            bool upload = false;
            string savedFileName = string.Empty;
            string filename = string.Empty;
            UserRepository userRepo = new UserRepository();
            var user = userRepo.GetDetailsbyUserId(CurrentUser.UserId);

            if (imageFile != null && imageFile.ContentType.Contains("image"))
            {
                if (imageFile.ContentLength > 0)
                {
                    Guid guid;
                    guid = Guid.NewGuid();
                    filename = guid.ToString() + Path.GetExtension(imageFile.FileName);
                    var saveDir = Path.Combine(Server.MapPath("~/UserPictures"), user.Username);
                    savedFileName = saveDir + "/" + filename;

                    if (!Directory.Exists(saveDir))
                    {
                        Directory.CreateDirectory(saveDir);
                    }

                    imageFile.SaveAs(savedFileName);

                    userRepo.UploadUserPhoto(CurrentUser.UserId, filename);
                }
            }

            //return RedirectToAction("Index", "Profile", new { tid = 3 });
            return Json(new { success = upload, filename = user.Username + "/" + filename });
        }

        private bool SendEmailPassword(string email, string password)
        {
            Boolean result = false;
            try
            {
                var config = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");

                var fromAddress = new MailAddress(config.Network.UserName, "SDG Group");
                var toAddress = new MailAddress(email, "");
                string fromPassword = config.Network.Password;
                const string subject = "SDG: New Passsword";
                string body = "Your new password is: " + password;

                var smtp = new SmtpClient
                {
                    Host = config.Network.Host,
                    Port = config.Network.Port,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (var message = new System.Net.Mail.MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }

                result = true;

            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }
    }
}
