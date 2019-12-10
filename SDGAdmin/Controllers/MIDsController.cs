using SDGAdmin;
using SDGAdmin.Models;
using SDGDAL.Entities;
using SDGDAL.Repositories;
using SDGUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SDGDAL;

namespace SDGAdmin.Controllers {
     [Authorize]
     [CustomAttributes.SessionExpireFilter]
     public class MIDsController : Controller {
          PartnerRepository _partnerRepo;
          ReferenceRepository _refRepo;
          MerchantRepository _merchantRepo;
          MidsRepository _midsRepo;
          MerchantBranchRepository _branchRepo;
          MerchantBranchPOSRepository _posRepo;
          MidsMerchantBranchPOSsRepository _midsmerchantbranchposRepository;

          string action = string.Empty;

          public MIDsController() {
               _partnerRepo = new PartnerRepository();
               _refRepo = new ReferenceRepository();
               _merchantRepo = new MerchantRepository();
               _midsRepo = new MidsRepository();
               _branchRepo = new MerchantBranchRepository();
               _posRepo = new MerchantBranchPOSRepository();
               _midsmerchantbranchposRepository = new MidsMerchantBranchPOSsRepository();
          }

          public void GetPartner() {
               try {
                    action = "fetching the partner list";

                    var partners = _partnerRepo.GetAllPartners();

                    var ddlPartners = new List<SelectListItem>();

                    ddlPartners.Add(new SelectListItem() {
                         Value = "0",//Utility.Encrypt("0"),
                         Text = "Select a Partner",
                         Selected = 0 == 0
                    });

                    ddlPartners.AddRange(partners.Select(p => new SelectListItem() {
                         Value = p.PartnerId.ToString(),//Utility.Encrypt(p.PartnerId.ToString()),
                         Text = p.CompanyName
                    }).ToList());

                    ViewBag.Partners = ddlPartners;
               } catch (Exception ex) {
                    var errorOnAction = "Error while " + action;

                    var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "GetPartner", ex.StackTrace);
               }
          }

