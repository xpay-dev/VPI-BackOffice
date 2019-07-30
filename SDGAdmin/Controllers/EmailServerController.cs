using System;
using SDGAdmin.Models;
using SDGDAL.Entities;
using SDGDAL.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SDGUtil;
using System.Web.Services;
using SDGDAL;

namespace SDGAdmin.Controllers
{
    [Authorize]
    [CustomAttributes.SessionExpireFilter]
    public class EmailServerController : Controller
    {
        PartnerRepository _partnerRepo;
        EmailServerRepository _emailRepository;
        ReferenceRepository _refRepo;

        string action = string.Empty;

        public EmailServerController()
        {
            _partnerRepo = new PartnerRepository();
            _emailRepository = new EmailServerRepository();
            _refRepo = new ReferenceRepository();
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

        public ActionResult Index()
        {
            var userActivity = "Entered Email Server Index Page";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "EmailServerIndex", "");

            if (Session["Success"] != null)
            {
                ViewBag.Success = Session["Success"];
            }
            Session["Success"] = null;

            GetPartner();

            return View();
        }

        public ActionResult Create(string id)
        {
            var userActivity = "Entered Email Server Create Page";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "Create", "");

            TempData["PartnerId"] = Convert.ToInt32(Utility.Decrypt(id.Contains(" ") == true ? id.Replace(" ", "+") : id));

            return View(new EmailServerModel());
        }

        [HttpPost]
        public ActionResult Create(EmailServerModel email)
        {
            try
            {
                action = "creating email server.";

                if (ModelState.IsValid && email != null)
                {
                    var eserver = new SDGDAL.Entities.EmailServer();
                    eserver.Email = email.EmailAddress;
                    eserver.EmailServerName = email.EmailServerName;
                    eserver.Host = email.Host;
                    eserver.Port = email.Port;
                    eserver.Username = email.Username;
                    eserver.Password = email.Password;
                    eserver.UseSSL = email.UseSSL;
                    eserver.DefaultCredential = email.DefaultCredential;
                    eserver.IsActive = true;
                    eserver.IsDeleted = false;
                    eserver.PartnerId = Convert.ToInt32(TempData["PartnerId"]);
                    eserver.IsPartnerDefaultEmail = email.DefaultEmailServer;

                    List<EmailServer> LEServer = _emailRepository.GetEmailServerByPartnerId(eserver.PartnerId);

                    if (LEServer.Count() > 0 && email.DefaultEmailServer)
                    {
                        TempData["OldEServer"] = LEServer[0];
                        TempData["PartnerId"] = eserver.PartnerId;
                        TempData["EServer"] = eserver;
                        ViewBag.Create = "Are you sure you want to make this email as a default email server?";
                        return View();
                    }
                    else
                    {
                        if (LEServer.Count() == 0 && !email.DefaultEmailServer)
                        {
                            eserver.IsPartnerDefaultEmail = true;
                        }

                        CreateEmailServer(eserver);

                        return RedirectToAction("Index");
                    }

                }
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "Index", ex.StackTrace);

                ErrorLog err = new ErrorLog();
                err.Action = "Create Email Server";
                err.Method = "Create";
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
        public ActionResult ConfirmationCreate(string submitButton)
        {
            switch (submitButton)
            {
                case "Yes":
                    EmailServer eServer = (EmailServer)TempData["OldEServer"];
                    eServer.IsPartnerDefaultEmail = false;
                    UpdateDefaultEmail(eServer);
                    CreateEmailServer((EmailServer)TempData["EServer"]);

                    return RedirectToAction("Index");
                case "No":
                    return RedirectToAction("Create", new { id = Convert.ToInt32(TempData["MerchantId"]) });


            }
            return View();
        }
        private void CreateEmailServer(EmailServer server)
        {
            try
            {
                var es = _emailRepository.CreateEmailServer(server);
                var userActivity = "Registered Email Server";
                var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "Create", "");
                Session["Success"] = "Email Server created.";
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "Index", ex.StackTrace);

                ErrorLog err = new ErrorLog();
                err.Action = "Create Email Server";
                err.Method = "Create";
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

        public ActionResult EmailServers(string id)
        {
            var userActivity = "Entered Email Server Page";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "EmailServers", "");

            if (Session["Success"] != null)
            {
                ViewBag.Success = Session["Success"];
            }

            Session["Success"] = null;
            if (id != string.Empty)
            {
                ViewBag.PartnerId = Convert.ToInt32(Utility.Decrypt(id.Contains(" ") == true ? id.Replace(" ", "+") : id));
            }
            else
            {
                ViewBag.PartnerId = Convert.ToInt32(TempData["pId"]);
            }

            return View();
        }

        public ActionResult UpdateEmailServerInfo(string id)
        {
            var userActivity = "Entered Update Email Server Page";

            var actRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "UpdateEmailServerInfo", "");

