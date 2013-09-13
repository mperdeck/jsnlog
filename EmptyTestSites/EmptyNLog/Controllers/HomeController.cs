using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NLog;

namespace EmptyLog4Net.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            Logger logger = LogManager.GetLogger("serverlogger");
            logger.Warn("Warn Message generated on server");

            return View();
        }

    }
}
