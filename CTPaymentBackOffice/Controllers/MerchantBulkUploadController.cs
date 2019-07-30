using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CTPaymentBackOffice.Models;
using SDGDAL;
using SDGUtil;
using SDGDAL.Repositories;

namespace CTPaymentBackOffice.Controllers
{
    [Authorize]
    [CustomAttributes.SessionExpireFilter]
    public class MerchantBulkUploadController : Controller
    {
        ResellerRepository _resellerRepo;

        string action = String.Empty;

        public MerchantBulkUploadController()
        {
            _resellerRepo = new ResellerRepository();
        }

        public ActionResult Index()
        {
            var userActivity = "Entered Merchant Boarding Index";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "Index", "");

            if (Session["Success"] != null)
            {
                ViewBag.Success = Session["Success"];
            }

            Session["Success"] = null;

            GetResellers();

            return View();
        }

        public void GetResellers()
        {
            try
            {
                action = "fetching the resller list";

                var resellers = _resellerRepo.GetAllResellersByPartner(CurrentUser.ParentId, "");

                var ddlresellers = new List<SelectListItem>();

                ddlresellers.Add(new SelectListItem()
                {
                    Value = "0",
                    Text = "Select a Reseller",
                    Selected = 0 == 0
                });

                ddlresellers.AddRange(resellers.Select(r => new SelectListItem()
                {
                    Value = r.ResellerId.ToString(),
                    Text = r.ResellerName
                }).ToList());

                ViewBag.Resellers = ddlresellers;
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGbackOffice", errorOnAction + "\n" + ex.Message, "GetResellers", ex.StackTrace);
            }
        }

        public ActionResult GetAllMerchantsData(string id)
        {
            MerchantBulkModel model = new MerchantBulkModel();

            var data = id;

            return View();
        }
    }
}
