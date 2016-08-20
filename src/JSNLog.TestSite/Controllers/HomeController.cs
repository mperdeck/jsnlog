using System;
using System.Collections.Generic;
using System.Linq;
using JSNLog;
using System.Web.Mvc;
using System.Web;
using JSNLog.Infrastructure;

namespace JSNLog.TestSite.Controllers
{
    public class HomeController : Controller
    {
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

        public ActionResult MaxMessagesTest()
        {
            return View();
        }

        public ActionResult MaxMessagesTest0()
        {
            return View();
        }

        public ActionResult MaxMessagesTestBatching()
        {
            return View();
        }

        public ActionResult RequestIdTest(string id)
        {
            ViewBag.RequestId = JavascriptLogging.RequestId();
            ViewBag.PassedInRequestId = id;

            return View();
        }

    }
}
