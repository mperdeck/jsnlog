using System.Web.Mvc;

namespace WebSite.Areas.Version1
{
    public class Version1AreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Version1";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Version1_default",
                "Version1/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "WebSite.Areas.Version1.Controllers" } 
            );
        }
    }
}
