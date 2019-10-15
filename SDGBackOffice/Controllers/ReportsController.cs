using SDGBackOffice;
using SDGBackOffice.Models;
using SDGDAL.Repositories;
using SDGUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDGBackOffice.Controllers {
     [Authorize]
     [CustomAttributes.SessionExpireFilter]
     public class ReportsController : Controller {
          //
          // GET: /Reports/
          MerchantRepository _merchantRepo;
          PartnerRepository _partnerRepo;
          ResellerRepository _resellerRepo;
          TransactionRepository _transactionRepo;
          MerchantBranchRepository _branchRepo;
          MerchantBranchPOSRepository _posRepo;

          string action = string.Empty;

          public ReportsController() {
               _partnerRepo = new PartnerRepository();
               _resellerRepo = new ResellerRepository();
               _merchantRepo = new MerchantRepository();
               _transactionRepo = new TransactionRepository();
               _branchRepo = new MerchantBranchRepository();
               _posRepo = new MerchantBranchPOSRepository();
          }

          public void GetPartnerByPartnerId() {
               try {
                    action = "fetching the partner list";

                    var partners = _partnerRepo.GetAllPartnersByParent(CurrentUser.UserId, "");

                    var ddlPartners = new List<SelectListItem>();

                    if (partners.Count == 0) {
                         ddlPartners.Add(new SelectListItem() {
                              Value = "0",
                              Text = "No Partners available",
                              Selected = 0 == 0
                         });
                    } else {
                         ddlPartners.Add(new SelectListItem() {
                              Value = "0",
                              Text = "Select a Partner",
                              Selected = 0 == 0
                         });

                         ddlPartners.AddRange(partners.Select(p => new SelectListItem() {
                              Value = p.PartnerId.ToString(),
                              Text = p.CompanyName
                         }).ToList());
                    }

                    ViewBag.Partners = ddlPartners;
               } catch (Exception ex) {
                    var errorOnAction = "Error while " + action;

                    var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "GetPartner", ex.StackTrace);
               }
          }

          public void GetReseller() {
               try {
                    action = "fetching the partner list";

                    var reseller = _resellerRepo.GetAllResellers();

                    var rId = reseller.Select(r => r.PartnerId == CurrentUser.UserId);

                    var partners = _resellerRepo.GetAllResellersByPartner(CurrentUser.ParentId, "");

                    var ddlReseller = new List<SelectListItem>();

                    if (partners.Count != 0) {
                         if (partners.Count == 1) {
                              ddlReseller.AddRange(partners.Select(p => new SelectListItem() {
                                   Value = p.ResellerId.ToString(),
                                   Text = p.ResellerName
                              }).ToList());
                         } else {
                              ddlReseller.Add(new SelectListItem() {
                                   Value = "0",
                                   Text = "Select a Reseller",
                                   Selected = 0 == 0
                              });

                              ddlReseller.AddRange(partners.Select(p => new SelectListItem() {
                                   Value = p.ResellerId.ToString(),
                                   Text = p.ResellerName
                              }).ToList());
                         }
                    } else {
                         ddlReseller.Add(new SelectListItem() {
                              Value = "0",
                              Text = "No Resellers available",
                              Selected = 0 == 0
                         });
                    }



                    ViewBag.Resellers = ddlReseller;
               } catch (Exception ex) {
                    var errorOnAction = "Error while " + action;

                    var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "GetPartner", ex.StackTrace);
               }
          }

          public void GetTransactionTypes() {
               try {
                    action = "getting the transaction action types.";

                    var ddlTransactionTypes = new List<SelectListItem>();

                    ddlTransactionTypes.Add(new SelectListItem() {
                         Value = "0",
                         Text = "All Actions"
                    });

                    ddlTransactionTypes.Add(new SelectListItem() {
                         Value = "3",
                         Text = "Purchased"
                    });

                    ddlTransactionTypes.Add(new SelectListItem() {
                         Value = "4",
                         Text = "Void"
                    });

                    ddlTransactionTypes.Add(new SelectListItem() {
                         Value = "6",
                         Text = "Declined"
                    });

                    ViewBag.TransactionTypes = ddlTransactionTypes;
               } catch (Exception ex) {
                    var errorOnAction = "Error while " + action;

                    var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "GetTransactionTypes", ex.StackTrace);
               }
          }

          public void GetMerchantByReseller() {
               try {
                    action = "getting merchants by reseller id.";

                    var merchants = _merchantRepo.GetAllMerchantsByReseller(CurrentUser.ParentId, "");

                    var ddlMerchants = new List<SelectListItem>();

                    ddlMerchants.Add(new SelectListItem() {
                         Value = "0",
                         Text = "Select a Merchant",
                         Selected = 0 == 0
                    });

                    ddlMerchants.AddRange(merchants.Select(p => new SelectListItem() {
                         Value = p.MerchantId.ToString(),
                         Text = p.MerchantName
                    }).ToList());

                    ViewBag.Merchants = ddlMerchants;
               } catch (Exception ex) {
                    var errorOnAction = "Error while " + action;

                    var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "GetTransactionTypes", ex.StackTrace);
               }
          }

          public void GetBrachesByMerchant() {
               try {
                    action = "getting branches by merchant id.";

                    var branches = _branchRepo.GetAllBranchesByMerchant(CurrentUser.ParentId, "");

                    var ddlBranches = new List<SelectListItem>();

                    ddlBranches.Add(new SelectListItem() {
                         Value = "0",
                         Text = "Select a Merchant Branch",
                         Selected = 0 == 0
                    });

                    ddlBranches.AddRange(branches.Select(p => new SelectListItem() {
                         Value = p.MerchantBranchId.ToString(),
                         Text = p.MerchantBranchName
                    }).ToList());

                    ViewBag.Branches = ddlBranches;
               } catch (Exception ex) {
                    var errorOnAction = "Error while " + action;

                    var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "GetBrachesByMerchant", ex.StackTrace);
               }
          }

          public void GetPOSByBranch() {
               try {
                    action = "getting pos by branch id.";

                    var pos = _posRepo.GetAllPOSsByBranch(CurrentUser.ParentId, "");

                    var ddlPos = new List<SelectListItem>();

                    ddlPos.Add(new SelectListItem() {
                         Value = "0",
                         Text = "Select a POS",
                         Selected = 0 == 0
                    });

                    ddlPos.AddRange(pos.Select(p => new SelectListItem() {
                         Value = p.MerchantPOSId.ToString(),
                         Text = p.MerchantPOSName
                    }).ToList());

                    ViewBag.POS = ddlPos;
               } catch (Exception ex) {
                    var errorOnAction = "Error while " + action;

                    var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "GetPOSByBranch", ex.StackTrace);
               }
          }

          public void GetTransType() {
               try {
                    action = "getting transaction types.";

                    var ddlTransTypes = new List<SelectListItem>();

                    ddlTransTypes.Add(new SelectListItem() {
                         Value = "0",
                         Text = "All Transaction Types",
                         Selected = 0 == 0
                    });

                    ddlTransTypes.Add(new SelectListItem() {
                         Value = "3",
                         Text = "Manual Credit Transaction"
                    });

                    ddlTransTypes.Add(new SelectListItem() {
                         Value = "5",
                         Text = "Credit Transaction"
                    });

                    ddlTransTypes.Add(new SelectListItem() {
                         Value = "4",
                         Text = "Debit Transaction"
                    });

                    ///Uncomment the 2 more transaction type if needed
                    //ddlTransTypes.Add(new SelectListItem()
                    //{
                    //    Value = "3",
                    //    Text = "ACH Transaction"
                    //});

                    //ddlTransTypes.Add(new SelectListItem()
                    //{
                    //    Value = "4",
                    //    Text = "Cash Transaction"
                    //});

                    ViewBag.TransTypes = ddlTransTypes;
               } catch (Exception ex) {
                    var errorOnAction = "Error while " + action;

                    var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "GetTransType", ex.StackTrace);
               }
          }

          public ActionResult Index(int? id) {
               var uType = "";

               var userActivity = "Entered Reports Central Index Page";

               var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "Index", "");

               if (CurrentUser.ParentType == Enums.ParentType.Partner) {
                    GetPartnerByPartnerId();
                    uType = "partner";
               }

               if (SDGBackOffice.CurrentUser.ParentType == SDGBackOffice.Enums.ParentType.Reseller) {
                    GetMerchantByReseller();
                    uType = "reseller";
               } else if (SDGBackOffice.CurrentUser.ParentType == SDGBackOffice.Enums.ParentType.Merchant) {
                    GetBrachesByMerchant();
                    uType = "merchant";
               } else if (SDGBackOffice.CurrentUser.ParentType == SDGBackOffice.Enums.ParentType.MerchantLocation) {
                    GetPOSByBranch();
                    uType = "branch";
               }

               ViewBag.UserType = uType;
               //ViewBag.User = CurrentUser.UserId;
               ViewBag.User = CurrentUser.ParentId;

               GetReseller();
               //GetPartnerByPartnerId();

               GetTransactionTypes();

               GetTransType();

               ViewBag.CurrentUser = CurrentUser.ParentId;

               return View();
          }

          public ActionResult ReportsGraph() {
               var uType = "";

               GetReseller();

               if (SDGBackOffice.CurrentUser.ParentType == SDGBackOffice.Enums.ParentType.Reseller) {
                    GetMerchantByReseller();
                    uType = "reseller";
               } else if (SDGBackOffice.CurrentUser.ParentType == SDGBackOffice.Enums.ParentType.Merchant) {
                    GetBrachesByMerchant();
                    uType = "merchant";
               } else if (SDGBackOffice.CurrentUser.ParentType == SDGBackOffice.Enums.ParentType.MerchantLocation) {
                    GetPOSByBranch();
                    uType = "branch";
               }

               GetTransType();

               ViewBag.UserType = uType;

               return View();
          }
     }
}