            EmailServerModel es = new EmailServerModel();
            try
            {
                action = "fetching the data of email server.";

                var e = _emailRepository.GetEmailServerById(Convert.ToInt32(Utility.Decrypt(id.Contains(" ") == true ? id.Replace(" ", "+") : id)));

                TempData["EmailServerId"] = e.EmailServerId;
                es.EmailServerName = e.EmailServerName;
                es.EmailAddress = e.Email;
                es.Host = e.Host;
                es.Port = e.Port;
                es.Username = e.Username;
                es.Password = e.Password;
                es.UseSSL = e.UseSSL;
                es.DefaultCredential = e.DefaultCredential;
                es.DateCreated = e.DateCreated;
                es.IsActive = e.IsActive;
                es.IsDeleted = e.IsDeleted;
                es.MerchantId = e.PartnerId;
                es.DefaultEmailServer = e.IsPartnerDefaultEmail;
                TempData["pId"] = e.PartnerId;

            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "UpdateEmailServerInfo", ex.StackTrace);
            }

            return View(es);
        }


        [HttpPost]
        public ActionResult UpdateEmailServerInfo(EmailServerModel email)
        {
            EmailServer es = new EmailServer();
            string actionName = string.Empty;
            try
            {
                action = "updating the email server info.";

                var e = _emailRepository.GetEmailServerById(Convert.ToInt32(TempData["EmailServerId"]));

                es.EmailServerId = Convert.ToInt32(TempData["EmailServerId"]);
                es.EmailServerName = email.EmailServerName;
                es.Email = email.EmailAddress;
                es.Host = email.Host;
                es.Port = email.Port;
                es.Username = email.Username;
                es.Password = email.Password;
                es.UseSSL = email.UseSSL;
                es.DefaultCredential = email.DefaultCredential;
                es.DateCreated = e.DateCreated;
                es.IsActive = e.IsActive;
                es.IsDeleted = e.IsDeleted;
                es.PartnerId = e.PartnerId;
                es.IsPartnerDefaultEmail = email.DefaultEmailServer;

                action = UpdateDefaultEmail(es);
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "UpdateEmailServerInfo", ex.StackTrace);

                ErrorLog err = new ErrorLog();
                err.Action = "Update Email Server Information";
                err.Method = "UpdateEmailServerInfo";
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
            return RedirectToAction(actionName);
        }
        [HttpPost]
        public ActionResult ConfirmationUpdate(string submitButton)
        {
            switch (submitButton)
            {
                case "Yes":
                    EmailServer server = (EmailServer)TempData["OldDefaultEmail"];
                    server.IsPartnerDefaultEmail = false;
                    var emailserver = _emailRepository.UpdateEmailServer(server);
                    var userActivity = "Updates Email Server Info";
                    var errRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "UpdateEmailServerInfo", "");

                    return RedirectToAction(UpdateDefaultEmail((EmailServer)TempData["NewDefaultEmail"]));
                case "No":
                    return RedirectToAction("UpdateEmailServerInfo", new { id = (int)TempData["EmailServerId"] });
                    break;
            }
            return View();
        }

        private string UpdateDefaultEmail(EmailServer es)
        {
            var emailserver = _emailRepository.UpdateEmailServer(es);
            var userActivity = "Updates Email Server Info";
            var errRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "UpdateEmailServerInfo", "");
            Session["Success"] = "Successfully Updated.";
            return "EmailServers";
        }


        [HttpPost]
        public ActionResult UpdateEmailServer(int id, int pId, bool isActive, bool isDeleted)
        {
            EmailServer es = new EmailServer();

            try
            {
                action = "updating/deleting the email server status";

                var e = _emailRepository.GetEmailServerById(id);

                es.EmailServerId = id;
                es.EmailServerName = e.EmailServerName;
                es.Email = e.Email;
                es.Host = e.Host;
                es.Port = e.Port;
                es.Username = e.Username;
                es.Password = e.Password;
                es.UseSSL = e.UseSSL;
                es.DefaultCredential = e.DefaultCredential;
                es.DateCreated = e.DateCreated;
                es.IsActive = isActive;
                es.IsDeleted = isDeleted;
                es.PartnerId = pId;

                var emailserver = _emailRepository.UpdateEmailServer(es);

                var userActivity = "Updates Email Server Status";

                var errRefNumber = ApplicationLog.UserActivityLog("SDGAdmin", userActivity, "UpdateEmailServer", "");

                return Json(emailserver);
            }
            catch (Exception ex)
            {
                var errorOnAction = "Error while " + action;

                var errRefNumber = ApplicationLog.LogError("SDGAdmin", errorOnAction + "\n" + ex.Message, "UpdateEmailServer", ex.StackTrace);

                ErrorLog err = new ErrorLog();
                err.Action = "Features: UpdateEmailServer";
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
    }
}
