using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebSite.Areas.Version1.Controllers
{
    public class DocumentationController : Controller
    {
        //
        // GET: /Documentation/

        public ActionResult Index()
        {
            return View("Benefits");
        }

        public ActionResult Benefits()
        {
            return View();
        }

        public ActionResult Install()
        {
            return View();
        }

        public ActionResult GettingStarted()
        {
            return View();
        }

        public ActionResult JavaScriptFunctions()
        {
            return View();
        }

        public ActionResult Configuration()
        {
            return View();
        }

    }
}
