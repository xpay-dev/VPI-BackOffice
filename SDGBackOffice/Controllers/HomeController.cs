using SDGBackOffice.Models;
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
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Text;
using System.Data.Entity;
//using SDGBackOffice.GeoService;

namespace SDGBackOffice.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        string action = string.Empty;
        AccountRepository _accountRepo;
        UserRepository _userRepo;
        MidsRepository _midsRepository;
        MerchantRepository _merchantRepo;
        MerchantBranchPOSRepository _posRepository;
        ReferenceRepository _refRepo;
        RequestedMerchantRepository _requestMerchantRepository;

        string country = "";

        string mailFrom = "";
        string host = "";
        string user = "";
        string pass = "";
        bool ssl = false;
        int port = 0;

        public HomeController()
        {
            _accountRepo = new AccountRepository();
            _userRepo = new UserRepository();
            _midsRepository = new MidsRepository();
            _merchantRepo = new MerchantRepository();
            _posRepository = new MerchantBranchPOSRepository();
            _refRepo = new ReferenceRepository();
            _requestMerchantRepository = new RequestedMerchantRepository();

            mailFrom = System.Configuration.ConfigurationManager.AppSettings["mailFrom"].ToString();
            host = System.Configuration.ConfigurationManager.AppSettings["host"].ToString();
            user = System.Configuration.ConfigurationManager.AppSettings["user"].ToString();
            pass = System.Configuration.ConfigurationManager.AppSettings["password"].ToString();
            port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["port"]);
            ssl = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["SSL"]);
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            FormsAuthentication.SignOut();

            if (Session["Success"] != null)
            {
                ViewBag.Success = Session["Success"];
            }

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
            string captcha = Convert.ToString(Request["captcha"]);

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
                        if (captcha == txtCaptcha)
                        {
                            Session["Captcha"] = null;

                            Boolean isNeedsUpdate;

                            var actRefNumber = ApplicationLog.UsersLoginInfo("SDGBackOffice", acc.AccountId, acc.UserId, acc.IsDeleted, acc.IPAddress, "");

                            string msg = "Restricted Access. Authorized users only.";
                        
                            CurrentUser.ParentId = acc.ParentId;
                            CurrentUser.ParentType = (Enums.ParentType)acc.ParentTypeId;
                            CurrentUser.DisplayName = acc.User.FirstName + " " + acc.User.LastName;
                            CurrentUser.Avatar = Url.Content("~/UserPictures/") + Username + "/" + acc.User.PhotoUrl;
                            CurrentUser.Role = ((Enums.PermissionType)acc.RoleId).ToString();
                            CurrentUser.UserId = acc.UserId;
                            CurrentUser.AccountId = acc.AccountId;

                            CurrentUser.Country = country;
                            #region User 1, 2 and 3
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

                                if ((CurrentUser.ParentType == Enums.ParentType.Partner) || CurrentUser.ParentType == Enums.ParentType.Marketing)
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
                                            if (acc.ParentTypeId == 6)
                                            {
                                                return RedirectToAction("Merchants");
                                            }
                                            else
                                            {
                                                return RedirectToAction("Dashboard");
                                            }
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
                                            return RedirectToAction("MerchantLocationsReport");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                ViewBag.Alert = msg;
                            }
                            #endregion
                        }
                        else
                        {
                            ViewBag.Alert = "Incorrect Captcha.";
                        }
                    }
            }
            catch (Exception ex)
            {
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
                            Session["Success"] = "Forgot Password Confirmation: Please check your email for your new password.";

                            Response.Redirect("Index");
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
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Your current username or password was incorrect.");
                    }
                }

            }
            catch (Exception ex)
            {
                ViewBag.Alert = "Unknown error occured." + ex.Message + "Please contact support.";
                Session["Alert"] = ex.Message;
            }
            return View();
        }

        public ActionResult Dashboard()
        {
            //var merchant = _accountRepo.GetTypeIdByAccountId(CurrentUser.UserId);

            bool e = Convert.ToBoolean(Session["Expiry"]);

            if (e == true)
            {
                ViewBag.PasswordExpiration = Session["Message"];
            }

            if (Session["Success"] != null)
            {
                ViewBag.Success = Session["Success"];
            }

            if (Session["Error"] != null)
            {
                ViewBag.Error = Session["Error"];
            }

            Session["Error"] = null;
            Session["Success"] = null;
            Session["Message"] = null;

            var id = CurrentUser.ParentId;//merchant.ParentId;
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
                if (Session["MerchantOutdated"] != null)
                {
                    ViewBag.Outdated = Session["MerchantOutdated"];
                }

                Session["MerchantOutdated"] = null;

                ViewBag.ParentType = "Merchant";
            }

            return View();
        }

        public ActionResult Merchants()
        {
            try
            {
                if (Session["Success"] != null)
                {
                    ViewBag.Success = Session["Success"];
                }

                Session["Success"] = null;
            }
            catch (Exception ex)
            {

            }

            return View();
        }

        public ActionResult RequestMerchants()
        {
            var countries = _refRepo.GetAllCountries();

            ViewBag.Countries = countries.Select(c => new SelectListItem()
            {
                Value = c.CountryId.ToString(),
                Text = c.CountryName
            }).ToList();

            var provinces = _refRepo.GetAllProvinces();

            ViewBag.Provinces = provinces.OrderBy(pr => pr.ProvinceName)
                .Select(c => new SelectListItem()
                {
                    Value = c.ProvinceId.ToString(),
                    Text = c.ProvinceName
                }).ToList();

            return View();
        }

        [HttpPost]
        public ActionResult RequestMerchants(RequestedMerchantModel merchant)
        {
            try
            {

                action = "requesting merchant";

                //string isoCode = null;

                if (!_userRepo.IsUserNameAvailable(merchant.Username))
                {
                    ModelState.AddModelError(string.Empty, "Username is not available.");
                }

                if (!_userRepo.IsEmailAvailable(merchant.MerchantEmail))
                {
                    ModelState.AddModelError(string.Empty, "Email Address is not available.");
                }

                if (ModelState.IsValid && merchant != null)
                {
                    var m = new SDGDAL.Entities.RequestedMerchant();

                    //Merchant Info
                    m.MerchantName = merchant.MerchantName;
                    m.FirstName = merchant.FirstName;
                    m.MiddleName = merchant.MiddleName;
                    m.LastName = merchant.LastName;
                    m.MerchantEmail = merchant.MerchantEmail;
                    m.Address = merchant.Address;
                    m.ProvinceId = merchant.ProvinceId;
                    m.City = merchant.City;
                    m.ZipCode = merchant.ZipCode;
                    m.CountryId = merchant.CountryId;
                    m.PrimaryContactNumber = merchant.PrimaryContactNumber;
                    m.IsActive = false;
                    m.IsDeleted = false;
                    m.DateCreated = DateTime.Now;
                    m.ParentId = CurrentUser.ParentId;

                    //MID Info
                    m.MID = merchant.MID;
                    m.SecureHash = merchant.SecureHash;
                    m.AccessCode = merchant.AccessCode;
                    m.Username = merchant.Username;
                    m.Password = merchant.Password;
                    m.CardTypeId = merchant.CardTypeId;

                    //Request Note
                    m.RequestNote = merchant.RequestNote;

                    var requestM = _requestMerchantRepository.CreateRequestedMerchant(m);

                    if (requestM.RequestedMerchantId > 0)
                    {
                        var userActivity = "Request a Merchant";

                        var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "RequestMerchants", "");

                        Session["Success"] = "Merchant Request has been Sent. Wait for Merchant Approval. Thanks You!";

                        return RedirectToAction("Merchants");
                    }
                }

            }
            catch (Exception ex)
            {

            }
            finally
            {
                var provinces = _refRepo.GetAllProvinces();

                ViewBag.Provinces = provinces.OrderBy(pr => pr.ProvinceName)
                    .Select(c => new SelectListItem()
                    {
                        Value = c.ProvinceId.ToString(),
                        Text = c.ProvinceName
                    }).ToList();

                var countries = _refRepo.GetAllCountries();

                ViewBag.Countries = countries.Select(c => new SelectListItem()
                {
                    Value = c.CountryId.ToString(),
                    Text = c.CountryName
                }).ToList();
            }

            return View();
        }

        public ActionResult UpdateRequestedMerchant(int id)
        {
            try
            {
                var m = new RequestedMerchantModel();

                var r = _requestMerchantRepository.GetMerchant(id);

                m.MerchantName = r.MerchantName;
                m.FirstName = r.FirstName;
                m.MiddleName = r.MiddleName;
                m.LastName = r.LastName;
                m.MerchantEmail = r.MerchantEmail;
                m.Address = r.Address;
                m.City = r.City;
                m.PrimaryContactNumber = r.PrimaryContactNumber;
                m.Fax = r.Fax;
                m.MobileNumber = r.MobileNumber;
                m.ProvinceId = r.ProvinceId;
                m.ZipCode = r.ZipCode;
                m.CountryId = r.CountryId;

                m.MID = r.MID;
                m.SecureHash = r.SecureHash;
                m.AccessCode = r.AccessCode;
                m.Username = r.Username;
                m.Password = r.Password;

                ViewBag.RequestedMerchantId = id;
                ViewBag.CardTypeId = r.CardTypeId;

                m.RequestNote = r.RequestNote;

                var countries = _refRepo.GetAllCountries();

                ViewBag.Countries = countries.Select(c => new SelectListItem()
                {
                    Value = c.CountryId.ToString(),
                    Text = c.CountryName
                }).ToList();

                var provinces = _refRepo.GetAllProvinces();

                ViewBag.Provinces = provinces.OrderBy(pr => pr.ProvinceName)
                    .Select(c => new SelectListItem()
                    {
                        Value = c.ProvinceId.ToString(),
                        Text = c.ProvinceName
                    }).ToList();

                return View(m);
            }
            catch (Exception ex)
            {

            }

            return View();
        }

        [HttpPost]
        public ActionResult UpdateRequestedMerchant(RequestedMerchantModel merchant)
        {

            try
            {
                RequestedMerchant m = new RequestedMerchant();

                var r = _requestMerchantRepository.GetMerchant(merchant.RequestedMerchantId);

                m.RequestedMerchantId = r.RequestedMerchantId;
                m.MerchantName = r.MerchantName;
                m.FirstName = merchant.FirstName;
                m.MiddleName = merchant.MiddleName;
                m.LastName = merchant.LastName;
                m.MerchantEmail = merchant.MerchantEmail;
                m.Address = merchant.Address;
                m.City = merchant.City;
                m.PrimaryContactNumber = merchant.PrimaryContactNumber;
                m.Fax = merchant.Fax;
                m.MobileNumber = merchant.MobileNumber;
                m.ProvinceId = merchant.ProvinceId;
                m.ZipCode = merchant.ZipCode;
                m.CountryId = merchant.CountryId;
                m.IsActive = false;
                m.IsDeleted = false;
                m.DateCreated = r.DateCreated;
                m.ParentId = r.ParentId;

                m.MID = r.MID;
                m.SecureHash = merchant.SecureHash;
                m.AccessCode = merchant.AccessCode;
                m.Username = merchant.Username;
                m.Password = merchant.Password;
                m.CardTypeId = merchant.CardTypeId;

                ViewBag.CardTypeId = r.CardTypeId;

                m.RequestNote = merchant.RequestNote;

                var update = _requestMerchantRepository.UpdateRequestedMerchant(m);

                if (update == true)
                {
                    var userActivity = "Update a Merchant";

                    var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "UpdateRequestedMerchant", "");

                    Session["Success"] = "Successfully Updated";

                    return RedirectToAction("Merchants");
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                var countries = _refRepo.GetAllCountries();

                ViewBag.Countries = countries.Select(c => new SelectListItem()
                {
                    Value = c.CountryId.ToString(),
                    Text = c.CountryName
                }).ToList();

                var provinces = _refRepo.GetAllProvinces();

                ViewBag.Provinces = provinces.OrderBy(pr => pr.ProvinceName)
                    .Select(c => new SelectListItem()
                    {
                        Value = c.ProvinceId.ToString(),
                        Text = c.ProvinceName
                    }).ToList();
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

            CurrentUser.Avatar = Url.Content("~/UserPictures/") + user.Username + "/" + filename;
            return Json(new { success = upload, filename = user.Username + "/" + filename });
        }

        private bool SendEmailPassword(string email, string password)
        {
            Boolean result = false;

            string msg = "Your new password is: " + password;

            #region Send Update Password
            var fromAddress = new MailAddress(user, "VeritasPay");
            var toAddress = new MailAddress(email, "");
            string fromPassword = pass;
            const string subject = "Update Passsword";
            string body = "Your new password is: " + password;

            var smtp = new SmtpClient
            {
                Host = host,
                Port = port,
                EnableSsl = ssl,
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
            #endregion

            result = true;

            return result;
        }

        [HttpPost]
        public JsonResult SendActivationCodeRequest()
        {
            SendActivation.SendActivation sendMail = new SendActivation.SendActivation();

            var merchant = _merchantRepo.GetDetailsbyMerchantId(CurrentUser.ParentId);

            int cId = merchant.ContactInformationId + 1;

            var emailUser = _userRepo.GetMerchantByContactInfoId(cId);

            var info = _merchantRepo.GetMerchantId(emailUser.UserId);

            string userName = info.Username;

            string password = SDGDAL.Utility.Decrypt(info.Password);

            string merchantEmail = "";

            if ((merchant.MerchantEmail != null) || (merchant.MerchantEmail != ""))
            {
                merchantEmail = merchant.MerchantEmail;
            }
            else
            {
                merchantEmail = "";
            }

            string message = "Merchant is requesting for a New Activation Code.<br />"
                        + "Merchant ID: " + "<b>" + CurrentUser.ParentId + "</b><br />"
                        + "Merchant Name: " + "<b>" + merchant.MerchantName + "</b><br />"
                        + "Merchant Username: " + "<b>" + userName + "</b><br />"
                        + "Merchant Password: " + "<b>" + password + "</b><br />"
                        + "Merchant Email: " + "<b>" + merchantEmail + "</b>";

            var send = sendMail.SendActivationCodeRequest(mailFrom, port, host, user, pass, message);

            return Json(send);
        }

        public ActionResult MerchantLocationsReport()
        {

            GetTransType();

            GetTransactionTypes();
            
            ViewBag.User = CurrentUser.ParentId;

            ViewBag.UserType = "branch";

            return View();
        }

        public void GetTransType()
        {
            try
            {
                action = "getting transaction types.";

                var ddlTransTypes = new List<SelectListItem>();

                ddlTransTypes.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = "Select Type",
                    Selected = 0 == 0
                });

                ddlTransTypes.Add(new SelectListItem()
                {
                    Value = "5",
                    Text = "Credit Transaction"
                });

                ddlTransTypes.Add(new SelectListItem()
                {
                    Value = "4",
                    Text = "Debit Transaction"
                });

                ///Uncomment the 2 more transaction type if needed
                //ddlTransTypes.Add(new SelectListItem()
                //{
                //    Value = "3",
                //    Text = "ACH Transaction"
                //});

                //ddlTransTypes.Add(new SelectListItem()
                //{
                //    Value = "4",
                //    Text = "Cash Transaction"
                //});

                ViewBag.TransTypes = ddlTransTypes;
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "GetTransType", ex.StackTrace);
            }
        }

        public void GetTransactionTypes()
        {
            try
            {
                action = "getting the transaction action types.";

                var ddlTransactionTypes = new List<SelectListItem>();

                ddlTransactionTypes.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = "Select Action"
                });

                ddlTransactionTypes.Add(new SelectListItem()
                {
                    Value = "3",
                    Text = "Purchased"
                });

                ddlTransactionTypes.Add(new SelectListItem()
                {
                    Value = "4",
                    Text = "Void"
                });

                ddlTransactionTypes.Add(new SelectListItem()
                {
                    Value = "6",
                    Text = "Declined"
                });

                ViewBag.TransactionTypes = ddlTransactionTypes;
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "GetTransactionTypes", ex.StackTrace);
            }
        }
     }
}
