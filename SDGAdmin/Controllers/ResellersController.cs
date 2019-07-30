using SDGAdmin.Models;
using SDGDAL.Entities;
using SDGDAL.Repositories;
using SDGUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SDGDAL;

namespace SDGAdmin.Controllers
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

        public void GetPartner()
        {
            try
            {
                action = "fetching the partner list";

                var partners = _partnerRepo.GetAllPartners();

                var ddlPartners = new List<SelectListItem>();

                ddlPartners.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = "Select a Partner",
                    Selected = 0 == 0
                });

                ddlPartners.AddRange(partners.Select(p => new SelectListItem()
                {
                    Value = p.PartnerId.ToString(),
                    Text = p.CompanyName
                }).ToList());

                ViewBag.Partners = ddlPartners;
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "GetPartner", ex.StackTrace);
            }
        }

        public ActionResult Index()
        {
            var userActivity = "Entered Partner Management Index";

            var errRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "Index", "");

            if (Session["Success"] != null)
            {
                ViewBag.Success = Session["Success"];
            }

            Session["Success"] = null;

            GetPartner();

            return View();
        }

        public ActionResult NewUser(int? id)
        {
            var userActivity = "Entered New User Page";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "NewUser", "");

            TempData["ResellerID"] = id;

            try
            {
                action = "fetching the country list";

                var countries = _refRepo.GetAllCountries();

                ViewBag.Countries = countries.Select(c => new SelectListItem()
                {
                    Value = c.CountryId.ToString(),
                    Text = c.CountryName
                }).ToList();
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "Index", ex.StackTrace);
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
                        CountryId = user.CountryId,
                        ZipCode = user.ZipCode,
                        PrimaryContactNumber = user.PrimaryContactNumber,
                        Fax = user.Fax,
                        MobileNumber = user.MobileNumber,
                        NeedsUpdate = true
                    };

                    var acc = new SDGDAL.Entities.Account();
                    acc.Username = user.Username;
                    acc.Password = user.Password;
                    acc.User = u;
                    acc.RoleId = 1;

                    var newUser = _resellerRepo.CreateResellerUser(r, acc);

                    if ((newUser.AccountId > 0) && (newUser.UserId > 0))
                    {
                        var userActivity = "Created New User for Reseller";

                        var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "NewUser", "");

                        Session["Success"] = "User account successfully created.";

                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "NewUser", ex.StackTrace);
            }
            finally
            {
                var countries = _refRepo.GetAllCountries();

                ViewBag.Countries = countries.Select(c => new SelectListItem()
                {
                    Value = c.CountryId.ToString(),
                    Text = c.CountryName
                }).ToList();
            }

            return View();
        }

        #region Update Info
        public ActionResult UpdateInfo(string id)
        {
            try
            {
                int rId = 0;

                if (id != null)
                {
                    Session["id"] = Utility.Decrypt(id.Contains(" ") == true ? id.Replace(" ", "+") : id);
                    rId = Convert.ToInt32(Utility.Decrypt(id.Contains(" ") == true ? id.Replace(" ", "+") : id));
                }
                else
                {
                    rId = Convert.ToInt32(Session["id"]);
                }

                action = "fetching the data for the reseller.";

                var userActivity = "Entered Update Info Page";

                var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "UpdateInfo", "");

                var reseller =
                rId != 0 ?
                _resellerRepo.GetDetailsbyResellerId(rId)
                :
                _resellerRepo.GetDetailsbyResellerId(CurrentUser.ParentId);

                var countries = _refRepo.GetAllCountries();

                ViewBag.Countries = countries.Select(c => new SelectListItem()
                {
                    Value = c.CountryId.ToString(),
                    Text = c.CountryName
                }).ToList();

                ResellerModel r = new ResellerModel();
                TempData["ResellerID"] = reseller.ResellerId;
                TempData["ContactInformationId"] = reseller.ContactInformationId;
                TempData["ResellerName"] = reseller.ResellerName;
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

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "UpdateInfo", ex.StackTrace);

                var countries = _refRepo.GetAllCountries();

                ViewBag.Countries = countries.Select(c => new SelectListItem()
                {
                    Value = c.CountryId.ToString(),
                    Text = c.CountryName
                }).ToList();

                return HttpNotFound();
            }
        }

        [HttpPost]
        public ActionResult UpdateInfo(ResellerModel reseller)
        {
            Reseller r = new Reseller();

            try
            {
                r.ResellerId = Convert.ToInt32(TempData["ResellerID"]);
                r.ResellerName = Convert.ToString(TempData["ResellerName"]);
                r.ResellerEmail = reseller.ResellerEmail;
                r.ContactInformationId = Convert.ToInt32(TempData["ContactInformationId"]);
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

                    var errRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "UpdateInfo", "");

                    Session["Success"] = "Successfully Updated.";

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "UpdateInfo", ex.StackTrace);
            }
            finally
            {
                TempData["ResellerID"] = reseller.ResellerId;
                TempData.Keep();
                var countries = _refRepo.GetAllCountries();

                ViewBag.Countries = countries.Select(c => new SelectListItem()
                {
                    Value = c.CountryId.ToString(),
                    Text = c.CountryName
                }).ToList();
            }

            return View(reseller);
        }

        #endregion

        public ActionResult UpdateMyInfo()
        {
            return View();
        }

        #region View  Reseller Info
        public ActionResult ViewInfo(string id)
        {
            var userActivity = "Entered Reseller Info Page";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "ViewInfo", "");

            try
            {
                action = "fetching the data for the reseller.";

                if (CurrentUser.ParentType == Enums.ParentType.Partner)
                {
                    var reseller =
                    id != string.Empty ?
                    _resellerRepo.GetDetailsbyResellerId(Convert.ToInt32(Utility.Decrypt(id.Contains(" ") == true ? id.Replace(" ", "+") : id)))
                    :
                    _resellerRepo.GetDetailsbyResellerId(CurrentUser.ParentId);

                    var countries = _refRepo.GetAllCountries();

                    ViewBag.Countries = countries.Select(c => new SelectListItem()
                    {
                        Value = c.CountryId.ToString(),
                        Text = c.CountryName
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

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "ViewInfo", ex.StackTrace);
            }

            return View(new ResellerModel());
        }

        public ActionResult ViewMyInfo()
        {
            return View();
        }

        #endregion

        public ActionResult Registration(string id)
        {
            var userActivity = "Entered Reseller Registration Page";

            var errRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "Registration", "");

            if (CurrentUser.ParentType != Enums.ParentType.Partner)
            {
                return HttpNotFound();
            }

            ResellerModel r = new ResellerModel();

            if (!string.IsNullOrEmpty(id))
            {
                TempData["PartnerId"] = Convert.ToInt32(Utility.Decrypt(id.Contains(" ") == true ? id.Replace(" ", "+") : id));//CurrentUser.ParentId;
            }
            //else
            //{
            //    if (_partnerRepo.IsSubPartner(Convert.ToInt32(Utility.Decrypt(id.Contains(" ") == true ? id.Replace(" ", "+") : id)), CurrentUser.ParentId))
            //        r.PartnerId = Convert.ToInt32(Utility.Decrypt(id.Contains(" ") == true ? id.Replace(" ", "+") : id));
            //    else
            //        r.PartnerId = CurrentUser.ParentId;
            //}

            if (CurrentUser.ParentType == Enums.ParentType.Partner)
            {
                GetPartner();
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

                int pId = 0;

                if (reseller.PartnerId > 0)
                {
                    pId = reseller.PartnerId;
                }
                else
                {
                    pId = Convert.ToInt32(TempData["PartnerId"]);
                }

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
                reseller.CountryId = 185;

                reseller.User.Address = "N/S";
                reseller.User.City = "N/S";
                reseller.User.PrimaryContactNumber = "N/S";
                reseller.User.ZipCode = "N/S";
                reseller.User.CountryId = 185;

                if (ModelState.IsValid && reseller != null)
                {
                    var p = new SDGDAL.Entities.Reseller();
                    p.ResellerName = reseller.ResellerName;
                    p.ResellerEmail = reseller.ResellerEmail;
                    p.PartnerId = pId;//reseller.PartnerId;
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

                        var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "Registration", "");

                        Session["Success"] = "Reseller successfully created.";

                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "Registration", ex.StackTrace);

                SDGDAL.Entities.ErrorLog err = new SDGDAL.Entities.ErrorLog();
                err.Action = "Partner Registration";
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
                TempData.Keep();

                if (CurrentUser.ParentType == Enums.ParentType.Partner)
                {
                    var partners = _partnerRepo.GetAllPartnersByParent(Convert.ToInt32(TempData["PartnerId"]), "");

                    var ddlPartners = new List<SelectListItem>();

                    ddlPartners.AddRange(partners.Select(p => new SelectListItem()
                    {
                        Value = p.PartnerId.ToString(),
                        Text = p.CompanyName
                    }).ToList());

                    ViewBag.Partners = ddlPartners;
                }
            }

            return View();
        }
    }
}
