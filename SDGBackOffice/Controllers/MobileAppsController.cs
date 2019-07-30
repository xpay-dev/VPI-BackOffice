using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDG_BackOffice_Views.Controllers
{
    public class MobileAppsController : Controller
    {
        //
        // GET: /MobileApps/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GenerateActivationCodes()
        {
            return View();
        }
        public ActionResult ViewActivationCodes()
        {
            return View();
        }
    }
}
