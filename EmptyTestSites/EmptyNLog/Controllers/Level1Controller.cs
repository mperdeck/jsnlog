using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NLog;

namespace EmptyLog4Net.Controllers
{
    public class Level1Controller : Controller
    {
        //
        // GET: /Home/

        public ActionResult Page1()
        {
            Logger logger = LogManager.GetLogger("serverlogger");
            logger.Warn("Warn Message generated on server, Level1/Page1");

            return View();
        }

    }
}
