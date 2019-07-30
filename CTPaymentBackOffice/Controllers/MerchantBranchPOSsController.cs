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
    public class MerchantBranchPOSsController : Controller
    {
        MerchantRepository _merchantRepo;
        UserRepository _userRepo;
        ResellerRepository _resellerRepo;
        MerchantBranchRepository _branchRepo;
        PartnerRepository _partnerRepo;
        ReferenceRepository _refRepo;
        MerchantBranchPOSRepository _posRepo;
        MobileAppFeaturesRepository _posFeatures;

        string action = string.Empty;

        public MerchantBranchPOSsController()
        {
            _posRepo = new MerchantBranchPOSRepository();
            _merchantRepo = new MerchantRepository();
            _userRepo = new UserRepository();
            _resellerRepo = new ResellerRepository();
            _branchRepo = new MerchantBranchRepository();
            _partnerRepo = new PartnerRepository();
            _refRepo = new ReferenceRepository();
            _posFeatures = new MobileAppFeaturesRepository();
        }

        public ActionResult Index(int? id)
        {
            if (CurrentUser.ParentType == Enums.ParentType.Partner)
            {
                var userActivity = "Entered Merchant Branch POSs Index Page";

                var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "Index", "");

                if (Session["Success"] != null)
                {
                    ViewBag.Success = Session["Success"];
                }

                Session["Success"] = null;

                var resellers = _resellerRepo.GetAllResellersByPartner(CurrentUser.ParentId, "");

                var merchants = _merchantRepo.GetAllMerchantsByPartner(CurrentUser.ParentId, "");

                var merchantBranches = _branchRepo.GetAllBranchesByPartner(CurrentUser.ParentId, "");

                var ddlResellers = new List<SelectListItem>();
                var ddlMerchants = new List<SelectListItem>();
                var ddlBranches = new List<SelectListItem>();


                ddlMerchants.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = merchants.Count == 0 ? "No Merchants available" : "All Merchants",
                    Selected = true
                });

                ddlBranches.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = merchants.Count == 0 ? "No Branches available" : "All Branches",
                    Selected = true
                });

                if (resellers.Count != 0)
                {

                    if (resellers.Count == 1)
                    {
                        ddlResellers.AddRange(resellers.Select(p => new SelectListItem()
                        {
                            Value = p.ResellerId.ToString(),
                            Text = p.ResellerName
                        }).ToList());
                    }
                    else
                    {

                        ddlResellers.Add(new SelectListItem()
                        {
                            Value = "0",
                            Text = "All Resellers",
                            Selected = true
                        });

                        ddlResellers.AddRange(resellers.Select(p => new SelectListItem()
                        {
                            Value = p.ResellerId.ToString(),
                            Text = p.ResellerName
                        }).ToList());
                    }
                }
                else
                {
                    ddlResellers.Add(new SelectListItem()
                    {
                        Value = "0",
                        Text = "No Resellers available",
                        Selected = true
                    });
                }

                ddlMerchants.AddRange(merchants.Select(p => new SelectListItem()
                {
                    Value = p.MerchantId.ToString(),
                    Text = p.MerchantName + " (R: " + p.Reseller.ResellerName + ")"
                }).ToList());

                ddlBranches.AddRange(merchantBranches.Select(p => new SelectListItem()
                {
                    Value = p.MerchantBranchId.ToString(),
                    Text = p.MerchantBranchName + " (M: " + p.Merchant.MerchantName + ")",
                    Selected = p.MerchantBranchId == ((id.HasValue) ? id.Value : 0)
                }).ToList());

                ViewBag.Resellers = ddlResellers;
                ViewBag.Merchants = ddlMerchants;
                ViewBag.Branches = ddlBranches;
            }
            else if (CurrentUser.ParentType == Enums.ParentType.Reseller)
            {
                int resellerId = id.HasValue ? id.Value : CurrentUser.ParentId;
                var resellerInfo = _resellerRepo.GetDetailsbyResellerId(resellerId);
                var reseller = _resellerRepo.GetResellerById(resellerId);
                var merchants = _merchantRepo.GetAllMerchantsByReseller(resellerId, "");
                var merchantBranches = _branchRepo.GetAllBranchesByReseller(resellerId, "");

                var ddlResellers = new List<SelectListItem>();
                var ddlMerchants = new List<SelectListItem>();
                var ddlBranches = new List<SelectListItem>();

                ddlMerchants.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = merchants.Count == 0 ? "No Merchants available" : "All Merchants",
                    Selected = true
                });

                ddlBranches.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = merchants.Count == 0 ? "No Branches available" : "All Branches",
                    Selected = true
                });

                ddlResellers.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = "All Resellers",
                    Selected = true
                });

                ddlResellers.AddRange(reseller.Select(p => new SelectListItem()
                {
                    Value = p.ResellerId.ToString(),
                    Text = p.ResellerName,
                    Selected = p.ResellerId == resellerId
                }).ToList());

                ddlMerchants.AddRange(merchants.Select(p => new SelectListItem()
                {
                    Value = p.MerchantId.ToString(),
                    Text = p.MerchantName + " (R: " + p.Reseller.ResellerName + ")",
                    Selected = p.MerchantId == ((id.HasValue) ? id.Value : 0)
                }).ToList());

                ddlBranches.AddRange(merchantBranches.Select(p => new SelectListItem()
                {
                    Value = p.MerchantBranchId.ToString(),
                    Text = p.MerchantBranchName + " (M: " + p.Merchant.MerchantName + ")",
                    Selected = p.MerchantBranchId == ((id.HasValue) ? id.Value : 0)
                }).ToList());

                ViewBag.Resellers = ddlResellers;
                ViewBag.Merchants = ddlMerchants;
                ViewBag.Branches = ddlBranches;
            }
            else if (CurrentUser.ParentType == Enums.ParentType.Merchant)
            {
                int merchantId = id.HasValue ? id.Value : CurrentUser.ParentId;
                var merchantInfo = _merchantRepo.GetDetailsbyMerchantId(merchantId);
                var reseller = _resellerRepo.GetResellerById(Convert.ToInt32(merchantInfo.ResellerId));
                var merchants = _merchantRepo.GetMerchantById(merchantId);
                var merchantBranches = _branchRepo.GetAllBranchesByMerchant(merchantId, "");

                var ddlResellers = new List<SelectListItem>();
                var ddlMerchants = new List<SelectListItem>();
                var ddlBranches = new List<SelectListItem>();

                ddlResellers.AddRange(reseller.Select(p => new SelectListItem()
                {
                    Value = p.ResellerId.ToString(),
                    Text = p.ResellerName,
                    Selected = p.ResellerId == merchantInfo.ResellerId
                }).ToList());

                ddlMerchants.AddRange(merchants.Select(p => new SelectListItem()
                {
                    Value = p.MerchantId.ToString(),
                    Text = p.MerchantName + " (R: " + p.Reseller.ResellerName + ")",
                    Selected = p.MerchantId == ((id.HasValue) ? id.Value : 0)
                }).ToList());

                ddlBranches.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = merchants.Count == 0 ? "No Branches available" : "All Branches",
                    Selected = true
                });

                ddlBranches.AddRange(merchantBranches.Select(p => new SelectListItem()
                {
                    Value = p.MerchantBranchId.ToString(),
                    Text = p.MerchantBranchName + " (M: " + p.Merchant.MerchantName + ")",
                    Selected = p.MerchantBranchId == ((id.HasValue) ? id.Value : 0)
                }).ToList());

                ViewBag.Resellers = ddlResellers;
                ViewBag.Merchants = ddlMerchants;
                ViewBag.Branches = ddlBranches;
            }

            return View();
        }

        public void GetResellersByPartnerId()
        {
            try
            {
                action = "fetching the partner list";

                var resellers = _resellerRepo.GetAllResellersByPartner(CurrentUser.ParentId, "");

                var ddlResellers = new List<SelectListItem>();

                if (resellers.Count != 0)
                {
                    if (resellers.Count == 1)
                    {
                        ddlResellers.AddRange(resellers.Select(p => new SelectListItem()
                        {
                            Value = p.ResellerId.ToString(),
                            Text = p.ResellerName
                        }).ToList());
                    }
                    else
                    {
                        ddlResellers.Add(new SelectListItem()
                        {
                            Value = "0",
                            Text = "Select a Reseller",
                            Selected = 0 == 0
                        });

                        ddlResellers.AddRange(resellers.Select(p => new SelectListItem()
                        {
                            Value = p.ResellerId.ToString(),
                            Text = p.ResellerName
                        }).ToList());
                    }
                }
                else
                {
                    ddlResellers.Add(new SelectListItem()
                    {
                        Value = "0",
                        Text = "No Partners available",
                        Selected = 0 == 0
                    });
                }

                ViewBag.Resellers = ddlResellers;
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "GetPartner", ex.StackTrace);
            }
        }

        public ActionResult Create(int? id)
        {
            var userActivity = "Entered Create Merchant Branch POSs Page";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "Create", "");

            GetResellersByPartnerId();

            int branchId = id.HasValue ? id.Value : 0;
            if (CurrentUser.ParentType == Enums.ParentType.Merchant)
            {
                ViewBag.UserId = CurrentUser.ParentId;
            }

            TempData["BranchId"] = branchId;

            List<MerchantPOSModel> poss = new List<MerchantPOSModel>();

            var x = new List<MerchantBranchModel>();

            int qty = 1;

            if (!string.IsNullOrEmpty(Request["qty"]))
            {
                int.TryParse(Request["qty"], out qty);
            }

            if (!string.IsNullOrEmpty(Request["posqty"]))
            {
                int.TryParse(Request["posqty"], out qty);
            }

            var codeGenerator = new SDGUtil.CodeGenerator();

            for (int i = 0; i < qty; i++)
            {
                poss.Add(new MerchantPOSModel()
                {
                    ActivationCode = codeGenerator.FormatCode(4, codeGenerator.GenerateCode(16), "-"),
                    POSName = "POS " + codeGenerator.GenerateCode(4),
                    MerchantBranchId = branchId
                });
            }

            return View(poss);
        }

        [HttpPost]
        public JsonResult GetBranchId(int id)
        {
            TempData["BranchId"] = id;

            return Json(id);
        }

        [HttpPost]
        public ActionResult Create(MerchantPOSModel[] pos, FormCollection form)
        {
            try
            {
                bool isCheck = false;
                int count = 0;

                action = "creating merchant branch pos";

                string merchantName = Convert.ToString(Request["merchantName"]);
                string location = Convert.ToString(Request["location"]);
                isCheck = Convert.ToBoolean(Request["isCheck"]);
                int mId = Convert.ToInt32(Request["merchantId"]);
                int bId = Convert.ToInt32(Request["bId"]);

                if (bId != 0)
                {
                    List<MobileApp> mApps = new List<MobileApp>();
                    for (int i = 0; i < pos.Length; i++)
                    {
                        var posFeatures = new SDGDAL.Entities.MobileAppFeatures();
                        count = i;
                        posFeatures.SystemMode = "LIVE";
                        posFeatures.LanguageCode = "EN-CA";
                        posFeatures.CurrencyId = 1;
                        posFeatures.GPSEnabled = false;
                        posFeatures.SMSEnabled = false;
                        posFeatures.EmailAllowed = false;
                        posFeatures.EmailLimit = 0;
                        posFeatures.CheckForEmailDuplicates = false;
                        posFeatures.BillingCyclesCheckEmail = 1;
                        posFeatures.PrintAllowed = false;
                        posFeatures.PrintLimit = 0;
                        posFeatures.CheckForPrintDuplicates = false;
                        posFeatures.BillingCyclesCheckPrint = 0;
                        posFeatures.ReferenceNumber = false;
                        posFeatures.CreditSignature = true;
                        posFeatures.DebitSignature = false;
                        posFeatures.ChequeSignature = false;
                        posFeatures.CashSignature = false;
                        posFeatures.CreditTransaction = true;
                        posFeatures.DebitTransaction = true;
                        posFeatures.ChequeTransaction = false;
                        posFeatures.CashTransaction = false;
                        posFeatures.ProofId = false;
                        posFeatures.ChequeType = false;
                        posFeatures.DebitRefund = false;
                        posFeatures.TOSRequired = false;
                        posFeatures.DisclaimerRequired = false;
                        posFeatures.Percentage1 = 0;
                        posFeatures.Percentage2 = 0;
                        posFeatures.Percentage3 = 0;
                        posFeatures.TipsEnabled = false;
                        posFeatures.Amount1 = 0;
                        posFeatures.Amount2 = 0;
                        posFeatures.Amount3 = 0;

                        var features = _posFeatures.CreateMobileAppFeatures(posFeatures);

                        mApps.Add(new MobileApp()
                        {
                            ActivationCode = pos[i].ActivationCode,
                            DateCreated = DateTime.Now,
                            ExpirationDate = DateTime.Now.AddYears(1),
                            UpdatePending = true,
                            MobileAppFeaturesId = features.MobileAppFeaturesId,
                            MerchantBranchPOS = new MerchantBranchPOS()
                            {
                                IsActive = true,
                                MerchantPOSName = pos[i].POSName,
                                DateCreated = DateTime.Now,
                                MerchantBranchId = bId
                            }
                        });
                    }

                    bool success = _posRepo.CreateMerchantBranchPOSs(mApps);

                    if (success)
                    {
                        if (isCheck == true)
                        {
                            SendActivation.SendActivation sendMail = new SendActivation.SendActivation();

                            var eMail = _merchantRepo.GetDetailsbyMerchantId(mId);

                            if (count == 0)
                            {
                                String message = "<h4>" + "Generate Activation Code successful! Details are listed below." + "</h4><br />"
                                    + "Merchant Name: " + "<b>" + merchantName + "</b><br />"
                                    + "Location: " + "<b>" + location + "</b><br />"
                                    + "POS Name: " + "<b>" + pos[0].POSName + "</b><br />"
                                    + "Activation Code: " + "<b>" + pos[0].ActivationCode + "</b><br />";

                                sendMail.SendActivationCode(message, eMail.MerchantEmail);
                            }
                            else
                            {
                                String message = "<h4>" + "Generate Activation Code successful! Details are listed below." + "</h4><br />"
                                                                   + "Merchant Name: " + "<b>" + merchantName + "</b><br />"
                                                                   + "Location: " + "<b>" + location + "</b><br />";
                                for (int i = 0; i < count + 1; i++)
                                {
                                    message += "POS Name: <b>" + pos[i].POSName + "</b>"
                                       + " Activation Code: <b>" + pos[i].ActivationCode + "</b><br />";
                                }
                                sendMail.SendActivationCode(message, eMail.MerchantEmail);
                            }
                        }

                        var userActivity = "Created Merchant Branch POSs";

                        var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "Create", "");

                        Session["Success"] = "Merchant POS(s) successfully created.";

                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Please select a Branch.");
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "Create", ex.StackTrace);

                ErrorLog err = new ErrorLog();
                err.Action = "POS Creation";
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

                GetResellersByPartnerId();
            }

            return View(pos);
        }

        public ActionResult ViewDefaultMids()
        {
            return View();
        }

        public ActionResult UpdateDefaultMids()
        {
            return View();
        }

        public ActionResult ViewInfo(int? id)
        {
            if (CurrentUser.ParentType == Enums.ParentType.Partner)
            {
                var userActivity = "Entered Merchant Branch POSs Info Page";

                var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "ViewInfo", "");

                var merchantpos = _posRepo.GetDetailsbyMerchantPOSId((int)id);

                MerchantPOSModel pos = new MerchantPOSModel();

                pos.POSName = merchantpos.MerchantPOSName;
                pos.IsActive = merchantpos.IsActive;
                pos.BranchName = merchantpos.MerchantBranch.MerchantBranchName;

                return View(pos);
            }

            return View();
        }

        public ActionResult UpdateInfo(int? id)
        {
            if (CurrentUser.ParentType == Enums.ParentType.Partner)
            {
                var userActivity = "Entered Merchant Branch POSs Update Info Page";

                var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "UpdateInfo", "");

                var merchantpos = _posRepo.GetDetailsbyMerchantPOSId((int)id);

                MerchantPOSModel pos = new MerchantPOSModel();
                pos.POSId = merchantpos.MerchantPOSId;
                pos.POSName = merchantpos.MerchantPOSName;

                return View(pos);
            }

            return View();
        }

        [HttpPost]
        public ActionResult UpdateInfo(MerchantPOSModel pos)
        {
            if (CurrentUser.ParentType == Enums.ParentType.Partner)
            {
                try
                {
                    action = "updating the merchant branch pos info";

                    MerchantBranchPOS objPos = new MerchantBranchPOS();
                    objPos.MerchantPOSName = pos.POSName;
                    objPos.MerchantPOSId = pos.POSId;
                    var res = _posRepo.UpdateMerchantBranchPOS(objPos);

                    if (res != null)
                    {
                        var userActivity = "Udates Merchant Branch POSs Info";

                        var errRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "UpdateInfo", "");

                        Session["Success"] = "Successfully Updated.";

                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    var errorOnAction = "Error while " + action;

                    var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "UpdateInfo", ex.StackTrace);

                    ErrorLog err = new ErrorLog();
                    err.Action = "Update POS Info";
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
            }
            else
            {
                return HttpNotFound();
            }

            return View();
        }

        [HttpPost]
        public ActionResult SendActivationCode(int posId, string actCode)
        {
            bool isActivated = false;

            SendActivation.SendActivation sendMail = new SendActivation.SendActivation();

            var userActivity = "Entered Merchant Branch POSs Info Page";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "ViewInfo", "");

            var merchantpos = _posRepo.GetDetailsbyMerchantPOSId(posId);

            MerchantPOSModel pos = new MerchantPOSModel();
            isActivated = merchantpos.MobileApp.Last().DateActivated.HasValue;

            if (isActivated == false)
            {
                String message = "<h4>" + "Generate Activation Code successful! Details are listed below." + "</h4><br />"
                                + "Merchant Name: " + "<b>" + merchantpos.MerchantBranch.Merchant.MerchantName + "</b><br />"
                                + "Location: " + "<b>" + merchantpos.MerchantBranch.MerchantBranchName + "</b><br />"
                                + "POS Name: " + "<b>" + merchantpos.MerchantPOSName + "</b><br />"
                                + "Activation Code: " + "<b>" + actCode + "</b><br />";

                try
                {
                    sendMail.SendActivationCode(message, merchantpos.MerchantBranch.Merchant.MerchantEmail);
                }
                catch (Exception ex)
                {
                    return Json(ex);
                }

                return Json("success");
            }
            else
            {
                return Json("activated");
            }
        }
    }
}
