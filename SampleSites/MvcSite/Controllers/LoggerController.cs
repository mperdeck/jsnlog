using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Web.Script.Serialization;

namespace MvcSite.Controllers
{
    public class LoggerController : Controller
    {
        //
        // GET: /Logger/

        public class LogItems : List<Dictionary<string, Object>>
        {
        }

        //
        // GET: /Default/

        public ActionResult Index(string data)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();

            LogItems logItems = js.Deserialize<LogItems>(data);


            return Json("ok", "text/x-json", System.Text.Encoding.UTF8);
        }

    }
}
