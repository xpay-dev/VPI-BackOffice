using SDGAdmin;
using SDGAdmin.Models;
using SDGDAL.Repositories;
using SDGUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDGAdmin.Controllers
{
    [Authorize]
    [CustomAttributes.SessionExpireFilter]
    public class ReportsController : Controller
    {
        //
        // GET: /Reports/
        MerchantRepository _merchantRepo;
        PartnerRepository _partnerRepo;
        TransactionRepository _transactionRepo;

        string action = string.Empty;

        public ReportsController()
        {
            _partnerRepo = new PartnerRepository();
            _merchantRepo = new MerchantRepository();
            _transactionRepo = new TransactionRepository();
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

        public void GetTransactionTypes()
        {
            try
            {
                action = "getting the transaction action types.";

                var ddlTransactionTypes = new List<SelectListItem>();

                ddlTransactionTypes.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = "All Actions"
                });

                ddlTransactionTypes.Add(new SelectListItem()
                {
                    Value = "3",
                    Text = "Purchased"
                });

                ddlTransactionTypes.Add(new SelectListItem()
                {
                    Value = "4",
                    Text = "Void"
                });

                ddlTransactionTypes.Add(new SelectListItem()
                {
                    Value = "6",
                    Text = "Declined"
                });

                ViewBag.TransactionTypes = ddlTransactionTypes;
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "GetTransactionTypes", ex.StackTrace);
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
                    Value = "3",
                    Text = "Manual Credit Transaction"
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
                                        
                ddlTransTypes.Add(new SelectListItem()
                {
                    Value = "9",
                    Text = "EMV"
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

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "GetTransType", ex.StackTrace);
            }
        }

        public ActionResult Index(int? id)
        {
            var userActivity = "Entered Reports Central Index Page";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "Index", "");

            GetPartner();

            GetTransactionTypes();

            GetTransType();

            return View();
        }

        public ActionResult ReportsGraph()
        {
            GetPartner();

            GetTransType();

            ViewBag.ParentId = CurrentUser.ParentId;

            return View();
        }
    }
}
