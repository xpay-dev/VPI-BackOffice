using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace SDGAdmin.Controllers
{
    [System.Web.Http.Authorize]
    [CustomAttributes.SessionExpireFilter]
    public class AuditTrailController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
