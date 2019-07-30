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

namespace CTPaymentBackOffice.Controllers
{
    [Authorize]
    [CustomAttributes.SessionExpireFilter]
    public class MIDsController : Controller
    {
        ReferenceRepository _refRepo;
        MerchantRepository _merchantRepo;
        MidsRepository _midsRepo;
        MerchantBranchRepository _branchRepo;
        MerchantBranchPOSRepository _posRepo;
        MidsMerchantBranchPOSsRepository _midsmerchantbranchposRepository;

        string action = string.Empty;

        public MIDsController()
        {
            _refRepo = new ReferenceRepository();
            _merchantRepo = new MerchantRepository();
            _midsRepo = new MidsRepository();
            _branchRepo = new MerchantBranchRepository();
            _posRepo = new MerchantBranchPOSRepository();
            _midsmerchantbranchposRepository = new MidsMerchantBranchPOSsRepository();
        }

        public ActionResult Index()
        {
            var userActivity = "Entered Mids Management Index Page";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "Index", "");

            if (Session["Success"] != null)
            {
                ViewBag.Success = Session["Success"];
            }

            Session["Success"] = null;

            string val = "";

            int value = 0;

            var merchant = _merchantRepo.GetAllMerchantsByPartner(CurrentUser.ParentId, "");

            var ddlmerchant = new List<SelectListItem>();

            if (merchant.Count == 0)
            {
                val = "No Merchant available";
            }
            else
            {
                val = "All Merchant";

                if (merchant.Count == 1)
                {
                    ddlmerchant.AddRange(merchant.Select(p => new SelectListItem()
                    {
                        Value = Convert.ToString(p.MerchantId),
                        Text = p.MerchantName
                    }).ToList());
                }
                else
                {
                    ddlmerchant.Add(new SelectListItem()
                    {
                        Value = "0",
                        Text = val,
                        Selected = value == 0
                    });

                    ddlmerchant.AddRange(merchant.Select(p => new SelectListItem()
                    {
                        Value = Convert.ToString(p.MerchantId),
                        Text = p.MerchantName
                    }).ToList());
                }
            }






            ViewBag.Merchants = ddlmerchant;

            return View();
        }

        public ActionResult MidsAssign(int? id, int mId)
        {
            var userActivity = "Entered Mids Assign Page";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "MidsAssign", "");

            TempData["MIDId"] = id;

            string val = "";

            int value = 0;

            var ddlbranch = new List<SelectListItem>();

            var branch = _branchRepo.GetAllBranchesByMerchant(mId, "");

            if (branch.Count == 0)
            {
                val = "No Merchant Branch available";
            }
            else
            {
                val = "All Merchant Branch";

                if (branch.Count == 1)
                {
                    ddlbranch.AddRange(branch.Select(p => new SelectListItem()
                    {
                        Value = Convert.ToString(p.MerchantBranchId),
                        Text = p.MerchantBranchName
                    }).ToList());
                }
                else
                {
                    ddlbranch.Add(new SelectListItem()
                    {
                        Value = "0",
                        Text = val,
                        Selected = value == 0
                    });

                    ddlbranch.AddRange(branch.Select(p => new SelectListItem()
                    {
                        Value = Convert.ToString(p.MerchantBranchId),
                        Text = p.MerchantBranchName
                    }).ToList());
                }
            }



            ViewBag.MerchantBranch = ddlbranch;

