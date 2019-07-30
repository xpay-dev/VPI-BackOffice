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
    public class SecurePaymentController : Controller
    {
        PartnerRepository _partnerRepo;
        UserRepository _userRepo;
        ReferenceRepository _refRepo;

        string action = string.Empty;

        public SecurePaymentController()
        {
            _partnerRepo = new PartnerRepository();
            _userRepo = new UserRepository();
            _refRepo = new ReferenceRepository();
        }

        //
        // GET: /Form/

        public ActionResult Index()
        {
            var userActivity = "Entered Partner Management Index";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "Index", "");

            if (Session["Success"] != null)
            {
                ViewBag.Success = Session["Success"];
            }

            Session["Success"] = null;

            return View();
        }

        public ActionResult Registration(int? id)
        {
            var userActivity = "Entered Partner Registration Page";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGBackOffice", userActivity, "Registration", "");

            if (id.HasValue)
            {
                var partner = new PartnerModel();
                partner.ParentPartner = new PartnerModel();
                partner.ParentPartner.PartnerId = id.Value;
                return View(partner);
            }
            return View(new PartnerModel());
        }

        public ActionResult Form1() 
        {

            return View();
        }

        public ActionResult Form2()
        {

            return View();
        }

        public ActionResult Form3()
        {

            return View();
        }


    }
}
