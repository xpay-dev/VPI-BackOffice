using SDGAdmin.Models;
using SDGDAL.Entities;
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
    public class SwitchesController : Controller
    {
        SwitchRepository _switchRepo;
        PartnerRepository _partnerRepo;

        string action = string.Empty;

        public SwitchesController()
        {
            _switchRepo = new SwitchRepository();
            _partnerRepo = new PartnerRepository();
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

        public ActionResult SwitchesIndex()
        {
            var userActivity = "Entered Switch Management Index Page";

            var errRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "SwitchesIndex", "");

            return View();
        }
        public ActionResult Create()
        {
            var userActivity = "Entered Create Switch Page";

            var errRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "Create", "");

            return View();
        }

        [HttpPost]
        public ActionResult Create(SwitchModel switches)
        {
            if (!_switchRepo.IsSwitchNameAvailable(switches.SwitchName))
            {
                ModelState.AddModelError(string.Empty, "The Switch Name " + switches.SwitchName + " is not available.");
            }

            try
            {
                action = "creating switch.";

                if (ModelState.IsValid && switches != null)
                {
                    var s = new SDGDAL.Entities.Switch();
                    s.SwitchName = switches.SwitchName;
                    s.IsActive = true;
                    s.IsAddressRequired = true;

                    var switchess = _switchRepo.CreateSwitch(s);

                    if (switchess.SwitchId > 0)
                    {
                        var userActivity = "Registered a Switch";

                        var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "Create", "");

                        RedirectToAction("SwitchesIndex");
                    }
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "Create", ex.StackTrace);
            }

            return View(switches);
        }

        [HttpPost]
        public JsonResult UpdateSwitchStatus(SwitchModel switches, bool status, int id)
        {
            Switch sw = new Switch();
            try
            {
                action = "updating switch status.";

                var s = _switchRepo.GetSwitchById(id);

                sw.SwitchId = id;
                sw.SwitchName = s.SwitchName;
                sw.IsActive = status;
                sw.IsAddressRequired = s.IsAddressRequired;
                sw.DateCreated = s.DateCreated;
                sw.SwitchCode = s.SwitchCode;

                var switchess = _switchRepo.UpdateSwitchStatus(sw);

                var userActivity = "Updates Switch Status";

                var errRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "UpdateSwitchStatus", "");

                return Json(switchess);
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "Index", ex.StackTrace);
            }

            return Json(null);
        }

        [HttpPost]
        public ActionResult EnableSwitchforPartner(int id, int pId, bool isEnabled)
        {
            bool isExist = false;
            int switchPartnerId = 0;
            try
            {
                action = "enabling switch for partner.";

                var switchPartner = _switchRepo.GetAllSwitchPartnerLink();

                foreach (var s in switchPartner)
                {
                    if (id == s.SwitchId)
                    {
                        if (pId == s.PartnerId)
                        {
                            isExist = true;
                            switchPartnerId = s.SwitchPartnerLinkId;
                        }
                    }
                }

                if (isExist == true)
                {
                    var s = new SDGDAL.Entities.SwitchPartnerLink();
                    s.SwitchPartnerLinkId = switchPartnerId;
                    s.SwitchId = id;
                    s.PartnerId = pId;
                    s.IsEnabled = isEnabled;

                    var spl = _switchRepo.UpdatePartnerSwitch(s);

                    var userActivity = "Enables Switch for Partner";

                    var errRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "EnableSwitchforPartner", "");

                    return Json(spl);
                }
                else
                {
                    var s = new SDGDAL.Entities.SwitchPartnerLink();
                    s.SwitchId = id;
                    s.PartnerId = pId;
                    s.IsEnabled = true;

                    var switchess = _switchRepo.SavePartnerSwitch(s);

                    var userActivity = "Enables Switch for Partner";

                    var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "EnableSwitchforPartner", "");

                    return Json(switchess);
                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "EnableSwitchforPartner", ex.StackTrace);
            }

            return RedirectToAction("PartnerSwitches");
        }

        public ActionResult PartnerSwitches()
        {
            var userActivity = "Entered Available Switches for Partner";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "PartnerSwitches", "");

            if (CurrentUser.ParentType == Enums.ParentType.Partner)
            {
                GetPartner();
            }

            return View();
        }
    }
}
