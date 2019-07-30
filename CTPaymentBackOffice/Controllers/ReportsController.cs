using CTPaymentBackOffice;
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
    public class ReportsController : Controller
    {
        //
        // GET: /Reports/
        MerchantRepository _merchantRepo;
        PartnerRepository _partnerRepo;
        ResellerRepository _resellerRepo;
        TransactionRepository _transactionRepo;
        MerchantBranchRepository _branchRepo;

        string action = string.Empty;

        public ReportsController()
        {
            _partnerRepo = new PartnerRepository();
            _resellerRepo = new ResellerRepository();
            _merchantRepo = new MerchantRepository();
            _transactionRepo = new TransactionRepository();
            _branchRepo = new MerchantBranchRepository();
        }

        public void GetPartnerByPartnerId()
        {
            try
            {
                action = "fetching the partner list";

                var partners = _partnerRepo.GetAllPartnersByParent(CurrentUser.UserId, "");

                var ddlPartners = new List<SelectListItem>();

                if (partners.Count == 0)
                {
                    ddlPartners.Add(new SelectListItem()
                    {
                        Value = "0",
                        Text = "No Partners available",
                        Selected = 0 == 0
                    });
                }
                else
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

                ViewBag.Partners = ddlPartners;
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "GetPartner", ex.StackTrace);
            }
        }

        public void GetReseller()
        {
            try
            {
                action = "fetching the partner list";

                var reseller = _resellerRepo.GetAllResellers();

                var rId = reseller.Select(r => r.PartnerId == CurrentUser.UserId);

                var partners = _resellerRepo.GetAllResellersByPartner(CurrentUser.ParentId, "");

                var ddlReseller = new List<SelectListItem>();

                if (partners.Count != 0)
                {
                    if (partners.Count == 1)
                    {
                        ddlReseller.AddRange(partners.Select(p => new SelectListItem()
                        {
                            Value = p.ResellerId.ToString(),
                            Text = p.ResellerName
                        }).ToList());
                    }
                    else
                    {
                        ddlReseller.Add(new SelectListItem()
                        {
                            Value = "0",
                            Text = "Select a Reseller",
                            Selected = 0 == 0
                        });

                        ddlReseller.AddRange(partners.Select(p => new SelectListItem()
                        {
                            Value = p.ResellerId.ToString(),
                            Text = p.ResellerName
                        }).ToList());
                    }
                }
                else 
                {
                    ddlReseller.Add(new SelectListItem()
                    {
                        Value = "0",
                        Text = "No Resellers available",
                        Selected = 0 == 0
                    });
                }

                

                ViewBag.Resellers = ddlReseller;
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "GetPartner", ex.StackTrace);
            }
        }

        public void GetTransactionTypes()
        {
            try
            {
                action = "getting the transaction action types.";

                var transactiontypes = _transactionRepo.GetTransactionTypes();

                var ddlTransactionTypes = new List<SelectListItem>();

                ddlTransactionTypes.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = "All Actions",
                    Selected = 0 == 0
                });

                ddlTransactionTypes.AddRange(transactiontypes.Select(p => new SelectListItem()
                {
                    Value = p.TransactionTypeId.ToString(),
                    Text = p.Name
                }).ToList());

                ViewBag.TransactionTypes = ddlTransactionTypes;
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "GetTransactionTypes", ex.StackTrace);
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

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "GetTransactionTypes", ex.StackTrace);
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

        public void GetTransType()
        {
            try
            {
                action = "getting transaction types.";

                var ddlTransTypes = new List<SelectListItem>();

                ddlTransTypes.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = "All Transaction Types",
                    Selected = 0 == 0
                });

                ddlTransTypes.Add(new SelectListItem()
                {
                    Value = "5",
                    Text = "Credit Transaction"
                });

                ddlTransTypes.Add(new SelectListItem()
                {
                    Value = "4",
                    Text = "Debit Transaction"
                });

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
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGBackOffice", errorOnAction + "\n" + ex.Message, "GetTransType", ex.StackTrace);
            }
        }

        public ActionResult Index(int? id)
        {
            var userActivity = "Entered Reports Central Index Page";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "Index", "");

            if (CTPaymentBackOffice.CurrentUser.ParentType == CTPaymentBackOffice.Enums.ParentType.Reseller)
            {
                GetMerchantByReseller();
            }
            else if (CTPaymentBackOffice.CurrentUser.ParentType == CTPaymentBackOffice.Enums.ParentType.Merchant)
            {
                GetBrachesByMerchant();
            }

            ViewBag.User = CurrentUser.UserId;

            GetReseller();
            //GetPartnerByPartnerId();

            GetTransactionTypes();

            GetTransType();

            ViewBag.CurrentUser = CurrentUser.ParentId;

            return View();
        }

        public ActionResult ReportsGraph()
        {
            GetReseller();

            return View();
        }
    }
}
