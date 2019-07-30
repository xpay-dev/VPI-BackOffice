using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CTPaymentBackOffice.Models;
using SDGDAL.Repositories;
using SDGDAL.Entities;
using SDGDAL;
using SDGUtil;

namespace CTPaymentBackOffice.Controllers
{
    [Authorize]
    [CustomAttributes.SessionExpireFilter]
    public class UsersController : Controller
    {
        ReferenceRepository _refRepo;
        PartnerRepository _partnerRepo;
        ResellerRepository _resellerRepo;
        MerchantRepository _merchantRepo;
        MerchantBranchRepository _branchRepo;
        UserRepository _userRepo;

        string action = string.Empty;

        public UsersController()
        {
            _userRepo = new UserRepository();
            _partnerRepo = new PartnerRepository();
            _resellerRepo = new ResellerRepository();
            _merchantRepo = new MerchantRepository();
            _branchRepo = new MerchantBranchRepository();
            _refRepo = new ReferenceRepository();
        }

        public void GetPartner()
        {
            try
            {
                action = "fetching the partner list";

                var partners = _partnerRepo.GetAllPartnersByParent(CurrentUser.ParentId, "");

                var ddlPartners = new List<SelectListItem>();
                if (partners.Count > 0)
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

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "GetPartner", ex.StackTrace);
            }
        }

        public void GetMerchantByReseller()
        {
            try
            {
                action = "getting merchants by reseller id.";

                var merchants = _merchantRepo.GetAllMerchantsByReseller(CurrentUser.ParentId, "");

                var ddlMerchants = new List<SelectListItem>();

                ddlMerchants.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = "Select a Merchant",
                    Selected = 0 == 0
                });

                ddlMerchants.AddRange(merchants.Select(p => new SelectListItem()
                {
                    Value = p.MerchantId.ToString(),
                    Text = p.MerchantName
                }).ToList());

