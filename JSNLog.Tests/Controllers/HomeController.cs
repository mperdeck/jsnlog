using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JSNLog.Tests.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult JSTests()
        {
            return View();
        }

        public ActionResult NotEnabledTest()
        {
            return View();
        }

    }
}
