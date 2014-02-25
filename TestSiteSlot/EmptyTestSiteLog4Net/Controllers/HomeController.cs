using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using log4net;

namespace EmptyTestSiteLog4Net.Controllers
{
    public class HomeController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("serverlogger");

        public ActionResult Index()
        {
            ILog log = LogManager.GetLogger("serverlogger");
            log.Info("Info Message generated on server");
            log.Error("Error Message generated on server");

            return View();
        }

    }
}
