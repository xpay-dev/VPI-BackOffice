using SDGAdmin.Models;
using SDGDAL.Entities;
using SDGDAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using SDGUtil;
using SDGDAL;

namespace SDGAdmin.Controllers
{
    [Authorize]
    [CustomAttributes.SessionExpireFilter]
    public class PartnersController : Controller
    {
        PartnerRepository _partnerRepo;
        UserRepository _userRepo;
        ReferenceRepository _refRepo;

        string action = string.Empty;

        public PartnersController()
        {
            _partnerRepo = new PartnerRepository();
            _userRepo = new UserRepository();
            _refRepo = new ReferenceRepository();
        }

        public void GetPartner()
        {
            try
            {
                action = "fetching the partner list";

                var partners = _partnerRepo.GetAllPartners();

                var ddlPartners = new List<SelectListItem>();

                if (partners.Count != 0)
                {
                    if (partners.Count == 1)
                    {
                        ddlPartners.AddRange(partners.Select(p => new SelectListItem()
                        {
                            Value = p.PartnerId.ToString(),
                            Text = p.CompanyName
                        }).ToList());
                    }
                    else
                    {
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
                    }
                }
                else
                {
                    ddlPartners.Add(new SelectListItem()
                    {
                        Value = "0",
                        Text = "No Partners available",
                        Selected = 0 == 0
                    });
                }



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

            var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "Index", "");

            if (Session["Success"] != null)
            {
                ViewBag.Success = Session["Success"];
            }

            Session["Success"] = null;

            return View();
        }

        public ActionResult Registration(int? id)
        {
            var userActivity = "Entered Partner Registration Page";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "Registration", "");

            return View();
        }

        [HttpPost]
        public ActionResult Registration(PartnerModel partner)
        {
            try
            {
                action = "creating partner.";

                if (CurrentUser.ParentType != Enums.ParentType.Partner)
                {
                    return HttpNotFound();
                }

                if (!_userRepo.IsUserNameAvailable(partner.User.Username))
                {
                    ModelState.AddModelError(string.Empty, "Username is not available.");
                }

                partner.Address = "N/S";
                partner.City = "N/S";
                partner.PrimaryContactNumber = "N/S";
                partner.ZipCode = "N/S";
                partner.CountryId = 185;

                partner.User.Address = "N/S";
                partner.User.City = "N/S";
                partner.User.PrimaryContactNumber = "N/S";
                partner.User.ZipCode = "N/S";
                partner.User.CountryId = 185;

                ModelState["Address"].Errors.Clear();
                ModelState["City"].Errors.Clear();
                ModelState["PrimaryContactNumber"].Errors.Clear();
                ModelState["ZipCode"].Errors.Clear();
                //ModelState["CountryId"].Errors.Clear();

                //ModelState["User.Address"].Errors.Clear();
                //ModelState["User.City"].Errors.Clear();
                //ModelState["User.PrimaryContactNumber"].Errors.Clear();
                //ModelState["User.ZipCode"].Errors.Clear();
                //ModelState["User.CountryId"].Errors.Clear();

                if (ModelState.IsValid && partner != null)
                {
                    var p = new SDGDAL.Entities.Partner();
                    p.CompanyName = partner.CompanyName;
                    p.CompanyEmail = partner.CompanyEmail;
                    p.ParentPartnerId = null;
                    p.IsActive = true;
                    p.MerchantDiscountRate = partner.MDR;

                    p.ContactInformation = new SDGDAL.Entities.ContactInformation()
                    {
                        Address = partner.Address,
                        City = partner.City,
                        StateProvince = partner.StateProvince,
                        CountryId = 1,
                        ZipCode = partner.ZipCode,
                        PrimaryContactNumber = partner.PrimaryContactNumber,
                        Fax = partner.Fax,
                        MobileNumber = partner.MobileNumber,
                        NeedsUpdate = true
                    };

                    var u = new SDGDAL.Entities.User();

                    u.FirstName = partner.User.FirstName;
                    u.LastName = partner.User.LastName;
                    u.MiddleName = partner.User.MiddleName;
                    u.EmailAddress = partner.User.EmailAddress;

                    u.ContactInformation = new SDGDAL.Entities.ContactInformation()
                    {
                        Address = partner.User.Address,
                        City = partner.User.City,
                        StateProvince = partner.User.StateProvince,
                        CountryId = 1,
                        ZipCode = partner.User.ZipCode,
                        PrimaryContactNumber = partner.User.PrimaryContactNumber,
                        Fax = partner.User.Fax,
                        MobileNumber = partner.User.MobileNumber,
                        NeedsUpdate = true
                    };

                    var acc = new SDGDAL.Entities.Account();
                    acc.Username = partner.User.Username;
                    acc.Password = partner.User.Password;
                    acc.User = u;
                    acc.RoleId = 1;

                    var newPartner = _partnerRepo.CreatePartnerWithUser(p, acc);

                    if (newPartner.PartnerId > 0)
                    {
                        var userActivity = "Registered a Partner";

                        var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "Registration", "");

                        Session["Success"] = "Partner successfully created.";

                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "Registration", ex.StackTrace);

                ErrorLog err = new ErrorLog();
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
                GetPartner();
            }

            return View(partner);
        }

