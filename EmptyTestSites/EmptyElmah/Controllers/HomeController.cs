using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Elmah;

namespace EmptyElmah.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Exception Message generated on server"));
            return View();
        }

    }
}
