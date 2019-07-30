using SDGAdmin.Models;
using SDGDAL.Entities;
using SDGDAL.Repositories;
using SDGUtil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mail;
using System.Web.Mvc;
using GatewayProcessor;
using CTPaymentUtilities;
using SDGDAL;

namespace SDGAdmin.Controllers
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
        AccountRepository _accountRepository;
        MidsRepository _midsRepository;
        MobileAppRepository _mobileAppRepo;
        MidsRepository _midsRepo;
        GatewayProcessor.Gateways _gateway;
        TransactionRepository _transRepo;

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
            _accountRepository = new AccountRepository();
            _midsRepository = new MidsRepository();
            _mobileAppRepo = new MobileAppRepository();
            _midsRepo = new MidsRepository();
            _gateway = new GatewayProcessor.Gateways();
            _transRepo = new TransactionRepository();

            mailFrom = System.Configuration.ConfigurationManager.AppSettings["mailFrom"].ToString();
            host = System.Configuration.ConfigurationManager.AppSettings["host"].ToString();
            user = System.Configuration.ConfigurationManager.AppSettings["user"].ToString();
            pass = System.Configuration.ConfigurationManager.AppSettings["password"].ToString();
            port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["port"]);
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
            if (CurrentUser.ParentType == Enums.ParentType.Partner)
            {
                var userActivity = "Entered Merchant Branch POSs Index Page";

                var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "Index", "");

                if (Session["Success"] != null)
                {
                    ViewBag.Success = Session["Success"];
                }

                Session["Success"] = null;

                GetPartner();
            }

            return View();
        }

        public ActionResult Create(string id)
        {
            var userActivity = "Entered Create Merchant Branch POSs Page";
            int branchId = 0;
            var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "Create", "");

            GetPartner();

            if (id != "0")
            {
                branchId = !string.IsNullOrEmpty(id) ? Convert.ToInt32(Utility.Decrypt(id.Contains(" ") == true ? id.Replace(" ", "+") : id)) : 0;
                TempData["BranchId"] = branchId;
            }

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
        public ActionResult Create(MerchantPOSModel[] pos)
        {
            bool isSuccess = false;
            bool doExist = false;

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

                    bool success = _posRepo.CreateMerchantBranchPOSs(mApps);

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

                            string header = "<b>Dear</b> " + merchantName + "<br />"
                                        + "Welcome and thank you for choosing Paymaya!<br />"
                                        + "Your Paymaya Swipe device has already been shipped and us now on its way to you. Please<br />"
                                        + "expect delivery to arrive in 5-7 business days.<br /><br />"
                                        + "Here is your account information:<br />"
                                        + "Merchant ID:" + "<b>" + merchId + "</b><br />";


                            if (count == 0)
                            {
                                string message = "<b><h4>" + "Get Started!" + "</h4></b><br />"
                                    + "1. Download the app from Google Play. Just look for \"Paymaya Swipe\"" + "<br />"
                                    + "2. Key in your Activation Code and PIN Code.<br /><br /><br />"
                                    + "&nbsp;&nbsp;&nbsp;&nbsp;" + "POS Name: " + pos[0].POSName + "<br />"
                                    + "&nbsp;&nbsp;&nbsp;&nbsp;" + "Activation Code: " + "<b>" + pos[0].ActivationCode + "</b><br />"
                                    + "&nbsp;&nbsp;&nbsp;&nbsp;" + "Pin: " + mPin + "<br /><br />"
                                    + "3. Start accepting credit card payments from anyone, anywhere!<br />";

                                fullMsg = header + message;

                                sendMail.SendActivationCode(mailFrom, port, host, user, pass, userName, password, merchantName, fullMsg, eMail.MerchantEmail, path, path1);
                                sendMail.SendActivationCodeBcc(mailFrom, port, host, user, pass, userName, password, merchantName, fullMsg, eMail.MerchantEmail, path, path1);
                            }
                            else
                            {
                                string message = "<b><h4>" + "Get Started!" + "</h4></b><br />"
                                    + "1. Download the app from Google Play. Just look for \"Paymaya Swipe\"" + "<br />"
                                    + "2. Key in your Activation Code and PIN Code.<br />";

                                for (int i = 0; i < count + 1; i++)
                                {
                                    message += "&nbsp;&nbsp;&nbsp;&nbsp;" + "POS Name: " + pos[i].POSName + "<br />"
                                    + "&nbsp;&nbsp;&nbsp;&nbsp;" + "Activation Code: " + "<b>" + pos[i].ActivationCode + "</b><br />"
                                    + "&nbsp;&nbsp;&nbsp;&nbsp;" + "Pin: " + mPin + "<br /><br />";
                                }

                                message += "3. Start accepting credit card payments from anyone, anywhere!<br />";

                                fullMsg = header + message;

                                sendMail.SendActivationCode(mailFrom, port, host, user, pass, userName, password, merchantName, fullMsg, eMail.MerchantEmail, path, path1);
                                sendMail.SendActivationCodeBcc(mailFrom, port, host, user, pass, userName, password, merchantName, fullMsg, eMail.MerchantEmail, path, path1);
                            }
                        }

                        var userActivity = "Created Merchant Branch POSs";

                        var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "Create", "");

                        Session["Success"] = "Merchant POS(s) successfully created.";

                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    ViewBag.Alert = "Please select a Branch";
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

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "Create", ex.StackTrace);

                ErrorLog err = new ErrorLog();
                err.Action = "POSs Creation";
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

        public ActionResult ViewInfo(string id)
        {
            var userActivity = "Entered Merchant Branch POSs Info Page";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "ViewInfo", "");

            var merchantpos = _posRepo.GetDetailsbyMerchantPOSId(Convert.ToInt32(Utility.Decrypt(id.Contains(" ") == true ? id.Replace(" ", "+") : id)));

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
            if (CurrentUser.ParentType == Enums.ParentType.Partner)
            {
                var userActivity = "Entered Merchant Branch POSs Update Info Page";

                var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "UpdateInfo", "");

                var merchantpos = _posRepo.GetDetailsbyMerchantPOSId(Convert.ToInt32(Utility.Decrypt(id.Contains(" ") == true ? id.Replace(" ", "+") : id)));

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

                        var errRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "UpdateInfo", "");

                        Session["Success"] = "Successfully Updated.";

                        return RedirectToAction("Index");
                    }
                }
                catch (Exception ex)
                {
                    var errorOnAction = "Error while " + action;

                    var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "UpdateInfo", ex.StackTrace);

                    ErrorLog err = new ErrorLog();
                    err.Action = "POS Update Info";
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

        #region Old Send Activation Code
        //[HttpPost]
        //public ActionResult SendActivationCode(int posId, string actCode)
        //{
        //    bool isActivated = false;

        //    SendActivation.SendActivation sendMail = new SendActivation.SendActivation();

        //    var userActivity = "Entered Merchant Branch POSs Info Page";

        //    var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "ViewInfo", "");

        //    var merchantpos = _posRepo.GetDetailsbyMerchantPOSId(posId);

        //    MerchantPOSModel pos = new MerchantPOSModel();
        //    isActivated = merchantpos.MobileApp.Last().DateActivated.HasValue;

        //    if (isActivated == false)
        //    {
        //        String message = "<h4>" + "Activation Code Information:" + "</h4><br />"
        //                        + "Merchant Name: " + "<b>" + merchantpos.MerchantBranch.Merchant.MerchantName + "</b><br />"
        //                        + "Location: " + "<b>" + merchantpos.MerchantBranch.MerchantBranchName + "</b><br />"
        //                        + "POS Name: " + "<b>" + merchantpos.MerchantPOSName + "</b><br />"
        //                        + "Activation Code: " + "<b>" + actCode + "</b><br />";

        //        try
        //        {
        //            sendMail.SendActivationCode(mailFrom, host, user, pass, message, merchantpos.MerchantBranch.Merchant.MerchantEmail);
        //        }
        //        catch (Exception ex)
        //        {
        //            return Json(ex);
        //        }

        //        return Json("success");
        //    }
        //    else
        //    {
        //        return Json("activated");
        //    }
        //}
        #endregion

        [HttpPost]
        public ActionResult SendActivationCode(string posId, string actCode, int? mId)
        {
            bool isActivated = false;

            string fullMsg = "";

            SendActivation.SendActivation sendMail = new SendActivation.SendActivation();

            var userActivity = "Entered Merchant Branch POSs Info Page";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "ViewInfo", "");

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

        public ActionResult CreatePosTransaction(string id)
        {
            TransactionModel model = new TransactionModel();
            

            if (id != string.Empty)
            {
                action = "fetching the users list";

                #region User DDL
                var mAppDetails = _mobileAppRepo.GetMobileAppDetailsByPosId(Convert.ToInt32(Utility.Decrypt(id.Contains(" ") == true ? id.Replace(" ", "+") : id)));

                var users = _userRepo.GetUsersbyMerchantIdAndBranchId(mAppDetails.MerchantBranchPOS.MerchantBranch.MerchantId, mAppDetails.MerchantBranchPOS.MerchantBranchId);

                var ddlUsers = new List<SelectListItem>();

                ddlUsers.AddRange(users.Select(u => new SelectListItem()
                {
                    Value = u.AccountId.ToString(),
                    Text = u.User.FirstName + " " + u.User.LastName
                }).ToList());

                ViewBag.Users = ddlUsers;
                #endregion

                #region Card Types
                var cardTypes = _refRepo.GetAllCardTypes();

                ViewBag.CardTypes = cardTypes.Select(c => new SelectListItem()
                {
                    Value = c.CardTypeId.ToString(),
                    Text = c.TypeName
                }).ToList();

                #endregion

                model.AccountId = user[0];
                model.MerchantPosId = Convert.ToInt32(Utility.Decrypt(id.Contains(" ") == true ? id.Replace(" ", "+") : id));
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult CreatePosTransaction(TransactionModel transaction)
        {
            TransactionModel model = new TransactionModel();
            List<TransactionModel> transDetail = new List<TransactionModel>();
            RedirectInput newSessIn = new RedirectInput();

            string purchaseURL = String.Empty;

            try
            {
                var mAppDetails = _mobileAppRepo.GetMobileAppDetailsByPosId(Convert.ToInt32(transaction.MerchantPosId));

                var users = _userRepo.GetUsersbyMerchantIdAndBranchId(mAppDetails.MerchantBranchPOS.MerchantBranch.MerchantId, mAppDetails.MerchantBranchPOS.MerchantBranchId);

                var ddlUsers = new List<SelectListItem>();

                ddlUsers.AddRange(users.Select(u => new SelectListItem()
                {
                    Value = u.AccountId.ToString(),
                    Text = u.User.FirstName + " " + u.User.LastName,
                    Selected = u.AccountId == transaction.AccountId
                }).ToList());

                ViewBag.Users = ddlUsers;

                var cardTypes = _refRepo.GetAllCardTypes();

                ViewBag.CardTypes = cardTypes.Select(c => new SelectListItem()
                {
                    Value = c.CardTypeId.ToString(),
                    Text = c.TypeName,
                    Selected = c.CardTypeId == transaction.CardTypeId
                }).ToList();

                model.AccountId = transaction.AccountId;
                model.Amount = transaction.Amount;
                model.MobileAppId = mAppDetails.MobileAppId;
                model.CardTypeId = transaction.CardTypeId;

                string amount;
                decimal finalAmount = Convert.ToDecimal(transaction.Amount) * 100;

                try
                {
                    amount = finalAmount.ToString().Remove(finalAmount.ToString().IndexOf('.'));
                }
                catch
                {
                    amount = finalAmount.ToString();
                }

                #region Checking Mids

                action = "checking mid status before setting up transaction entry.";
                var mid = new SDGDAL.Entities.Mid();

                mid = _midsRepo.GetMidByPosIdAndCardTypeId(transaction.MerchantPosId, transaction.CardTypeId);

                if (mid == null)
                {
                    ModelState.AddModelError(string.Empty, "Mid not found.");
                }
                else
                {
                    if (!mid.IsActive || mid.IsDeleted)
                    {
                        ModelState.AddModelError(string.Empty, "Mid is Inactive.");
                    }

                    if (!mid.Switch.IsActive)
                    {
                        ModelState.AddModelError(string.Empty, "Switch is Inactive.");
                    }
                }
                #endregion

                #region Checking if CTPayment Switch

                action = "checking if switch is active. ";
                if (!mid.Switch.IsActive)
                {
                    ModelState.AddModelError(string.Empty, "Switch is not active.");
                }

                #region CTPayment Live
                if (mid.Switch.SwitchCode == "CTPayment")
                {
                    purchaseURL = System.Configuration.ConfigurationManager.AppSettings["CTPaymentPurchaseURL"].ToString();

                    newSessIn.companyNumberField = "00326"; ///TODO: should be dynamic
                    newSessIn.merchantNumberField = mid.Param_2;
                    newSessIn.customerNumberField = "00000000";
                    newSessIn.amountField = amount.PadLeft(11, '0');
                    newSessIn.billNumberField = (String.Format("{0:d9}", (DateTime.Now.Ticks / 10) % 1000000000)).PadLeft(12, '0');
                    newSessIn.originalBillNumberField = (String.Format("{0:d9}", (DateTime.Now.Ticks / 10) % 1000000000)).PadLeft(12, '0');
                    newSessIn.inputTypeField = "I";
                    newSessIn.merchantTerminalNumberField = "BBPOS002"; //BBPOS002 //mid.Param_6
                    newSessIn.languageCodeField = "E";
                    newSessIn.currencyCodeField = mid.Currency.CurrencyCode;
                    newSessIn.operatorIDField = "00000000";
                    newSessIn.eMailField = users[0].User.EmailAddress;
                    newSessIn.successURLField = System.Configuration.ConfigurationManager.AppSettings["CTPaymentSuccessURL"].ToString();
                    newSessIn.failureURLField = System.Configuration.ConfigurationManager.AppSettings["CTPaymentFailedURL"].ToString(); ;
                }
                #endregion

                #region CTPayment Offline
                else if (mid.Switch.SwitchCode == "CTPaymentDemo")
                {
                    purchaseURL = System.Configuration.ConfigurationManager.AppSettings["CTPaymentPurchaseDemoURL"].ToString();

                    newSessIn.companyNumberField = "00326";
                    newSessIn.merchantNumberField = mid.Param_2;
                    newSessIn.customerNumberField = "00000000";
                    newSessIn.amountField = amount.PadLeft(11, '0');
                    newSessIn.billNumberField = (String.Format("{0:d9}", (DateTime.Now.Ticks / 10) % 1000000000)).PadLeft(12, '0');
                    newSessIn.originalBillNumberField = (String.Format("{0:d9}", (DateTime.Now.Ticks / 10) % 1000000000)).PadLeft(12, '0');
                    newSessIn.inputTypeField = "I";
                    newSessIn.merchantTerminalNumberField = "     ";
                    newSessIn.languageCodeField = "E";
                    newSessIn.currencyCodeField = "CAD";
                    newSessIn.operatorIDField = "00000000";
                    newSessIn.eMailField = users[0].User.EmailAddress;
                    newSessIn.successURLField = System.Configuration.ConfigurationManager.AppSettings["CTPaymentSuccessURL"].ToString();
                    newSessIn.failureURLField = System.Configuration.ConfigurationManager.AppSettings["CTPaymentFailedURL"].ToString(); ;
                }
                #endregion

                else
                {
                    ModelState.AddModelError(string.Empty, "Transaction failed. Switch not yet supported.");
                }
                #endregion

                if (ModelState.IsValid && transaction != null)
                {
                    var transOutput = _gateway.ProcessCTPaymentPurchase(newSessIn);

                    if (transOutput.Result.secureIDField != null)
                    {
                        TempData["SecureID"] = transOutput.Result.secureIDField;
                        Process.Start(String.Format(purchaseURL + "?SecureID={0}&SecureTYPE=GET", transOutput.Result.secureIDField));

                        transDetail.Add(new TransactionModel()
                        {
                            AccountId = transaction.AccountId,
                            Amount = transaction.Amount,
                            Currency = mid.Currency.CurrencyCode,
                            MerchantPosId = transaction.MerchantPosId,
                            MobileAppId = mAppDetails.MobileAppId,
                            MidId = mid.MidId,
                            TransactionChargesId = mid.TransactionChargesId
                        });

                        Session["CTPaymentTransaction"] = transDetail;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Transaction cannot completed.");
                    }
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "CreatePosTransaction", ex.StackTrace);

                ErrorLog err = new ErrorLog();
                err.Action = "Create POS Transaction";
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

            return View(model);
        }

        public ActionResult SuccessTransaction()
        {
            try
            {
                string track1 = "";
                string track2 = "";
                string track3 = "";

                action = "processing session data.";
                List<TransactionModel> objTransactionModel = (List<TransactionModel>)Session["CTPaymentTransaction"];

                action = "processing ct payment ecom transaction.";
                var response = _gateway.ProcessCTPaymentTransaction(TempData["SecureID"].ToString());

                if (response.Result != null)
                {
                    var resConfirm = _gateway.AcceptCTAcknowledge(response.Result.trxNumberField);

                    if (resConfirm.Result.returnCodeField == "true")
                    {
                        #region Transaction

                        var transaction = new SDGDAL.Entities.Transaction();
                        var transactionAttempt = new SDGDAL.Entities.TransactionAttempt();

                        action = "setting up transaction and transactionattempt details.";

                        int cardTypeId = SDGUtil.Functions.GetCTCardType(response.Result.cardTypeField);

                        if (cardTypeId != 0)
                        {
                            transaction.CardTypeId = cardTypeId;
                        }

                        transaction.CardNumber = response.Result.cardNumberField;
                        transaction.NameOnCard = String.Empty;
                        transaction.ExpMonth = response.Result.expirationDateField.Substring(2, 2);
                        transaction.ExpYear = response.Result.expirationDateField.Substring(2);
                        transaction.CSC = "";
                        transaction.OriginalAmount = Convert.ToDecimal(objTransactionModel[0].Amount);
                        transaction.TaxAmount = 0;

                        ///TODO: Tax should compute? 
                        transaction.FinalAmount = Convert.ToDecimal(objTransactionModel[0].Amount);

                        try
                        {
                            transaction.CurrencyId = _transRepo.GetCurrencyIdByCurrencyName(objTransactionModel[0].Currency);
                        }
                        catch
                        {
                            //invalid Currency
                        }

                        transaction.Track1 = track1;
                        transaction.Track2 = track2;
                        transaction.Track3 = track3;

                        transaction.DateCreated = DateTime.Now;
                        transaction.RefNumApp = String.Empty;
                        transaction.RefNumSales = response.Result.referenceNumberField;
                        transaction.Notes = ((SDGDAL.Enums.CardTypes)transaction.CardTypeId).ToString() + " Ecom Purchase";
                        transaction.TransactionEntryTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionEntryType.CreditManual);
                        transaction.MerchantPOSId = objTransactionModel[0].MerchantPosId;

                        transactionAttempt.TransNumber = response.Result.trxNumberField;
                        transactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Sale);
                        transactionAttempt.AccountId = objTransactionModel[0].AccountId;
                        transactionAttempt.MobileAppId = objTransactionModel[0].MobileAppId;
                        transactionAttempt.DateSent = DateTime.Now;
                        transactionAttempt.DateReceived = DateTime.Now;
                        transactionAttempt.DepositDate = DateTime.Now;
                        transactionAttempt.DeviceId = 1;
                        transactionAttempt.GPSLat = 0;
                        transactionAttempt.GPSLong = 0;
                        transactionAttempt.Notes = "API CTPayment " + ((SDGDAL.Enums.CardTypes)transaction.CardTypeId).ToString() + " Purchase Approved";

                        #region Handle Transaction response

                        action = "setting up transaction entry for database.";
                        try
                        {
                            action = "trying to encrypt card data.";

                            #region Encrypt Card Data
                            //ENCRYPT CARD DATA
                            string NE_CARD = transaction.CardNumber;
                            string NE_EMONTH = transaction.ExpMonth;
                            string NE_EYEAR = transaction.ExpYear;
                            string NE_CSC = transaction.CSC;
                            string E_CARD;
                            string E_EMONTH, E_EYEAR;
                            string E_CSC;
                            byte[] desKey;
                            byte[] desIV;

                            //card number masking
                            string s = NE_CARD.Substring(NE_CARD.Length - 4);
                            string r = new string('*', NE_CARD.Length);
                            string MASK_CARD = r + s;
                            //month masking
                            string MASK_EMONTH = new string('*', NE_EMONTH.Length);
                            //year masking
                            string MASK_EYEAR = new string('*', NE_EYEAR.Length);
                            //CSC masking
                            string MASK_CSC = new string('*', NE_CSC.Length);

                            E_CARD = Utility.GenerateSymmetricKeyAndEcryptData(MASK_CARD, out desKey, out desIV);

                            transaction.Key = new SDGDAL.Entities._Key();
                            transaction.Key.Key = Convert.ToBase64String(desKey);
                            transaction.Key.IV = Convert.ToBase64String(desIV);

                            E_EMONTH = Utility.EncryptDataWithExistingKey(NE_EMONTH, desKey, desIV);
                            E_EYEAR = Utility.EncryptDataWithExistingKey(NE_EYEAR, desKey, desIV);
                            E_CSC = Utility.EncryptDataWithExistingKey(NE_CSC, desKey, desIV);
                            #endregion

                            transactionAttempt.TransactionChargesId = objTransactionModel[0].TransactionChargesId;
                            transactionAttempt.Amount = transaction.OriginalAmount;
                            transactionAttempt.AuthNumber = response.Result.authorizationNumberField;
                            transactionAttempt.BatchNumber = response.Result.batchNumberField;
                            transactionAttempt.SeqNumber = response.Result.sequenceNumberField;
                            transactionAttempt.ReturnCode = response.Result.returnCodeField.Replace(" ", "");
                            transactionAttempt.DisplayReceipt = response.Result.receiptDispField;
                            transactionAttempt.DisplayTerminal = response.Result.terminalDispField;

                            action = "saving transaction details to database.";
                            var nTransaction = new SDGDAL.Entities.Transaction();
                            nTransaction.CopyProperties(transaction);

                            nTransaction.CardNumber = E_CARD;
                            nTransaction.ExpMonth = E_EMONTH;
                            nTransaction.ExpYear = E_EYEAR;
                            nTransaction.CSC = E_CSC;
                            nTransaction.CurrencyId = transaction.CurrencyId;
                            nTransaction.MidId = objTransactionModel[0].MidId;
                            nTransaction.Track1 = "";
                            nTransaction.Track2 = "";
                            nTransaction.Track3 = "";

                            var rTransaction = _transRepo.CreateTransaction(nTransaction, transactionAttempt);

                            if (rTransaction != null)
                            {
                                Session["Success"] = "Transaction successfully created.";
                                Session["Error"] = String.Empty;
                            }
                        }
                        catch (Exception ex)
                        {
                            var errorOnAction = "Error while " + action;

                            var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "SuccessTransaction", ex.StackTrace);

                            ErrorLog err = new ErrorLog();
                            err.Action = "Success Transaction";
                            err.Method = "OnLoad";
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
                        #endregion

                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "SuccessTransaction", ex.StackTrace);

                ErrorLog err = new ErrorLog();
                err.Action = "CT Ecom Success Transaction";
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

        public ActionResult FailedTransaction()
        {
            TransactionModel model = new TransactionModel();
            try
            {
                string track1 = "";
                string track2 = "";
                string track3 = "";

                action = "processing session data.";
                List<TransactionModel> objTransactionModel = (List<TransactionModel>)Session["CTPaymentTransaction"];

                action = "processing ct payment ecom transaction.";
                var response = _gateway.ProcessCTPaymentTransaction(TempData["SecureID"].ToString());

                if (response.Result != null)
                {
                    model.ErrNumber = response.Result.returnCodeField;
                    model.ErrMessage = response.Result.receiptDispField + "-" + response.Result.terminalDispField;

                    var resConfirm = _gateway.AcceptCTAcknowledge(response.Result.trxNumberField);

                    if (resConfirm.Result.returnCodeField == "true")
                    {
                        #region Transaction

                        var transaction = new SDGDAL.Entities.Transaction();
                        var transactionAttempt = new SDGDAL.Entities.TransactionAttempt();

                        action = "setting up transaction and transactionattempt details.";

                        int cardTypeId = SDGUtil.Functions.GetCTCardType(response.Result.cardTypeField);

                        if (cardTypeId != 0)
                        {
                            transaction.CardTypeId = cardTypeId;
                        }

                        transaction.CardNumber = response.Result.cardNumberField;
                        transaction.NameOnCard = String.Empty;
                        transaction.ExpMonth = response.Result.expirationDateField.Substring(2, 2);
                        transaction.ExpYear = response.Result.expirationDateField.Substring(2);
                        transaction.CSC = "";
                        transaction.OriginalAmount = Convert.ToDecimal(objTransactionModel[0].Amount);
                        transaction.TaxAmount = 0;

                        ///TODO: Tax should compute? 
                        transaction.FinalAmount = Convert.ToDecimal(objTransactionModel[0].Amount);

                        try
                        {
                            transaction.CurrencyId = _transRepo.GetCurrencyIdByCurrencyName(objTransactionModel[0].Currency);
                        }
                        catch
                        {
                            //invalid Currency
                        }

                        transaction.Track1 = track1;
                        transaction.Track2 = track2;
                        transaction.Track3 = track3;

                        transaction.DateCreated = DateTime.Now;
                        transaction.RefNumApp = String.Empty;
                        transaction.RefNumSales = response.Result.referenceNumberField;
                        transaction.Notes = ((SDGDAL.Enums.CardTypes)transaction.CardTypeId).ToString() + " Ecom Purchase";
                        transaction.TransactionEntryTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionEntryType.CreditManual);
                        transaction.MerchantPOSId = objTransactionModel[0].MerchantPosId;

                        transactionAttempt.TransactionTypeId = Convert.ToInt32(SDGDAL.Enums.TransactionType.Declined);
                        transactionAttempt.AccountId = objTransactionModel[0].AccountId;
                        transactionAttempt.MobileAppId = objTransactionModel[0].MobileAppId;
                        transactionAttempt.DateSent = DateTime.Now;
                        transactionAttempt.DateReceived = DateTime.Now;
                        transactionAttempt.DepositDate = DateTime.Now;
                        transactionAttempt.DeviceId = 1;
                        transactionAttempt.GPSLat = 0;
                        transactionAttempt.GPSLong = 0;
                        transactionAttempt.Notes = "API CTPayment " + ((SDGDAL.Enums.CardTypes)transaction.CardTypeId).ToString() + " Purchase Declined";

                        #region Handle Transaction response

                        action = "setting up transaction entry for database.";
                        try
                        {
                            action = "trying to encrypt card data.";

                            #region Encrypt Card Data
                            //ENCRYPT CARD DATA
                            string NE_CARD = transaction.CardNumber;
                            string NE_EMONTH = transaction.ExpMonth;
                            string NE_EYEAR = transaction.ExpYear;
                            string NE_CSC = transaction.CSC;
                            string E_CARD;
                            string E_EMONTH, E_EYEAR;
                            string E_CSC;
                            byte[] desKey;
                            byte[] desIV;

                            //card number masking
                            string s = NE_CARD.Substring(NE_CARD.Length - 4);
                            string r = new string('*', NE_CARD.Length);
                            string MASK_CARD = r + s;
                            //month masking
                            string MASK_EMONTH = new string('*', NE_EMONTH.Length);
                            //year masking
                            string MASK_EYEAR = new string('*', NE_EYEAR.Length);
                            //CSC masking
                            string MASK_CSC = new string('*', NE_CSC.Length);

                            E_CARD = Utility.GenerateSymmetricKeyAndEcryptData(MASK_CARD, out desKey, out desIV);

                            transaction.Key = new SDGDAL.Entities._Key();
                            transaction.Key.Key = Convert.ToBase64String(desKey);
                            transaction.Key.IV = Convert.ToBase64String(desIV);

                            E_EMONTH = Utility.EncryptDataWithExistingKey(NE_EMONTH, desKey, desIV);
                            E_EYEAR = Utility.EncryptDataWithExistingKey(NE_EYEAR, desKey, desIV);
                            E_CSC = Utility.EncryptDataWithExistingKey(NE_CSC, desKey, desIV);
                            #endregion

                            transactionAttempt.TransNumber = response.Result.trxNumberField;
                            transactionAttempt.TransactionChargesId = objTransactionModel[0].TransactionChargesId;
                            transactionAttempt.Amount = transaction.OriginalAmount;
                            transactionAttempt.AuthNumber = response.Result.authorizationNumberField;
                            transactionAttempt.BatchNumber = response.Result.batchNumberField;
                            transactionAttempt.SeqNumber = response.Result.sequenceNumberField;
                            transactionAttempt.ReturnCode = response.Result.returnCodeField;
                            transactionAttempt.DisplayReceipt = response.Result.receiptDispField;
                            transactionAttempt.DisplayTerminal = response.Result.terminalDispField;

                            action = "saving transaction details to database.";
                            var nTransaction = new SDGDAL.Entities.Transaction();
                            nTransaction.CopyProperties(transaction);

                            nTransaction.CardNumber = E_CARD;
                            nTransaction.ExpMonth = E_EMONTH;
                            nTransaction.ExpYear = E_EYEAR;
                            nTransaction.CSC = E_CSC;
                            nTransaction.CurrencyId = transaction.CurrencyId;
                            nTransaction.MidId = objTransactionModel[0].MidId;
                            nTransaction.Track1 = "";
                            nTransaction.Track2 = "";
                            nTransaction.Track3 = "";

                            var rTransaction = _transRepo.CreateTransaction(nTransaction, transactionAttempt);

                            if (rTransaction != null)
                            {
                                Session["Success"] = String.Empty;
                            }
                        }
                        catch (Exception ex)
                        {
                            var errorOnAction = "Error while " + action;

                            var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "FailedTransaction", ex.StackTrace);

                            ErrorLog err = new ErrorLog();
                            err.Action = "Failed Transaction";
                            err.Method = "OnLoad";
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
                        #endregion

                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "SuccessTransaction", ex.StackTrace);

                ErrorLog err = new ErrorLog();
                err.Action = "CT Ecom Success Transaction";
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

            return View(model);
        }
    }
}
