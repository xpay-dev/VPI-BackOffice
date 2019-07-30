using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDGBackOffice.Controllers
{
    public class ErrorsController : Controller
    {
        //
        // GET: /Errors/

        public ActionResult ServerError()
        {
            return View();
        }

        public ActionResult PageNotFound()
        {
            return View();
        }
    }
}