          public void GetEncPartner() {
               try {
                    action = "fetching the partner list";

                    var partners = _partnerRepo.GetAllPartners();

                    var ddlPartners = new List<SelectListItem>();

                    ddlPartners.Add(new SelectListItem() {
                         Value = Utility.Encrypt("0"),
                         Text = "Select a Partner",
                         Selected = 0 == 0
                    });

                    ddlPartners.AddRange(partners.Select(p => new SelectListItem() {
                         Value = Utility.Encrypt(p.PartnerId.ToString()),
                         Text = p.CompanyName
                    }).ToList());

                    ViewBag.Partners = ddlPartners;
               } catch (Exception ex) {
                    var errorOnAction = "Error while " + action;

                    var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "GetPartner", ex.StackTrace);
               }
          }

          public ActionResult Index() {
               var userActivity = "Entered Mids Management Index Page";

               var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "Index", "");

               if (Session["Success"] != null) {
                    ViewBag.Success = Session["Success"];
               }

               if (Session["FailedPOSs"] != null) {
                    ViewBag.CardTypeError = Session["FailedPOSs"];
               }

               Session["FailedPOSs"] = null;

               Session["Success"] = null;

               GetEncPartner();

               return View();
          }

          public ActionResult MidsAssign(string id, string mId, string cId) {
               try {
                    var userActivity = "Entered Mids Assign Page";

                    var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "MidsAssign", "");

                    TempData["CardTypeId"] = cId.Contains(" ") == true ? Utility.Decrypt(cId.Replace(" ", "+")) : Utility.Decrypt(cId);

                    TempData["MIDId"] = id.Contains(" ") == true ? Utility.Decrypt(id.Replace(" ", "+")) : Utility.Decrypt(id);

                    TempData["MerchantId"] = mId.Contains(" ") == true ? Utility.Decrypt(mId.Replace(" ", "+")) : Utility.Decrypt(mId);

                    int value = 0;

                    var ddlbranch = new List<SelectListItem>();

                    var branch = _branchRepo.GetAllBranchesByMerchant(Convert.ToInt32(Utility.Decrypt(mId.Contains(" ") == true ? mId.Replace(" ", "+") : mId)), "");

                    if (branch.Count > 0) {
                         if (branch.Count == 1) {
                              ddlbranch.AddRange(branch.Select(p => new SelectListItem() {
                                   Value = Convert.ToString(p.MerchantBranchId),
                                   Text = p.MerchantBranchName
                              }).ToList());
                         } else {
                              ddlbranch.Add(new SelectListItem() {
                                   Value = "0",
                                   Text = "Select a Merchant Branch",
                                   Selected = value == 0
                              });

                              ddlbranch.AddRange(branch.Select(p => new SelectListItem() {
                                   Value = Convert.ToString(p.MerchantBranchId),
                                   Text = p.MerchantBranchName
                              }).ToList());
                         }
                    } else {
                         ddlbranch.Add(new SelectListItem() {
                              Value = "0",
                              Text = "No Merchant Branch available",
                              Selected = value == 0
                         });
                    }

                    ViewBag.MerchantBranch = ddlbranch;

                    return View();
               } catch (Exception ex) {
                    return View();
               }
          }

          [HttpPost]
          public ActionResult MidsAssign(SDMasterModel ass) {
               var mbranchpos = new MidsMerchantBranchPOSs();
               try {
                    action = "assigning mid to merchant branch and merchant PoSs.";

                    TempData.Keep("CardTypeId");
                    TempData.Keep("MIDId");
                    TempData.Keep("MerchantId");
                    string posName = "";
                    var id = new List<string>();

                    bool isExist = false;
                    bool isDeleted = false;

                    string s = Convert.ToString(Request["hdnCtrl"]);
                    string[] pId = s.Split(',');

                    var midC = _midsRepo.GetAllMids(Convert.ToInt32(TempData["MerchantId"])).Select(md => md.CardTypeId).ToList();

                    foreach (string p in pId) {
                         var mList = _midsmerchantbranchposRepository.GetMidsMerchantBranchesPossByPOSIdandMid(Convert.ToInt32(p), Convert.ToInt32(TempData["MIDId"]));
                         var mmbposC = _midsmerchantbranchposRepository.GetMidsMerchantBranchesPossByPOSIds(Convert.ToInt32(p));
                         var midId = mmbposC.Where(m => m.Mid.CardTypeId == Convert.ToInt32(TempData["CardTypeId"])).ToList();

                         var pos = _posRepo.GetDetailsbyMerchantPOSId(Convert.ToInt32(p));
                         isExist = mList != null ? true : false;

                         isDeleted = midId.Any(d => d.IsDeleted == false);

                         if (!isExist) {
                              if (!isDeleted) {
                                   var bpos = new SDGDAL.Entities.MidsMerchantBranchPOSs();
                                   bpos.MidId = Convert.ToInt32(TempData["MIDId"]);
                                   bpos.MerchantBranchPOSId = Convert.ToInt32(p);
                                   bpos.IsActive = true;
                                   bpos.IsDeleted = false;
                                   bpos.DateCreated = DateTime.Now;
                                   bpos.DateUpdated = DateTime.Now;

                                   var b = new SDGDAL.Entities.MidsMerchantBranches();
                                   b.MidId = Convert.ToInt32(TempData["MIDId"]);
                                   b.MerchantBranchId = Convert.ToInt32(TempData["BranchID"]);
                                   b.IsActive = true;
                                   b.IsDeleted = false;
                                   b.DateCreated = DateTime.Now;
                                   b.DateUpdated = DateTime.Now;

                                   mbranchpos = _midsRepo.AssignMerchantBranchPOSs(b, bpos);
                              } else {
                                   if (pId.Count() == 1) {
                                        ModelState.AddModelError(string.Empty, "Failed to assign MID to POS. Card Type Already Exist");
                                        return View();
                                   }

                                   posName += pos.MerchantPOSName + ", ";
                              }
                         } else {
                              if (!isDeleted) {
                                   List<MidsMerchantBranchPOSs> midsBranchPOS = new List<MidsMerchantBranchPOSs>();

                                   midsBranchPOS = _midsmerchantbranchposRepository.GetAllMidsMerchantBranchPOSs();

                                   var info = _midsmerchantbranchposRepository.GetMidsMerchantBranchesPossByPOSIdandMid(Convert.ToInt32(p), Convert.ToInt32(TempData["MIDId"]));

                                   var mmb = new SDGDAL.Entities.MidsMerchantBranchPOSs();
                                   mmb.Id = info.Id;
                                   mmb.MidId = info.MidId;
                                   mmb.MerchantBranchPOSId = info.MerchantBranchPOSId;
                                   mmb.IsActive = info.IsActive;
                                   mmb.IsDeleted = false;
                                   mmb.DateCreated = info.DateCreated;
                                   mmb.DateUpdated = DateTime.Now;

                                   mbranchpos = _midsRepo.UpdateMidsMerchantBranchPOSs(mmb);
                              } else {
                                   if (pId.Count() == 1) {
                                        ModelState.AddModelError(string.Empty, "Failed to assign MID to POS. Card Type Already Exist");
                                        return View();
                                   }

                                   posName += pos.MerchantPOSName + ", ";
                              }
                         }
                    }

                    if ((mbranchpos.Id > 0) || (mbranchpos != null)) {
                         var userActivity = "Assigned a MID";

                         var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "MidsAssign", "");

                         Session["Success"] = "MID successfully assigned.";

                         Session["FailedPOSs"] = posName == "" ? posName : posName.Remove(posName.Length - 2);

                         return RedirectToAction("Index");
                    }
               } catch (Exception ex) {
                    var errorOnAction = "Error while " + action;

                    var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "MidsAssign", ex.StackTrace);
               } finally {
                    int value = 0;

                    var ddlbranch = new List<SelectListItem>();

                    var branch = _branchRepo.GetAllBranchesByMerchant(Convert.ToInt32(TempData["MerchantId"]), "");

                    if (branch.Count > 0) {
                         if (branch.Count == 1) {
                              ddlbranch.AddRange(branch.Select(p => new SelectListItem() {
                                   Value = Convert.ToString(p.MerchantBranchId),
                                   Text = p.MerchantBranchName
                              }).ToList());
                         } else {
                              ddlbranch.Add(new SelectListItem() {
                                   Value = "0",
                                   Text = "Select a Merchant Branch",
                                   Selected = value == 0
                              });

                              ddlbranch.AddRange(branch.Select(p => new SelectListItem() {
                                   Value = Convert.ToString(p.MerchantBranchId),
                                   Text = p.MerchantBranchName
                              }).ToList());
                         }
                    } else {
                         ddlbranch.Add(new SelectListItem() {
                              Value = "0",
                              Text = "No Merchant Branch available",
                              Selected = value == 0
                         });
                    }

                    ViewBag.MerchantBranch = ddlbranch;
               }

               return View();
          }

          public ActionResult RemoveAssignedMid(string id, string mId) {
               var userActivity = "Entered Remove Assigned MID Page";

               var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "RemoveAssignedMid", "");

               TempData["MIDId"] = id.Contains(" ") == true ? Convert.ToInt32(Utility.Decrypt(id.Replace(" ", "+"))) : Convert.ToInt32(Utility.Decrypt(id));

               int value = 0;

               var ddlbranch = new List<SelectListItem>();

               var branch = _branchRepo.GetAllBranchesByMerchant(Convert.ToInt32(Utility.Decrypt(mId.Contains(" ") == true ? mId.Replace(" ", "+") : mId)), "");

               if (branch.Count > 0) {
                    if (branch.Count == 1) {
                         ddlbranch.AddRange(branch.Select(p => new SelectListItem() {
                              Value = Convert.ToString(p.MerchantBranchId),
                              Text = p.MerchantBranchName
                         }).ToList());
                    } else {
                         ddlbranch.Add(new SelectListItem() {
                              Value = "0",
                              Text = "Select a Merchant Branch",
                              Selected = value == 0
                         });

                         ddlbranch.AddRange(branch.Select(p => new SelectListItem() {
                              Value = Convert.ToString(p.MerchantBranchId),
                              Text = p.MerchantBranchName
                         }).ToList());
                    }
               } else {
                    ddlbranch.Add(new SelectListItem() {
                         Value = "0",
                         Text = "No Merchant Branch available",
                         Selected = value == 0
                    });
               }

               ViewBag.MerchantBranch = ddlbranch;

               return View();
          }

          [HttpPost]
          public ActionResult RemoveAssignedMid(int id, int midId) {
               try {
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
               } catch (Exception ex) {
                    var errorOnAction = "Error while " + action;

                    var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "RemoveAssignedMid", ex.StackTrace);

                    return RedirectToAction("Index");
               }
          }

          public ActionResult MidsLocationAssign() {
               return View();
          }

          public ActionResult MidsLocationIndex() {
               return View();
          }

          public ActionResult MidsLocationViewInfo() {
               return View();
          }

          public ActionResult MidsPOSAssign() {
               return View();
          }

          public ActionResult MidsPOSIndex() {
               return View();
          }

          public ActionResult MidsPOSViewInfo() {
               return View();
          }

          public ActionResult MidsUpdateInfo(string id) {
               try {
                    var userActivity = "Entered Mids Update Info Page";

                    var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "MidsUpdateInfo", "");

                    action = "fetching the data for mids";

                    var mid =
                        id != string.Empty ?
                        _midsRepo.GetDetailsbyMidId(Convert.ToInt32(Utility.Decrypt(id.Contains(" ") == true ? id.Replace(" ", "+") : id)))
                        :
                        _midsRepo.GetDetailsbyMidId(CurrentUser.ParentId);


                    var currencies = _refRepo.GetAllCurrencies();
                    var switches = _refRepo.GetAllSwitches();
                    var cardTypes = _refRepo.GetAllCardTypes();
                    var countries = _refRepo.GetAllCountries();

                    ViewBag.Switches = switches.Select(s => new SelectListItem() {
                         Value = s.SwitchId.ToString(),
                         Text = s.SwitchName
                    }).ToList();

                    ViewBag.CardTypes = cardTypes.Select(c => new SelectListItem() {
                         Value = c.CardTypeId.ToString(),
                         Text = c.TypeName
                    }).ToList();

                    ViewBag.Currencies = currencies.Select(c => new SelectListItem() {
                         Value = c.CurrencyId.ToString(),
                         Text = c.CurrencyCode
                    }).ToList();

                    ViewBag.Countries = countries.Select(c => new SelectListItem() {
                         Value = c.CountryCode.ToString(),
                         Text = c.CountryName
                    }).ToList();

                    TempData["MidId"] = mid.MidId;
                    TempData["CardTypeId"] = mid.CardTypeId;
                    MerchantMidModel m = new MerchantMidModel();
                    m.MerchantName = mid.Merchant.MerchantName;
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
                    m.Param_6 = mid.Param_6;
                    m.Param_7 = mid.Param_7;
                    m.Param_8 = mid.Param_8;
                    m.Param_9 = mid.Param_9;
                    m.Param_10 = mid.Param_10;
                    m.Param_11 = mid.Param_11;
                    m.Param_12 = mid.Param_12;
                    m.Param_13 = mid.Param_13;
                    m.Param_14 = mid.Param_14;
                    m.Param_15 = mid.Param_15;
                    m.Param_16 = mid.Param_16;
                    m.Param_17 = mid.Param_17;
                    m.Param_18 = mid.Param_18;
                    m.Param_19 = mid.Param_19;
                    m.Param_20 = mid.Param_20;
                    m.Param_21 = mid.Param_21;
                    m.Param_22 = mid.Param_22;
                    m.Param_23 = mid.Param_23;
                    m.Param_24 = mid.Param_24;
                    m.SetLikeMerchantId = mid.SetLikeMerchantId;
                    m.SetLikeTerminalId = mid.SetLikeTerminalId;

                    m.NeedAddBulk = mid.NeedAddBulk;
                    m.NeedUpdateBulk = mid.NeedUpdateBulk;
                    m.NeedDeleteBulk = mid.NeedDeleteBulk;
                    m.NeedAddTerminalId = mid.NeedAddTerminal;
                    m.AcquiringBin = mid.AcquiringBin;
                    m.City = mid.City;
                    m.Country = mid.Country;

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

                    if (mid.Switch.SwitchCode == "CTPayment") {
                         TempData["IsCTPayment"] = true;
                    }

                    TempData["MerchantId"] = mid.MerchantId;
                    TempData["MerchantName"] = mid.Merchant.MerchantName;

                    return View(m);
               } catch (Exception ex) {
                    var errorOnAction = "Error while " + action;

                    var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "MidsUpdateInfo", ex.StackTrace);
               } finally {
                    var currencies = _refRepo.GetAllCurrencies();
                    var switches = _refRepo.GetAllSwitches();
                    var cardTypes = _refRepo.GetAllCardTypes();

                    ViewBag.Switches = switches.Select(s => new SelectListItem() {
                         Value = s.SwitchId.ToString(),
                         Text = s.SwitchName
                    }).ToList();

                    ViewBag.CardTypes = cardTypes.Select(c => new SelectListItem() {
                         Value = c.CardTypeId.ToString(),
                         Text = c.TypeName
                    }).ToList();

                    ViewBag.Currencies = currencies.Select(c => new SelectListItem() {
                         Value = c.CurrencyId.ToString(),
                         Text = c.CurrencyCode
                    }).ToList();
               }

               return View();
          }

          [HttpPost]
          public ActionResult MidsUpdateInfo(MerchantMidModel mid) {
               Mid m = new Mid();

               if (CurrentUser.ParentType == Enums.ParentType.Partner) {
                    try {
                         action = "updating mids info.";
                         TempData.Keep();
                         bool isError = false;

                         if (mid.SwitchId == 22) {
                              var midsInfo = _midsRepo.GetDetailsbyParam2AndSwitchCode(mid.Param_2);

                              //var cardType = midsInfo.CardTypeId

                              if (midsInfo != null) {
                                   if (midsInfo.MerchantId == Convert.ToInt32(TempData["MerchantId"])) {
                                        var mInfo = _midsRepo.GetMidsBySwitchIdAndMerchantIdAndCardTypeId(midsInfo.MerchantId, mid.SwitchId, Convert.ToInt32(TempData["CardTypeId"]));

                                        var mids = _midsRepo.CheckCardType(midsInfo.MerchantId, midsInfo.CardTypeId);

                                        foreach (var c in mInfo) {
                                             if (c.CardTypeId == mid.CardTypeId) {
                                                  ModelState.AddModelError(string.Empty, "Cannot save with same Card type to CT Payment Switch.");
                                                  isError = true;
                                             }
                                        }
                                   } else {
                                        ModelState.AddModelError(string.Empty, "Merchant ID is already used.");
                                        isError = true;
                                   }
                              }
                         }

                         #region CTPayment
                         int tId = mid.Param_6 == null ? 0 : mid.Param_6.Length;
                         int mbTransNumber = mid.Param_7 == null ? 0 : mid.Param_7.Length;
                         int mNumber = mid.Param_8 == null ? 0 : mid.Param_8.Length;
                         int mAccNumber = mid.Param_9 == null ? 0 : mid.Param_9.Length;
                         int oId = mid.Param_10 == null ? 0 : mid.Param_10.Length;
                         int mCCode = mid.Param_11 == null ? 0 : mid.Param_11.Length;
                         int dmId = mid.Param_12 == null ? 0 : mid.Param_12.Length;
                         #endregion

                         #region MasterCard & Visa
                         int pfName = mid.Param_16 == null ? 0 : mid.Param_16.Length;
                         int subMerchantState = mid.Param_20 == null ? 0 : mid.Param_20.Length;
                         int subMerchantPCCode = mid.Param_22 == null ? 0 : mid.Param_22.Length;
                         #endregion

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

                         if (mid.SwitchId == 27) {
                              //m.Param_6 = mid.Param_6;
                         }

                         if ((mid.SwitchId == 20) || (mid.SwitchId == 24)) {
                              //m.Param_6 = mid.Param_6;
                              m.Param_13 = mid.Param_13;
                              m.Param_14 = mid.Param_14;
                              m.Param_15 = mid.Param_15;
                              m.Param_16 = mid.Param_16;
                              m.Param_17 = mid.Param_17;
                              m.Param_18 = mid.Param_18;
                              m.Param_19 = mid.Param_19;
                              m.Param_20 = mid.Param_20;
                              m.Param_21 = mid.Param_21;
                              m.Param_22 = mid.Param_22;
                              m.Param_23 = mid.Param_23;
                              m.Param_24 = mid.Param_24;
                         }

                         #region CT Payment
                         if (mid.SwitchId == 22) {
                              if ((tId < 1) || (tId > 8)) {
                                   ModelState.AddModelError(string.Empty, "Terminal Id should be consist of 1-8 characters.");
                                   isError = true;
                              } else if ((mbTransNumber < 5) || (mbTransNumber > 5)) {
                                   ModelState.AddModelError(string.Empty, "Invalid Merchant Branch Transit Number. Enter a 5-digit number.");
                                   isError = true;
                              } else if ((mNumber < 3) || mNumber > 3) {
                                   ModelState.AddModelError(string.Empty, "Invalid Merchant Bank Number. Enter a 3-digit number.");
                                   isError = true;
                              } else if ((mAccNumber < 1) || (mAccNumber > 12)) {
                                   ModelState.AddModelError(string.Empty, "Merchant Account Number should be consist of 1-12 characters.");
                                   isError = true;
                              } else if ((oId < 1) || (oId > 10)) {
                                   ModelState.AddModelError(string.Empty, "Originator Id should be consist of 1-10 characters.");
                                   isError = true;
                              } else if ((mCCode < 4) || (mCCode > 4)) {
                                   ModelState.AddModelError(string.Empty, "Invalid Category Code. Enter a 4-digit number.");
                                   isError = true;
                              } else if ((dmId < 8) || (dmId > 20)) {
                                   ModelState.AddModelError(string.Empty, "Deposit Merchant Id should be consist of 8-20 characters.");
                                   isError = true;
                              } else {
                                   m.Param_6 = mid.Param_6;
                                   m.Param_7 = mid.Param_7;
                                   m.Param_8 = mid.Param_8;
                                   m.Param_9 = mid.Param_9;
                                   m.Param_10 = mid.Param_10;
                                   m.Param_11 = mid.Param_11;
                                   m.Param_12 = mid.Param_12;
                              }
                         }
                         #endregion

                         #region Validation Master Card New Fields
                         if ((mid.SwitchId == 20) || (mid.SwitchId == 24)) {
                              bool isFacilitatorName = false;
                              bool isValidsubMerchantState = false;

                              switch (pfName) {
                                   case 0:
                                   isFacilitatorName = true;
                                   break;

                                   case 3:
                                   isFacilitatorName = true;
                                   break;

                                   case 7:
                                   isFacilitatorName = true;
                                   break;

                                   case 12:
                                   isFacilitatorName = true;
                                   break;

                                   default:
                                   isFacilitatorName = false;
                                   break;

                              }

                              switch (subMerchantState) {
                                   case 0:
                                   isValidsubMerchantState = true;
                                   break;

                                   case 2:
                                   isValidsubMerchantState = true;
                                   break;

                                   case 3:
                                   isValidsubMerchantState = true;
                                   break;

                                   default:
                                   isValidsubMerchantState = false;
                                   break;
                              }

                              if (isFacilitatorName != true) {
                                   ModelState.AddModelError(string.Empty, "Payment Facilitator Name must be consist of only 3, 7 or 12 characters.");
                                   isError = true;
                              }
                              if (isValidsubMerchantState != true) {
                                   ModelState.AddModelError(string.Empty, "Sub-Merchant State must consist of 2 or 3 characters.");
                                   isError = true;
                              }
                              if (subMerchantPCCode > 0) {
                                   if ((subMerchantPCCode < 3) || (subMerchantPCCode > 3)) {
                                        ModelState.AddModelError(string.Empty, "Sub-Merchant POS Country Code must consist of 3 characters.");
                                        isError = true;
                                   }
                              }
                         }
                         #endregion

                         m.AcquiringBin = mid.AcquiringBin;
                         m.City = mid.City;
                         m.Country = mid.Country;
                         m.Param_11 = mid.Param_11;
                         m.Param_12 = mid.Param_12;

                         m.NeedAddBulk = mid.NeedAddBulk;
                         m.NeedUpdateBulk = mid.NeedAddBulk ? false : true;
                         m.NeedDeleteBulk = mid.NeedDeleteBulk;
                         m.NeedAddTerminal = mid.NeedAddTerminalId;
                         m.SetLikeMerchantId = mid.SetLikeMerchantId;
                         m.SetLikeTerminalId = mid.SetLikeTerminalId;

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

                         if (isError == false) {
                              var imids = _midsRepo.UpdateMids(m);

                              if (imids.MidId > 0) {
                                   var userActivity = "Updates Mid Info";

                                   var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "MidsUpdateInfo", "");

                                   Session["Success"] = "Successfully Updated.";

                                   return RedirectToAction("Index");
                              } else {
                                   throw new Exception(imids.MidName);
                              }
                         }
                    } catch (Exception ex) {
                         var errorOnAction = "Error while " + action;

                         var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "MidsUpdateInfo", ex.StackTrace);
                    } finally {

                         TempData.Keep("MerchantName");
                         TempData.Keep("MidId");
                         TempData.Keep("TransactionChargesId");
                         TempData.Keep("MerchantId");
                         TempData.Keep("CardTypeId");
                         mid.MerchantName = Convert.ToString(TempData["MerchantName"]);
                         var currencies = _refRepo.GetAllCurrencies();
                         var switches = _refRepo.GetAllSwitches();
                         var cardTypes = _refRepo.GetAllCardTypes();

                         ViewBag.Switches = switches.Select(s => new SelectListItem() {
                              Value = s.SwitchId.ToString(),
                              Text = s.SwitchName
                         }).ToList();

                         ViewBag.CardTypes = cardTypes.Select(c => new SelectListItem() {
                              Value = c.CardTypeId.ToString(),
                              Text = c.TypeName
                         }).ToList();

                         ViewBag.Currencies = currencies.Select(c => new SelectListItem() {
                              Value = c.CurrencyId.ToString(),
                              Text = c.CurrencyCode
                         }).ToList();
                    }

                    return View(mid);
               } else {
                    return HttpNotFound();
               }
          }

          public ActionResult MidsViewInfo(string id) {
               var userActivity = "Entered Mids View Info Page";

               var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "MidsViewInfo", "");

               if (CurrentUser.ParentType == Enums.ParentType.Partner) {
                    var mid =
                        id != string.Empty ?
                        _midsRepo.GetDetailsbyMidId(Convert.ToInt32(Utility.Decrypt(id.Contains(" ") == true ? id.Replace(" ", "+") : id)))
                        :
                        _midsRepo.GetDetailsbyMidId(CurrentUser.ParentId);

                    TempData["MidId"] = mid.MidId;
                    MerchantMidModel m = new MerchantMidModel();
                    //m.Merchant = new MerchantModel();
                    m.MerchantName = mid.Merchant.MerchantName;
                    m.MIDName = mid.MidName;
                    m.IsActive = mid.IsActive;
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
                    m.Param_11 = mid.Param_11;
                    m.Param_12 = mid.Param_12;
                    m.Param_13 = mid.Param_13;
                    m.Param_14 = mid.Param_14;
                    m.Param_15 = mid.Param_15;
                    m.Param_16 = mid.Param_16;
                    m.Param_17 = mid.Param_17;
                    m.Param_18 = mid.Param_18;
                    m.Param_19 = mid.Param_19;
                    m.Param_20 = mid.Param_20;
                    m.Param_21 = mid.Param_21;
                    m.Param_22 = mid.Param_22;
                    m.Param_23 = mid.Param_23;
                    m.Param_24 = mid.Param_24;
                    m.SwitchId = mid.SwitchId;
                    m.SetLikeTerminalId = mid.SetLikeTerminalId;
                    m.SetLikeMerchantId = mid.SetLikeMerchantId;
                    m.AcquiringBin = mid.AcquiringBin;
                    m.City = mid.City;

                    ViewBag.SwitchName = mid.Switch.SwitchName;
                    ViewBag.CardType = mid.CardType.TypeName;
                    ViewBag.Currency = mid.Currency.CurrencyName;
                    ViewBag.Countries = mid.Country;

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

                    if (mid.Param_6 != null) {
                         ViewBag.IsNull = false;
                    }

                    return View(m);
               } else {
                    return HttpNotFound();
               }
          }

          public ActionResult Registration(int? id) {
               var userActivity = "Entered Mids Registration Page";

               var errRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "Registration", "");

               GetPartner();

               GenerateDataForDropDown(id);

               return View();
          }

          [HttpPost]
          public ActionResult Registration(MerchantMidModel mid) {
               GenerateDataForDropDown(mid.MerchantId);

               try {
                    action = "creating mids.";

                    removeProperty();

                    if (ModelState.IsValid && mid != null) {
                         var m = new SDGDAL.Entities.Mid();
                         m.MerchantId = mid.MerchantId;
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
                         m.Param_6 = mid.Param_6;
                         m.SetLikeMerchantId = mid.SetLikeMerchantId;
                         m.SetLikeTerminalId = mid.SetLikeTerminalId;
                         m.AcquiringBin = mid.AcquiringBin;
                         m.City = mid.City;
                         m.Country = mid.Country;
                         m.Param_11 = mid.Param_11;
                         m.Param_12 = mid.Param_12;
                         m.NeedAddBulk = true;
                         m.NeedDeleteBulk = false;
                         m.NeedUpdateBulk = false;
                         m.NeedAddTerminal = true;

                         if ((mid.SwitchId == 20) || (mid.SwitchId == 24)) {
                              m.Param_13 = mid.Param_13;
                              m.Param_14 = mid.Param_14;
                              m.Param_15 = mid.Param_15;
                              m.Param_16 = mid.Param_16;
                              m.Param_17 = mid.Param_17;
                              m.Param_18 = mid.Param_18;
                              m.Param_19 = mid.Param_19;
                              m.Param_20 = mid.Param_20;
                              m.Param_21 = mid.Param_21;
                              m.Param_22 = mid.Param_22;
                              m.Param_23 = mid.Param_23;
                              m.Param_24 = mid.Param_24;
                         }

                         if (mid.SwitchId == 22) {
                              m.Param_7 = mid.Param_7;
                              m.Param_8 = mid.Param_8;
                              m.Param_9 = mid.Param_9;
                              m.Param_10 = mid.Param_10;
                              m.Param_11 = mid.Param_11;
                              m.Param_12 = mid.Param_12;
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

                         var newMid = _midsRepo.CreateMid(m);

                         if (newMid.MidId > 0) {
                              var userActivity = "Registered a MID";

                              var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "Registration", "");

                              Session["Success"] = "MID successfully created.";

                              return RedirectToAction("Index");
                         }
                    }
               } catch (Exception ex) {
                    var errorOnAction = "Error while " + action;

                    var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "Registration", ex.StackTrace);
               } finally {
                    GetPartner();
               }

               return View();
          }

          private bool CheckMidIfExist(int MerchantID, int cardTypeID, string Action, int midID) {
               List<Mid> mids = _midsRepo.GetMidByMerchantId(MerchantID, cardTypeID, Action, midID);

               return mids.Count() > 0 ? true : false;
          }

          private void GenerateDataForDropDown(int? merchantId) {
               try {
                    action = "generating data for the dropdown.";

                    var currencies = _refRepo.GetAllCurrencies();
                    var switches = _refRepo.GetAllSwitches();
                    var cardTypes = _refRepo.GetAllCardTypes();
                    var countries = _refRepo.GetAllCountries();

                    List<SDGDAL.Entities.Merchant> merchants = new List<SDGDAL.Entities.Merchant>();

                    var ddlm = new List<SelectListItem>();
                    var val = 0;

                    if (CurrentUser.ParentType == Enums.ParentType.Partner) {
                         merchants = _merchantRepo.GetAllMerchantsByPartner(CurrentUser.ParentId, "");
                         if (merchants.Count != 0) {
                              ddlm.Add(new SelectListItem() {
                                   Text = "Select a Merchant",
                                   Value = "0",
                                   Selected = val == 0
                              });
                              ddlm.AddRange(merchants.Select(m => new SelectListItem() {
                                   Value = m.MerchantId.ToString(),
                                   Text = m.MerchantName + " (Reseller: " + m.Reseller.ResellerName + ")",
                                   Selected = merchantId.HasValue ? m.MerchantId == merchantId.Value : false
                              }).ToList());
                         } else {
                              ddlm.Add(new SelectListItem() {
                                   Text = "No Merchants available",
                                   Value = "0",
                                   Selected = val == 0
                              });
                         }
                    } else if (CurrentUser.ParentType == Enums.ParentType.Reseller) {
                         merchants = _merchantRepo.GetAllMerchantsByReseller(CurrentUser.ParentId, "");

                         ViewBag.Merchants = merchants.Select(m => new SelectListItem() {
                              Value = m.MerchantId.ToString(),
                              Text = m.MerchantName,
                              Selected = merchantId.HasValue ? m.MerchantId == merchantId.Value : false
                         }).ToList();
                    } else if (CurrentUser.ParentType == Enums.ParentType.Merchant) {
                         var merch = _merchantRepo.GetMerchantDetails(CurrentUser.ParentId);
                         if (merchants.Count != 0) {
                              merchants.Add(merch);

                              ddlm.AddRange(merchants.Where(m => m.MerchantId == CurrentUser.ParentId).Select(m => new SelectListItem() {
                                   Value = m.MerchantId.ToString(),
                                   Text = m.MerchantName
                              }).ToList());
                         } else {
                              ddlm.Add(new SelectListItem() {
                                   Text = "No Merchants available",
                                   Value = "0",
                                   Selected = val == 0
                              });
                         }
                    }

                    var ddlswitch = new List<SelectListItem>();
                    var ddlcardtype = new List<SelectListItem>();
                    var ddlcurrency = new List<SelectListItem>();
                    var ddlcountry = new List<SelectListItem>();

                    ddlswitch.Add(new SelectListItem() {
                         Value = "0",
                         Text = "Select Switch",
                         Selected = val == 0
                    });

                    ddlswitch.AddRange(switches.Where(a => a.IsActive == true).Select(c => new SelectListItem() {
                         Value = c.SwitchId.ToString(),
                         Text = c.SwitchName.ToString()
                    }).ToList());

                    ViewBag.Switches = ddlswitch;

                    ddlcurrency.Add(new SelectListItem() {
                         Value = "0",
                         Text = "Select Currency",
                         Selected = val == 0
                    });

                    ddlcurrency.AddRange(currencies.Select(c => new SelectListItem() {
                         Value = c.CurrencyId.ToString(),
                         Text = c.CurrencyCode
                    }).ToList());

                    ViewBag.Currencies = ddlcurrency;

                    ddlcardtype.Add(new SelectListItem() {
                         Value = "0",
                         Text = "Select Card Type",
                         Selected = val == 0
                    });

                    ddlcardtype.AddRange(cardTypes.Select(c => new SelectListItem() {
                         Value = c.CardTypeId.ToString(),
                         Text = c.TypeName
                    }).ToList());

                    ddlcountry.AddRange(countries.Select(c => new SelectListItem() {
                         Value = c.CountryCode.ToString(),
                         Text = c.CountryName
                    }).ToList());

                    ViewBag.Countries = ddlcountry;
                    ViewBag.CardTypes = ddlcardtype;
                    ViewBag.Merchants = ddlm;
               } catch (Exception ex) {
                    var errorOnAction = "Error while " + action;

                    var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "GenerateDataForDropDown", ex.StackTrace);
               }
          }

          public void removeProperty() {
               ModelState.Remove("Merchant.MerchantName");
               ModelState.Remove("Merchant.Address");
               ModelState.Remove("Merchant.City");
               ModelState.Remove("Merchant.ZipCode");
               ModelState.Remove("Merchant.PrimaryContactNumber");
          }
     }
}
