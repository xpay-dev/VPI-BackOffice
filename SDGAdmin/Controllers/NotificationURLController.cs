using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace SDGAdmin.Controllers
{
    public class NotificationURLController : Controller
    {
        public ActionResult NovaToPayNotification()
        {
            return View();
        }
    }
}
