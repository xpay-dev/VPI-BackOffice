using SDGAdmin.Models;
using SDGDAL.Entities;
using SDGDAL.Repositories;
using SDGUtil;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SDGAdmin.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        string action = string.Empty;
        UserRepository _userRepo;
        string country = "";

        public HomeController()
        {
            _userRepo = new UserRepository();
        }

        public ActionResult Payment()
        {
            return View();
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
                ViewBag.SessionExpired = "Session has expired. Please relogin.";
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [CustomAttributes.SessionExpireFilter]
        public ActionResult Index(string Username, string Password, string txtCaptcha, bool RememberMe, string ReturnUrl, FormCollection form)
        {
            string strCaptcha = Convert.ToString(Request["captcha"]);

            UserRepository userRepo = new UserRepository();

            try
            {
                #region Get the visitors IP Address
                //string externalIP;
                //externalIP = (new WebClient()).DownloadString("http://checkip.dyndns.org/");
                //externalIP = (new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}"))
                //             .Matches(externalIP)[0].ToString();

                //IPToCountry.IPToCountry ipToCountry = new IPToCountry.IPToCountry();
                #endregion
                country = "Philippines";//ipToCountry.GetCountryNameByIP(externalIP);

                action = "user is logging-in";
                
                var acc = userRepo.GetUserByCredentials(Username, Password, Request.ServerVariables["REMOTE_ADDR"]);

                DateTime pe = acc.PasswordExpirationDate;
                TimeSpan d = pe.Subtract(DateTime.Today);

                if (d.Days <= 3)
                {
                    Session["Expiry"] = true;
                    Session["Message"] = "Your password will expire in " + d.Days + " days. Please contact support for assistance.";
                }

                if (acc != null)
                {
                    if (strCaptcha == txtCaptcha)
                    {
                        Session["Captcha"] = null;
                        var actRefNumber = ApplicationLog.UsersLoginInfo("SDGAdmin", acc.AccountId, acc.UserId, acc.IsDeleted, acc.IPAddress, "");

                        string msg = "Restricted Access. Authorized users only.";

                        CurrentUser.ParentId = acc.ParentId;
                        CurrentUser.ParentType = (Enums.ParentType)acc.ParentTypeId;
                        CurrentUser.DisplayName = acc.User.FirstName + " " + acc.User.LastName;
                        CurrentUser.Avatar = Url.Content("~/UserPictures/") + Username + "/" + acc.User.PhotoUrl;
                        CurrentUser.Role = ((Enums.PermissionType)acc.RoleId).ToString();
                        CurrentUser.UserId = acc.UserId;
                        CurrentUser.Country = country;

                        if ((acc.RoleId).Equals(4) || (acc.RoleId).Equals(5))
                        {
                            string userData = string.Concat(acc.UserId, "|", acc.User.FirstName, "|", acc.User.LastName);

                            HttpCookie authCookie = FormsAuthentication.GetAuthCookie(Username, false);

                            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);

                            FormsAuthenticationTicket newTicket = new FormsAuthenticationTicket(ticket.Version, ticket.Name, ticket.IssueDate, ticket.Expiration, ticket.IsPersistent, userData);

                            authCookie.Value = FormsAuthentication.Encrypt(newTicket);

                            Response.Cookies.Add(authCookie);

                            return RedirectToAction("Dashboard");
                        }
                        else
                        {
                            ViewBag.Alert = msg;
                        }
                    }
                    else
                    {
                        ViewBag.Alert = "Incorrect Captcha.";
                    }
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "Index", ex.StackTrace);

                ViewBag.Alert = ex.Message;

                Session["Alert"] = ex.Message;

                if (ex.Message == "Password expired. Please contact support for assistance.")
                {
                    Session["PasswordExpired"] = "Your password is expired, please change now.";
                    return RedirectToAction("ChangePassword");
                }
            }

            return View();
        }

        [AllowAnonymous]
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
                    var passwordGenerator = new SDGDAL.PasswordGenerator();

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
                action = "chaing password.";

                if (password.OldPassword == password.NewPassword)
                {
                    ModelState.AddModelError(string.Empty, "New password should not match the current password.");
                    return View();
                }

                ModelState.Remove("UserId");

                if (ModelState.IsValid && password != null)
                {
                    var user = _userRepo.GetUserByUserAndPassword(password.UserName, password.OldPassword);

                    if (user != null)
                    {
                        UserRepository userRepo = new UserRepository();

                        Account account = new Account();
                        account.UserId = user.UserId;
                        account.Password = password.NewPassword;
                        account.NeedsUpdate = false;
                        account.PasswordExpirationDate = user.PasswordExpirationDate;

                        var result = userRepo.UpdatePasswordByUserId(account);

                        if (result != null)
                        {
                            string Username = result.Username;
                            var acc = userRepo.GetUserByCredentials(Username, password.NewPassword, Request.ServerVariables["REMOTE_ADDR"]);

                            var actRefNumber = ApplicationLog.UsersLoginInfo("SDGAdmin", acc.AccountId, acc.UserId, acc.IsDeleted, acc.IPAddress, "");

                            string msg = "Restricted Access. Authorized users only.";

                            CurrentUser.ParentId = acc.ParentId;
                            CurrentUser.ParentType = (Enums.ParentType)acc.ParentTypeId;
                            CurrentUser.DisplayName = acc.User.FirstName + " " + acc.User.LastName;
                            CurrentUser.Avatar = "/UserPictures/" + Username + "/" + acc.User.PhotoUrl;
                            CurrentUser.Role = ((Enums.PermissionType)acc.RoleId).ToString();
                            CurrentUser.UserId = acc.UserId;
                            if ((acc.RoleId).Equals(4) || (acc.RoleId).Equals(5))
                            {
                                string userData = string.Concat(acc.UserId, "|", acc.User.FirstName, "|", acc.User.LastName);

                                HttpCookie authCookie = FormsAuthentication.GetAuthCookie(Username, false);

                                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);

                                FormsAuthenticationTicket newTicket = new FormsAuthenticationTicket(ticket.Version, ticket.Name, ticket.IssueDate, ticket.Expiration, ticket.IsPersistent, userData);

                                authCookie.Value = FormsAuthentication.Encrypt(newTicket);

                                Response.Cookies.Add(authCookie);

                                return RedirectToAction("Dashboard");
                            }
                            else
                            {
                                ViewBag.Alert = msg;
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Your current username or password was incorrect.");
                    }
                }
                else
                {
                    var userActivity = action;

                    var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", "Change Password has invalid model state", "ChangePassword", "");
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "ChangePassword", ex.StackTrace);

                ViewBag.Alert = ex.Message;
            }

            return View();
        }


        public ActionResult Dashboard()
        {
            var userActivity = "Entered Dashboard";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "Dashboard", "");

            bool e = Convert.ToBoolean(Session["Expiry"]);

            if (e == true)
            {
                ViewBag.PasswordExpiration = Session["Message"];
            }

            Session["Message"] = null;

            return View();
        }

        [HttpPost]
        public JsonResult UploadUserPhoto(HttpPostedFileBase imageFile)
        {
            bool upload = false;
            string savedFileName = string.Empty;
            string filename = string.Empty;
            UserRepository userRepo = new UserRepository();
            try
            {
                action = "while uploading a photo";

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
                CurrentUser.Avatar = Url.Content("~/UserPictures/") + user.Username + "/" + filename;
                return Json(new { success = upload, filename = user.Username + "/" + filename });
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "Index", ex.StackTrace);
            }

            return Json(null);
        }

        private bool SendEmailPassword(string email, string password)
        {
            Boolean result = false;

            string msg = "Your new password is: " + password;

            SmtpClient client = new SmtpClient();
            client.EnableSsl = false;
            client.Port = 25;
            client.Host = "smtpout.asia.secureserver.net";
            client.Timeout = 90000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("support@hybridpayasia.com", "Hybrid1!");

            System.Net.Mail.MailMessage mm = new System.Net.Mail.MailMessage("support@hybridpayasia.com", email, "Forgot Passsword", msg);
            mm.IsBodyHtml = true;
            mm.BodyEncoding = UTF8Encoding.UTF8;
            mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            try
            {
                client.Send(mm);
            }
            catch (Exception ex)
            {
                return false;
            }

            #region Old code for sending forgot password
            //var config = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");

            //var fromAddress = new MailAddress(config.Network.UserName, "SDG Group");
            //var toAddress = new MailAddress(email, "");
            //string fromPassword = config.Network.Password;
            //const string subject = "SDG: New Passsword";
            //string body = "Your new password is: " + password;

            //var smtp = new SmtpClient
            //{
            //    Host = config.Network.Host,
            //    Port = config.Network.Port,
            //    EnableSsl = true,
            //    DeliveryMethod = SmtpDeliveryMethod.Network,
            //    UseDefaultCredentials = false,
            //    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            //};
            //using (var message = new System.Net.Mail.MailMessage(fromAddress, toAddress)
            //{
            //    Subject = subject,
            //    Body = body
            //})
            //{
            //    smtp.Send(message);
            //}
            #endregion

            result = true;

            return result;
        }
    }
}
