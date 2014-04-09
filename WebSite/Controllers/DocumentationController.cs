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
            string url = HttpContext.Request.Url.AbsolutePath.Trim(new char[] {'/'});
            string view = Views.ByUrl(url).ViewPath;
            return View(view);
        }

    }
}
