using System.Web;
using System.Web.Mvc;

namespace JSNLog.Tests
{
    internal class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}