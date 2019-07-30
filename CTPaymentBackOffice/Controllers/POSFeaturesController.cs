using CTPaymentBackOffice;
using CTPaymentBackOffice.Models;
using SDGDAL.Entities;
using SDGDAL.Repositories;
using SDGUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDG_BackOffice_Views.Controllers
{
    [Authorize]
    [CustomAttributes.SessionExpireFilter]
    public class POSFeaturesController : Controller
    {
        MerchantRepository _merchantRepo;
        UserRepository _userRepo;
        ResellerRepository _resellerRepo;
        MerchantBranchRepository _branchRepo;
        PartnerRepository _partnerRepo;
        ReferenceRepository _refRepo;
        MerchantBranchPOSRepository _posRepo;
        MobileAppRepository _mobileAppRepo;
        MobileAppFeaturesRepository _posFeatures;

        string action = string.Empty;

        public POSFeaturesController()
        {
            _posRepo = new MerchantBranchPOSRepository(); 
            _merchantRepo = new MerchantRepository();
            _userRepo = new UserRepository();
            _resellerRepo = new ResellerRepository();
            _branchRepo = new MerchantBranchRepository();
            _partnerRepo = new PartnerRepository();
            _refRepo = new ReferenceRepository();
            _mobileAppRepo = new MobileAppRepository();
            _posFeatures = new MobileAppFeaturesRepository();
        }

        public ActionResult Index(int? id)
        {
            if (CurrentUser.ParentType == Enums.ParentType.Partner)
            {
                var userActivity = "Entered POS Features Index Page";

                var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "Index", "");

                try
                {
                    action = "generating dropdown.";

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
                catch (Exception ex)
                {
                    var errorOnAction = "Error while " + action;

                    var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "Index", ex.StackTrace);
                }
            }

            return View();
        }

        public ActionResult Feature(int id)
        {
            var userActivity = "Entered POS Features Page";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "Feature", "");

            MobileAppFeaturesModel mobileAppModel = new MobileAppFeaturesModel();

            TempData["MerchantPosId"] = id;

            try
            {
                action = "fetching data for pos.";

                var mobileAppResult = _mobileAppRepo.GetMobileAppDetailsByPosId(id);

                if (mobileAppResult.MobileAppFeaturesId != null)
                {
                    var mobileAppFeatures = _posFeatures.GetMobileAppFeaturesById((int)mobileAppResult.MobileAppFeaturesId);

                    GenerateDataForDropDown();

                    TempData["MobileAppFeaturesId"] = mobileAppFeatures.MobileAppFeaturesId;

                    mobileAppModel.GPSEnabled = mobileAppFeatures.GPSEnabled;
                    mobileAppModel.CreditTransaction = mobileAppFeatures.CreditTransaction;
                    mobileAppModel.DebitTransaction = mobileAppFeatures.DebitTransaction;
                    mobileAppModel.CashTransaction = mobileAppFeatures.CashTransaction;
                    mobileAppModel.ChequeTransaction = mobileAppFeatures.ChequeTransaction;
                    mobileAppModel.CreditSignature = mobileAppFeatures.CreditSignature;
                    mobileAppModel.DebitSignature = mobileAppFeatures.DebitSignature;
                    mobileAppModel.CashSignature = mobileAppFeatures.CashSignature;
                    mobileAppModel.ChequeSignature = mobileAppFeatures.ChequeSignature;
                    mobileAppModel.DebitRefund = mobileAppFeatures.DebitRefund;
                    mobileAppModel.ChequeType = mobileAppFeatures.ChequeType;
                    mobileAppModel.ProofId = mobileAppFeatures.ProofId;
                    mobileAppModel.ReferenceNumber = mobileAppFeatures.ReferenceNumber;
                    mobileAppModel.TOSRequired = mobileAppFeatures.TOSRequired;
                    mobileAppModel.DisclaimerRequired = mobileAppFeatures.DisclaimerRequired;

                    mobileAppModel.SMSEnabled = mobileAppFeatures.SMSEnabled;
                    mobileAppModel.EmailAllowed = mobileAppFeatures.EmailAllowed;
                    mobileAppModel.EmailLimit = mobileAppFeatures.EmailLimit;
                    mobileAppModel.CheckForEmailDuplicates = mobileAppFeatures.CheckForEmailDuplicates;
                    mobileAppModel.BillingCyclesCheckEmail = mobileAppFeatures.BillingCyclesCheckEmail;
                    mobileAppModel.PrintAllowed = mobileAppFeatures.PrintAllowed;
                    mobileAppModel.PrintLimit = mobileAppFeatures.PrintLimit;
                    mobileAppModel.CheckForPrintDuplicates = mobileAppFeatures.CheckForPrintDuplicates;
                    mobileAppModel.BillingCyclesCheckPrint = mobileAppFeatures.BillingCyclesCheckPrint;

                    mobileAppModel.TipsEnabled = mobileAppFeatures.TipsEnabled;
                    mobileAppModel.Amount1 = mobileAppFeatures.Amount1;
                    mobileAppModel.Amount2 = mobileAppFeatures.Amount2;
                    mobileAppModel.Amount3 = mobileAppFeatures.Amount3;
                    mobileAppModel.Percentage1 = mobileAppFeatures.Percentage1;
                    mobileAppModel.Percentage2 = mobileAppFeatures.Percentage2;
                    mobileAppModel.Percentage3 = mobileAppFeatures.Percentage3;

                    return View(mobileAppModel);
                }
                else
                {
                    GenerateDataForDropDown();

                    return View(mobileAppModel);
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "Feature", ex.StackTrace);
            }

            return View();
        }

        [HttpPost]
        public ActionResult Feature(MobileAppFeatures mobileAppModel)
        {
            try
            {
                action = "updating pos feature.";

                var posFeatures = new SDGDAL.Entities.MobileAppFeatures();

                posFeatures.SystemMode = mobileAppModel.SystemMode;
                posFeatures.LanguageCode = mobileAppModel.LanguageCode;
                posFeatures.CurrencyId = mobileAppModel.CurrencyId;
                posFeatures.GPSEnabled = mobileAppModel.GPSEnabled;
                posFeatures.SMSEnabled = mobileAppModel.SMSEnabled;
                posFeatures.EmailAllowed = mobileAppModel.EmailAllowed;
                posFeatures.EmailLimit = mobileAppModel.EmailLimit;
                posFeatures.CheckForEmailDuplicates = mobileAppModel.CheckForEmailDuplicates;
                posFeatures.BillingCyclesCheckEmail = mobileAppModel.BillingCyclesCheckEmail;
                posFeatures.PrintAllowed = mobileAppModel.PrintAllowed;
                posFeatures.PrintLimit = mobileAppModel.PrintLimit;
                posFeatures.CheckForPrintDuplicates = mobileAppModel.CheckForPrintDuplicates;
                posFeatures.BillingCyclesCheckPrint = mobileAppModel.BillingCyclesCheckPrint;
                posFeatures.ReferenceNumber = mobileAppModel.ReferenceNumber;
                posFeatures.CreditSignature = mobileAppModel.CreditSignature;
                posFeatures.DebitSignature = mobileAppModel.DebitSignature;
                posFeatures.ChequeSignature = mobileAppModel.ChequeSignature;
                posFeatures.CashSignature = mobileAppModel.CashSignature;
                posFeatures.CreditTransaction = mobileAppModel.CreditTransaction;
                posFeatures.DebitTransaction = mobileAppModel.DebitTransaction;
                posFeatures.ChequeTransaction = mobileAppModel.ChequeTransaction;
                posFeatures.CashTransaction = mobileAppModel.CashTransaction;
                posFeatures.ProofId = mobileAppModel.ProofId;
                posFeatures.ChequeType = mobileAppModel.ChequeType;
                posFeatures.DebitRefund = mobileAppModel.DebitRefund;
                posFeatures.TOSRequired = mobileAppModel.TOSRequired;
                posFeatures.DisclaimerRequired = mobileAppModel.DisclaimerRequired;
                posFeatures.Percentage1 = mobileAppModel.Percentage1;
                posFeatures.Percentage2 = mobileAppModel.Percentage2;
                posFeatures.Percentage3 = mobileAppModel.Percentage3;
                posFeatures.TipsEnabled = mobileAppModel.TipsEnabled;
                posFeatures.Amount1 = mobileAppModel.Amount1;
                posFeatures.Amount2 = mobileAppModel.Amount2;
                posFeatures.Amount3 = mobileAppModel.Amount3;

                if (TempData["MobileAppFeaturesId"] != null)
                {
                    posFeatures.MobileAppFeaturesId = Convert.ToInt32(TempData["MobileAppFeaturesId"]);

                    var result = _posFeatures.UpdateMobileAppFeatures(posFeatures);

                    if (result != null)
                    {
                        var userActivity = "POS Features Updated";

                        var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "Feature", "");

                        Session["Success"] = "Successfully Updated.";

                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    int posId = Convert.ToInt32(TempData["MerchantPosId"]);

                    MobileApp mobileApp = new MobileApp();
                    mobileApp.MerchantBranchPOSId = posId;

                    var result = _posFeatures.CreateMobileAppFeatures(posFeatures, mobileApp);

                    if (result != null)
                    {
                        var userActivity = "POS Features Updated";

                        var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "Feature", "");

                        Session["Success"] = "Successfully Updated.";

                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "Feature", ex.StackTrace);
            }
            return View();
        }

        private void GenerateDataForDropDown()
        {
            try
            {
                action = "generating dropdown.";

                var value = "";
                var currencies = _refRepo.GetAllCurrencies();

                var ddlSystemMode = new List<SelectListItem>()
            {
                new SelectListItem() {
                    Text = "LIVE",
                    Value = "LIVE",
                    Selected = value == "LIVE"
                },
                new SelectListItem() {
                    Text = "TESTAPPROVED",
                    Value = "TESTAPPROVED"
                }
            };

                ViewBag.Language = new List<SelectListItem>()
            {
                new SelectListItem() {
                    Text = "EN-CA",
                    Value = "EN-CA",
                    Selected = value == "EN-CA"
                }
            };

                var ddlcurrency = new List<SelectListItem>();

                ddlcurrency.AddRange(currencies.Select(c => new SelectListItem()
                {
                    Value = c.CurrencyId.ToString(),
                    Text = c.CurrencyCode,
                    Selected = c.CurrencyId == 1 //hard-code for php
                }).ToList());

                var ddlPosDefault = new List<SelectListItem>()
            {
                new SelectListItem() {
                    Text = "No",
                    Value = "false"
                },
                new SelectListItem() {
                    Text = "Yes",
                    Value = "true"
                }
            };

                var ddlBillingCycle = new List<SelectListItem>()
            {
                new SelectListItem() {
                    Text = "1",
                    Value = "1"
                },
                new SelectListItem() {
                    Text = "2",
                    Value = "2"
                },
                new SelectListItem() {
                    Text = "3",
                    Value = "3"
                }
            };

                ViewBag.Currencies = ddlcurrency;
                ViewBag.DefaultPos = ddlPosDefault;
                ViewBag.SystemMode = ddlSystemMode;
                ViewBag.BillingCycle = ddlBillingCycle;
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "GenerateDataForDropDown", ex.StackTrace);
            }
        } 
    }
}
