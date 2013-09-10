using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewExtensions;

namespace MainSite.Controllers
{
    public class DocumentationController : Controller
    {
        public ActionResult Index(string pathInfo)
        {
            string view = Views.ByUrl(pathInfo).ViewPath;
            return View(view);
        }

    }
}
