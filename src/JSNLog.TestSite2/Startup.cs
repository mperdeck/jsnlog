using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(JSNLog.TestSite2.Startup))]
namespace JSNLog.TestSite2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
