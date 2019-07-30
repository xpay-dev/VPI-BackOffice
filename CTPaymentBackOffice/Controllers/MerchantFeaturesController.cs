using CTPaymentBackOffice.Models;
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
    public class MerchantFeaturesController : Controller
    {
        MerchantFeatureRepository _merchantfeatureRepository;
        MerchantRepository _merchantRepository;
        ResellerRepository _resellerRepo;

        string action = string.Empty;

        public MerchantFeaturesController()
        {
            _merchantfeatureRepository = new MerchantFeatureRepository();
            _merchantRepository = new MerchantRepository();
            _resellerRepo = new ResellerRepository();
        }
        //
        // GET: /MerchantFeatures/

        public ActionResult Index()
        {
            var userActivity = "Entered Merchant Features Index Page";

            var errRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "Index", "");

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

            Session["Success"] = null;

            return View();
        }

        
        public ActionResult Agreements(int? id)
        {
            var userActivity = "Entered Merchant Features Agreements Page";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "Agreements", "");

            var m = _merchantRepository.GetDetailsbyMerchantId(id.Value);
            int mId = 0;
            try
            {
                action = "fetching data of merchant for agreements setting";

                mId = m.MerchantFeaturesId.Value;
                TempData["MerchantFeaturesId"] = mId;
                TempData["BillingCycleId"] = m.MerchantFeatures.BillingCycleId;
                TempData["CurrencyId"] = m.MerchantFeatures.CurrencyId.Value;
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "Agreements", ex.StackTrace);

                TempData["BillingCycleId"] = 0;
                TempData["CurrencyId"] = 0;
            }


            var feature = _merchantfeatureRepository.GetDetailsByMerchantFeaturesId(mId);

            if (feature != null)
            {

                ViewBag.TOS = new List<SelectListItem>()
                {
                    new SelectListItem() {
                        Text = "Yes",
                        Value = "true"
                    },
                    new SelectListItem() {
                        Text = "No",
                        Value = "false"
                    }
                };

                MerchantFeaturesModel mf = new MerchantFeaturesModel();
                mf.TermsOfServiceEnabled = feature.TermsOfServiceEnabled;
                mf.DisclaimerEnabled = feature.DisclaimerEnabled;

                return View(mf);
            }

            TempData["NeedsUpdate"] = "Merchant is not updated. Please register another one.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Agreements(MerchantFeaturesModel features)
        {
            try
            {
                action = "updating the agreements setting for the merchant";

                var m = new SDGDAL.Entities.MerchantFeatures();
                m.MerchantFeaturesId = Convert.ToInt32(TempData["MerchantFeaturesId"]);
                m.BillingCycleId = Convert.ToInt32(TempData["BillingCycleId"]);
                m.CurrencyId = Convert.ToInt32(TempData["CurrencyId"]);
                m.TermsOfServiceEnabled = features.TermsOfServiceEnabled;
                m.DisclaimerEnabled = features.DisclaimerEnabled;

                var f = _merchantfeatureRepository.UpdateAgreementsStatus(m);

                if (f != null)
                {
                    var userActivity = "Updates Agreements";

                    var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "Agreements", "");

                    Session["Success"] = "Successfully Updated.";

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "Agreements", ex.StackTrace);
            }

            return View();
        }

        public ActionResult AgreementsText(int? id)
        {
            var userActivity = "Entered Agreements Text Page";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "AgreementsText", "");
            try
            {
                action = "fetching the agreements detail for the merchant.";

                var a = _merchantfeatureRepository.GetDetailsByAgreementId(id.Value);

                if (a != null)
                {
                    AgreementsModel am = new AgreementsModel();
                    am.LanguageCode = a.LanguageCode;
                    am.TermsOfService = a.TermsOfService;
                    am.Disclaimer = a.Disclaimer;

                    return View(am);
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "AgreementsText", ex.StackTrace);
            }

            return View();

        }

        [HttpPost]
        public ActionResult AgreementsText(AgreementsModel agreements, int? id)
        {
            try
            {
                action = "creating/updating the merchant agreements text.";

                bool isExist = false;
                var aId = _merchantfeatureRepository.GetAgreementsId();
                int agreementsId = 0;

                foreach (var m in aId)
                {
                    if (id.Value == m.MerchantId)
                    {
                        isExist = true;
                        agreementsId = m.AgreementsId;
                    }
                }

                if (isExist == true)
                {
                    var u = new SDGDAL.Entities.Agreements();
                    u.AgreementsId = agreementsId;
                    u.TermsOfService = agreements.TermsOfService;
                    u.Disclaimer = agreements.Disclaimer;
                    u.LanguageCode = agreements.LanguageCode;
                    u.MerchantId = id.Value;

                    var update = _merchantfeatureRepository.UpdateTermsOfService(u);
                    if (update != null)
                    {
                        var userActivity = "Creates/Updates Merchants TOS and Disclaimer";

                        var errRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "AgreementsText", "");

                        Session["Success"] = "Successfully Updated.";

                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    var agreement = new SDGDAL.Entities.Agreements();
                    agreement.TermsOfService = agreements.TermsOfService;
                    agreement.Disclaimer = agreements.Disclaimer;
                    agreement.LanguageCode = agreements.LanguageCode;
                    agreement.IsCustom = false;
                    agreement.MerchantId = id.Value;

                    var a = _merchantfeatureRepository.CreateTermsOfService(agreement);

                    if (a.AgreementsId > 0)
                    {
                        var userActivity = "Creates/Updates Merchants TOS and Disclaimer";

                        var errRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "AgreementsText", "");

                        Session["Success"] = "Successfully Updated.";

                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "AgreementsText", ex.StackTrace);
            }

            return View();
        }

        public ActionResult EmailServer()
        {
            return View();
        }
        public ActionResult Receipts()
        {
            return View();
        }
        public ActionResult Tips()
        {
            return View();
        }
        public ActionResult Transactions()
        {
            return View();
        }
    }
}
