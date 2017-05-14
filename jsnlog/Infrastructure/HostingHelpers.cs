using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JSNLog.Infrastructure
{
    public class HostingHelpers
    {
        public static string VirtualToAbsolutePath(string virtualPath)
        {
#if NET40
            return System.Web.VirtualPathUtility.ToAbsolute(virtualPath);
#else
            //TODO: virtual path transalation for DNX.
            // Probably get an instance of IApplicationEnvironment and then get the info from there.
            // If the application is in a virtual directory,
            // ~/myfile.jpg may have absolute url /myapp/myfile.jpg.
            // See
            // http://stackoverflow.com/questions/28082869/how-to-map-virtual-path-to-physical-path
            // http://stackoverflow.com/questions/33253608/asp-net-5-beta8-app-with-virtual-directories-applications
            // http://stackoverflow.com/questions/32631066/how-to-consistently-get-application-base-path-for-asp-net-5-dnx-project-on-both
            // http://stackoverflow.com/questions/30111920/how-do-i-access-the-iapplicationenvironment-from-a-unit-test

            // For now, just remove ~ from the left of the url
            return virtualPath.TrimStart('~');
#endif
        }
    }
}
