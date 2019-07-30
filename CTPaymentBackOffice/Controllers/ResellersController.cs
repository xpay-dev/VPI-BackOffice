using CTPaymentBackOffice.Models;
using SDGDAL.Entities;
using SDGDAL.Repositories;
using SDGUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CTPaymentBackOffice.Controllers
{
    [Authorize]
    [CustomAttributes.SessionExpireFilter]
    public class ResellersController : Controller
    {
        ResellerRepository _resellerRepo;
        UserRepository _userRepo;
        PartnerRepository _partnerRepo;
        ReferenceRepository _refRepo;

        string action = string.Empty;

        public ResellersController()
        {
            _resellerRepo = new ResellerRepository();
            _userRepo = new UserRepository();
            _partnerRepo = new PartnerRepository();
            _refRepo = new ReferenceRepository();
        }

        //
        // GET: /Resellers/
        public ActionResult Index()
        {
            var userActivity = "Entered Partner Management Index";

            var errRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "Index", "");

            if (Session["Success"] != null)
            {
                ViewBag.Success = Session["Success"];
            }

            Session["Success"] = null;

            return View();
        }

        public ActionResult NewUser(int? id)
        {
            var userActivity = "Entered New User Page";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "NewUser", "");

            try
            {
                action = "fetching the country list";

                int resellerId = id.HasValue ? id.Value : CurrentUser.ParentId;

                TempData["ResellerID"] = resellerId;

                var countries = _refRepo.GetAllCountries();

                ViewBag.Countries = countries.Select(c => new SelectListItem()
                {
                    Value = c.CountryId.ToString(),
                    Text = c.CountryName,
                    Selected = c.CountryId == 1
                }).ToList();

            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "Index", ex.StackTrace);

                ErrorLog err = new ErrorLog();
                err.Action = "New Reseller User";
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

        [HttpPost]
        public ActionResult NewUser(UserModel user)
        {
            try
            {
                action = "creating new user for reseller.";

                if (!_userRepo.IsUserNameAvailable(user.Username))
                {
                    ModelState.AddModelError(string.Empty, "Username is not available.");
                    return View();
                }

                if (ModelState.IsValid && user != null)
                {
                    var r = new SDGDAL.Entities.Reseller();
                    r.ResellerId = Convert.ToInt32(TempData["ResellerID"]);

                    var u = new SDGDAL.Entities.User();

                    u.FirstName = user.FirstName;
                    u.LastName = user.LastName;
                    u.MiddleName = user.MiddleName;
                    u.EmailAddress = user.EmailAddress;

                    u.ContactInformation = new SDGDAL.Entities.ContactInformation()
                    {
                        Address = user.Address,
                        City = user.City,
                        StateProvince = user.StateProvince,
                        CountryId = 1,
                        ZipCode = user.ZipCode,
                        PrimaryContactNumber = user.PrimaryContactNumber,
                        Fax = user.Fax,
                        MobileNumber = user.MobileNumber,
                        NeedsUpdate = true
                    };

                    var acc = new SDGDAL.Entities.Account();
                    acc.Username = user.Username;
                    acc.Password = user.Password;
                    acc.ParentId = r.ResellerId;
                    acc.User = u;
                    acc.RoleId = 1;

                    var newUser = _resellerRepo.CreateResellerUser(r, acc);

                    if (CurrentUser.ParentType != Enums.ParentType.Reseller)
                    {
                        if ((newUser.AccountId > 0) && (newUser.UserId > 0))
                        {
                            var userActivity = "Created New User for Reseller";

                            var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "NewUser", "");

                            Session["Success"] = "User account successfully created.";

                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        return RedirectToAction("Dashboard", "Home");
                    }
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "NewUser", ex.StackTrace);

                ErrorLog err = new ErrorLog();
                err.Action = "Create User for Reseller";
                err.Method = "NewUser";
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
            finally
            {
                var countries = _refRepo.GetAllCountries();

                ViewBag.Countries = countries.Select(c => new SelectListItem()
                {
                    Value = c.CountryId.ToString(),
                    Text = c.CountryName,
                    Selected = c.CountryId == 1
                }).ToList();
            }

            return View();
        }

        #region Update Info

        public ActionResult UpdateInfo(int? id)
        {
            try
            {
                action = "fetching the data for the reseller.";

                var userActivity = "Entered Update Info Page";

                var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "UpdateInfo", "");

                var reseller =
                id.HasValue ?
                _resellerRepo.GetDetailsbyResellerId(id.Value)
                :
                _resellerRepo.GetDetailsbyResellerId(CurrentUser.ParentId);

                var countries = _refRepo.GetAllCountries();

                ViewBag.Countries = countries.Select(c => new SelectListItem()
                {
                    Value = c.CountryId.ToString(),
                    Text = c.CountryName,
                    Selected = c.CountryId == reseller.ContactInformation.CountryId
                }).ToList();

                ResellerModel r = new ResellerModel();
                TempData["ResellerID"] = reseller.ResellerId;
                TempData["ContactInformationId"] = reseller.ContactInformationId;
                r.ResellerId = reseller.ResellerId;
                r.ResellerName = reseller.ResellerName;
                r.ResellerEmail = reseller.ResellerEmail;
                r.Address = reseller.ContactInformation.Address;
                r.City = reseller.ContactInformation.City;
                r.StateProvince = reseller.ContactInformation.StateProvince;
                r.CountryId = reseller.ContactInformation.CountryId;
                r.ZipCode = reseller.ContactInformation.ZipCode;
                r.PrimaryContactNumber = reseller.ContactInformation.PrimaryContactNumber;
                r.Fax = reseller.ContactInformation.Fax;
                r.MobileNumber = reseller.ContactInformation.MobileNumber;
                r.IsActive = reseller.IsActive;

                return View(r);
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "UpdateInfo", ex.StackTrace);

                return HttpNotFound();
            }
        }

        [HttpPost]
        public ActionResult UpdateInfo(ResellerModel reseller)
        {
            Reseller r = new Reseller();

            try
            {
                r.ContactInformation = new ContactInformation();

                r.ContactInformation.ContactInformationId = Convert.ToInt32(TempData["ContactInformationId"]);
                r.ContactInformation.Address = reseller.Address;
                r.ContactInformation.City = reseller.City;
                r.ContactInformation.StateProvince = reseller.StateProvince;
                r.ContactInformation.ZipCode = reseller.ZipCode;
                r.ContactInformation.CountryId = reseller.CountryId;
                r.ContactInformation.PrimaryContactNumber = reseller.PrimaryContactNumber;
                r.ContactInformation.MobileNumber = reseller.MobileNumber;
                r.ContactInformation.Fax = reseller.Fax;
                r.ContactInformation.NeedsUpdate = false;

                var ireseller = _resellerRepo.UpdateReseller(r);

                if (ireseller != null)
                {
                    var userActivity = "Updates Reseller Info";

                    var errRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "UpdateInfo", "");

                    Session["Success"] = "Successfully Updated.";

                    if (CurrentUser.ParentType == Enums.ParentType.Reseller)
                    {
                        return RedirectToAction("Dashboard", "Home");
                    }

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "UpdateInfo", ex.StackTrace);

                ErrorLog err = new ErrorLog();
                err.Action = "Update Reseller Info";
                err.Method = "UpdateInfo";
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
            finally
            {
                TempData["ResellerID"] = reseller.ResellerId;
                var countries = _refRepo.GetAllCountries();

                ViewBag.Countries = countries.Select(c => new SelectListItem()
                {
                    Value = c.CountryId.ToString(),
                    Text = c.CountryName,
                    Selected = c.CountryId == reseller.CountryId
                }).ToList();
            }

            return View(reseller);
        }

        #endregion

        public ActionResult UpdateMyInfo(int? id)
        {
            try
            {
                action = "fetching the data for the reseller.";

                var userActivity = "Entered Update Info Page";

                var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "UpdateMyInfo", "");

                var reseller =
                id.HasValue ?
                _resellerRepo.GetDetailsbyResellerId(id.Value)
                :
                _resellerRepo.GetDetailsbyResellerId(CurrentUser.ParentId);

                var countries = _refRepo.GetAllCountries();

                ViewBag.Countries = countries.Select(c => new SelectListItem()
                {
                    Value = c.CountryId.ToString(),
                    Text = c.CountryName,
                    Selected = c.CountryId == reseller.ContactInformation.CountryId
                }).ToList();

                ResellerModel r = new ResellerModel();
                TempData["ResellerID"] = reseller.ResellerId;
                r.ResellerId = reseller.ResellerId;
                r.ResellerName = reseller.ResellerName;
                r.ResellerEmail = reseller.ResellerEmail;
                r.Address = reseller.ContactInformation.Address;
                r.City = reseller.ContactInformation.City;
                r.StateProvince = reseller.ContactInformation.StateProvince;
                r.CountryId = reseller.ContactInformation.CountryId;
                r.ZipCode = reseller.ContactInformation.ZipCode;
                r.PrimaryContactNumber = reseller.ContactInformation.PrimaryContactNumber;
                r.Fax = reseller.ContactInformation.Fax;
                r.MobileNumber = reseller.ContactInformation.MobileNumber;
                r.IsActive = reseller.IsActive;

                return View(r);
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "UpdateMyInfo", ex.StackTrace);

                ErrorLog err = new ErrorLog();
                err.Action = "ViewMy Info: Reseller Info";
                err.Method = "View";
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

        [HttpPost]
        public ActionResult UpdateMyInfo(ResellerModel reseller)
        {
            try
            {
                action = "updating the reseller info.";

                reseller.ResellerId = Convert.ToInt32(TempData["ResellerID"]);
                reseller.NeedsUpdate = false;
                var param = SDGDAL.Utility.GenerateParams(reseller, new string[] { "MDR", "PartnerId" }, true);

                var ireseller = _resellerRepo.UpdateReseller(param);

                if (ireseller != null)
                {
                    var userActivity = "Updates Reseller Info";

                    var errRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "UpdateMyInfo", "");

                    Session["Success"] = "Successfully Updated.";

                    return RedirectToAction("Dashboard", "Home");
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "UpdateMyInfo", ex.StackTrace);

                ErrorLog err = new ErrorLog();
                err.Action = "Update Reseller Info";
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
            finally
            {
                TempData["ResellerID"] = reseller.ResellerId;
                var countries = _refRepo.GetAllCountries();

                ViewBag.Countries = countries.Select(c => new SelectListItem()
                {
                    Value = c.CountryId.ToString(),
                    Text = c.CountryName,
                    Selected = c.CountryId == reseller.CountryId
                }).ToList();
            }

            return View(reseller);
        }

        #region View  Reseller Info
        public ActionResult ViewInfo(int? id)
        {
            var userActivity = "Entered Reseller Info Page";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "ViewInfo", "");

            try
            {
                action = "fetching the data for the reseller.";

                if (CurrentUser.ParentType == Enums.ParentType.Partner)
                {
                    var reseller =
                    id.HasValue ?
                    _resellerRepo.GetDetailsbyResellerId(id.Value)
                    :
                    _resellerRepo.GetDetailsbyResellerId(CurrentUser.ParentId);

                    var countries = _refRepo.GetAllCountries();

                    ViewBag.Countries = countries.Select(c => new SelectListItem()
                    {
                        Value = c.CountryId.ToString(),
                        Text = c.CountryName,
                        Selected = c.CountryId == reseller.ContactInformation.CountryId
                    }).ToList();

                    ResellerModel r = new ResellerModel();
                    TempData["ResellerID"] = reseller.ResellerId;
                    r.ResellerId = reseller.ResellerId;
                    r.ResellerName = reseller.ResellerName;
                    r.ResellerEmail = reseller.ResellerEmail;
                    r.Address = reseller.ContactInformation.Address;
                    r.City = reseller.ContactInformation.City;
                    r.StateProvince = reseller.ContactInformation.StateProvince;
                    r.CountryId = reseller.ContactInformation.CountryId;
                    r.ZipCode = reseller.ContactInformation.ZipCode;
                    r.PrimaryContactNumber = reseller.ContactInformation.PrimaryContactNumber;
                    r.Fax = reseller.ContactInformation.Fax;
                    r.MobileNumber = reseller.ContactInformation.MobileNumber;
                    r.IsActive = reseller.IsActive;

                    return View(r);
                }
                else
                {
                    return HttpNotFound();
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "ViewInfo", ex.StackTrace);
            }

            return View(new ResellerModel());
        }

        public ActionResult ViewMyInfo(int? id)
        {
            var userActivity = "Entered Reseller Info Page";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "ViewMyInfo", "");

            try
            {
                action = "fetching the data for the reseller.";

                var reseller =
                id.HasValue ?
                _resellerRepo.GetDetailsbyResellerId(id.Value)
                :
                _resellerRepo.GetDetailsbyResellerId(CurrentUser.ParentId);

                var countries = _refRepo.GetAllCountries();

                ViewBag.Countries = countries.Select(c => new SelectListItem()
                {
                    Value = c.CountryId.ToString(),
                    Text = c.CountryName,
                    Selected = c.CountryId == reseller.ContactInformation.CountryId
                }).ToList();

                ResellerModel r = new ResellerModel();
                TempData["ResellerID"] = reseller.ResellerId;
                r.ResellerId = reseller.ResellerId;
                r.ResellerName = reseller.ResellerName;
                r.ResellerEmail = reseller.ResellerEmail;
                r.Address = reseller.ContactInformation.Address;
                r.City = reseller.ContactInformation.City;
                r.StateProvince = reseller.ContactInformation.StateProvince;
                r.CountryId = reseller.ContactInformation.CountryId;
                r.ZipCode = reseller.ContactInformation.ZipCode;
                r.PrimaryContactNumber = reseller.ContactInformation.PrimaryContactNumber;
                r.Fax = reseller.ContactInformation.Fax;
                r.MobileNumber = reseller.ContactInformation.MobileNumber;
                r.IsActive = reseller.IsActive;

                return View(r);
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "ViewMyInfo", ex.StackTrace);

                ErrorLog err = new ErrorLog();
                err.Action = "ViewMy Info: Reseller Info";
                err.Method = "View";
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

        #endregion

        public ActionResult Registration(int? id)
        {
            var userActivity = "Entered Reseller Registration Page";

            var errRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "Registration", "");

            if (CurrentUser.ParentType != Enums.ParentType.Partner)
            {
                return HttpNotFound();
            }

            ResellerModel r = new ResellerModel();

            if (!id.HasValue)
            {
                r.PartnerId = CurrentUser.ParentId;
            }
            else
            {
                if (_partnerRepo.IsSubPartner(id.Value, CurrentUser.ParentId))
                    r.PartnerId = id.Value;
                else
                    r.PartnerId = CurrentUser.ParentId;
            }

            if (CurrentUser.ParentType == Enums.ParentType.Partner)
            {
                var partners = _partnerRepo.GetAllPartnersByParent(r.PartnerId, "");

                var ddlPartners = new List<SelectListItem>();

                ddlPartners.AddRange(partners.Select(p => new SelectListItem()
                {
                    Value = p.PartnerId.ToString(),
                    Text = p.CompanyName,
                    Selected = r.PartnerId == p.PartnerId
                }).ToList());

                ViewBag.Partners = ddlPartners;
            }

            return View(r);
        }

        [HttpPost]
        public ActionResult Registration(ResellerModel reseller)
        {
            //TODO: Do something so that reseller can create their sub resellers

            try
            {
                action = "creating reseller.";

                if (CurrentUser.ParentType != Enums.ParentType.Partner)
                {
                    return HttpNotFound();
                }

                if (!_userRepo.IsUserNameAvailable(reseller.User.Username))
                {
                    ModelState.AddModelError(string.Empty, "Username is not available.");
                }

                reseller.Address = "N/S";
                reseller.City = "N/S";
                reseller.PrimaryContactNumber = "N/S";
                reseller.ZipCode = "N/S";
                reseller.CountryId = 1;

                reseller.User.Address = "N/S";
                reseller.User.City = "N/S";
                reseller.User.PrimaryContactNumber = "N/S";
                reseller.User.ZipCode = "N/S";
                reseller.User.CountryId = 1;

                ModelState["Address"].Errors.Clear();
                ModelState["City"].Errors.Clear();
                ModelState["PrimaryContactNumber"].Errors.Clear();
                ModelState["ZipCode"].Errors.Clear();
                //ModelState["CountryId"].Errors.Clear();

                ModelState["User.Address"].Errors.Clear();
                ModelState["User.City"].Errors.Clear();
                ModelState["User.PrimaryContactNumber"].Errors.Clear();
                ModelState["User.ZipCode"].Errors.Clear();
                //ModelState["User.CountryId"].Errors.Clear();

                if (ModelState.IsValid && reseller != null)
                {
                    var p = new SDGDAL.Entities.Reseller();
                    p.ResellerName = reseller.ResellerName;
                    p.ResellerEmail = reseller.ResellerEmail;
                    p.PartnerId = reseller.PartnerId;
                    p.IsActive = true;

                    p.ContactInformation = new SDGDAL.Entities.ContactInformation()
                    {
                        Address = reseller.Address,
                        City = reseller.City,
                        StateProvince = reseller.StateProvince,
                        CountryId = reseller.CountryId,
                        ZipCode = reseller.ZipCode,
                        PrimaryContactNumber = reseller.PrimaryContactNumber,
                        Fax = reseller.Fax,
                        MobileNumber = reseller.MobileNumber,
                        NeedsUpdate = true
                    };

                    var u = new SDGDAL.Entities.User();

                    u.FirstName = reseller.User.FirstName;
                    u.LastName = reseller.User.LastName;
                    u.MiddleName = reseller.User.MiddleName;
                    u.EmailAddress = reseller.User.EmailAddress;

                    u.ContactInformation = new SDGDAL.Entities.ContactInformation()
                    {
                        Address = reseller.User.Address,
                        City = reseller.User.City,
                        StateProvince = reseller.User.StateProvince,
                        CountryId = reseller.User.CountryId,
                        ZipCode = reseller.User.ZipCode,
                        PrimaryContactNumber = reseller.User.PrimaryContactNumber,
                        Fax = reseller.User.Fax,
                        MobileNumber = reseller.User.MobileNumber,
                        NeedsUpdate = true
                    };

                    var acc = new SDGDAL.Entities.Account();
                    acc.Username = reseller.User.Username;
                    acc.Password = reseller.User.Password;
                    acc.User = u;
                    acc.RoleId = 1;

                    var newReseller = _resellerRepo.CreateResellerWithUser(p, acc);

                    if (newReseller.ResellerId > 0)
                    {
                        var userActivity = "Registered a Reseller";

                        var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "Registration", "");

                        Session["Success"] = "Reseller successfully created.";

                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "Registration", ex.StackTrace);

                SDGDAL.Entities.ErrorLog err = new SDGDAL.Entities.ErrorLog();
                err.Action = "Reseller Registration";
                err.Method = "Registration";
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

            return View(reseller);
        }
    }
}
