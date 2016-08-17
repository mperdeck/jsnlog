using System;
using System.Collections.Generic;
using System.Linq;
using JSNLog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using JSNLog.Infrastructure;

namespace JSNLog.Tests.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

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
            ViewBag.RequestId = _httpContextAccessor.HttpContext.GetRequestId();
            ViewBag.PassedInRequestId = id;

            return View();
        }

    }
}
