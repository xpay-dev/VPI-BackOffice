using SDGBackOffice.Models;
using SDGDAL.Entities;
using SDGDAL.Repositories;
using SDGUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SDGDAL;

namespace SDGBackOffice.Controllers
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
        MidsRepository _midsRepository;
        MobileAppRepository _mobileAppRepo;

        string action = string.Empty;

        string mailFrom = "";
        string host = "";
        string user = "";
        string pass = "";
        int port = 0;

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
            _midsRepository = new MidsRepository();
            _mobileAppRepo = new MobileAppRepository();

            mailFrom = System.Configuration.ConfigurationManager.AppSettings["mailFrom"].ToString();
            host = System.Configuration.ConfigurationManager.AppSettings["host"].ToString();
            user = System.Configuration.ConfigurationManager.AppSettings["user"].ToString();
            pass = System.Configuration.ConfigurationManager.AppSettings["password"].ToString();
            port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["port"]);
        }

        public ActionResult Index(int? id)
        {
            var userActivity = "Entered Merchant Branch POSs Index Page";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "Index", "");

            if (Session["Success"] != null)
            {
                ViewBag.Success = Session["Success"];
            }

            Session["Success"] = null;

            if (CurrentUser.ParentType == Enums.ParentType.Partner)
            {
                var resellers = _resellerRepo.GetAllResellersByPartner(CurrentUser.ParentId, "");

                var merchants = _merchantRepo.GetAllMerchantsByPartner(CurrentUser.ParentId, "");

                var merchantBranches = _branchRepo.GetAllBranchesByPartner(CurrentUser.ParentId, "");

                var ddlResellers = new List<SelectListItem>();
                var ddlMerchants = new List<SelectListItem>();
                var ddlBranches = new List<SelectListItem>();


                ddlMerchants.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = merchants.Count == 0 ? "No Merchants available" : "Select Merchant",
                    Selected = true
                });

                ddlBranches.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = merchants.Count == 0 ? "No Branches available" : "Select Branche",
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
                            Text = "Select Reseller",
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
                    Text = merchants.Count == 0 ? "No Merchants available" : "Select Merchant",
                    Selected = true
                });

                ddlBranches.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = merchants.Count == 0 ? "No Branches available" : "Select Branche",
                    Selected = true
                });

                ddlResellers.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = "Select Reseller",
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
                var merchantBranches = _branchRepo.GetAllBranchesByMerchant(CurrentUser.ParentId, "");
                var ddlBranches = new List<SelectListItem>();

                if (merchantBranches.Count > 0)
                {
                    if (merchantBranches.Count == 1)
                    {
                        ddlBranches.AddRange(merchantBranches.Select(p => new SelectListItem()
                        {
                            Value = p.MerchantBranchId.ToString(),
                            Text = p.MerchantBranchName + " (M: " + p.Merchant.MerchantName + ")"
                        }).ToList());
                    }
                    else
                    {
                        ddlBranches.Add(new SelectListItem()
                        {
                            Value = "0",
                            Text = "Select a Branch"
                        });

                        ddlBranches.AddRange(merchantBranches.Select(p => new SelectListItem()
                        {
                            Value = p.MerchantBranchId.ToString(),
                            Text = p.MerchantBranchName + " (M: " + p.Merchant.MerchantName + ")"
                        }).ToList());
                    }
                }
                else
                {
                    ddlBranches.Add(new SelectListItem()
                    {
                        Value = "0",
                        Text = "No Merchant Branches available"
                    });
                }

                ViewBag.Branches = ddlBranches;

                ViewBag.ParentId = CurrentUser.ParentId;
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

            if (CurrentUser.ParentType == Enums.ParentType.Reseller)
            {
                ViewBag.UserId = CurrentUser.ParentId;
                ViewBag.ParentType = 2;
            }
            else if (CurrentUser.ParentType == Enums.ParentType.Merchant)
            {
                ViewBag.UserId = CurrentUser.ParentId;
                ViewBag.ParenType = 3;
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
            bool isSuccess = false;
            bool doExist = false;
            bool success = false;

            try
            {
                bool isCheck = false;

                int count = 0;

                action = "creating merchant branch pos";

                string merchantName = Convert.ToString(Request["merchantName"]);
                string location = Convert.ToString(Request["location"]);
                isCheck = Convert.ToBoolean(Request["isCheck"]);

                int mId = 0;

                if (CurrentUser.ParentType == Enums.ParentType.Partner)
                {
                    mId = Convert.ToInt32(Request["merchantId"]);
                }
                else if (CurrentUser.ParentType == Enums.ParentType.Reseller)
                {
                    mId = Convert.ToInt32(CurrentUser.ParentId);
                }
                else if (CurrentUser.ParentType == Enums.ParentType.Merchant)
                {
                    mId = Convert.ToInt32(CurrentUser.ParentId);
                }
                int bId = Convert.ToInt32(Request["bId"]);

                if (bId != 0)
                {
                    string actCode = "";
                    List<MobileApp> mApps = new List<MobileApp>();
                    for (int i = 0; i < pos.Length; i++)
                    {
                        if (!_mobileAppRepo.CheckExistingActivationCode(pos[i].ActivationCode))
                        {
                            actCode += "," + pos[i].ActivationCode;
                            doExist = true;
                        }

                        if (doExist == true)
                        {
                            ModelState.AddModelError(string.Empty, "Activation Code " + actCode.Substring(1) + " is not available.");
                        }

                        var posFeatures = new SDGDAL.Entities.MobileAppFeatures();
                        count = i;
                        posFeatures.SystemMode = "LIVE";
                        posFeatures.LanguageCode = "EN-CA";
                        posFeatures.CurrencyId = 1;
                        posFeatures.GPSEnabled = false;
                        posFeatures.SMSEnabled = true;
                        posFeatures.EmailAllowed = true;
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

                    if (ModelState.IsValid)
                    {
                        success = _posRepo.CreateMerchantBranchPOSs(mApps);
                    }

                    if (success)
                    {
                        isSuccess = true;

                        if (isCheck == true)
                        {
                            SendActivation.SendActivation sendMail = new SendActivation.SendActivation();

                            string fullMsg = "";

                            var eMail = _merchantRepo.GetDetailsbyMerchantId(mId);

                            var branchPIN = _branchRepo.GetDetailsbyMerchantBranchId(bId);

                            int cId = eMail.ContactInformationId + 1;

                            int cId2 = branchPIN.ContactInformationId + 1;

                            var emailUser2 = _userRepo.GetMerchantByContactInfoId(cId2);

                            var info2 = _merchantRepo.GetMerchantId(emailUser2.UserId);

                            string mPin2 = SDGDAL.Utility.Decrypt(info2.PIN);

                            var emailUser = _userRepo.GetMerchantByContactInfoId(cId);

                            var info = _merchantRepo.GetMerchantId(emailUser.UserId);

                            string mPin = SDGDAL.Utility.Decrypt(info.PIN);

                            string userName = info.Username;

                            string password = SDGDAL.Utility.Decrypt(info.Password);

                            var path = Server.MapPath("~/img/logo_paymaya_2.jpg");

                            var path1 = Server.MapPath("~/img/logo_paymaya_3.jpg");

                            var mids = _midsRepository.GetAllMidsByMerchantId(mId);

                            var merchantId = mids.FirstOrDefault(p => p.Param_2 != null);

                            if (mids.Count > 1)
                            {
                                merchantId = mids.Where(c => c.CardTypeId == 1).FirstOrDefault(p => p.Param_2 != null);
                            }

                            string merchId = merchantId == null ? "None" : merchantId.Param_2;

                            string header = "<b>Dear</b> " + merchantName + ",<br />"
                                        + "Welcome to PayMaya Swipe and thank you for choosing us!<br />"
                                        + "Your Paymaya Swipe device has already been shipped and us now on its way to you. Please<br />"
                                        + "expect delivery to arrive in 5-7 business days.<br /><br />";


                            if (count == 0)
                            {
                                string message = "<b><h4>" + "Two easy steps to get Started!" + "</h4></b><br />"
                                    + "1. Download the app from Google Play. Just look for \"Paymaya Swipe\"" + "<br />"
                                    + "2. Key in your Activation Code and PIN Code.<br />"
                                    + "&nbsp;&nbsp;&nbsp;&nbsp;" + "POS Name: " + pos[0].POSName + "<br />"
                                    + "&nbsp;&nbsp;&nbsp;&nbsp;" + "Activation Code: " + "<b>" + pos[0].ActivationCode + "</b><br />"
                                    + "&nbsp;&nbsp;&nbsp;&nbsp;" + "Pin: " + mPin2 + "<br />"
                                    + "3. Start accepting credit card payments from anyone, anywhere!<br />";

                                fullMsg = header + message;

                                sendMail.SendActivationCode(mailFrom, port, host, user, pass, userName, password, merchantName, fullMsg, eMail.MerchantEmail, path, path1);
                                sendMail.SendActivationCodeBcc(mailFrom, port, host, user, pass, userName, password, merchantName, fullMsg, eMail.MerchantEmail, path, path1);
                            }
                            else
                            {
                                string message = "<b><h4>" + "Two easy steps to get Started!" + "</h4></b><br />"
                                    + "1. Download the app from Google Play. Just look for \"Paymaya Swipe\"" + "<br />"
                                    + "2. Key in your Activation Code and PIN Code.<br /><br /><br />";

                                for (int i = 0; i < count + 1; i++)
                                {
                                    message += "&nbsp;&nbsp;&nbsp;&nbsp;" + "POS Name: " + pos[i].POSName + "<br />"
                                    + "&nbsp;&nbsp;&nbsp;&nbsp;" + "Activation Code: " + "<b>" + pos[i].ActivationCode + "</b><br />"
                                    + "&nbsp;&nbsp;&nbsp;&nbsp;" + "Pin: " + mPin2 + "<br /><br />";
                                }

                                message += "3. Start accepting credit card payments from anyone, anywhere!<br />";

                                fullMsg = header + message;

                                sendMail.SendActivationCode(mailFrom, port, host, user, pass, userName, password, merchantName, fullMsg, eMail.MerchantEmail, path, path1);
                                sendMail.SendActivationCodeBcc(mailFrom, port, host, user, pass, userName, password, merchantName, fullMsg, eMail.MerchantEmail, path, path1);
                            }
                        }

                        var userActivity = "Created Merchant Branch POSs";

                        var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "Create", "");

                        Session["Success"] = "Merchant POS successfully created.";

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
                if (isSuccess == true)
                {
                    Session["Success"] = "Something went wrong while sending the email. Please contact support. Your Merchant POS has been successfully created.";

                    return RedirectToAction("Index");
                }

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

        public ActionResult ViewInfo(/*string*/int id)
        {
            var userActivity = "Entered Merchant Branch POSs Info Page";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "ViewInfo", "");

            var merchantpos = _posRepo.GetDetailsbyMerchantPOSId(/*Convert.ToInt32(Utility.Decrypt(id.Contains(" ") == true ? id.Replace(" ", "+") : id))*/id);

            MerchantPOSModel pos = new MerchantPOSModel();
            string status = "";
            bool isActive = merchantpos.IsActive;
            if (isActive == true)
            {
                status = "Activated";
            }
            else
            {
                status = "Deactivated";
            }

            pos.POSName = merchantpos.MerchantPOSName;
            pos.POSStatus = status;
            pos.BranchName = merchantpos.MerchantBranch.MerchantBranchName;

            return View(pos);
        }

        public ActionResult UpdateInfo(string id)
        {
            var userActivity = "Entered Merchant Branch POSs Update Info Page";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "UpdateInfo", "");

            var merchantpos = _posRepo.GetDetailsbyMerchantPOSId(Convert.ToInt32(Utility.Decrypt(id.Contains(" ") == true ? id.Replace(" ", "+") : id)));

            MerchantPOSModel pos = new MerchantPOSModel();
            pos.POSId = merchantpos.MerchantPOSId;
            pos.POSName = merchantpos.MerchantPOSName;

            return View(pos);
        }

        [HttpPost]
        public ActionResult UpdateInfo(MerchantPOSModel pos)
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

            return View();
        }

        [HttpPost]
        public ActionResult SendActivationCode(string posId, string actCode, int? mId)
        {
            bool isActivated = false;

            string fullMsg = "";

            SendActivation.SendActivation sendMail = new SendActivation.SendActivation();

            var userActivity = "Entered Merchant Branch POSs Info Page";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "ViewInfo", "");

            var merchantpos = _posRepo.GetDetailsbyMerchantPOSId(Convert.ToInt32(Utility.Decrypt(posId.Contains(" ") == true ? posId.Replace(" ", "+") : posId)));

            if ((mId == 0) || (!mId.HasValue))
            {
                mId = CurrentUser.ParentId;
            }

            var eMail = _merchantRepo.GetDetailsbyMerchantId(mId.Value);

            int cId = eMail.ContactInformationId + 1;

            var emailUser = _userRepo.GetMerchantByContactInfoId(cId);

            var info = _merchantRepo.GetMerchantId(emailUser.UserId);

            string mPin = SDGDAL.Utility.Decrypt(info.PIN);

            string userName = info.Username;

            string password = SDGDAL.Utility.Decrypt(info.Password);

            var path = Server.MapPath("~/img/logo_paymaya_2.jpg");

            var path1 = Server.MapPath("~/img/logo_paymaya_3.jpg");

            var mids = _midsRepository.GetAllMidsByMerchantId(mId.Value);

            var merchantId = mids.FirstOrDefault(p => p.Param_2 != null);

            if (mids.Count > 1)
            {
                merchantId = mids.Where(c => c.CardTypeId == 1).FirstOrDefault(p => p.Param_2 != null);
            }

            string merchId = merchantId == null ? "None" : merchantId.Param_2;



            MerchantPOSModel pos = new MerchantPOSModel();
            isActivated = merchantpos.MobileApp.Last().DateActivated.HasValue;

            if (isActivated == false)
            {

                string header = "<b>Dear</b> " + eMail.MerchantName + ",<br />"
                                        + "Welcome to PayMaya Swipe and thank you for choosing us!<br />"
                                        + "Your Paymaya Swipe device has already been shipped and us now on its way to you. Please<br />"
                                        + "expect delivery to arrive in 5-7 business days.<br /><br />";

                string message = "<b><h4>" + "Get Started!" + "</h4></b><br />"
                                    + "1. Download the app from Google Play. Just look for \"Paymaya Swipe\"" + "<br />"
                                    + "2. Key in your Activation Code and PIN Code. <br />"
                                    + "&nbsp;&nbsp;&nbsp;&nbsp;" + "POS Name: " + merchantpos.MerchantPOSName + "<br />"
                                    + "&nbsp;&nbsp;&nbsp;&nbsp;" + "Activation Code: " + "<b>" + actCode + "</b><br />"
                                    + "&nbsp;&nbsp;&nbsp;&nbsp;" + "Pin: " + mPin + "<br />"
                                    + "3. Start accepting credit card payments from anyone, anywhere!<br />";


                fullMsg = header + message;

                try
                {
                    sendMail.SendActivationCode(mailFrom, port, host, user, pass, userName, password, eMail.MerchantName, fullMsg, eMail.MerchantEmail, path, path1);
                    //sendMail.SendActivationCodeBcc(userName, password, eMail.MerchantName, fullMsg, eMail.MerchantEmail, path, path1);
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

        [HttpPost]
        public JsonResult UpdateMerchantBranchPOSStatus(string pId, bool status)
        {
            MerchantBranchPOS mbp = new MerchantBranchPOS();

            var branchPOS = _posRepo.GetDetailsbyMerchantPOSId(Convert.ToInt32(Utility.Decrypt(pId.Contains(" ") == true ? pId.Replace(" ", "+") : pId)));

            mbp.MerchantPOSId = branchPOS.MerchantPOSId;
            mbp.MerchantPOSName = branchPOS.MerchantPOSName;
            mbp.IsActive = status;
            mbp.IsDeleted = branchPOS.IsDeleted;
            mbp.DateCreated = branchPOS.DateCreated;
            mbp.MerchantBranchId = branchPOS.MerchantBranchId;

            var update = _posRepo.UpdateMerchantBranchPOSStatus(mbp);

            return Json(update);
        }
    }
}
