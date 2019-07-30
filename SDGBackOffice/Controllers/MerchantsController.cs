using SDGBackOffice.Models;
using SDGDAL.Entities;
using SDGDAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SDGDAL;
using SDGUtil;
using System.IO;
using System.Data;
using NPOI.HSSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Globalization;

namespace SDGBackOffice.Controllers
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
        RequestedMerchantRepository _requestedMerchantRepo;
        MerchantBranchRepository _branchRepo;
        MidsRepository _midsRepo;

        string action = string.Empty;

        public MerchantsController()
        {
            _merchantRepo = new MerchantRepository();
            _userRepo = new UserRepository();
            _resellerRepo = new ResellerRepository();
            _refRepo = new ReferenceRepository();
            _deviceRepo = new DeviceRepository();
            _merchantFeatureRepo = new MerchantFeatureRepository();
            _requestedMerchantRepo = new RequestedMerchantRepository();
            _branchRepo = new MerchantBranchRepository();
            _midsRepo = new MidsRepository();

        }

        public ActionResult Index()
        {
            var userActivity = "Entered Merchant Management Index";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "Index", "");

            if (CurrentUser.ParentType == Enums.ParentType.Partner)
            {
                ViewBag.UserId = 0;
            }
            else if (CurrentUser.ParentType == Enums.ParentType.Reseller)
            {
                ViewBag.UserId = CurrentUser.ParentId;
            }

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
                        Text = "Select Reseller",
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

                string isoCode = null;
                string country = Convert.ToString(CurrentUser.Country);

                if (!_userRepo.IsUserNameAvailable(merchant.User.Username))
                {
                    ModelState.AddModelError(string.Empty, "Username is not available.");
                }

                if (!_userRepo.IsEmailAvailable(merchant.User.EmailAddress))
                {
                    ModelState.AddModelError(string.Empty, "Email Address is not available.");
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

                    var provinces = _refRepo.GetAllProvinces();

                    if ((country == "Philippines") || country == "Philippine")
                    {
                        merchant.CountryId = 185;
                        merchant.StateProvince = provinces[80].ProvinceName;
                        isoCode = provinces[80].IsoCode;
                    }
                    else
                    {
                        merchant.CountryId = 1;
                    }

                    merchant.User.Address = "N/S";
                    merchant.User.City = "N/S";
                    merchant.User.PrimaryContactNumber = "N/S";
                    merchant.User.ZipCode = "N/S";

                    if ((country == "Philippines") || country == "Philippine")
                    {
                        merchant.User.CountryId = 185;
                        merchant.User.StateProvince = provinces[80].ProvinceName;
                        isoCode = provinces[80].IsoCode;
                    }
                    else
                    {
                        merchant.User.CountryId = 1;
                    }

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
                        p.CurrencyId = merchant.CurrencyId;

                        var reseller = _resellerRepo.GetDetailsbyResellerId(merchant.ResellerId);
                        p.PartnerId = reseller.PartnerId;

                        p.IsActive = true;
                        p.NeedAddToCT = true;
                        p.NeedUpdateToCT = false;

                        p.MerchantFeatures = new SDGDAL.Entities.MerchantFeatures();
                        p.CurrencyId = merchant.CurrencyId;
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
                            NeedsUpdate = true,
                            ProvIsoCode = isoCode
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
                            NeedsUpdate = true,
                            ProvIsoCode = isoCode
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
                        p.MerchantEmail = merchant.User.EmailAddress;
                        p.ResellerId = CurrentUser.ParentId;

                        var reseller = _resellerRepo.GetDetailsbyResellerId(CurrentUser.ParentId);
                        p.PartnerId = reseller.PartnerId;

                        p.IsActive = true;
                        p.CurrencyId = merchant.CurrencyId;

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

        #region Merchant Bulk Boarding
        public ActionResult MerchantBulkBoarding()
        {
            var userActivity = "Entered Merchant Bulk Boarding Page";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "MerchantBulkBoarding", "");

            TempData["ResellerId"] = 0;
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

            return View();
        }

        [HttpPost]
        public ActionResult MerchantBulkBoarding(FormCollection formCollection)
        {
            try
            {
                string name = RegionInfo.CurrentRegion.DisplayName;

                action = "merchant bulk uploading.";

                bool notAvailable = false;
                bool isNull = false;
                string user = "";
                string email = "";
                string rowNumber = "";
                string dateToday = DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Year;

                List<Merchant> p = new List<Merchant>();

                List<Account> acc = new List<Account>();

                List<Mid> mid = new List<Mid>();

                int rId = Convert.ToInt32(Request["rId"]);
                int rowNum = 0;
                HSSFWorkbook hssfwb;
                XSSFWorkbook xssfwb;

                ISheet sheet = null;

                if (Request != null)
                {

                    HttpPostedFileBase file = Request.Files["fileUpload"];
                    string baseDirectory = AppDomain.CurrentDomain.BaseDirectory + "\\ExcelFiles" + "\\" + dateToday;

                    if (!Directory.Exists(baseDirectory))
                    {
                        Directory.CreateDirectory(baseDirectory);
                    }

                    string savePath = Path.Combine(baseDirectory, Path.GetFileName(file.FileName));
                    file.SaveAs(savePath);
                    string extension = Path.GetExtension(savePath);

                    try
                    {
                        if (extension.Equals(".xls"))
                        {
                            using (FileStream files = new FileStream(savePath, FileMode.Open, FileAccess.Read))
                            {
                                hssfwb = new HSSFWorkbook(files);
                            }

                            sheet = hssfwb.GetSheetAt(0);
                        }

                        if (extension.Equals(".xlsx"))
                        {
                            using (FileStream files = new FileStream(savePath, FileMode.Open, FileAccess.Read))
                            {
                                xssfwb = new XSSFWorkbook(files);
                            }

                            sheet = xssfwb.GetSheetAt(0);
                        }

                        //Check Required value if null
                        for (int i = 1; i <= sheet.LastRowNum; i++)
                        {
                            string telNumber = Convert.ToString(sheet.GetRow(i).GetCell(6));
                            bool check1 = sheet.GetRow(i).GetCell(0).StringCellValue == "" ? true : false;
                            bool check2 = sheet.GetRow(i).GetCell(1).StringCellValue == "" ? true : false;
                            bool check3 = sheet.GetRow(i).GetCell(3).StringCellValue == "" ? true : false;
                            bool check4 = sheet.GetRow(i).GetCell(4).StringCellValue == "" ? true : false;
                            bool check5 = sheet.GetRow(i).GetCell(5).StringCellValue == "" ? true : false;
                            bool check6 = telNumber == "" ? true : false;
                            bool check7 = sheet.GetRow(i).GetCell(7).StringCellValue == "" ? true : false;
                            bool check8 = sheet.GetRow(i).GetCell(8).StringCellValue == "" ? true : false;
                            bool check9 = sheet.GetRow(i).GetCell(9).NumericCellValue == 0 ? true : false;
                            bool check10 = sheet.GetRow(i).GetCell(10).StringCellValue == "" ? true : false;
                            bool check11 = sheet.GetRow(i).GetCell(11).StringCellValue == "" ? true : false;
                            bool check12 = sheet.GetRow(i).GetCell(12).StringCellValue == "" ? true : false;
                            bool check13 = sheet.GetRow(i).GetCell(13).StringCellValue == "" ? true : false;
                            bool check14 = sheet.GetRow(i).GetCell(14).StringCellValue == "" ? true : false;
                            bool check15 = sheet.GetRow(i).GetCell(15).StringCellValue == "" ? true : false;

                            if ((check1 == true) || (check2 == true) || (check3 == true) || (check4 == true) || (check5 == true) ||
                                (check6 == true) || (check7 == true) || (check8 == true) || (check9 == true) || (check10 == true) ||
                                (check11 == true) || (check12 == true) || (check13 == true) || (check14 == true) || (check15 == true))
                            {
                                isNull = true;
                                rowNumber += i + ", ";
                            }
                        }

                        for (int row = 1; row <= sheet.LastRowNum; row++)
                        {
                            string userName = Convert.ToString(sheet.GetRow(row).GetCell(7).StringCellValue);
                            string emailAddress = Convert.ToString(sheet.GetRow(row).GetCell(4).StringCellValue);



                            if (!_userRepo.IsUserNameAvailable(userName))
                            {
                                notAvailable = true;
                                user += userName + ", ";
                            }

                            if (!_userRepo.IsEmailAvailable(emailAddress))
                            {
                                notAvailable = true;
                                email += emailAddress + ", ";
                            }

                            #region Partner and Reseller
                            if (CurrentUser.ParentType == Enums.ParentType.Partner)
                            {
                                var reseller = _resellerRepo.GetDetailsbyResellerId(rId);

                                p.Add(new Merchant()
                                {
                                    MerchantName = Convert.ToString(sheet.GetRow(row).GetCell(0).StringCellValue),
                                    MerchantEmail = Convert.ToString(sheet.GetRow(row).GetCell(4).StringCellValue),
                                    ResellerId = rId,
                                    PartnerId = reseller.PartnerId,
                                    IsActive = true,
                                    DateCreated = DateTime.Now,
                                    MerchantFeatures = new SDGDAL.Entities.MerchantFeatures()
                                    {
                                        BillingCycleId = 3,
                                        UseDefaultAgreements = true,
                                        CurrencyId = 1,
                                        LanguageCode = "EN_CA"
                                    },
                                    ContactInformation = new SDGDAL.Entities.ContactInformation()
                                    {
                                        Address = Convert.ToString(sheet.GetRow(row).GetCell(5).StringCellValue),
                                        City = "N/S",
                                        StateProvince = "N/S",
                                        CountryId = 1,
                                        ZipCode = "N/S",
                                        PrimaryContactNumber = Convert.ToString(sheet.GetRow(row).GetCell(6)),
                                        Fax = "N/S",
                                        MobileNumber = "N/S",
                                        NeedsUpdate = true
                                    },
                                });

                                var u = new SDGDAL.Entities.User();


                                string middleName;

                                try
                                {
                                    middleName = sheet.GetRow(row).GetCell(2).StringCellValue;
                                }
                                catch
                                {
                                    middleName = null;
                                }

                                u.FirstName = Convert.ToString(sheet.GetRow(row).GetCell(1).StringCellValue);
                                u.LastName = Convert.ToString(sheet.GetRow(row).GetCell(3).StringCellValue);
                                u.MiddleName = middleName;
                                u.EmailAddress = Convert.ToString(sheet.GetRow(row).GetCell(4).StringCellValue);
                                u.Price = 0;
                                u.ContactInformation = new SDGDAL.Entities.ContactInformation()
                                {
                                    Address = Convert.ToString(sheet.GetRow(row).GetCell(5).StringCellValue),
                                    City = "N/S",
                                    StateProvince = "N/S",
                                    CountryId = 1,
                                    ZipCode = "N/S",
                                    PrimaryContactNumber = Convert.ToString(sheet.GetRow(row).GetCell(6)),
                                    Fax = "N/S",
                                    MobileNumber = "N/S",
                                    NeedsUpdate = true
                                };

                                //var acc = new SDGDAL.Entities.Account();

                                acc.Add(new Account()
                                {
                                    Username = Convert.ToString(sheet.GetRow(row).GetCell(7).StringCellValue),
                                    Password = Convert.ToString(sheet.GetRow(row).GetCell(8).StringCellValue),
                                    User = u,
                                    RoleId = 1,
                                    PIN = Utility.Encrypt(Convert.ToString(sheet.GetRow(row).GetCell(9).NumericCellValue))
                                });

                                var t = new SDGDAL.Entities.TransactionCharges();
                                t.DiscountRate = sheet.GetRow(row).GetCell(16) == null ? 0 : Convert.ToDecimal(sheet.GetRow(row).GetCell(16).NumericCellValue);
                                t.CardNotPresent = sheet.GetRow(row).GetCell(17) == null ? 0 : Convert.ToDecimal(sheet.GetRow(row).GetCell(17).NumericCellValue);
                                t.Declined = sheet.GetRow(row).GetCell(18) == null ? 0 : Convert.ToDecimal(sheet.GetRow(row).GetCell(18).NumericCellValue);
                                t.eCommerce = sheet.GetRow(row).GetCell(19) == null ? 0 : Convert.ToDecimal(sheet.GetRow(row).GetCell(19).NumericCellValue);
                                t.PreAuth = sheet.GetRow(row).GetCell(20) == null ? 0 : Convert.ToDecimal(sheet.GetRow(row).GetCell(20).NumericCellValue);
                                t.Capture = sheet.GetRow(row).GetCell(21) == null ? 0 : Convert.ToDecimal(sheet.GetRow(row).GetCell(21).NumericCellValue);
                                t.Purchased = sheet.GetRow(row).GetCell(22) == null ? 0 : Convert.ToDecimal(sheet.GetRow(row).GetCell(22).NumericCellValue);
                                t.Refund = sheet.GetRow(row).GetCell(23) == null ? 0 : Convert.ToDecimal(sheet.GetRow(row).GetCell(23).NumericCellValue);
                                t.Void = sheet.GetRow(row).GetCell(24) == null ? 0 : Convert.ToDecimal(sheet.GetRow(row).GetCell(24).NumericCellValue);
                                t.CashBack = sheet.GetRow(row).GetCell(25) == null ? 0 : Convert.ToDecimal(sheet.GetRow(row).GetCell(25).NumericCellValue);

                                mid.Add(new Mid()
                                {
                                    MidName = Convert.ToString(sheet.GetRow(row).GetCell(10).StringCellValue),
                                    IsActive = true,
                                    IsDeleted = false,
                                    SwitchId = 20,
                                    CardTypeId = 1,
                                    CurrencyId = 1,
                                    MidsPricingId = 0,
                                    Param_1 = Convert.ToString(sheet.GetRow(row).GetCell(11).StringCellValue),
                                    Param_2 = Convert.ToString(sheet.GetRow(row).GetCell(12).StringCellValue),
                                    Param_3 = Convert.ToString(sheet.GetRow(row).GetCell(13).StringCellValue),
                                    Param_4 = Convert.ToString(sheet.GetRow(row).GetCell(14).StringCellValue),
                                    Param_5 = Convert.ToString(sheet.GetRow(row).GetCell(15).StringCellValue),

                                    NeedAddBulk = true,
                                    NeedDeleteBulk = false,
                                    NeedUpdateBulk = false,

                                    TransactionCharges = t
                                });
                            }
                            else if (CurrentUser.ParentType == Enums.ParentType.Reseller)
                            {
                                var reseller = _resellerRepo.GetDetailsbyResellerId(CurrentUser.ParentId);

                                p.Add(new Merchant()
                                {
                                    MerchantName = Convert.ToString(sheet.GetRow(row).GetCell(0).StringCellValue),
                                    MerchantEmail = Convert.ToString(sheet.GetRow(row).GetCell(4).StringCellValue),
                                    ResellerId = CurrentUser.ParentId,
                                    PartnerId = reseller.PartnerId,
                                    IsActive = true,
                                    DateCreated = DateTime.Now,
                                    MerchantFeatures = new SDGDAL.Entities.MerchantFeatures()
                                    {
                                        BillingCycleId = 3,
                                        UseDefaultAgreements = true,
                                        CurrencyId = 1,
                                        LanguageCode = "EN_CA"
                                    },
                                    ContactInformation = new SDGDAL.Entities.ContactInformation()
                                    {
                                        Address = Convert.ToString(sheet.GetRow(row).GetCell(5).StringCellValue),
                                        City = "N/S",
                                        StateProvince = "N/S",
                                        CountryId = 1,
                                        ZipCode = "N/S",
                                        PrimaryContactNumber = Convert.ToString(sheet.GetRow(row).GetCell(6)),
                                        Fax = "N/S",
                                        MobileNumber = "N/S",
                                        NeedsUpdate = true
                                    },
                                });

                                var u = new SDGDAL.Entities.User();

                                u.FirstName = Convert.ToString(sheet.GetRow(row).GetCell(1).StringCellValue);
                                u.LastName = Convert.ToString(sheet.GetRow(row).GetCell(3).StringCellValue);
                                u.MiddleName = Convert.ToString(sheet.GetRow(row).GetCell(2).StringCellValue == "" ? null : sheet.GetRow(row).GetCell(2).StringCellValue);
                                u.EmailAddress = Convert.ToString(sheet.GetRow(row).GetCell(4).StringCellValue);
                                u.Price = 0;
                                u.ContactInformation = new SDGDAL.Entities.ContactInformation()
                                {
                                    Address = Convert.ToString(sheet.GetRow(row).GetCell(5).StringCellValue),
                                    City = "N/S",
                                    StateProvince = "N/S",
                                    CountryId = 1,
                                    ZipCode = "N/S",
                                    PrimaryContactNumber = Convert.ToString(sheet.GetRow(row).GetCell(6)),
                                    Fax = "N/S",
                                    MobileNumber = "N/S",
                                    NeedsUpdate = true
                                };

                                //var acc = new SDGDAL.Entities.Account();

                                acc.Add(new Account()
                                {
                                    Username = Convert.ToString(sheet.GetRow(row).GetCell(7).StringCellValue),
                                    Password = Convert.ToString(sheet.GetRow(row).GetCell(8).StringCellValue),
                                    User = u,
                                    RoleId = 1,
                                    PIN = Utility.Encrypt(Convert.ToString(sheet.GetRow(row).GetCell(9).NumericCellValue))
                                });

                                var t = new SDGDAL.Entities.TransactionCharges();
                                t.DiscountRate = sheet.GetRow(row).GetCell(16) == null ? 0 : Convert.ToDecimal(sheet.GetRow(row).GetCell(16).NumericCellValue);
                                t.CardNotPresent = sheet.GetRow(row).GetCell(17) == null ? 0 : Convert.ToDecimal(sheet.GetRow(row).GetCell(17).NumericCellValue);
                                t.Declined = sheet.GetRow(row).GetCell(18) == null ? 0 : Convert.ToDecimal(sheet.GetRow(row).GetCell(18).NumericCellValue);
                                t.eCommerce = sheet.GetRow(row).GetCell(19) == null ? 0 : Convert.ToDecimal(sheet.GetRow(row).GetCell(19).NumericCellValue);
                                t.PreAuth = sheet.GetRow(row).GetCell(20) == null ? 0 : Convert.ToDecimal(sheet.GetRow(row).GetCell(20).NumericCellValue);
                                t.Capture = sheet.GetRow(row).GetCell(21) == null ? 0 : Convert.ToDecimal(sheet.GetRow(row).GetCell(21).NumericCellValue);
                                t.Purchased = sheet.GetRow(row).GetCell(22) == null ? 0 : Convert.ToDecimal(sheet.GetRow(row).GetCell(22).NumericCellValue);
                                t.Refund = sheet.GetRow(row).GetCell(23) == null ? 0 : Convert.ToDecimal(sheet.GetRow(row).GetCell(23).NumericCellValue);
                                t.Void = sheet.GetRow(row).GetCell(24) == null ? 0 : Convert.ToDecimal(sheet.GetRow(row).GetCell(24).NumericCellValue);
                                t.CashBack = sheet.GetRow(row).GetCell(25) == null ? 0 : Convert.ToDecimal(sheet.GetRow(row).GetCell(25).NumericCellValue);

                                mid.Add(new Mid()
                                {
                                    MidName = Convert.ToString(sheet.GetRow(row).GetCell(10).StringCellValue),
                                    IsActive = true,
                                    IsDeleted = false,
                                    SwitchId = 20,
                                    CardTypeId = 1,
                                    CurrencyId = 1,
                                    MidsPricingId = 0,
                                    Param_1 = Convert.ToString(sheet.GetRow(row).GetCell(11).StringCellValue),
                                    Param_2 = Convert.ToString(sheet.GetRow(row).GetCell(12).StringCellValue),
                                    Param_3 = Convert.ToString(sheet.GetRow(row).GetCell(13).StringCellValue),
                                    Param_4 = Convert.ToString(sheet.GetRow(row).GetCell(14).StringCellValue),
                                    Param_5 = Convert.ToString(sheet.GetRow(row).GetCell(15).StringCellValue),

                                    NeedAddBulk = true,
                                    NeedDeleteBulk = false,
                                    NeedUpdateBulk = false,

                                    TransactionCharges = t
                                });
                            }
                            #endregion
                        }

                        if (isNull == false)
                        {
                            if (notAvailable == false)
                            {
                                var success = _merchantRepo.CreateMerchantWithUserAndMid(p, acc, mid);

                                if (success)
                                {
                                    var userActivity = "Registered Multiple Merchant";

                                    var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "MerchantBulkBoarding", "");

                                    Session["Success"] = "Merchant successfully created.";

                                    return RedirectToAction("Index");
                                }
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, "Username " + user.Remove(user.Length - 2) + " is not available.");
                                ModelState.AddModelError(string.Empty, "Email Address " + email.Remove(email.Length - 2) + " is not available.");
                                ModelState.AddModelError(string.Empty, "Please change the Username or Email Address provided.");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Please check row(s) " + rowNumber.Remove(rowNumber.Length - 2) + " if there is a blank field. Note: Row count starts after the header.");
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError(string.Empty, "Can't Register Merchant(s). Please check the data in your Excel File.");
                        string exception = ex.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Can't Register Merchant(s). Please check the data in your Excel File.");
                string exception = ex.Message;
            }
            finally
            {
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
            }

            return View();
        }
        #endregion

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

                if (!_userRepo.IsUserNameAvailable(user.Username))
                {
                    ModelState.AddModelError(string.Empty, "Username is not available.");
                    return View();
                }

                if (!_userRepo.IsPinAvailable(user.PIN, Convert.ToInt32(TempData["MerchantID"])))
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
        public ActionResult UpdateInfo(string id)
        {
            MerchantModel m = new MerchantModel();

            try
            {
                action = "fetching the data for the merchant.";

                var userActivity = "Entered Update Info Page";

                var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "UpdateInfo", "");

                var merchant =
                    !string.IsNullOrEmpty(id) ?
                    _merchantRepo.GetDetailsbyMerchantId(Convert.ToInt32(Utility.Decrypt(id.Contains(" ") == true ? id.Replace(" ", "+") : id)))
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
                TempData["CountryId"] = merchant.ContactInformation.CountryId;

                var countries = _refRepo.GetAllCountries();

                ViewBag.Countries = countries.Select(c => new SelectListItem()
                {
                    Value = c.CountryId.ToString(),
                    Text = c.CountryName
                }).ToList();

                var provinces = _refRepo.GetAllProvincesByCountry(merchant.ContactInformation.CountryId);

                ViewBag.Provinces = provinces.OrderBy(pr => pr.ProvinceName)
                    .Select(c => new SelectListItem()
                    {
                        Value = c.ProvinceId.ToString(),
                        Text = c.ProvinceName
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

                #region Currency Dropdown
                var currencies = _refRepo.GetAllCurrencies();
                var ddlcurrency = new List<SelectListItem>();

                ddlcurrency.AddRange(currencies.Select(c => new SelectListItem()
                {
                    Value = c.CurrencyId.ToString(),
                    Text = c.CurrencyCode
                }).ToList());

                ViewBag.Currencies = ddlcurrency;
                #endregion

                if (merchant.ContactInformation.ProvIsoCode != null)
                {
                    var pName = provinces.SingleOrDefault(prv => (prv.ProvinceName == merchant.ContactInformation.StateProvince)
                                                         && (prv.IsoCode == merchant.ContactInformation.ProvIsoCode));
                    if (pName != null)
                    {
                        m.ProvinceId = pName.ProvinceId;
                        ViewBag.WithIsoCode = 1;
                        TempData["WithISO"] = true;
                    }
                }
                else
                {
                    m.StateProvince = merchant.ContactInformation.StateProvince;
                }

                TempData["MerchantName"] = merchant.MerchantName;
                merchant.Currency = new Currency();
                m.MerchantName = merchant.MerchantName;
                m.MerchantEmail = merchant.MerchantEmail;
                m.Address = merchant.ContactInformation.Address;
                m.City = merchant.ContactInformation.City;
                m.ZipCode = merchant.ContactInformation.ZipCode;
                m.CountryId = merchant.ContactInformation.CountryId;
                m.PrimaryContactNumber = merchant.ContactInformation.PrimaryContactNumber;
                m.Fax = merchant.ContactInformation.Fax;
                m.MobileNumber = merchant.ContactInformation.MobileNumber;
                m.BillingCycleId = merchant.MerchantFeatures.BillingCycleId;
                m.CurrencyId = merchant.Currency.CurrencyId;
                m.IsActive = merchant.IsActive;
                m.MerchantFeaturesId = Convert.ToInt32(merchant.MerchantFeaturesId);
                m.NeedAddToCT = merchant.NeedAddToCT;
                m.NeedUpdateToCT = merchant.NeedUpdateToCT;

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

                #region Dropdown
                var countries = _refRepo.GetAllCountries();

                ViewBag.Countries = countries.Select(c => new SelectListItem()
                {
                    Value = c.CountryId.ToString(),
                    Text = c.CountryName
                }).ToList();

                var provinces = _refRepo.GetAllProvincesByCountry(Convert.ToInt32(TempData["CountryId"]));

                ViewBag.Provinces = provinces.OrderBy(pr => pr.ProvinceName)
                    .Select(c => new SelectListItem()
                    {
                        Value = c.ProvinceId.ToString(),
                        Text = c.ProvinceName
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
                #endregion

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

                bool isIso = Convert.ToBoolean(TempData["WithISO"]);
                var provinces = _refRepo.GetAllProvinces();

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

                if (merchant.CountryId == 185) //Philippines
                {
                    var isoCode = provinces.SingleOrDefault(pr => pr.ProvinceId == merchant.ProvinceId);
                    m.ContactInformation.StateProvince = isoCode.ProvinceName;
                    m.ContactInformation.ProvIsoCode = isoCode.IsoCode;
                }
                else if(merchant.CountryId == 40) //Canada
                {
                    var isoCode = provinces.SingleOrDefault(pr => pr.ProvinceId == merchant.ProvinceId);
                    m.ContactInformation.StateProvince = isoCode.ProvinceName;
                    m.ContactInformation.ProvIsoCode = isoCode.IsoCode;
                }
                else
                {
                    m.ContactInformation.StateProvince = merchant.StateProvince;
                    m.ContactInformation.ProvIsoCode = null;
                }

                var mf = _merchantFeatureRepo.GetDetailsByMerchantFeaturesId(merch.MerchantFeaturesId.Value);

                m.MerchantId = Convert.ToInt32(TempData["MerchantId"]);
                m.MerchantName = Convert.ToString(TempData["MerchantName"]);//merch.MerchantName;
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
                m.NeedAddToCT = merchant.NeedAddToCT;
                m.NeedUpdateToCT = merchant.NeedAddToCT ? false : true;
                m.CurrencyId = merchant.CurrencyId;

                m.ContactInformation.ContactInformationId = Convert.ToInt32(TempData["ContactInformationId"]);
                m.ContactInformation.Address = merchant.Address;
                m.ContactInformation.City = merchant.City;
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
                TempData.Keep("MerchantID");
                TempData.Keep("ContactInformationId");
                TempData.Keep("MerchantFeaturesId");
                TempData.Keep("BillingCycleId");
                TempData.Keep("MerchantEmail");
                TempData.Keep("MerchantName");
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

                var currencies = _refRepo.GetAllCurrencies();
                var ddlcurrency = new List<SelectListItem>();

                ddlcurrency.AddRange(currencies.Select(c => new SelectListItem()
                {
                    Value = c.CurrencyId.ToString(),
                    Text = c.CurrencyCode
                }).ToList());

                ViewBag.Currencies = ddlcurrency;

                if (Convert.ToBoolean(TempData["WithISO"]) == true)
                {
                    ViewBag.WithIsoCode = 1;
                }

                TempData.Keep();
            }

            return View(merchant);
        }

        [HttpGet]
        public ActionResult UpdateMyInfo(int? id)
        {
            MerchantModel m = new MerchantModel();

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

                    var countries = _refRepo.GetAllCountries();

                    ViewBag.Countries = countries.Select(c => new SelectListItem()
                    {
                        Value = c.CountryId.ToString(),
                        Text = c.CountryName
                    }).ToList();

                    var provinces = _refRepo.GetAllProvincesByCountry(merchant.ContactInformation.CountryId);

                    ViewBag.Provinces = provinces.OrderBy(pr => pr.ProvinceName)
                        .Select(c => new SelectListItem()
                        {
                            Value = c.ProvinceId.ToString(),
                            Text = c.ProvinceName
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

                    if (merchant.ContactInformation.ProvIsoCode != null)
                    {
                        var pName = provinces.SingleOrDefault(prv => (prv.ProvinceName == merchant.ContactInformation.StateProvince)
                                                             && (prv.IsoCode == merchant.ContactInformation.ProvIsoCode));
                        m.ProvinceId = pName.ProvinceId;
                        ViewBag.WithIsoCode = 1;
                        TempData["WithISO"] = true;
                    }
                    else
                    {
                        m.StateProvince = merchant.ContactInformation.StateProvince;
                    }

                    TempData["MerchantName"] = merchant.MerchantName;
                    m.MerchantName = merchant.MerchantName;
                    m.MerchantEmail = merchant.MerchantEmail;
                    m.Address = merchant.ContactInformation.Address;
                    m.City = merchant.ContactInformation.City;
                    m.ZipCode = merchant.ContactInformation.ZipCode;
                    m.CountryId = merchant.ContactInformation.CountryId;
                    m.PrimaryContactNumber = merchant.ContactInformation.PrimaryContactNumber;
                    m.Fax = merchant.ContactInformation.Fax;
                    m.MobileNumber = merchant.ContactInformation.MobileNumber;
                    m.BillingCycleId = merchant.MerchantFeatures.BillingCycleId;
                    m.IsActive = merchant.IsActive;
                    m.MerchantFeaturesId = Convert.ToInt32(merchant.MerchantFeaturesId);


                    TempData["MerchantId"] = merchant.MerchantId;
                    TempData["ContactInformationId"] = merchant.ContactInformationId;
                    TempData["MerchantFeaturesId"] = merchant.MerchantFeaturesId;
                    TempData["BillingCycleId"] = merchant.MerchantFeatures.BillingCycleId;
                    TempData["MerchantEmail"] = merchant.MerchantEmail;

                    return View(m);
                }
                catch (Exception ex)
                {
                    var errorOnAction = "Error while " + action;

                    var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "UpdateMyInfo", ex.StackTrace);

                    Session["MerchantOutdated"] = "Your Merchant is not updated. Please contact support for assistance.";
                }
            }
            else
            {
                return HttpNotFound();
            }

            return RedirectToAction("Dashboard", "Home");
        }

        [HttpPost]
        public ActionResult UpdateMyInfo(MerchantModel merchant)
        {
            Merchant m = new Merchant();

            try
            {
                action = "updating the merchant info.";

                bool isIso = Convert.ToBoolean(TempData["WithISO"]);
                var provinces = _refRepo.GetAllProvinces();

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

                if (isIso == true)
                {
                    var isoCode = provinces.SingleOrDefault(pr => pr.ProvinceId == merchant.ProvinceId);
                    m.ContactInformation.StateProvince = isoCode.ProvinceName;
                    m.ContactInformation.ProvIsoCode = isoCode.IsoCode;
                }
                else
                {
                    m.ContactInformation.StateProvince = merchant.StateProvince;
                }

                var mf = _merchantFeatureRepo.GetDetailsByMerchantFeaturesId(merch.MerchantFeaturesId.Value);

                m.MerchantId = Convert.ToInt32(TempData["MerchantId"]);
                m.MerchantName = Convert.ToString(TempData["MerchantName"]);//merch.MerchantName;
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

                    var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "UpdateMyInfo", "");

                    Session["Success"] = "Successfully Updated.";

                    return RedirectToAction("Dashboard", "Home", new { area = "" });
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

                if (Convert.ToBoolean(TempData["WithISO"]) == true)
                {
                    ViewBag.WithIsoCode = 1;
                }
            }

            return View(merchant);
        }
        #endregion

        public ActionResult ViewAssignedDevices(string id)
        {
            var userActivity = "Entered View Assigned Devices Page";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "ViewAssignedDevices", "");

            MerchantDeviceModel merchModel = new MerchantDeviceModel();
            merchModel.MerchantId = Convert.ToInt32(Utility.Decrypt(id.Contains(" ") == true ? id.Replace(" ", "+") : id));

            return View(merchModel);
        }

        public ActionResult ViewInfo(string id)
        {
            if (CurrentUser.ParentType == Enums.ParentType.Partner || CurrentUser.ParentType == Enums.ParentType.Reseller)
            {
                var userActivity = "Entered View Info Page.";

                var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "ViewInfo", "");

                try
                {
                    action = "fetching the merchant data.";
                    var merchant =
                        id != string.Empty ?
                        _merchantRepo.GetDetailsbyMerchantId(Convert.ToInt32(Utility.Decrypt(id.Contains(" ") == true ? id.Replace(" ", "+") : id)))
                        :
                        _merchantRepo.GetDetailsbyMerchantId(CurrentUser.ParentId);

                    TempData["MerchantId"] = merchant.MerchantId;

                    var countries = _refRepo.GetAllCountries();

                    ViewBag.Countries = countries.Select(c => new SelectListItem()
                    {
                        Value = c.CountryId.ToString(),
                        Text = c.CountryName
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
                        Text = c.CountryName
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

        public ActionResult AssignDevice(string id)
        {
            try
            {
                action = "fetching master device info.";

                var userActivity = "Entered Assign Device Page";

                var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "AssignDevice", "");

                TempData["MerchantId"] = Convert.ToInt32(Utility.Decrypt(id.Contains(" ") == true ? id.Replace(" ", "+") : id));
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

        [HttpPost]
        public JsonResult UpdateMerchantStatus(string mId, bool status)
        {
            Merchant m = new Merchant();

            var merchant = _merchantRepo.GetDetailsbyMerchantId(Convert.ToInt32(Utility.Decrypt(mId.Contains(" ") == true ? mId.Replace(" ", "+") : mId)));

            m.MerchantId = merchant.MerchantId;
            m.MerchantName = merchant.MerchantName;
            m.MerchantEmail = merchant.MerchantEmail;
            m.ContactInformationId = merchant.ContactInformationId;
            m.MerchantFeaturesId = merchant.MerchantFeaturesId;
            m.IsActive = status;
            m.IsDeleted = merchant.IsDeleted;
            m.DateCreated = merchant.DateCreated;
            m.CanCreateSubMerchants = merchant.CanCreateSubMerchants;
            m.ParentMerchantId = merchant.ParentMerchantId;
            m.ResellerId = merchant.ResellerId;
            m.PartnerId = merchant.PartnerId;
            m.EmailServerId = merchant.EmailServerId;

            var update = _merchantRepo.UpdateMerchantStatus(m);

            return Json(update);
        }

        public ActionResult RequestedMerchants()
        {
            ViewBag.ParentId = CurrentUser.ParentId;

            #region Reseller Dropdown
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
                        Text = "Select Reseller",
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
            #endregion

            #region Switch Dropdown
            var switches = _refRepo.GetAllSwitches();
            var ddlswitch = new List<SelectListItem>();

            ddlswitch.Add(new SelectListItem()
            {
                Value = "0",
                Text = "Select Switch",
                Selected = val == 0
            });

            ddlswitch.AddRange(switches.Where(a => a.IsActive == true).Select(c => new SelectListItem()
            {
                Value = c.SwitchId.ToString(),
                Text = c.SwitchName.ToString()
            }).ToList());

            ViewBag.Switches = ddlswitch;
            #endregion

            #region Card Type Dropdown
            var cardTypes = _refRepo.GetAllCardTypes();
            var ddlcardtype = new List<SelectListItem>();

            ddlcardtype.Add(new SelectListItem()
            {
                Value = "0",
                Text = "Select Card Type",
                Selected = val == 0
            });

            ddlcardtype.AddRange(cardTypes.Select(c => new SelectListItem()
            {
                Value = c.CardTypeId.ToString(),
                Text = c.TypeName
            }).ToList());

            ViewBag.CardTypes = ddlcardtype;
            #endregion

            #region Currency Dropdown
            var currencies = _refRepo.GetAllCurrencies();
            var ddlcurrency = new List<SelectListItem>();

            ddlcurrency.Add(new SelectListItem()
            {
                Value = "0",
                Text = "Select Currency",
                Selected = val == 0
            });

            ddlcurrency.AddRange(currencies.Select(c => new SelectListItem()
            {
                Value = c.CurrencyId.ToString(),
                Text = c.CurrencyCode
            }).ToList());

            ViewBag.Currencies = ddlcurrency;
            #endregion

            var countries = _refRepo.GetAllCountries();

            ViewBag.Countries = countries.Select(c => new SelectListItem()
            {
                Value = c.CountryId.ToString(),
                Text = c.CountryName,
                Selected = c.CountryId == 1
            }).ToList();

            return View();
        }

        [HttpPost]
        public JsonResult RegisterRequestedMerchants(int rId, int rmId)
        {
            bool usernameError = false;
            bool emailError = false;
            string msg = "";
            var chars = "1234567890";
            var stringChars = new char[4];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var randomPin = new String(stringChars);

            #region PasswordGenerator
            var chars1 = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var stringChars1 = new char[10];

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars1[i] = chars1[random.Next(chars1.Length)];
            }

            var randomPassword = new String(stringChars1);
            #endregion

            var r = _requestedMerchantRepo.GetMerchant(rmId);

            var provinces = _refRepo.GetAllProvinces();

            var prov = provinces.SingleOrDefault(pr => pr.ProvinceId == r.ProvinceId);

            try
            {
                if (!_userRepo.IsUserNameAvailable(r.Username))
                {
                    usernameError = true;
                }

                if (!_userRepo.IsEmailAvailable(r.MerchantEmail))
                {
                    emailError = true;
                }

                if (usernameError == true)
                {
                    msg = "Username is not available";
                }
                else if (emailError == true)
                {
                    msg = "Email Address is not available";
                }
                else if ((emailError == true) && (usernameError == true))
                {
                    msg = "Username and Email Address is not available";
                }
                else
                {
                    if (r != null)
                    {
                        var p = new SDGDAL.Entities.Merchant();
                        p.MerchantName = r.MerchantName;
                        p.MerchantEmail = r.MerchantEmail;

                        p.ResellerId = rId;

                        var reseller = _resellerRepo.GetDetailsbyResellerId(rId);
                        p.PartnerId = reseller.PartnerId;

                        p.IsActive = true;
                        p.NeedAddToCT = true;
                        p.NeedUpdateToCT = false;

                        p.MerchantFeatures = new SDGDAL.Entities.MerchantFeatures();
                        p.MerchantFeatures.BillingCycleId = 1;
                        p.MerchantFeatures.UseDefaultAgreements = true;

                        p.ContactInformation = new SDGDAL.Entities.ContactInformation()
                        {
                            Address = r.Address,
                            City = r.City,
                            StateProvince = prov.ProvinceName,
                            CountryId = 185,
                            ZipCode = r.ZipCode,
                            PrimaryContactNumber = r.PrimaryContactNumber,
                            Fax = r.Fax,
                            MobileNumber = r.MobileNumber,
                            NeedsUpdate = true,
                            ProvIsoCode = prov.IsoCode
                        };

                        var u = new SDGDAL.Entities.User();

                        u.FirstName = r.FirstName;
                        u.LastName = r.LastName;
                        u.MiddleName = r.MiddleName;
                        u.EmailAddress = r.MerchantEmail;
                        u.Price = 0;
                        u.ContactInformation = new SDGDAL.Entities.ContactInformation()
                        {
                            Address = r.Address,
                            City = r.City,
                            StateProvince = prov.ProvinceName,
                            CountryId = 185,
                            ZipCode = r.ZipCode,
                            PrimaryContactNumber = r.PrimaryContactNumber,
                            Fax = r.Fax,
                            MobileNumber = r.MobileNumber,
                            NeedsUpdate = true,
                            ProvIsoCode = prov.IsoCode
                        };

                        var acc = new SDGDAL.Entities.Account();
                        acc.Username = r.MerchantEmail;
                        acc.Password = randomPassword;
                        acc.User = u;
                        acc.RoleId = 1;
                        acc.PIN = Utility.Encrypt(randomPin);

                        var newMerchant = _merchantRepo.CreateMerchantWithUser(p, acc);

                        if (newMerchant.MerchantId > 0)
                        {
                            #region Update Requested Merchant Record if Registration is successful
                            r.IsActive = true;
                            var updateMerchant = _requestedMerchantRepo.UpdateRequestedMerchantStatus(r);
                            #endregion

                            var userActivity = "Registered a Merchant";

                            var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "Registration", "");

                            return Json(new { data = newMerchant, data2 = r });
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return Json(new { error = msg });
        }

        public ActionResult RegisterMerchantBranchAndMID()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RegisterMerchantBranchAndMID(BranchMIDModel bm)
        {
            try
            {

                int tId = bm.Param_6 == null ? 0 : bm.Param_6.Length;
                int mbTransNumber = bm.Param_7 == null ? 0 : bm.Param_7.Length;
                int mNumber = bm.Param_8 == null ? 0 : bm.Param_8.Length;
                int mAccNumber = bm.Param_9 == null ? 0 : bm.Param_9.Length;
                int oId = bm.Param_10 == null ? 0 : bm.Param_10.Length;
                int mCCode = bm.Param_11 == null ? 0 : bm.Param_11.Length;
                int dmId = bm.Param_12 == null ? 0 : bm.Param_12.Length;

                if (!_userRepo.IsUserNameAvailable(bm.User.Username))
                {
                    ModelState.AddModelError(string.Empty, "Username is not available.");
                }

                if (!_midsRepo.CheckTerminalIdIsExist(tId))
                {
                    ModelState.AddModelError(string.Empty, "Terminal ID is already used.");
                }

                var midsInfo = _midsRepo.GetDetailsbyParam2AndSwitchCode(bm.Param_2);

                if (midsInfo != null)
                {
                    if (midsInfo.MerchantId == bm.MerchantId)
                    {
                        if (!_midsRepo.IsSameCardType(bm.Param_2, bm.CardTypeId))
                        {
                            ModelState.AddModelError(string.Empty, "Cannot save with same Card type to CT Payment Switch.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Merchant ID is already used.");
                    }
                }

                if (ModelState.IsValid && bm != null)
                {
                    #region Creating Merchant Branch/Location
                    var p = new SDGDAL.Entities.MerchantBranch();
                    p.MerchantBranchName = bm.BranchName;
                    p.MerchantId = bm.MerchantId;
                    p.IsActive = true;

                    p.ContactInformation = new SDGDAL.Entities.ContactInformation()
                    {
                        Address = bm.Address,
                        City = bm.City,
                        StateProvince = bm.StateProvince,
                        CountryId = bm.CountryId,
                        ZipCode = bm.ZipCode,
                        PrimaryContactNumber = bm.PrimaryContactNumber,
                        Fax = bm.Fax,
                        MobileNumber = bm.MobileNumber
                    };

                    var u = new SDGDAL.Entities.User();

                    u.FirstName = bm.User.FirstName;
                    u.LastName = bm.User.LastName;
                    u.MiddleName = bm.User.MiddleName;
                    u.EmailAddress = bm.User.EmailAddress;

                    if (bm.User.AddressSameAsParent)
                    {
                        u.ContactInformation = new SDGDAL.Entities.ContactInformation()
                        {
                            Address = bm.Address,
                            City = bm.City,
                            StateProvince = bm.StateProvince,
                            CountryId = bm.CountryId,
                            ZipCode = bm.ZipCode,
                            PrimaryContactNumber = bm.PrimaryContactNumber,
                            Fax = bm.Fax,
                            MobileNumber = bm.MobileNumber
                        };
                    }
                    else
                    {
                        u.ContactInformation = new SDGDAL.Entities.ContactInformation()
                        {
                            Address = bm.User.Address,
                            City = bm.User.City,
                            StateProvince = bm.User.StateProvince,
                            CountryId = bm.User.CountryId,
                            ZipCode = bm.User.ZipCode,
                            PrimaryContactNumber = bm.User.PrimaryContactNumber,
                            Fax = bm.User.Fax,
                            MobileNumber = bm.User.MobileNumber
                        };
                    }

                    var acc = new SDGDAL.Entities.Account();
                    acc.Username = bm.User.Username;
                    acc.Password = bm.User.Password;
                    acc.User = u;
                    acc.RoleId = 1;
                    acc.PIN = Utility.Encrypt(bm.User.PIN);

                    var newBranch = _branchRepo.CreateBranchtWithUser(p, acc);
                    #endregion

                    #region Creating MID
                    var m = new SDGDAL.Entities.Mid();
                    m.MerchantId = bm.MerchantId;
                    m.MidName = bm.MIDName;
                    m.IsActive = bm.IsActive;
                    m.IsDeleted = false;
                    m.SwitchId = bm.SwitchId;
                    m.CardTypeId = bm.CardTypeId;
                    m.CurrencyId = bm.CurrencyId;
                    m.MidsPricingId = 0;
                    m.Param_1 = bm.Param_1;
                    m.Param_2 = bm.Param_2;
                    m.Param_3 = bm.Param_3;
                    m.Param_4 = bm.Param_4;
                    m.Param_5 = bm.Param_5;

                    m.NeedAddBulk = true;
                    m.NeedDeleteBulk = false;
                    m.NeedUpdateBulk = false;

                    if (bm.SwitchId == 27)
                    {
                        m.Param_6 = bm.Param_6;
                    }

                    if (bm.SwitchId == 22)
                    {
                        if ((tId < 1) || (tId > 8))
                        {
                            ModelState.AddModelError(string.Empty, "Terminal Id should be consist of 1-8 characters.");
                        }
                        else if ((mbTransNumber < 5) || (mbTransNumber > 5))
                        {
                            ModelState.AddModelError(string.Empty, "Invalid Merchant Branch Transit Number. Enter a 5-digit number.");
                        }
                        else if ((mNumber < 3) || mNumber > 3)
                        {
                            ModelState.AddModelError(string.Empty, "Invalid Merchant Bank Number. Enter a 3-digit number.");
                        }
                        else if ((mAccNumber < 1) || (mAccNumber > 12))
                        {
                            ModelState.AddModelError(string.Empty, "Merchant Account Number should be consist of 1-12 characters.");
                        }
                        else if ((oId < 1) || (oId > 10))
                        {
                            ModelState.AddModelError(string.Empty, "Originator Id should be consist of 1-10 characters.");
                        }
                        else if ((mCCode < 4) || (mCCode > 4))
                        {
                            ModelState.AddModelError(string.Empty, "Invalid Category Code. Enter a 4-digit number.");
                        }
                        else if ((dmId < 8) || (dmId > 20))
                        {
                            ModelState.AddModelError(string.Empty, "Deposit Merchant Id should be consist of 8-20 characters.");
                        }
                        else
                        {
                            m.Param_6 = bm.Param_6;
                            m.Param_7 = bm.Param_7;
                            m.Param_8 = bm.Param_8;
                            m.Param_9 = bm.Param_9;
                            m.Param_10 = bm.Param_10;
                            m.Param_11 = bm.Param_11;
                            m.Param_12 = bm.Param_12;
                        }
                    }

                    var t = new SDGDAL.Entities.TransactionCharges();
                    t.DiscountRate = bm.TC_Discount;
                    t.CardNotPresent = bm.TC_CardNotPresent;
                    t.Declined = bm.TC_Decline;
                    t.eCommerce = bm.TC_eCommerce;
                    t.PreAuth = bm.TC_PreAuth;
                    t.Capture = bm.TC_Capture;
                    t.Purchased = bm.TC_Purchased;
                    t.Refund = bm.TC_Refund;
                    t.Void = bm.TC_Void;
                    t.CashBack = bm.TC_CashBack;


                    m.TransactionCharges = t;

                    var newMid = _midsRepo.CreateMid(m);
                    #endregion

                    if ((newBranch.MerchantBranchId > 0) && (newMid.MidId > 0))
                    {
                        var userActivity = "Registered a Merchant Branch and MID";

                        var errRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "RegisterMerchantBranchAndMID", "");

                        Session["Success"] = "Merchant Branch and MID successfully created.";

                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return RedirectToAction("RequestedMerchants");
        }
    }
}
