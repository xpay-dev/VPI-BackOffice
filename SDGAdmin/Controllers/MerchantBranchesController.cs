using SDGAdmin.Models;
using SDGDAL;
using SDGDAL.Entities;
using SDGDAL.Repositories;
using SDGUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SDGAdmin.Controllers
{
    [Authorize]
    [CustomAttributes.SessionExpireFilter]
    public class MerchantBranchesController : Controller
    {
        private MerchantRepository _merchantRepo;
        private UserRepository _userRepo;
        private ResellerRepository _resellerRepo;
        private MerchantBranchRepository _branchRepo;
        private PartnerRepository _partnerRepo;
        private ReferenceRepository _refRepo;

        private string action = string.Empty;

        public MerchantBranchesController()
        {
            _merchantRepo = new MerchantRepository();
            _userRepo = new UserRepository();
            _resellerRepo = new ResellerRepository();
            _branchRepo = new MerchantBranchRepository();
            _partnerRepo = new PartnerRepository();
            _refRepo = new ReferenceRepository();
        }

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

        public ActionResult Index(int? id)
        {
            var userActivity = "Entered Merchant Branch Index";

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

            var errRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "NewUser", "");

            try
            {
                TempData["MerchantBranchID"] = id;

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

                var actRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "NewUser", ex.StackTrace);
            }

            return View();
        }

        [HttpPost]
        public ActionResult NewUser(UserModel user)
        {
            try
            {
                action = "creating new user for merchant branch.";

                var merchant = _branchRepo.GetDetailsbyMerchantBranchId(Convert.ToInt32(TempData["MerchantBranchID"]));

                if (!_userRepo.IsUserNameAvailable(user.Username))
                {
                    ModelState.AddModelError(string.Empty, "Username is not available.");
                    return View();
                }

                if (!_userRepo.IsPinAvailable(user.PIN, merchant.MerchantId))
                {
                    ModelState.AddModelError(string.Empty, "PIN is not available.");
                }

                if (ModelState.IsValid && user != null)
                {
                    var m = new SDGDAL.Entities.MerchantBranch();
                    m.MerchantBranchId = Convert.ToInt32(TempData["MerchantBranchID"]);

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
                    acc.PIN = Utility.Encrypt(user.PIN);
                    acc.User = u;
                    acc.RoleId = 1;

                    var newUser = _branchRepo.CreateBranchUser(m, acc);

                    if ((newUser.AccountId > 0) && (newUser.UserId > 0))
                    {
                        var userActivity = "Created New User for Merchant Branch";

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
                    Text = c.CountryName,
                    Selected = c.CountryId == 1
                }).ToList();
            }

            return View();
        }

        public ActionResult Registration(string id)
        {
            var userActivity = "Entered Merchant Branch Registration Page";

            var errRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "Registration", "");

            if (!string.IsNullOrEmpty(id))
            {
                GenerateMerchantsForDropDown(Convert.ToInt32(Utility.Decrypt(id.Contains(" ") == true ? id.Replace(" ", "+") : id)));
            }

            GetCountries();

            GetPartner();
            return View(new MerchantBranchModel());
        }

        [HttpPost]
        public ActionResult Registration(MerchantBranchModel branch)
        {
            try
            {
                action = "creating new merchant branch";

                int mId = 0;
                if (branch.MerchantId > 0)
                {
                    mId = branch.MerchantId;
                }
                else
                {
                    mId = Convert.ToInt32(Utility.Decrypt(Convert.ToString(Request["mId"])));
                }

                if (CurrentUser.ParentType != Enums.ParentType.Partner)
                {
                    return HttpNotFound();
                }

                if (!_userRepo.IsUserNameAvailable(branch.User.Username))
                {
                    ModelState.AddModelError(string.Empty, "Username is not available.");
                }

                if (!_userRepo.IsBranchNameAvailable(branch.BranchName))
                {
                    ModelState.AddModelError(string.Empty, "Branch name is not available.");
                }

                if (!_userRepo.IsPinAvailable(branch.User.PIN, mId))
                {
                    ModelState.AddModelError(string.Empty, "PIN is not available.");
                }

                //GenerateMerchantsForDropDown(branch.ParentMerchant.MerchantId);
                GenerateMerchantsForDropDown(mId);

                if (ModelState.IsValid && branch != null)
                {
                    var p = new SDGDAL.Entities.MerchantBranch();
                    p.MerchantBranchName = branch.BranchName;
                    p.MerchantId = mId;
                    p.IsActive = true;

                    p.ContactInformation = new SDGDAL.Entities.ContactInformation()
                    {
                        Address = branch.Address,
                        City = branch.City,
                        StateProvince = branch.StateProvince,
                        CountryId = branch.CountryId,
                        ZipCode = branch.ZipCode,
                        PrimaryContactNumber = branch.PrimaryContactNumber,
                        Fax = branch.Fax,
                        MobileNumber = branch.MobileNumber
                    };

                    var u = new SDGDAL.Entities.User();

                    u.FirstName = branch.User.FirstName;
                    u.LastName = branch.User.LastName;
                    u.MiddleName = branch.User.MiddleName;
                    u.EmailAddress = branch.User.EmailAddress;

                    if (branch.User.AddressSameAsParent)
                    {
                        u.ContactInformation = new SDGDAL.Entities.ContactInformation()
                        {
                            Address = branch.Address,
                            City = branch.City,
                            StateProvince = branch.StateProvince,
                            CountryId = branch.CountryId,
                            ZipCode = branch.ZipCode,
                            PrimaryContactNumber = branch.PrimaryContactNumber,
                            Fax = branch.Fax,
                            MobileNumber = branch.MobileNumber
                        };
                    }
                    else
                    {
                        u.ContactInformation = new SDGDAL.Entities.ContactInformation()
                        {
                            Address = branch.User.Address,
                            City = branch.User.City,
                            StateProvince = branch.User.StateProvince,
                            CountryId = branch.User.CountryId,
                            ZipCode = branch.User.ZipCode,
                            PrimaryContactNumber = branch.User.PrimaryContactNumber,
                            Fax = branch.User.Fax,
                            MobileNumber = branch.User.MobileNumber
                        };
                    }

                    var acc = new SDGDAL.Entities.Account();
                    acc.Username = branch.User.Username;
                    acc.Password = branch.User.Password;
                    acc.User = u;
                    acc.RoleId = 1;
                    acc.PIN = Utility.Encrypt(branch.User.PIN);

                    var newBranch = _branchRepo.CreateBranchtWithUser(p, acc);

                    if (newBranch.MerchantBranchId > 0)
                    {
                        var userActivity = "Registered a Merchant Branch";

                        var errRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "Registration", "");

                        Session["Success"] = "Merchant Branch successfully created.";

                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "Registration", ex.StackTrace);

                ErrorLog err = new ErrorLog();
                err.Action = "Merchant Branch Registration";
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

            return View(branch);
        }

        public ActionResult UpdateInfo(string id)
        {
            try
            {
                action = "fetching data for merchant branch";

                var userActivity = "Entered Update Info Page";

                var errRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "UpdateInfo", "");

                var merchantbranch =
                    id != string.Empty ?
                    _branchRepo.GetDetailsbyMerchantBranchId(Convert.ToInt32(Utility.Decrypt(id.Contains(" ") == true ? id.Replace(" ", "+") : id)))
                    :
                    _branchRepo.GetDetailsbyMerchantBranchId(CurrentUser.ParentId);

                TempData["MerchantBranchId"] = merchantbranch.MerchantBranchId;
                TempData["ContactInformationId"] = merchantbranch.ContactInformation.ContactInformationId;

                var countries = _refRepo.GetAllCountries();

                ViewBag.Countries = countries.Select(c => new SelectListItem()
                {
                    Value = c.CountryId.ToString(),
                    Text = c.CountryName,
                    Selected = c.CountryId == merchantbranch.ContactInformation.CountryId
                }).ToList();

                MerchantBranchModel m = new MerchantBranchModel();
                m.BranchName = merchantbranch.MerchantBranchName;
                m.Address = merchantbranch.ContactInformation.Address;
                m.City = merchantbranch.ContactInformation.City;
                m.StateProvince = merchantbranch.ContactInformation.StateProvince;
                m.ZipCode = merchantbranch.ContactInformation.ZipCode;
                m.CountryId = merchantbranch.ContactInformation.CountryId;
                m.PrimaryContactNumber = merchantbranch.ContactInformation.PrimaryContactNumber;
                m.Fax = merchantbranch.ContactInformation.Fax;
                m.MobileNumber = merchantbranch.ContactInformation.MobileNumber;
                m.IsActive = merchantbranch.IsActive;

                return View(m);
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "UpdateInfo", ex.StackTrace);

                return HttpNotFound();
            }
        }

        [HttpPost]
        public ActionResult UpdateInfo(MerchantBranchModel branch)
        {
            MerchantBranch b = new MerchantBranch();

            try
            {
                b.ContactInformation = new ContactInformation();

                b.ContactInformation.ContactInformationId = Convert.ToInt32(TempData["ContactInformationId"]);
                b.ContactInformation.Address = branch.Address;
                b.ContactInformation.City = branch.City;
                b.ContactInformation.StateProvince = branch.StateProvince;
                b.ContactInformation.ZipCode = branch.ZipCode;
                b.ContactInformation.CountryId = branch.CountryId;
                b.ContactInformation.PrimaryContactNumber = branch.PrimaryContactNumber;
                b.ContactInformation.MobileNumber = branch.MobileNumber;
                b.ContactInformation.Fax = branch.Fax;
                b.ContactInformation.NeedsUpdate = false;

                action = "updating the merchant branch info";

                var response = _branchRepo.UpdateMerchantBranch(b);

                if (response != null)
                {
                    var userActivity = "Updates Merchant Branch Info";

                    var errRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "UpdateInfo", "");

                    Session["Success"] = "Successfully Updated.";

                    return RedirectToAction("Index");
                }
                else
                {
                    throw new Exception(response.MerchantBranchName);
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "UpdateInfo", ex.StackTrace);

                return HttpNotFound();
            }
            finally
            {
                TempData["MerchantBranchId"] = branch.MerchantBranchId;
                var countries = _refRepo.GetAllCountries();

                ViewBag.Countries = countries.Select(c => new SelectListItem()
                {
                    Value = c.CountryId.ToString(),
                    Text = c.CountryName,
                    Selected = c.CountryId == branch.CountryId
                }).ToList();
            }
        }

        public ActionResult UpdateMyInfo()
        {
            return View();
        }

        public ActionResult ViewInfo(string id)
        {
            try
            {
                action = "fetching data for merchant branch";

                var userActivity = "Entered Update Info Page";

                var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "ViewInfo", "");

                var merchantbranch =
                    id != string.Empty ?
                    _branchRepo.GetDetailsbyMerchantBranchId(Convert.ToInt32(Utility.Decrypt(id.Contains(" ") == true ? id.Replace(" ", "+") : id)))
                    :
                    _branchRepo.GetDetailsbyMerchantBranchId(CurrentUser.ParentId);

                TempData["MerchantBranchId"] = merchantbranch.MerchantBranchId;
                TempData["CountryId"] = merchantbranch.ContactInformation.CountryId;
                var countries = _refRepo.GetAllCountries();

                ViewBag.Countries = countries.Select(c => new SelectListItem()
                {
                    Value = c.CountryId.ToString(),
                    Text = c.CountryName,
                    Selected = c.CountryId == merchantbranch.ContactInformation.CountryId
                }).ToList();

                MerchantBranchModel m = new MerchantBranchModel();
                m.BranchName = merchantbranch.MerchantBranchName;
                m.Address = merchantbranch.ContactInformation.Address;
                m.City = merchantbranch.ContactInformation.City;
                m.StateProvince = merchantbranch.ContactInformation.StateProvince;
                m.ZipCode = merchantbranch.ContactInformation.ZipCode;
                m.CountryId = merchantbranch.ContactInformation.CountryId;
                m.PrimaryContactNumber = merchantbranch.ContactInformation.PrimaryContactNumber;
                m.Fax = merchantbranch.ContactInformation.Fax;
                m.MobileNumber = merchantbranch.ContactInformation.MobileNumber;
                m.IsActive = merchantbranch.IsActive;

                return View(m);
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "ViewInfo", ex.StackTrace);

                var countries = _refRepo.GetAllCountries();

                ViewBag.Countries = countries.Select(c => new SelectListItem()
                {
                    Value = c.CountryId.ToString(),
                    Text = c.CountryName,
                    Selected = c.CountryId == Convert.ToInt32(TempData["CountryId"])
                }).ToList();

                return HttpNotFound();
            }
        }

        public ActionResult ViewMyInfo()
        {
            return View();
        }

        private void GenerateMerchantsForDropDown(int? merchantId)
        {
            List<SDGDAL.Entities.Merchant> merchants = new List<SDGDAL.Entities.Merchant>();
            try
            {
                action = "generating merchant dropdown for merchant branch.";

                var ddlm = new List<SelectListItem>();
                var val = 0;

                if (CurrentUser.ParentType == Enums.ParentType.Partner)
                {
                    merchants = _merchantRepo.GetAllMerchantsByPartner(CurrentUser.ParentId, "");
                    if (merchants.Count != 0)
                    {
                        ddlm.Add(new SelectListItem()
                        {
                            Text = "Select a Merchant",
                            Value = "0",
                            Selected = val == 0
                        });

                        ddlm.AddRange(merchants.Select(m => new SelectListItem()
                        {
                            Value = m.MerchantId.ToString(),
                            Text = m.MerchantName + " (Reseller: " + m.Reseller.ResellerName + ")",
                            Selected = merchantId.HasValue ? m.MerchantId == merchantId.Value : false
                        }).ToList());
                    }
                    else
                    {
                        ddlm.Add(new SelectListItem()
                        {
                            Text = "No Merchants available",
                            Value = "0",
                            Selected = val == 0
                        });
                    }
                    ViewBag.Merchants = ddlm;
                }
                else if (CurrentUser.ParentType == Enums.ParentType.Reseller)
                {
                    var merch = _merchantRepo.GetMerchantDetails(CurrentUser.ParentId);

                    if (merchants.Count != 0)
                    {
                        merchants.Add(merch);

                        ddlm.AddRange(merchants.Where(m => m.MerchantId == CurrentUser.ParentId).Select(m => new SelectListItem()
                        {
                            Value = m.MerchantId.ToString(),
                            Text = m.MerchantName
                        }).ToList());
                    }
                    else
                    {
                        ddlm.Add(new SelectListItem()
                        {
                            Text = "No Merchants available",
                            Value = "0",
                            Selected = val == 0
                        });
                    }

                    ViewBag.Merchants = ddlm;
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "GenerateMerchantsForDropDown", ex.StackTrace);
            }
        }

        public void GetCountries()
        {
            var countries = _refRepo.GetAllCountries();

            ViewBag.Countries = countries.Select(c => new SelectListItem()
            {
                Value = c.CountryId.ToString(),
                Text = c.CountryName,
                Selected = c.CountryId == 1
            }).ToList();
        }

        [HttpPost]
        public JsonResult updateMerchantBranchStatus(string bId, bool status)
        {
            MerchantBranch mb = new MerchantBranch();

            var merchantBranch = _branchRepo.GetDetailsbyMerchantBranchId(Convert.ToInt32(Utility.Decrypt(bId.Contains(" ") == true ? bId.Replace(" ", "+") : bId)));

            mb.MerchantBranchId = merchantBranch.MerchantBranchId;
            mb.MerchantBranchName = merchantBranch.MerchantBranchName;
            mb.ContactInformationId = merchantBranch.ContactInformationId;
            mb.IsActive = status;
            mb.IsDeleted = merchantBranch.IsDeleted;
            mb.DateCreated = merchantBranch.DateCreated;
            mb.MerchantId = merchantBranch.MerchantId;

            var update = _branchRepo.UpdateMerchantBranchStatus(mb);

            return Json(update);
        }
    }
}