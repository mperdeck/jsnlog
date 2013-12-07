using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JSNLog;

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
