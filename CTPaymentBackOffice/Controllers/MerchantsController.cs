using CTPaymentBackOffice.Models;
using SDGDAL.Entities;
using SDGDAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SDGDAL;
using SDGUtil;

namespace CTPaymentBackOffice.Controllers
{
    [Authorize]
    [CustomAttributes.SessionExpireFilter]
    public class MerchantsController : Controller
    {
        MerchantRepository _merchantRepo;
        UserRepository _userRepo;
        ResellerRepository _resellerRepo;
        ReferenceRepository _refRepo;
        DeviceRepository _deviceRepo;
        MerchantFeatureRepository _merchantFeatureRepo;

        string action = string.Empty;

        public MerchantsController()
        {
            _merchantRepo = new MerchantRepository();
            _userRepo = new UserRepository();
            _resellerRepo = new ResellerRepository();
            _refRepo = new ReferenceRepository();
            _deviceRepo = new DeviceRepository();
            _merchantFeatureRepo = new MerchantFeatureRepository();
        }

        public ActionResult Index()
        {
            var userActivity = "Entered Merchant Management Index";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "Index", "");

            int val = 0;

            var reseller = _resellerRepo.GetAllResellersByPartner(CurrentUser.ParentId, "");

            var ddlresellers = new List<SelectListItem>();
            if (reseller.Count < 0)
            {
                ddlresellers.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = "No Reseller available",
                    Selected = val == 0
                });
            }
            else
            {
                if (reseller.Count == 1)
                {
                    ddlresellers.AddRange(reseller.Select(r => new SelectListItem
                    {
                        Value = Convert.ToString(r.ResellerId),
                        Text = r.ResellerName
                    }).ToList());
                }
                else
                {
                    ddlresellers.Add(new SelectListItem()
                    {
                        Value = "0",
                        Text = "All Resellers",
                        Selected = val == 0
                    });

                    ddlresellers.AddRange(reseller.Select(r => new SelectListItem
                    {
                        Value = Convert.ToString(r.ResellerId),
                        Text = r.ResellerName
                    }).ToList());
                }
            }

            ViewBag.Resellers = ddlresellers;

            if (Session["Success"] != null)
            {
                ViewBag.Success = Session["Success"];
            }

            if (TempData["NeedsUpdate"] != null)
            {
                ViewBag.Outdated = TempData["NeedsUpdate"];
            }

            TempData["NeedsUpdate"] = null;

            Session["Success"] = null;

            return View();
        }

        public ActionResult Registration(int? id)
        {
            var userActivity = "Entered Merchant Registration Page";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "Registration", "");

            TempData["ResellerId"] = 0;
            if (id.HasValue)
            {
                TempData["ResellerId"] = id.Value;
            }

            var ddlResellers = new List<SelectListItem>();

            var resellers = _resellerRepo.GetAllResellersByPartner(CurrentUser.ParentId, "");

            var val = 0;

            if (resellers.Count != 0)
            {
                if (resellers.Count == 1)
                {
                    ddlResellers.AddRange(resellers.Select(r => new SelectListItem()
                    {
                        Text = r.ResellerName,
                        Value = r.ResellerId.ToString(),
                        Selected = Convert.ToInt32(TempData["ResellerId"]) == r.ResellerId
                    }).ToList());
                }
                else
                {
                    ddlResellers.Add(new SelectListItem()
                    {
                        Text = "Select a Reseller",
                        Value = "0",
                        Selected = val == 0
                    });

                    ddlResellers.AddRange(resellers.Select(r => new SelectListItem()
                    {
                        Text = r.ResellerName,
                        Value = r.ResellerId.ToString(),
                        Selected = Convert.ToInt32(TempData["ResellerId"]) == r.ResellerId
                    }).ToList());
                }
            }
            else
            {
                ddlResellers.Add(new SelectListItem()
                {
                    Text = "No Resellers available",
                    Value = "0",
                    Selected = val == 0
                });
            }

            ViewBag.Resellers = ddlResellers;

            var currencies = _refRepo.GetAllCurrencies();
            ViewBag.Currencies = currencies.Select(c => new SelectListItem()
            {
                Text = c.CurrencyName + " (" + c.CurrencyCode + ")",
                Value = c.CurrencyId.ToString()
            }).ToList();

            ViewBag.BillingCycles = new List<SelectListItem>()
            {
                new SelectListItem() {
                    Text = "Weekly",
                    Value = "1"
                },
                new SelectListItem() {
                    Text = "Bi-Weekly",
                    Value = "2"
                },
                new SelectListItem() {
                    Text = "Monthly",
                    Value = "3"
                },
                new SelectListItem() {
                    Text = "Quarterly",
                    Value = "4"
                },
                new SelectListItem() {
                    Text = "Annually",
                    Value = "5"
                },
            };

            return View(new MerchantModel());
        }

        [HttpPost]
        public ActionResult Registration(MerchantModel merchant)
        {
            try
            {
                action = "creating merchant";

                if (!_userRepo.IsUserNameAvailable(merchant.User.Username))
                {
                    ModelState.AddModelError(string.Empty, "Username is not available.");
                }

                if (!_userRepo.IsEmailAvailable(merchant.User.EmailAddress))
                {
                    ModelState.AddModelError(string.Empty, "Email Address is not available.");
                }

                if (!_userRepo.IsPinAvailable(merchant.User.PIN))
                {
                    ModelState.AddModelError(string.Empty, "PIN is not available.");
                }

                if (CurrentUser.ParentType == Enums.ParentType.Partner)
                {
                    var resellers = _resellerRepo.GetAllResellersByPartner(CurrentUser.ParentId, "");
                    ViewBag.Resellers = resellers.Select(r => new SelectListItem()
                    {
                        Text = r.ResellerName,
                        Value = r.ResellerId.ToString(),
                        Selected = Convert.ToInt32(TempData["ResellerId"]) == r.ResellerId
                    }).ToList();

                    ViewBag.BillingCycles = new List<SelectListItem>()
                {
                    new SelectListItem() {
                        Text = "Weekly",
                        Value = "1",
                        Selected = 1 == merchant.BillingCycleId
                    },
                    new SelectListItem() {
                        Text = "Bi-Weekly",
                        Value = "2",
                        Selected = 2 == merchant.BillingCycleId
                    },
                    new SelectListItem() {
                        Text = "Monthly",
                        Value = "3",
                        Selected = 3 == merchant.BillingCycleId
                    },
                    new SelectListItem() {
                        Text = "Quarterly",
                        Value = "4",
                        Selected = 4 == merchant.BillingCycleId
                    },
                    new SelectListItem() {
                        Text = "Annually",
                        Value = "5",
                        Selected = 5 == merchant.BillingCycleId
                    },
                };

                    var currencies = _refRepo.GetAllCurrencies();
                    ViewBag.Currencies = currencies.Select(c => new SelectListItem()
                    {
                        Text = c.CurrencyName + " (" + c.CurrencyCode + ")",
                        Value = c.CurrencyId.ToString(),
                        Selected = merchant.CurrencyId == c.CurrencyId
                    }).ToList();

                    merchant.Address = "N/S";
                    merchant.City = "N/S";
                    merchant.PrimaryContactNumber = "N/S";
                    merchant.ZipCode = "N/S";
                    merchant.CountryId = 1;

                    merchant.User.Address = "N/S";
                    merchant.User.City = "N/S";
                    merchant.User.PrimaryContactNumber = "N/S";
                    merchant.User.ZipCode = "N/S";
                    merchant.User.CountryId = 1;

                    ModelState["Address"].Errors.Clear();
                    ModelState["City"].Errors.Clear();
                    ModelState["PrimaryContactNumber"].Errors.Clear();
                    ModelState["ZipCode"].Errors.Clear();

                    ModelState["User.Address"].Errors.Clear();
                    ModelState["User.City"].Errors.Clear();
                    ModelState["User.PrimaryContactNumber"].Errors.Clear();
                    ModelState["User.ZipCode"].Errors.Clear();

                    if (ModelState.IsValid && merchant != null)
                    {
                        var p = new SDGDAL.Entities.Merchant();
                        p.MerchantName = merchant.MerchantName;
                        p.MerchantEmail = merchant.User.EmailAddress;

                        p.ResellerId = merchant.ResellerId;

                        var reseller = _resellerRepo.GetDetailsbyResellerId(merchant.ResellerId);
                        p.PartnerId = reseller.PartnerId;

                        p.IsActive = true;

                        p.MerchantFeatures = new SDGDAL.Entities.MerchantFeatures();
                        p.MerchantFeatures.BillingCycleId = merchant.BillingCycleId;
                        p.MerchantFeatures.UseDefaultAgreements = true;

                        p.ContactInformation = new SDGDAL.Entities.ContactInformation()
                        {
                            Address = merchant.Address,
                            City = merchant.City,
                            StateProvince = merchant.StateProvince,
                            CountryId = merchant.CountryId,
                            ZipCode = merchant.ZipCode,
                            PrimaryContactNumber = merchant.PrimaryContactNumber,
                            Fax = merchant.Fax,
                            MobileNumber = merchant.MobileNumber,
                            NeedsUpdate = true
                        };

                        var u = new SDGDAL.Entities.User();

                        u.FirstName = merchant.User.FirstName;
                        u.LastName = merchant.User.LastName;
                        u.MiddleName = merchant.User.MiddleName;
                        u.EmailAddress = merchant.User.EmailAddress;
                        u.Price = 0;
                        u.ContactInformation = new SDGDAL.Entities.ContactInformation()
                        {
                            Address = merchant.User.Address,
                            City = merchant.User.City,
                            StateProvince = merchant.User.StateProvince,
                            CountryId = merchant.User.CountryId,
                            ZipCode = merchant.User.ZipCode,
                            PrimaryContactNumber = merchant.User.PrimaryContactNumber,
                            Fax = merchant.User.Fax,
                            MobileNumber = merchant.User.MobileNumber,
                            NeedsUpdate = true
                        };

                        var acc = new SDGDAL.Entities.Account();
                        acc.Username = merchant.User.Username;
                        acc.Password = merchant.User.Password;
                        acc.User = u;
                        acc.RoleId = 1;
                        acc.PIN = Utility.Encrypt(merchant.User.PIN);

                        var newMerchant = _merchantRepo.CreateMerchantWithUser(p, acc);

                        if (newMerchant.MerchantId > 0)
                        {
                            var userActivity = "Registered a Merchant";

                            var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "Registration", "");

                            Session["Success"] = "Merchant successfully created.";

                            return RedirectToAction("Index");
                        }
                    }
                }
                else if (CurrentUser.ParentType == Enums.ParentType.Reseller)
                {
                    ViewBag.BillingCycles = new List<SelectListItem>()
                    {
                        new SelectListItem() {
                            Text = "Weekly",
                            Value = "1",
                            Selected = 1 == merchant.BillingCycleId
                        },
                        new SelectListItem() {
                            Text = "Bi-Weekly",
                            Value = "2",
                            Selected = 2 == merchant.BillingCycleId
                        },
                        new SelectListItem() {
                            Text = "Monthly",
                            Value = "3",
                            Selected = 3 == merchant.BillingCycleId
                        },
                        new SelectListItem() {
                            Text = "Quarterly",
                            Value = "4",
                            Selected = 4 == merchant.BillingCycleId
                        },
                        new SelectListItem() {
                            Text = "Annually",
                            Value = "5",
                            Selected = 5 == merchant.BillingCycleId
                        },
                    };

                    var currencies = _refRepo.GetAllCurrencies();
                    ViewBag.Currencies = currencies.Select(c => new SelectListItem()
                    {
                        Text = c.CurrencyName + " (" + c.CurrencyCode + ")",
                        Value = c.CurrencyId.ToString(),
                        Selected = merchant.CurrencyId == c.CurrencyId
                    }).ToList();

                    merchant.Address = "N/S";
                    merchant.City = "N/S";
                    merchant.PrimaryContactNumber = "N/S";
                    merchant.ZipCode = "N/S";
                    merchant.CountryId = 1;

                    merchant.User.Address = "N/S";
                    merchant.User.City = "N/S";
                    merchant.User.PrimaryContactNumber = "N/S";
                    merchant.User.ZipCode = "N/S";
                    merchant.User.CountryId = 1;

                    ModelState["Address"].Errors.Clear();
                    ModelState["City"].Errors.Clear();
                    ModelState["PrimaryContactNumber"].Errors.Clear();
                    ModelState["ZipCode"].Errors.Clear();

                    ModelState["User.Address"].Errors.Clear();
                    ModelState["User.City"].Errors.Clear();
                    ModelState["User.PrimaryContactNumber"].Errors.Clear();
                    ModelState["User.ZipCode"].Errors.Clear();

                    if (ModelState.IsValid && merchant != null)
                    {
                        var p = new SDGDAL.Entities.Merchant();
                        p.MerchantName = merchant.MerchantName;
                        p.ResellerId = CurrentUser.ParentId;

                        var reseller = _resellerRepo.GetDetailsbyResellerId(CurrentUser.ParentId);
                        p.PartnerId = reseller.PartnerId;

                        p.IsActive = true;

                        p.MerchantFeatures = new SDGDAL.Entities.MerchantFeatures();
                        p.MerchantFeatures.BillingCycleId = merchant.BillingCycleId;
                        p.MerchantFeatures.UseDefaultAgreements = true;

                        p.ContactInformation = new SDGDAL.Entities.ContactInformation()
                        {
                            Address = merchant.Address,
                            City = merchant.City,
                            StateProvince = merchant.StateProvince,
                            CountryId = merchant.CountryId,
                            ZipCode = merchant.ZipCode,
                            PrimaryContactNumber = merchant.PrimaryContactNumber,
                            Fax = merchant.Fax,
                            MobileNumber = merchant.MobileNumber,
                            NeedsUpdate = true
                        };

                        var u = new SDGDAL.Entities.User();

                        u.FirstName = merchant.User.FirstName;
                        u.LastName = merchant.User.LastName;
                        u.MiddleName = merchant.User.MiddleName;
                        u.EmailAddress = merchant.User.EmailAddress;
                        u.Price = 0;
                        u.ContactInformation = new SDGDAL.Entities.ContactInformation()
                        {
                            Address = merchant.User.Address,
                            City = merchant.User.City,
                            StateProvince = merchant.User.StateProvince,
                            CountryId = merchant.User.CountryId,
                            ZipCode = merchant.User.ZipCode,
                            PrimaryContactNumber = merchant.User.PrimaryContactNumber,
                            Fax = merchant.User.Fax,
                            MobileNumber = merchant.User.MobileNumber,
                            NeedsUpdate = true
                        };

                        var acc = new SDGDAL.Entities.Account();
                        acc.Username = merchant.User.Username;
                        acc.Password = merchant.User.Password;
                        acc.User = u;
                        acc.RoleId = 1;
                        acc.PIN = Utility.Encrypt(merchant.User.PIN);

                        var newMerchant = _merchantRepo.CreateMerchantWithUser(p, acc);

                        if (newMerchant.MerchantId > 0)
                        {
                            var userActivity = "Registered a Merchant";

                            var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "Registration", "");

                            Session["Success"] = "Merchant successfully created.";

                            return RedirectToAction("Index");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "Registration", ex.StackTrace);

                ErrorLog err = new ErrorLog();
                err.Action = "Merchants Registration";
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
                merchant.Address = "N/S";
                merchant.City = "N/S";
                merchant.PrimaryContactNumber = "N/S";
                merchant.ZipCode = "N/S";
                merchant.CountryId = 1;

                merchant.User.Address = "N/S";
                merchant.User.City = "N/S";
                merchant.User.PrimaryContactNumber = "N/S";
                merchant.User.ZipCode = "N/S";
                merchant.User.CountryId = 1;

                ViewBag.BillingCycles = new List<SelectListItem>()
                {
                    new SelectListItem() {
                        Text = "Weekly",
                        Value = "1",
                        Selected = 1 == merchant.BillingCycleId
                    },
                    new SelectListItem() {
                        Text = "Bi-Weekly",
                        Value = "2",
                        Selected = 2 == merchant.BillingCycleId
                    },
                    new SelectListItem() {
                        Text = "Monthly",
                        Value = "3",
                        Selected = 3 == merchant.BillingCycleId
                    },
                    new SelectListItem() {
                        Text = "Quarterly",
                        Value = "4",
                        Selected = 4 == merchant.BillingCycleId
                    },
                    new SelectListItem() {
                        Text = "Annually",
                        Value = "5",
                        Selected = 5 == merchant.BillingCycleId
                    },
                };

                var currencies = _refRepo.GetAllCurrencies();
                ViewBag.Currencies = currencies.Select(c => new SelectListItem()
                {
                    Text = c.CurrencyName + " (" + c.CurrencyCode + ")",
                    Value = c.CurrencyId.ToString(),
                    Selected = merchant.CurrencyId == c.CurrencyId
                }).ToList();
            }

            return View(merchant);
        }

        public ActionResult NewUser(int? id)
        {
            try
            {
                var userActivity = "Entered New User Page";

                var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "NewUser", "");

                int merchantId = id.HasValue ? id.Value : CurrentUser.ParentId;

                var merchant = _merchantRepo.GetDetailsbyMerchantId(merchantId);

                TempData["MerchantID"] = merchantId;

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

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "NewUser", ex.StackTrace);

                ErrorLog err = new ErrorLog();
                err.Action = "New Merchant User";
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
                action = "creating new user for the merchant";

                int merchantId = Convert.ToInt32(TempData["MerchantID"]);

                if (!_userRepo.IsUserNameAvailable(user.Username))
                {
                    ModelState.AddModelError(string.Empty, "Username is not available.");
                    return View();
                }

                if (!_userRepo.IsPinAvailable(user.PIN))
                {
                    ModelState.AddModelError(string.Empty, "PIN is not available.");
                }

                if (ModelState.IsValid && user != null)
                {
                    var m = new SDGDAL.Entities.Merchant();
                    m.MerchantId = Convert.ToInt32(TempData["MerchantID"]);

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
                    acc.PIN = Utility.Encrypt(user.PIN);
                    acc.User = u;
                    acc.ParentId = m.MerchantId;
                    acc.RoleId = 1;

                    var newUser = _merchantRepo.CreateMerchantUser(m, acc);

                    if ((newUser.AccountId > 0) && (newUser.UserId > 0))
                    {
                        var userActivity = "Created New User for Merchant";

                        var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "NewUser", "");

                        Session["Success"] = "User account successfully created.";

                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "NewUser", ex.StackTrace);

                ModelState.AddModelError(string.Empty, ex.Message);

                ErrorLog err = new ErrorLog();
                err.Action = "Merchants User Registration";
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
                action = "fetching the data for the merchant.";

                var userActivity = "Entered Update Info Page";

                var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "UpdateInfo", "");

                var merchant =
                    id.HasValue ?
                    _merchantRepo.GetDetailsbyMerchantId(id.Value)
                    :
                    _merchantRepo.GetDetailsbyMerchantId(CurrentUser.ParentId);

                if (merchant.MerchantFeatures == null)
                {
                    TempData["NeedsUpdate"] = "Merchant is not updated. Please register another one.";

                    return RedirectToAction("Index");
                }

                TempData["MerchantId"] = merchant.MerchantId;
                TempData["ContactInformationId"] = merchant.ContactInformationId;
                TempData["MerchantFeaturesId"] = merchant.MerchantFeaturesId;
                TempData["BillingCycleId"] = merchant.MerchantFeatures.BillingCycleId;

                var countries = _refRepo.GetAllCountries();

                ViewBag.Countries = countries.Select(c => new SelectListItem()
                {
                    Value = c.CountryId.ToString(),
                    Text = c.CountryName,
                    Selected = c.CountryId == merchant.ContactInformation.CountryId
                }).ToList();

                ViewBag.BillingCycles = new List<SelectListItem>()
                {
                    new SelectListItem() {
                        Text = "Weekly",
                        Value = "1"
                    },
                    new SelectListItem() {
                        Text = "Bi-Weekly",
                        Value = "2"
                    },
                    new SelectListItem() {
                        Text = "Monthly",
                        Value = "3"
                    },
                    new SelectListItem() {
                        Text = "Quarterly",
                        Value = "4"
                    },
                    new SelectListItem() {
                        Text = "Annually",
                        Value = "5"
                    },
                };

                MerchantModel m = new MerchantModel();
                m.MerchantName = merchant.MerchantName;
                m.MerchantEmail = merchant.MerchantEmail;
                m.Address = merchant.ContactInformation.Address;
                m.City = merchant.ContactInformation.City;
                m.StateProvince = merchant.ContactInformation.StateProvince;
                m.ZipCode = merchant.ContactInformation.ZipCode;
                m.CountryId = merchant.ContactInformation.CountryId;
                m.PrimaryContactNumber = merchant.ContactInformation.PrimaryContactNumber;
                m.Fax = merchant.ContactInformation.Fax;
                m.MobileNumber = merchant.ContactInformation.MobileNumber;
                m.BillingCycleId = merchant.MerchantFeatures.BillingCycleId;
                m.IsActive = merchant.IsActive;
                m.MerchantFeaturesId = Convert.ToInt32(merchant.MerchantFeaturesId);

                TempData["MerchantEmail"] = merchant.MerchantEmail;

                return View(m);
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "UpdateInfo", ex.StackTrace);

                ErrorLog err = new ErrorLog();
                err.Action = "Merchants: UpdateInfo";
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

                Session["MerchantOutdated"] = "Your Merchant is not updated. Please contact support for assistance.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UpdateInfo(MerchantModel merchant)
        {
            Merchant m = new Merchant();

            try
            {
                action = "updating the merchant info.";

                var merch = _merchantRepo.GetDetailsbyMerchantId(Convert.ToInt32(TempData["MerchantId"]));

                if (Convert.ToString(TempData["MerchantEmail"]) != merchant.MerchantEmail)
                {
                    if (!_userRepo.IsEmailAvailable(merchant.MerchantEmail))
                    {
                        ModelState.AddModelError(string.Empty, "Email Address is not available.");
                        return View(merchant);
                    }
                }

                m.ContactInformation = new ContactInformation();
                m.MerchantFeatures = new MerchantFeatures();


                var mf = _merchantFeatureRepo.GetDetailsByMerchantFeaturesId(merch.MerchantFeaturesId.Value);

                m.MerchantId = Convert.ToInt32(TempData["MerchantId"]);
                m.MerchantName = merch.MerchantName;
                m.ContactInformationId = Convert.ToInt32(TempData["ContactInformationId"]);
                m.IsActive = merch.IsActive;
                m.IsDeleted = merch.IsDeleted;
                m.DateCreated = merch.DateCreated;
                m.CanCreateSubMerchants = merch.CanCreateSubMerchants;
                m.ParentMerchantId = merch.ParentMerchantId;
                m.ResellerId = merch.ResellerId;
                m.PartnerId = merch.PartnerId;
                m.MerchantFeatures.MerchantFeaturesId = merch.MerchantFeaturesId.Value;
                m.MerchantFeaturesId = merch.MerchantFeaturesId;
                m.EmailServerId = merch.EmailServerId;
                m.MerchantEmail = merchant.MerchantEmail;

                m.ContactInformation.ContactInformationId = Convert.ToInt32(TempData["ContactInformationId"]);
                m.ContactInformation.Address = merchant.Address;
                m.ContactInformation.City = merchant.City;
                m.ContactInformation.StateProvince = merchant.StateProvince;
                m.ContactInformation.ZipCode = merchant.ZipCode;
                m.ContactInformation.CountryId = merchant.CountryId;
                m.ContactInformation.PrimaryContactNumber = merchant.PrimaryContactNumber;
                m.ContactInformation.MobileNumber = merchant.MobileNumber;
                m.ContactInformation.Fax = merchant.Fax;
                m.ContactInformation.NeedsUpdate = false;

                m.MerchantFeaturesId = merch.MerchantFeaturesId.Value;
                m.MerchantFeatures.BillingCycleId = merchant.BillingCycleId;
                m.MerchantFeatures.CurrencyId = mf.CurrencyId;
                m.MerchantFeatures.TermsOfServiceEnabled = mf.TermsOfServiceEnabled;
                m.MerchantFeatures.DisclaimerEnabled = mf.DisclaimerEnabled;
                m.MerchantFeatures.UseDefaultAgreements = mf.UseDefaultAgreements;
                m.MerchantFeatures.LanguageCode = mf.LanguageCode;



                var imerchantcontact = _merchantRepo.UpdateMerchantContact(m);

                if (imerchantcontact.MerchantId > 0)
                {
                    var userActivity = "Updates Merchant Info";

                    var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "UpdateInfo", "");

                    Session["Success"] = "Successfully Updated.";
                    if (CurrentUser.ParentType == Enums.ParentType.Merchant)
                    {
                        return RedirectToAction("Dashboard", "Home");
                    }

                    return RedirectToAction("Index");
                }
                else
                {
                    throw new Exception(merch.MerchantName);
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "UpdateInfo", ex.StackTrace);

                ErrorLog err = new ErrorLog();
                err.Action = "Update Merchant Info";
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
                TempData["MerchantID"] = TempData["MerchantID"];
                TempData["ContactInformationId"] = TempData["ContactInformationId"];
                TempData["MerchantFeaturesId"] = merchant.MerchantFeaturesId;
                TempData["BillingCycleId"] = merchant.BillingCycleId;
                TempData["MerchantEmail"] = TempData["MerchantEmail"];
                TempData["MerchantName"] = TempData["MerchantName"];
                var countries = _refRepo.GetAllCountries();

                ViewBag.Countries = countries.Select(c => new SelectListItem()
                {
                    Value = c.CountryId.ToString(),
                    Text = c.CountryName,
                    Selected = c.CountryId == merchant.CountryId
                }).ToList();

                ViewBag.BillingCycles = new List<SelectListItem>()
                {
                    new SelectListItem() {
                        Text = "Weekly",
                        Value = "1"
                    },
                    new SelectListItem() {
                        Text = "Bi-Weekly",
                        Value = "2"
                    },
                    new SelectListItem() {
                        Text = "Monthly",
                        Value = "3"
                    },
                    new SelectListItem() {
                        Text = "Quarterly",
                        Value = "4"
                    },
                    new SelectListItem() {
                        Text = "Annually",
                        Value = "5"
                    },
                };
            }

            return View(merchant);
        }

        public ActionResult UpdateMyInfo(int? id)
        {
            if (CurrentUser.ParentType == Enums.ParentType.Merchant)
            {
                try
                {
                    action = "fetching the data for the merchant.";

                    var userActivity = "Entered Update Info Page";

                    var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "UpdateMyInfo", "");

                    if (Session["Success"] != null)
                    {
                        ViewBag.Success = Session["Success"];
                    }

                    Session["Success"] = null;

                    var merchant = id.HasValue ? _merchantRepo.GetDetailsbyMerchantId(id.Value)
                    :
                    _merchantRepo.GetDetailsbyMerchantId(CurrentUser.ParentId);

                    TempData["MerchantId"] = merchant.MerchantId;

                    var countries = _refRepo.GetAllCountries();

                    ViewBag.Countries = countries.Select(c => new SelectListItem()
                    {
                        Value = c.CountryId.ToString(),
                        Text = c.CountryName,
                        Selected = c.CountryId == merchant.ContactInformation.CountryId
                    }).ToList();

                    MerchantModel m = new MerchantModel();

                    m.MerchantName = merchant.MerchantName;
                    m.Address = merchant.ContactInformation.Address;
                    m.City = merchant.ContactInformation.City;
                    m.StateProvince = merchant.ContactInformation.StateProvince;
                    m.CountryId = merchant.ContactInformation.CountryId;
                    m.ZipCode = merchant.ContactInformation.ZipCode;
                    m.PrimaryContactNumber = merchant.ContactInformation.PrimaryContactNumber;
                    m.Fax = merchant.ContactInformation.Fax;
                    m.MobileNumber = merchant.ContactInformation.MobileNumber;
                    m.IsActive = merchant.IsActive;

                    return View(m);
                }
                catch (Exception ex)
                {
                    var errorOnAction = "Error while " + action;

                    var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "UpdateMyInfo", ex.StackTrace);
                }
            }
            else
            {
                return HttpNotFound();
            }

            return View(new MerchantModel());
        }

        [HttpPost]
        public ActionResult UpdateMyInfo(MerchantModel merchant)
        {
            try
            {
                action = "updating the merchant info.";

                merchant.MerchantId = Convert.ToInt32(TempData["MerchantId"]);

                merchant.NeedsUpdate = false;

                var param = SDGDAL.Utility.GenerateParams(merchant, new string[] { "MDR", "PIN", "PartnerId", "ResellerId", "PrinterReceiptsPrice", "GiftReceiptsPrice", "EmailReceiptsPrice", "CurrencyId" }, true);

                var imerchant = _merchantRepo.UpdateMerchant(param);

                if (imerchant.MerchantId > 0)
                {
                    var userActivity = "Updates Merchant Info";

                    var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "UpdateInfo", "");

                    Session["Success"] = "Successfully Updated.";

                    return RedirectToAction("UpdateMyInfo");
                }
                else
                {
                    throw new Exception(imerchant.MerchantName);
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "UpdateMyInfo", ex.StackTrace);

                ErrorLog err = new ErrorLog();
                err.Action = "Update Merchant Info";
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
                TempData["MerchantID"] = merchant.MerchantId;
                var countries = _refRepo.GetAllCountries();

                ViewBag.Countries = countries.Select(c => new SelectListItem()
                {
                    Value = c.CountryId.ToString(),
                    Text = c.CountryName,
                    Selected = c.CountryId == merchant.CountryId
                }).ToList();

                ViewBag.BillingCycles = new List<SelectListItem>()
                {
                    new SelectListItem() {
                        Text = "Weekly",
                        Value = "1"
                    },
                    new SelectListItem() {
                        Text = "Bi-Weekly",
                        Value = "2"
                    },
                    new SelectListItem() {
                        Text = "Monthly",
                        Value = "3"
                    },
                    new SelectListItem() {
                        Text = "Quarterly",
                        Value = "4"
                    },
                    new SelectListItem() {
                        Text = "Annually",
                        Value = "5"
                    },
                };
            }

            return View(merchant);
        }
        #endregion

        public ActionResult ViewAssignedDevices()
        {
            return View();
        }

        public ActionResult ViewInfo(int? id)
        {
            if (CurrentUser.ParentType == Enums.ParentType.Partner || CurrentUser.ParentType == Enums.ParentType.Reseller)
            {
                var userActivity = "Entered View Info Page.";

                var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "ViewInfo", "");

                try
                {
                    action = "fetching the merchant data.";
                    var merchant =
                        id.HasValue ?
                        _merchantRepo.GetDetailsbyMerchantId(id.Value)
                        :
                        _merchantRepo.GetDetailsbyMerchantId(CurrentUser.ParentId);

                    TempData["MerchantId"] = merchant.MerchantId;

                    var countries = _refRepo.GetAllCountries();

                    ViewBag.Countries = countries.Select(c => new SelectListItem()
                    {
                        Value = c.CountryId.ToString(),
                        Text = c.CountryName,
                        Selected = c.CountryId == merchant.ContactInformation.CountryId
                    }).ToList();

                    ViewBag.BillingCycles = new List<SelectListItem>()
                {
                    new SelectListItem() {
                        Text = "Weekly",
                        Value = "1"
                    },
                    new SelectListItem() {
                        Text = "Bi-Weekly",
                        Value = "2"
                    },
                    new SelectListItem() {
                        Text = "Monthly",
                        Value = "3"
                    },
                    new SelectListItem() {
                        Text = "Quarterly",
                        Value = "4"
                    },
                    new SelectListItem() {
                        Text = "Annually",
                        Value = "5"
                    },
                };

                    MerchantModel m = new MerchantModel();
                    m.MerchantName = merchant.MerchantName;
                    m.Address = merchant.ContactInformation.Address;
                    m.City = merchant.ContactInformation.City;
                    m.StateProvince = merchant.ContactInformation.StateProvince;
                    m.ZipCode = merchant.ContactInformation.ZipCode;
                    m.CountryId = merchant.ContactInformation.CountryId;
                    m.PrimaryContactNumber = merchant.ContactInformation.PrimaryContactNumber;
                    m.Fax = merchant.ContactInformation.Fax;
                    m.MobileNumber = merchant.ContactInformation.MobileNumber;
                    if (merchant.MerchantFeatures != null)
                    {
                        m.BillingCycleId = merchant.MerchantFeatures.BillingCycleId;
                    }
                    else
                    {
                        m.BillingCycleId = 1;
                    }
                    m.IsActive = merchant.IsActive;

                    return View(m);
                }
                catch (Exception ex)
                {
                    var errorOnAction = "Error while " + action;

                    var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "ViewInfo", ex.StackTrace);
                }
            }
            else
            {
                return HttpNotFound();
            }
            return View(new MerchantModel());
        }

        public ActionResult ViewMyInfo(int? id)
        {
            if (CurrentUser.ParentType == Enums.ParentType.Merchant)
            {
                var userActivity = "Entered View Info Page.";

                var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "ViewMyInfo", "");

                try
                {
                    action = "fetching the merchant data.";

                    var merchant = id.HasValue ? _merchantRepo.GetDetailsbyMerchantId(id.Value)
                    :
                    _merchantRepo.GetDetailsbyMerchantId(CurrentUser.ParentId);

                    var countries = _refRepo.GetAllCountries();

                    ViewBag.Countries = countries.Select(c => new SelectListItem()
                    {
                        Value = c.CountryId.ToString(),
                        Text = c.CountryName,
                        Selected = c.CountryId == merchant.ContactInformation.CountryId
                    }).ToList();

                    MerchantModel m = new MerchantModel();

                    m.MerchantName = merchant.MerchantName;
                    m.Address = merchant.ContactInformation.Address;
                    m.City = merchant.ContactInformation.City;
                    m.StateProvince = merchant.ContactInformation.StateProvince;
                    m.CountryId = merchant.ContactInformation.CountryId;
                    m.ZipCode = merchant.ContactInformation.ZipCode;
                    m.PrimaryContactNumber = merchant.ContactInformation.PrimaryContactNumber;
                    m.Fax = merchant.ContactInformation.Fax;
                    m.MobileNumber = merchant.ContactInformation.MobileNumber;
                    m.IsActive = merchant.IsActive;

                    return View(m);
                }
                catch (Exception ex)
                {
                    var errorOnAction = "Error while " + action;

                    var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "ViewMyInfo", ex.StackTrace);
                }
            }
            else
            {
                return HttpNotFound();
            }
            return View(new MerchantModel());
        }

        public ActionResult AssignDevice(int? id)
        {
            try
            {
                action = "fetching master device info.";

                var userActivity = "Entered Assign Device Page";

                var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "AssignDevice", "");

                TempData["MerchantId"] = id;
                string val = "";
                int value = 0;

                var masterDevice = _deviceRepo.GetAllMasterDevice();

                if (masterDevice.Count == 0)
                    val = "No Device Available";
                else
                    val = "Select Device Name";

                var ddldevice = new List<SelectListItem>();

                ddldevice.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = val,
                    Selected = value == 0
                });

                ddldevice.AddRange(masterDevice.Select(p => new SelectListItem()
                {
                    Value = Convert.ToString(p.MasterDeviceId),
                    Text = p.DeviceName
                }).ToList());

                ViewBag.MasterDevice = ddldevice;

                return View();
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var actRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "AssignDevice", ex.StackTrace);

                ErrorLog err = new ErrorLog();
                err.Action = "ActionResult Merchant AssignDevice";
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
            finally
            {
                string val = "";
                int value = 0;

                var masterDevice = _deviceRepo.GetAllMasterDevice();

                if (masterDevice.Count == 0)
                    val = "No Device Available";
                else
                    val = "Select Device Name";

                var ddldevice = new List<SelectListItem>();

                ddldevice.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = val,
                    Selected = value == 0
                });

                ddldevice.AddRange(masterDevice.Select(p => new SelectListItem()
                {
                    Value = Convert.ToString(p.MasterDeviceId),
                    Text = p.DeviceName
                }).ToList());

                ViewBag.MasterDevice = ddldevice;
            }

            return View();
        }

        [HttpPost]
        public ActionResult AssignDevice(MerchantDeviceModel md)
        {
            try
            {
                action = "assigning device to merchant.";

                DeviceMerchantLink assignDevice = new DeviceMerchantLink();

                string s = Convert.ToString(Request["hdnCtrl"]);
                string[] deviceId = s.Split(',');
                foreach (string i in deviceId)
                {
                    var mDevice = new SDGDAL.Entities.DeviceMerchantLink();
                    mDevice.MerchantId = Convert.ToInt32(TempData["MerchantId"]);
                    mDevice.DeviceId = Convert.ToInt32(i);
                    mDevice.IsDeleted = false;

                    assignDevice = _merchantRepo.AssignMerchantDevice(mDevice);
                }

                if (assignDevice.DeviceMerchantLinkId > 0)
                {
                    var userActivity = "Assigned a Device";

                    var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "AssignDevice", "");

                    Session["Success"] = "Successfully Assigned a device.";

                    return RedirectToAction("Index");
                }

            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "AssignDevice", ex.StackTrace);

                ErrorLog err = new ErrorLog();
                err.Action = "Merchant: Assign Device";
                err.Method = "Assign";
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
                string val = "";
                int value = 0;

                var masterDevice = _deviceRepo.GetAllMasterDevice();

                if (masterDevice.Count == 0)
                    val = "No Device Available";
                else
                    val = "Select Device Name";

                var ddldevice = new List<SelectListItem>();

                ddldevice.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = val,
                    Selected = value == 0
                });

                ddldevice.AddRange(masterDevice.Select(p => new SelectListItem()
                {
                    Value = Convert.ToString(p.MasterDeviceId),
                    Text = p.DeviceName
                }).ToList());

                ViewBag.MasterDevice = ddldevice;
            }

            return View();
        }
    }
}