            return View();
        }

        [HttpPost]
        public ActionResult MidsAssign(SDMasterModel ass)
        {
            var mbranchpos = new MidsMerchantBranchPOSs();
            var bpos = new SDGDAL.Entities.MidsMerchantBranchPOSs();
            var b = new SDGDAL.Entities.MidsMerchantBranches();

            try
            {
                action = "assigning mid to merchant branch and merchant PoSs.";

                string s = Convert.ToString(Request["hdnCtrl"]);
                string[] pId = s.Split(',');

                bool isExist = _midsRepo.IsMidsMerchantBranchesExist(Convert.ToInt32(TempData["BranchID"]));

                foreach (var j in pId)
                {
                    bool isPoSExist = _midsmerchantbranchposRepository.IsPoSAssignedExist(Convert.ToInt32(j));

                    if (!isPoSExist)
                    {
                        var info = _midsmerchantbranchposRepository.GetMidsMerchantBranchesPossByPOSId(Convert.ToInt32(j));
                        bpos.Id = info.Id;
                        bpos.MidId = info.MidId;
                        bpos.MerchantBranchPOSId = info.MerchantBranchPOSId;
                        bpos.IsActive = info.IsActive;
                        bpos.IsDeleted = false;
                        bpos.DateCreated = info.DateCreated;
                        bpos.DateUpdated = info.DateUpdated;

                        var p = _midsmerchantbranchposRepository.UpdateIsDeleted(bpos);

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        bpos.MidId = Convert.ToInt32(TempData["MIDId"]);
                        bpos.MerchantBranchPOSId = Convert.ToInt32(j);
                        bpos.IsActive = true;
                        bpos.IsDeleted = false;
                        bpos.DateCreated = DateTime.Now;
                        bpos.DateUpdated = DateTime.Now;

                        if (!isExist)
                        {
                            var userActivity = "Assigned a MID";

                            var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "MidsAssign", "");

                            Session["Success"] = "MID successfully assigned.";

                            b.MidId = Convert.ToInt32(TempData["MIDId"]);
                            b.MerchantBranchId = Convert.ToInt32(TempData["BranchID"]);
                            b.IsActive = true;
                            b.IsDeleted = false;
                            b.DateCreated = DateTime.Now;
                            b.DateUpdated = DateTime.Now;
                            isExist = true;
                        }
                        else
                        {
                            b = null;
                        }

                        mbranchpos = _midsRepo.AssignMerchantBranchPOSs(b, bpos);

                        Session["Success"] = "MID successfully assigned.";
                    }
                }
                if (mbranchpos.Id > 0)
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "MidsAssign", ex.StackTrace);

                ErrorLog err = new ErrorLog();
                err.Action = "MIDs Assignment";
                err.Method = "MidsAssign";
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

                var branch = _branchRepo.GetAllBranchesByPartner(CurrentUser.ParentId, "");

                if (branch.Count == 0)
                    val = "No Merchant Branch available";
                else
                    val = "All Merchant Branch";

                var ddlbranch = new List<SelectListItem>();

                ddlbranch.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = val,
                    Selected = value == 0
                });

                ddlbranch.AddRange(branch.Select(p => new SelectListItem()
                {
                    Value = Convert.ToString(p.MerchantBranchId),
                    Text = p.MerchantBranchName
                }).ToList());

                ViewBag.MerchantBranch = ddlbranch;
            }

            return View();
        }


        public ActionResult RemoveAssignedMid(int? id, int mId)
        {
            var userActivity = "Entered Remove Assigned MID Page";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "RemoveAssignedMid", "");

            TempData["MIDId"] = id;

            string val = "";

            int value = 0;

            var ddlbranch = new List<SelectListItem>();

            var branch = _branchRepo.GetAllBranchesByMerchant(mId, "");

            if (branch.Count == 0)
            {
                val = "No Merchant Branch available";
            }
            else
            {
                val = "All Merchant Branch";

                if (branch.Count == 1)
                {
                    ddlbranch.AddRange(branch.Select(p => new SelectListItem()
                    {
                        Value = Convert.ToString(p.MerchantBranchId),
                        Text = p.MerchantBranchName
                    }).ToList());
                }
                else
                {
                    ddlbranch.Add(new SelectListItem()
                    {
                        Value = "0",
                        Text = val,
                        Selected = value == 0
                    });

                    ddlbranch.AddRange(branch.Select(p => new SelectListItem()
                    {
                        Value = Convert.ToString(p.MerchantBranchId),
                        Text = p.MerchantBranchName
                    }).ToList());
                }
            }


            ViewBag.MerchantBranch = ddlbranch;

            return View();
        }

        [HttpPost]
        public ActionResult RemoveAssignedMid(int id, int midId)
        {
            try
            {
                action = "removing mid";

                List<MidsMerchantBranchPOSs> midsBranchPOS = new List<MidsMerchantBranchPOSs>();

                midsBranchPOS = _midsmerchantbranchposRepository.GetAllMidsMerchantBranchPOSs();

                var info = _midsmerchantbranchposRepository.GetMidsMerchantBranchesPossByPOSIdandMid(id, midId);

                var mmb = new SDGDAL.Entities.MidsMerchantBranchPOSs();
                mmb.Id = info.Id;
                mmb.MidId = info.MidId;
                mmb.MerchantBranchPOSId = info.MerchantBranchPOSId;
                mmb.IsActive = info.IsActive;
                mmb.IsDeleted = true;
                mmb.DateCreated = info.DateCreated;
                mmb.DateUpdated = info.DateUpdated;

                var s = _midsmerchantbranchposRepository.UpdateIsDeleted(mmb);

                return Json(s);
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "RemoveAssignedMid", ex.StackTrace);
            }

            return null;
        }

        public ActionResult MidsLocationAssign()
        {
            return View();
        }
        public ActionResult MidsLocationIndex()
        {
            return View();
        }
        public ActionResult MidsLocationViewInfo()
        {
            return View();
        }
        public ActionResult MidsPOSAssign()
        {
            return View();
        }
        public ActionResult MidsPOSIndex()
        {
            return View();
        }
        public ActionResult MidsPOSViewInfo()
        {
            return View();
        }

        public ActionResult MidsUpdateInfo(int? id)
        {
            if (CurrentUser.ParentType == Enums.ParentType.Partner)
            {
                try
                {

                    var userActivity = "Entered Mids Update Info Page";

                    var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "MidsUpdateInfo", "");

                    action = "fetching the data for mids";

                    var mid =
                        id.HasValue ?
                        _midsRepo.GetDetailsbyMidId(id.Value)
                        :
                        _midsRepo.GetDetailsbyMidId(CurrentUser.ParentId);


                    var currencies = _refRepo.GetAllCurrencies();
                    var switches = _refRepo.GetAllSwitches();
                    var cardTypes = _refRepo.GetAllCardTypes();

                    ViewBag.Switches = switches.Select(s => new SelectListItem()
                    {
                        Value = s.SwitchId.ToString(),
                        Text = s.SwitchName
                    }).ToList();

                    ViewBag.CardTypes = cardTypes.Select(c => new SelectListItem()
                    {
                        Value = c.CardTypeId.ToString(),
                        Text = c.TypeName
                    }).ToList();

                    ViewBag.Currencies = currencies.Select(c => new SelectListItem()
                    {
                        Value = c.CurrencyId.ToString(),
                        Text = c.CurrencyCode
                    }).ToList();

                    TempData["MidId"] = mid.MidId;
                    MerchantMidModel m = new MerchantMidModel();
                    m.Merchant = new MerchantModel();
                    m.Merchant.MerchantName = mid.Merchant.MerchantName;
                    m.MIDName = mid.MidName;
                    m.IsActive = mid.IsActive;
                    m.SwitchId = mid.SwitchId;
                    m.CardTypeId = mid.CardTypeId;
                    m.CurrencyId = mid.CurrencyId;
                    m.Param_1 = mid.Param_1;
                    m.Param_2 = mid.Param_2;
                    m.Param_3 = mid.Param_3;
                    m.Param_4 = mid.Param_4;
                    m.Param_5 = mid.Param_5;
                    m.Param_6 = null;
                    m.Param_7 = null;
                    m.Param_8 = null;
                    m.Param_9 = null;
                    m.Param_10 = null;

                    TempData["TransactionChargesId"] = mid.TransactionChargesId;
                    m.TransactionChargeId = mid.TransactionChargesId;
                    m.TC_Discount = mid.TransactionCharges.DiscountRate;
                    m.TC_CardNotPresent = mid.TransactionCharges.CardNotPresent;
                    m.TC_Decline = mid.TransactionCharges.Declined;
                    m.TC_eCommerce = mid.TransactionCharges.eCommerce;
                    m.TC_PreAuth = mid.TransactionCharges.PreAuth;
                    m.TC_Capture = mid.TransactionCharges.Capture;
                    m.TC_Purchased = mid.TransactionCharges.Purchased;
                    m.TC_Refund = mid.TransactionCharges.Refund;
                    m.TC_Void = mid.TransactionCharges.Void;
                    m.TC_CashBack = mid.TransactionCharges.CashBack;
                    //m.TC_BalanceInquiry = Convert.ToDecimal(mid.Param_11);


                    return View(m);
                }
                catch (Exception ex)
                {
                    var errorOnAction = "Error while " + action;

                    var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "MidsUpdateInfo", ex.StackTrace);
                }
                finally
                {
                    var currencies = _refRepo.GetAllCurrencies();
                    var switches = _refRepo.GetAllSwitches();
                    var cardTypes = _refRepo.GetAllCardTypes();

                    ViewBag.Switches = switches.Select(s => new SelectListItem()
                    {
                        Value = s.SwitchId.ToString(),
                        Text = s.SwitchName
                    }).ToList();

                    ViewBag.CardTypes = cardTypes.Select(c => new SelectListItem()
                    {
                        Value = c.CardTypeId.ToString(),
                        Text = c.TypeName
                    }).ToList();

                    ViewBag.Currencies = currencies.Select(c => new SelectListItem()
                    {
                        Value = c.CurrencyId.ToString(),
                        Text = c.CurrencyCode
                    }).ToList();
                }
            }
            else
            {
                return HttpNotFound();
            }

            return View();
        }

        [HttpPost]
        public ActionResult MidsUpdateInfo(MerchantMidModel mid)
        {
            Mid m = new Mid();

            if (CurrentUser.ParentType == Enums.ParentType.Partner)
            {
                try
                {
                    //if (!CheckMidIfExist(Convert.ToInt32(TempData["MerchantID"]), mid.CardTypeId, "Updating", Convert.ToInt32(TempData["MidId"])))
                    //{
                    action = "updating mids info.";

                    //Need to clarify method -> "CheckMidIfExist"
                    //if (!CheckMidIfExist(Convert.ToInt32(TempData["MerchantID"]), mid.CardTypeId, "Updating", Convert.ToInt32(TempData["MidId"])))
                    //{

                    action = "updating mids info.";

                    m.MidId = Convert.ToInt32(TempData["MidId"]);
                    m.TransactionChargesId = Convert.ToInt32(TempData["TransactionChargesId"]);
                    m.MidName = mid.MIDName;
                    m.IsActive = mid.IsActive;
                    m.SwitchId = mid.SwitchId;
                    m.CardTypeId = mid.CardTypeId;
                    m.CurrencyId = mid.CurrencyId;
                    m.Param_1 = mid.Param_1;
                    m.Param_2 = mid.Param_2;
                    m.Param_3 = mid.Param_3;
                    m.Param_4 = mid.Param_4;
                    m.Param_5 = mid.Param_5;
                    m.Param_6 = mid.Param_6;
                    m.Param_7 = mid.Param_7;
                    m.Param_8 = mid.Param_8;
                    m.Param_9 = mid.Param_9;
                    m.Param_10 = mid.Param_10;

                    m.TransactionCharges = new TransactionCharges();
                    m.TransactionCharges.TransactionChargeId = Convert.ToInt32(TempData["TransactionChargesId"]);
                    m.TransactionCharges.DiscountRate = mid.TC_Discount;
                    m.TransactionCharges.CardNotPresent = mid.TC_CardNotPresent;
                    m.TransactionCharges.Declined = mid.TC_Decline;
                    m.TransactionCharges.eCommerce = mid.TC_eCommerce;
                    m.TransactionCharges.PreAuth = mid.TC_PreAuth;
                    m.TransactionCharges.Capture = mid.TC_Capture;
                    m.TransactionCharges.Purchased = mid.TC_Purchased;
                    m.TransactionCharges.Refund = mid.TC_Refund;
                    m.TransactionCharges.Void = mid.TC_Void;
                    m.TransactionCharges.CashBack = mid.TC_CashBack;

                    var imids = _midsRepo.UpdateMids(m);

                    if (imids.MidId > 0)
                    {
                        var userActivity = "Updates Mid Info";

                        var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "MidsUpdateInfo", "");

                        Session["Success"] = "Successfully Updated.";

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        throw new Exception(imids.MidName);
                    }
                    //}
                    //else
                    //{
                    //TODO : CREATE A MESSAGE
                    //    ModelState.AddModelError("CardType", "Card type already exist in the same merchant");
                    //}
                }
                catch (Exception ex)
                {
                    var errorOnAction = "Error while " + action;

                    var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "MidsUpdateInfo", ex.StackTrace);

                    ErrorLog err = new ErrorLog();
                    err.Action = "Update Mids";
                    err.Method = "MidsUpdateInfo";
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
                    TempData["MidId"] = mid.MIDId;
                    var currencies = _refRepo.GetAllCurrencies();
                    var switches = _refRepo.GetAllSwitches();
                    var cardTypes = _refRepo.GetAllCardTypes();

                    ViewBag.Switches = switches.Select(s => new SelectListItem()
                    {
                        Value = s.SwitchId.ToString(),
                        Text = s.SwitchName
                    }).ToList();

                    ViewBag.CardTypes = cardTypes.Select(c => new SelectListItem()
                    {
                        Value = c.CardTypeId.ToString(),
                        Text = c.TypeName
                    }).ToList();

                    ViewBag.Currencies = currencies.Select(c => new SelectListItem()
                    {
                        Value = c.CurrencyId.ToString(),
                        Text = c.CurrencyCode
                    }).ToList();
                }

                return View(mid);
            }
            else
            {
                return HttpNotFound();
            }
        }

        public ActionResult MidsViewInfo(int? id)
        {
            if (CurrentUser.ParentType == Enums.ParentType.Partner)
            {
                var userActivity = "Entered Mids View Info Page";

                var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "MidsViewInfo", "");

                var mid =
                    id.HasValue ?
                    _midsRepo.GetDetailsbyMidId(id.Value)
                    :
                    _midsRepo.GetDetailsbyMidId(CurrentUser.ParentId);

                TempData["MidId"] = mid.MidId;
                MerchantMidModel m = new MerchantMidModel();
                m.Merchant = new MerchantModel();
                m.Merchant.MerchantName = mid.Merchant.MerchantName;
                m.MIDName = mid.MidName;
                m.IsActive = mid.IsActive;
                m.Param_1 = mid.Param_1;
                m.Param_2 = mid.Param_2;
                m.Param_3 = mid.Param_3;
                m.Param_4 = mid.Param_4;
                m.Param_5 = mid.Param_5;
                m.Param_6 = mid.Param_6;
                m.Param_7 = null;
                m.Param_8 = null;
                m.Param_9 = null;
                m.Param_10 = null;
                ViewBag.SwitchName = mid.Switch.SwitchName;
                ViewBag.CardType = mid.CardType.TypeName;
                ViewBag.Currency = mid.Currency.CurrencyName;

                m.TransactionChargeId = mid.TransactionChargesId;
                m.TC_Discount = mid.TransactionCharges.DiscountRate;
                m.TC_CardNotPresent = mid.TransactionCharges.CardNotPresent;
                m.TC_Decline = mid.TransactionCharges.Declined;
                m.TC_eCommerce = mid.TransactionCharges.eCommerce;
                m.TC_PreAuth = mid.TransactionCharges.PreAuth;
                m.TC_Capture = mid.TransactionCharges.Capture;
                m.TC_Purchased = mid.TransactionCharges.Purchased;
                m.TC_Refund = mid.TransactionCharges.Refund;
                m.TC_Void = mid.TransactionCharges.Void;
                m.TC_CashBack = mid.TransactionCharges.CashBack;
                //m.TC_BalanceInquiry = Convert.ToDecimal(mid.Param_11);

                if (mid.Param_6 != null)
                {
                    ViewBag.IsNull = false;
                }

                return View(m);
            }
            else
            {
                return HttpNotFound();
            }
        }

        public ActionResult Registration(int? id)
        {
            GenerateDataForDropDown(id);

            return View();
        }

        [HttpPost]
        public ActionResult Registration(MerchantMidModel mid)
        {
            GenerateDataForDropDown(mid.Merchant.MerchantId);

            try
            {
                action = "creating mids.";

                removeProperty();
                if (ModelState.IsValid && mid != null)
                {
                    var m = new SDGDAL.Entities.Mid();
                    m.MerchantId = mid.Merchant.MerchantId;
                    m.MidName = mid.MIDName;
                    m.IsActive = mid.IsActive;
                    m.IsDeleted = false;
                    m.SwitchId = mid.SwitchId;
                    m.CardTypeId = mid.CardTypeId;
                    m.CurrencyId = mid.CurrencyId;
                    m.MidsPricingId = 0;
                    m.Param_1 = mid.Param_1;
                    m.Param_2 = mid.Param_2;
                    m.Param_3 = mid.Param_3;
                    m.Param_4 = mid.Param_4;
                    m.Param_5 = mid.Param_5;

                    if ((mid.Param_6 != null) || (mid.Param_6 != ""))
                    {
                        m.Param_6 = mid.Param_6;
                    }

                    var t = new SDGDAL.Entities.TransactionCharges();
                    t.DiscountRate = mid.TC_Discount;
                    t.CardNotPresent = mid.TC_CardNotPresent;
                    t.Declined = mid.TC_Decline;
                    t.eCommerce = mid.TC_eCommerce;
                    t.PreAuth = mid.TC_PreAuth;
                    t.Capture = mid.TC_Capture;
                    t.Purchased = mid.TC_Purchased;
                    t.Refund = mid.TC_Refund;
                    t.Void = mid.TC_Void;
                    t.CashBack = mid.TC_CashBack;


                    m.TransactionCharges = t;

                    if (!CheckMidIfExist(m.MerchantId, m.CardTypeId, "Adding", 0))
                    {
                        var newMid = _midsRepo.CreateMid(m);

                        if (newMid.MidId > 0)
                        {
                            var userActivity = "Registered a MID";

                            var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "Registration", "");

                            Session["Success"] = "MID successfully created.";

                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        //TODO : CREATE A MESSAGE
                        ModelState.AddModelError("CardType", "Cannot create same card type for the same merchant");
                    }
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "Registration", ex.StackTrace);

                ErrorLog err = new ErrorLog();
                err.Action = "MIDs Registration";
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

            return View();
        }
        private bool CheckMidIfExist(int MerchantID, int cardTypeID, string Action, int midID)
        {
            List<Mid> mids = _midsRepo.GetMidByMerchantId(MerchantID, cardTypeID, Action, midID);

            return mids.Count() > 0 ? true : false;
        }
        private void GenerateDataForDropDown(int? merchantId)
        {
            try
            {
                action = "generating data for the dropdown.";

                var currencies = _refRepo.GetAllCurrencies();
                var switches = _refRepo.GetAllSwitches();
                var cardTypes = _refRepo.GetAllCardTypes();

                List<SDGDAL.Entities.Merchant> merchants = new List<SDGDAL.Entities.Merchant>();

                var ddlm = new List<SelectListItem>();
                var val = 0;

                if (CurrentUser.ParentType == Enums.ParentType.Partner)
                {
                    merchants = _merchantRepo.GetAllMerchantsByPartner(CurrentUser.ParentId, "");
                    if (merchants.Count != 0)
                    {
                        if (merchants.Count == 1)
                        {
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
                }
                else if (CurrentUser.ParentType == Enums.ParentType.Reseller)
                {
                    merchants = _merchantRepo.GetAllMerchantsByReseller(CurrentUser.ParentId, "");

                    ViewBag.Merchants = merchants.Select(m => new SelectListItem()
                    {
                        Value = m.MerchantId.ToString(),
                        Text = m.MerchantName,
                        Selected = merchantId.HasValue ? m.MerchantId == merchantId.Value : false
                    }).ToList();
                }
                else if (CurrentUser.ParentType == Enums.ParentType.Merchant)
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
                }

                var ddlswitch = new List<SelectListItem>();
                var ddlcardtype = new List<SelectListItem>();
                var ddlcurrency = new List<SelectListItem>();

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
                ViewBag.Merchants = ddlm;
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "GenerateDataForDropDown", ex.StackTrace);
            }
        }

        public void removeProperty()
        {
            ModelState.Remove("Merchant.MerchantName");
            ModelState.Remove("Merchant.Address");
            ModelState.Remove("Merchant.City");
            ModelState.Remove("Merchant.ZipCode");
            ModelState.Remove("Merchant.PrimaryContactNumber");
        }
    }
}