        public ActionResult RegistrationSubPartner(int? id)
        {
            var userActivity = "Entered Partner Registration Page";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "RegistrationSubPartner", "");

            GetPartner();

            if (id.HasValue)
            {
                var partner = new PartnerModel();
                partner.ParentPartner = new PartnerModel();
                partner.ParentPartner.PartnerId = id.Value;
                return View(partner);
            }

            return View(new PartnerModel());
        }

        #region Register Partner

        [HttpPost]
        public ActionResult RegistrationSubPartner(PartnerModel partner)
        {
            try
            {
                action = "creating partner.";

                if (CurrentUser.ParentType != Enums.ParentType.Partner)
                {
                    return HttpNotFound();
                }

                if (!_userRepo.IsUserNameAvailable(partner.User.Username))
                {
                    ModelState.AddModelError(string.Empty, "Username is not available.");
                }

                partner.Address = "N/S";
                partner.City = "N/S";
                partner.PrimaryContactNumber = "N/S";
                partner.ZipCode = "N/S";
                partner.CountryId = 185;

                partner.User.Address = "N/S";
                partner.User.City = "N/S";
                partner.User.PrimaryContactNumber = "N/S";
                partner.User.ZipCode = "N/S";
                partner.User.CountryId = 185;

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

                if (ModelState.IsValid && partner != null)
                {
                    var p = new SDGDAL.Entities.Partner();
                    p.CompanyName = partner.CompanyName;
                    p.CompanyEmail = partner.CompanyEmail;
                    p.ParentPartnerId = partner.PartnerId;
                    p.IsActive = true;
                    p.MerchantDiscountRate = partner.MDR;

                    p.ContactInformation = new SDGDAL.Entities.ContactInformation()
                    {
                        Address = partner.Address,
                        City = partner.City,
                        StateProvince = partner.StateProvince,
                        CountryId = 185,
                        ZipCode = partner.ZipCode,
                        PrimaryContactNumber = partner.PrimaryContactNumber,
                        Fax = partner.Fax,
                        MobileNumber = partner.MobileNumber,
                        NeedsUpdate = true
                    };

                    var u = new SDGDAL.Entities.User();

                    u.FirstName = partner.User.FirstName;
                    u.LastName = partner.User.LastName;
                    u.MiddleName = partner.User.MiddleName;
                    u.EmailAddress = partner.User.EmailAddress;

                    u.ContactInformation = new SDGDAL.Entities.ContactInformation()
                    {
                        Address = partner.User.Address,
                        City = partner.User.City,
                        StateProvince = partner.User.StateProvince,
                        CountryId = 185,
                        ZipCode = partner.User.ZipCode,
                        PrimaryContactNumber = partner.User.PrimaryContactNumber,
                        Fax = partner.User.Fax,
                        MobileNumber = partner.User.MobileNumber,
                        NeedsUpdate = true
                    };

                    var acc = new SDGDAL.Entities.Account();
                    acc.Username = partner.User.Username;
                    acc.Password = partner.User.Password;
                    acc.User = u;
                    acc.RoleId = 1;

                    var newPartner = _partnerRepo.CreatePartnerWithUser(p, acc);

                    if (newPartner.PartnerId > 0)
                    {
                        var userActivity = "Registered a Partner";

                        var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "RegistrationSubPartner", "");

                        Session["Success"] = "Partner successfully created.";

                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "RegistrationSubPartner", ex.StackTrace);

                ErrorLog err = new ErrorLog();
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
                GetPartner();
            }

            return View(partner);
        }

        #endregion

        #region View Info
        public ActionResult ViewInfo(string id)
        {
            if (CurrentUser.ParentType == Enums.ParentType.Partner)
            {
                try
                {
                    action = "fetching data for partner.";

                    var userActivity = "View Partner Info";

                    var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "ViewInfo", "");

                    var partner =
                    id != string.Empty ?
                    _partnerRepo.GetDetailsbyPartnerId(Convert.ToInt32(Utility.Decrypt(id.Contains(" ") == true ? id.Replace(" ", "+") : id)))
                    :
                    _partnerRepo.GetDetailsbyPartnerId(CurrentUser.ParentId);

                    var countries = _refRepo.GetAllCountries();

                    ViewBag.Countries = countries.Select(c => new SelectListItem()
                    {
                        Value = c.CountryId.ToString(),
                        Text = c.CountryName
                    }).ToList();

                    PartnerModel p = new PartnerModel();

                    p.CompanyName = partner.CompanyName;
                    p.CompanyEmail = partner.CompanyEmail;
                    p.LogoUrl = partner.LogoUrl;
                    p.Address = partner.ContactInformation.Address;
                    p.City = partner.ContactInformation.City;
                    p.StateProvince = partner.ContactInformation.StateProvince;
                    p.CountryId = partner.ContactInformation.CountryId;
                    p.ZipCode = partner.ContactInformation.ZipCode;
                    p.PrimaryContactNumber = partner.ContactInformation.PrimaryContactNumber;
                    p.Fax = partner.ContactInformation.Fax;
                    p.MobileNumber = partner.ContactInformation.MobileNumber;
                    p.IsActive = partner.IsActive;

                    return View(p);
                }
                catch (Exception ex)
                {
                    var errorOnAction = "Error while " + action;

                    var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "ViewInfo", ex.StackTrace);

                    var countries = _refRepo.GetAllCountries();

                    ViewBag.Countries = countries.Select(c => new SelectListItem()
                    {
                        Value = c.CountryId.ToString(),
                        Text = c.CountryName
                    }).ToList();
                }
            }
            else
            {
                return HttpNotFound();
            }

            return View(new PartnerModel());
        }
        #endregion

        #region Update Info
        public ActionResult UpdateInfo(string id)
        {
            int pId = 0;
            if (CurrentUser.ParentType == Enums.ParentType.Partner)
            {
                PartnerModel p = new PartnerModel();

                try
                {
                    if (id != null)
                    {
                        Session["id"] = Utility.Decrypt(id.Contains(" ") == true ? id.Replace(" ", "+") : id);
                        pId = Convert.ToInt32(Utility.Decrypt(id.Contains(" ") == true ? id.Replace(" ", "+") : id));
                    }
                    else
                    {
                        pId = Convert.ToInt32(Session["id"]);
                    }

                    action = "fetching the partner info.";

                    var userActivity = "Entered Update Info Page";

                    var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "UpdateInfo", "");

                    var partner =
                    pId != 0 ?
                    _partnerRepo.GetDetailsbyPartnerId(pId)
                    :
                    _partnerRepo.GetDetailsbyPartnerId(CurrentUser.ParentId);


                    var countries = _refRepo.GetAllCountries();

                    ViewBag.Countries = countries.Select(c => new SelectListItem()
                    {
                        Value = c.CountryId.ToString(),
                        Text = c.CountryName
                    }).ToList();


                    TempData["PartnerId"] = partner.PartnerId;//Utility.Decrypt(Convert.ToString(partner.PartnerId));
                    TempData["ContactInformationId"] = partner.ContactInformationId;
                    TempData["CompanyName"] = partner.CompanyName;
                    p.CompanyName = partner.CompanyName;
                    p.CompanyEmail = partner.CompanyEmail;
                    p.LogoUrl = partner.LogoUrl;
                    p.Address = partner.ContactInformation.Address;
                    p.City = partner.ContactInformation.City;
                    p.StateProvince = partner.ContactInformation.StateProvince;
                    p.CountryId = partner.ContactInformation.CountryId;
                    p.ZipCode = partner.ContactInformation.ZipCode;
                    p.PrimaryContactNumber = partner.ContactInformation.PrimaryContactNumber;
                    p.Fax = partner.ContactInformation.Fax;
                    p.MobileNumber = partner.ContactInformation.MobileNumber;
                    p.IsActive = partner.IsActive;

                    return View(p);
                }
                catch (Exception ex)
                {
                    var errorOnAction = "Error while " + action;

                    var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "UpdateInfo", ex.StackTrace);
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
            }
            else
            {
                return HttpNotFound();
            }

            return View(new PartnerModel());
        }

        [HttpPost]
        public ActionResult UpdateInfo(PartnerModel partner)
        {
            if (CurrentUser.ParentType == Enums.ParentType.Partner)
            {
                Partner p = new Partner();

                try
                {
                    action = "updating the partner info.";

                    p.CompanyEmail = partner.CompanyEmail;
                    p.CompanyName = Convert.ToString(TempData["CompanyName"]);
                    p.ContactInformation = new ContactInformation();
                    p.ContactInformationId = Convert.ToInt32(TempData["ContactInformationId"]);
                    p.PartnerId = Convert.ToInt32(TempData["PartnerId"]);
                    p.ContactInformation.ContactInformationId = Convert.ToInt32(TempData["ContactInformationId"]);
                    p.ContactInformation.Address = partner.Address;
                    p.ContactInformation.City = partner.City;
                    p.ContactInformation.StateProvince = partner.StateProvince;
                    p.ContactInformation.ZipCode = partner.ZipCode;
                    p.ContactInformation.CountryId = partner.CountryId;
                    p.ContactInformation.PrimaryContactNumber = partner.PrimaryContactNumber;
                    p.ContactInformation.MobileNumber = partner.MobileNumber;
                    p.ContactInformation.Fax = partner.Fax;
                    p.ContactInformation.NeedsUpdate = false;

                    var ipartner = _partnerRepo.UpdatePartner(p);

                    if (ipartner != null)
                    {
                        var userActivity = "Updates Partner Info";

                        var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "UpdateInfo", "");

                        Session["Success"] = "Successfully Updated.";

                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    var errorOnAction = "Error while " + action;

                    var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "UpdateInfo", ex.StackTrace);

                    ErrorLog err = new ErrorLog();
                    err.Action = "Update Partner Info";
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
                    TempData["PartnerId"] = p.PartnerId;
                    TempData["ContactInformationId"] = p.ContactInformation.ContactInformationId;
                    TempData.Keep();
                    var countries = _refRepo.GetAllCountries();

                    ViewBag.Countries = countries.Select(c => new SelectListItem()
                    {
                        Value = c.CountryId.ToString(),
                        Text = c.CountryName
                    }).ToList();
                }

                return View(partner);
            }
            else
            {
                return HttpNotFound();
            }
        }
        #endregion

        #region New User
        public ActionResult NewUser(int? id)
        {
            var userActivity = "Entered New User Page";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "NewUser", "");

            TempData["PartnerID"] = id;
            var countries = _refRepo.GetAllCountries();

            ViewBag.Countries = countries.Select(c => new SelectListItem()
            {
                Value = c.CountryId.ToString(),
                Text = c.CountryName
            }).ToList();

            return View();
        }

        [HttpPost]
        public ActionResult NewUser(UserModel user)
        {
            try
            {
                action = "creating new user for partner.";

                if (!_userRepo.IsUserNameAvailable(user.Username))
                {
                    ModelState.AddModelError(string.Empty, "Username is not available.");
                    return View();
                }

                if (ModelState.IsValid && user != null)
                {
                    var p = new SDGDAL.Entities.Partner();
                    p.ParentPartnerId = CurrentUser.ParentId;
                    p.PartnerId = Convert.ToInt32(TempData["PartnerID"]);

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

                    var newUser = _partnerRepo.CreatePartnerUser(p, acc);

                    if ((newUser.AccountId > 0) && (newUser.UserId > 0))
                    {
                        var userActivity = "Created New User for Partner";

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

                ErrorLog err = new ErrorLog();
                err.Action = "Create New User for Partner";
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
                    Text = c.CountryName
                }).ToList();
            }

            return View();
        }
        #endregion
    }
}