                ViewBag.Merchants = ddlMerchants;
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "GetMerchantByReseller", ex.StackTrace);
            }
        }

        public void GetBrachesByMerchant()
        {
            try
            {
                action = "getting branches by merchant id.";

                var branches = _branchRepo.GetAllBranchesByMerchant(CurrentUser.ParentId, "");

                var ddlBranches = new List<SelectListItem>();

                ddlBranches.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = "Select a Merchant Branch",
                    Selected = 0 == 0
                });

                ddlBranches.AddRange(branches.Select(p => new SelectListItem()
                {
                    Value = p.MerchantBranchId.ToString(),
                    Text = p.MerchantBranchName
                }).ToList());

                ViewBag.Branches = ddlBranches;
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "GetMerchantByReseller", ex.StackTrace);
            }
        }

        public ActionResult Index(int? id)
        {
            var userActivity = "Entered User Management Info Page";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "Index", "");

            if (Session["Success"] != null)
            {
                ViewBag.Success = Session["Success"];
            }

            Session["Success"] = null;

            GetPartner();

            var ddlusertype = new List<SelectListItem>();

            ddlusertype.Add(new SelectListItem()
            {
                Value = "0",
                Text = "Select a User Type"
            });

            if (CTPaymentBackOffice.CurrentUser.ParentType == CTPaymentBackOffice.Enums.ParentType.Partner)
            {
                ddlusertype.Add(new SelectListItem()
                {
                    Value = "1",
                    Text = "Partner"
                });

                ddlusertype.Add(new SelectListItem()
                {
                    Value = "2",
                    Text = "Reseller"
                });

                ddlusertype.Add(new SelectListItem()
                {
                    Value = "3",
                    Text = "Merchant"
                });

                ddlusertype.Add(new SelectListItem()
                {
                    Value = "4",
                    Text = "Merchant Branch"
                });
            }
            else if (CTPaymentBackOffice.CurrentUser.ParentType == CTPaymentBackOffice.Enums.ParentType.Reseller)
            {
                GetMerchantByReseller();

                ddlusertype.Add(new SelectListItem()
                {
                    Value = "3",
                    Text = "Merchant"
                });

                ddlusertype.Add(new SelectListItem()
                {
                    Value = "4",
                    Text = "Merchant Branch"
                });
            }
            else if (CTPaymentBackOffice.CurrentUser.ParentType == CTPaymentBackOffice.Enums.ParentType.Merchant)
            {
                GetBrachesByMerchant();

                ddlusertype.Add(new SelectListItem()
                {
                    Value = "4",
                    Text = "Merchant Branch"
                });
            }
            else
            {
                ddlusertype.Add(new SelectListItem()
                {
                    Value = "1",
                    Text = "Partner"
                });

                ddlusertype.Add(new SelectListItem()
                {
                    Value = "2",
                    Text = "Reseller"
                });

                ddlusertype.Add(new SelectListItem()
                {
                    Value = "3",
                    Text = "Merchant"
                });

                ddlusertype.Add(new SelectListItem()
                {
                    Value = "4",
                    Text = "Merchant Branch"
                });
            }



            ViewBag.UserType = ddlusertype;

            return View();
        }

        #region User Registration
        public ActionResult Registration(int? id)
        {
            var userActivity = "Entered User Management Registration Page";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "Registration", "");

            if (CTPaymentBackOffice.CurrentUser.ParentType == CTPaymentBackOffice.Enums.ParentType.Partner)
            {
                GetPartner();
            }
            else if (CTPaymentBackOffice.CurrentUser.ParentType == CTPaymentBackOffice.Enums.ParentType.Reseller)
            {
                GetMerchantByReseller();
            }
            else if (CTPaymentBackOffice.CurrentUser.ParentType == CTPaymentBackOffice.Enums.ParentType.Merchant)
            {
                GetBrachesByMerchant();
                ViewBag.MerchantId = CurrentUser.ParentId;
                TempData["mId"] = id;
            }
            try
            {
                action = "fetching the country list.";

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

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "Registration", ex.StackTrace);
            }

            return View();
        }

        [HttpPost]
        public ActionResult Registration(UserModel user)
        {
            try
            {
                int uId = Convert.ToInt32(Request["parentId"]);

                int ptId = Convert.ToInt32(Request["parentTypeId"]);

                action = "creating new user";

                if (!_userRepo.IsUserNameAvailable(user.Username))
                {
                    ModelState.AddModelError(string.Empty, "Username is not available.");
                    return View();
                }

                if (ModelState.IsValid && user != null)
                {
                    var p = new SDGDAL.Entities.Partner();
                    var r = new SDGDAL.Entities.Reseller();
                    var m = new SDGDAL.Entities.Merchant();
                    var b = new SDGDAL.Entities.MerchantBranch();

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
                    acc.ParentTypeId = ptId;
                    acc.RoleId = 1;

                    if (ptId == 1)
                    {
                        p.ParentPartnerId = uId;

                        var newUser = _partnerRepo.CreatePartnerUser(p, acc);

                        if ((newUser.AccountId > 0) && (newUser.UserId > 0))
                        {
                            var userActivity = "Created a User Account";

                            var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "Registration", "");

                            Session["Success"] = "User account successfully created.";

                            return RedirectToAction("Index");
                        }
                    }
                    else if (ptId == 2)
                    {
                        r.ResellerId = uId;

                        var newUser = _resellerRepo.CreateResellerUser(r, acc);

                        if ((newUser.AccountId > 0) && (newUser.UserId > 0))
                        {
                            var userActivity = "Created a User Account";

                            var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "Registration", "");

                            Session["Success"] = "User account successfully created.";

                            return RedirectToAction("Index");
                        }
                    }
                    else if (ptId == 3)
                    {
                        m.MerchantId = uId;
                        acc.PIN = Utility.Encrypt(user.PIN);

                        if (!_userRepo.IsPinAvailable(user.PIN))
                        {
                            ModelState.AddModelError(string.Empty, "PIN is not available.");
                        }

                        var newUser = _merchantRepo.CreateMerchantUser(m, acc);

                        if ((newUser.AccountId > 0) && (newUser.UserId > 0))
                        {
                            var userActivity = "Created a User Account";

                            var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "Registration", "");

                            Session["Success"] = "User account successfully created.";

                            return RedirectToAction("Index");
                        }
                    }
                    else if (ptId == 4)
                    {
                        b.MerchantBranchId = uId;
                        acc.PIN = Utility.Encrypt(user.PIN);

                        if (!_userRepo.IsPinAvailable(user.PIN))
                        {
                            ModelState.AddModelError(string.Empty, "PIN is not available.");
                        }

                        var newUser = _branchRepo.CreateBranchUser(b, acc);

                        if ((newUser.AccountId > 0) && (newUser.UserId > 0))
                        {
                            var userActivity = "Created a User Account";

                            var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "Registration", "");

                            Session["Success"] = "User account successfully created.";

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
                err.Action = "Create New User for an Entity";
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
                GetPartner();

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
        #endregion



        #region Update User Info and Status

        public ActionResult UpdateUserInfo(int? id)
        {
            var userActivity = "Entered Update User Info Page";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "UpdateUserInfo", "");

            try
            {
                action = "fetching data for the user info.";

                var a = _userRepo.GetDetailsbyAccountId(id.Value);

                var countries = _refRepo.GetAllCountries();

                ViewBag.Countries = countries.Select(c => new SelectListItem()
                {
                    Value = c.CountryId.ToString(),
                    Text = c.CountryName,
                    Selected = c.CountryId == a.User.ContactInformation.CountryId
                }).ToList();

                UserModel u = new UserModel();
                TempData["ParentId"] = a.ParentId;
                TempData["ParentTypeId"] = a.ParentTypeId;
                u.UserId = a.UserId;
                u.FirstName = a.User.FirstName;
                u.MiddleName = a.User.MiddleName;
                u.LastName = a.User.LastName;
                u.Address = a.User.ContactInformation.Address;
                u.City = a.User.ContactInformation.City;
                u.StateProvince = a.User.ContactInformation.StateProvince;
                u.ZipCode = a.User.ContactInformation.ZipCode;
                u.CountryId = a.User.ContactInformation.CountryId;
                u.PrimaryContactNumber = a.User.ContactInformation.PrimaryContactNumber;
                u.Fax = a.User.ContactInformation.Fax;
                u.MobileNumber = a.User.ContactInformation.MobileNumber;
                u.EmailAddress = a.User.EmailAddress;
                u.Username = a.Username;
                u.Password = Utility.Decrypt(a.Password);
                u.ConfirmPassword = Utility.Decrypt(a.Password);

                if (a.ParentTypeId == 3)
                {
                    u.PIN = Utility.Decrypt(a.PIN);
                }

                TempData["Username"] = a.Username;

                ViewBag.ParentTypeId = a.ParentTypeId;

                return View(u);
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "UpdateUserInfo", ex.StackTrace);
            }

            return View(new UserModel());
        }

        [HttpPost]
        public ActionResult UpdateUserInfo(UserModel info)
        {
            User u = new User();
            try
            {
                action = "updating user info.";

                if (Convert.ToString(TempData["Username"]) != info.Username)
                {
                    if (!_userRepo.IsUserNameAvailable(info.Username))
                    {
                        ModelState.AddModelError(string.Empty, "Username is not available.");
                        return View();
                    }
                }
                var au = _userRepo.GetDetailsbyUserId(info.UserId);

                u.UserId = au.UserId;
                u.PhotoUrl = au.User.PhotoUrl;
                u.FirstName = info.FirstName;
                u.MiddleName = info.MiddleName;
                u.LastName = info.LastName;
                u.EmailAddress = info.EmailAddress;
                u.ContactInformationId = au.User.ContactInformationId;

                u.ContactInformation = new ContactInformation();
                u.ContactInformation.ContactInformationId = au.User.ContactInformationId;
                u.ContactInformation.Address = info.Address;
                u.ContactInformation.City = info.City;
                u.ContactInformation.StateProvince = info.StateProvince;
                u.ContactInformation.ZipCode = info.ZipCode;
                u.ContactInformation.CountryId = info.CountryId;
                u.ContactInformation.PrimaryContactNumber = info.PrimaryContactNumber;
                u.ContactInformation.Fax = info.Fax;
                u.ContactInformation.MobileNumber = info.MobileNumber;


                var a = new SDGDAL.Entities.Account();
                a.UserId = au.UserId;
                a.AccountId = au.AccountId;
                a.Username = info.Username;
                a.Password = Utility.Encrypt(info.Password);
                a.PIN = au.PIN;
                a.ParentId = au.ParentId;
                a.ParentTypeId = au.ParentTypeId;
                a.RoleId = au.RoleId;
                a.IsActive = au.IsActive;
                a.IsDeleted = au.IsDeleted;
                a.LogTries = au.LogTries;
                a.IPAddress = au.IPAddress;
                a.AccountAvailableDate = au.AccountAvailableDate;
                a.LastLoggedIn = au.LastLoggedIn;
                a.PasswordExpirationDate = au.PasswordExpirationDate;
                a.DateCreated = au.DateCreated;
                a.NeedsUpdate = au.NeedsUpdate;

                var us = _userRepo.UpdateUser(u);

                var user = _userRepo.UpdateUserInfo(u);

                var acc = _userRepo.UpdateAccountInfo(a);

                if ((user != null) && (acc != null) && (us != null))
                {
                    var userActivity = "Updates User Info";

                    var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "UpdateUserInfo", "");

                    Session["Success"] = "Successfully Updated.";

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "UpdateUserInfo", ex.StackTrace);

            }
            finally
            {
                var countries = _refRepo.GetAllCountries();

                ViewBag.Countries = countries.Select(c => new SelectListItem()
                {
                    Value = c.CountryId.ToString(),
                    Text = c.CountryName,
                    Selected = c.CountryId == info.CountryId
                }).ToList();

                TempData["Username"] = TempData["Username"];
            }

            return View();
        }

        [HttpPost]
        public ActionResult UpdateUserStatus(int aId, bool isEnable)
        {
            Account a = new Account();
            try
            {
                action = "updating user status.";

                var au = _userRepo.GetDetailsbyAccountId(aId);

                a.AccountId = au.AccountId;
                a.Username = au.Username;
                a.Password = au.Password;
                a.PIN = au.PIN;
                a.ParentId = au.ParentId;
                a.ParentTypeId = au.ParentTypeId;
                a.RoleId = au.RoleId;
                a.IsActive = isEnable;
                a.IsDeleted = au.IsDeleted;
                a.LogTries = au.LogTries;
                a.IPAddress = au.IPAddress;
                a.AccountAvailableDate = au.AccountAvailableDate;
                a.LastLoggedIn = au.LastLoggedIn;
                a.PasswordExpirationDate = au.PasswordExpirationDate;
                a.DateCreated = au.DateCreated;
                a.UserId = au.UserId;
                a.NeedsUpdate = au.NeedsUpdate;


                var account = _userRepo.UpdateUserStatus(a);

                var userActivity = "Updates User Status";

                var errRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "UpdateUserStatus", "");

                return Json(account);
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "UpdateUserStatus", ex.StackTrace);

            }
            return View();
        }
        #endregion

        public ActionResult ViewInfo(int? id)
        {
            var userActivity = "Entered Update User Info Page";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "UpdateUserInfo", "");

            try
            {
                action = "fetching data for the user info.";

                var a = _userRepo.GetDetailsbyAccountId(id.Value);

                var countries = _refRepo.GetAllCountries();

                ViewBag.Countries = countries.Select(c => new SelectListItem()
                {
                    Value = c.CountryId.ToString(),
                    Text = c.CountryName,
                    Selected = c.CountryId == a.User.ContactInformation.CountryId
                }).ToList();

                UserModel u = new UserModel();
                TempData["ParentId"] = a.ParentId;
                TempData["ParentTypeId"] = a.ParentTypeId;
                u.UserId = a.UserId;
                u.FirstName = a.User.FirstName;
                u.MiddleName = a.User.MiddleName;
                u.LastName = a.User.LastName;
                u.Address = a.User.ContactInformation.Address;
                u.City = a.User.ContactInformation.City;
                u.StateProvince = a.User.ContactInformation.StateProvince;
                u.ZipCode = a.User.ContactInformation.ZipCode;
                u.CountryId = a.User.ContactInformation.CountryId;
                u.PrimaryContactNumber = a.User.ContactInformation.PrimaryContactNumber;
                u.Fax = a.User.ContactInformation.Fax;
                u.MobileNumber = a.User.ContactInformation.MobileNumber;
                u.EmailAddress = a.User.EmailAddress;
                u.Username = a.Username;
                u.Password = Utility.Decrypt(a.Password);
                u.ConfirmPassword = Utility.Decrypt(a.Password);

                return View(u);
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "UpdateUserInfo", ex.StackTrace);
            }

            return View(new UserModel());
        }
    }
}
