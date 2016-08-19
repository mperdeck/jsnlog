using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JSNLog.Tests.IntegrationTests
{
    public class WebServer
    {
        private Process _iisServerProcess = null;

        /// <summary>
        /// Starts a web server for given web site.
        /// </summary>
        /// <param name="sitePath">
        /// Path to the files making up the site (that is, the directory with the web.config).
        /// </param>
        /// <returns>
        /// Process running the web server. Pass to StopWebServer to stop the web server.
        /// </returns>
        public void StartSite(string sitePath)
        {
            if (_iisServerProcess != null)
            {
                throw new Exception("A WebServer object can run only 1 site at the time. Call StopSite before running a new site.");
            }

            for(int port = 5000; port < 5100; port++)
            {
                string arguments = string.Format(@"/path:""{0}"" /port:{1}", sitePath, port);

                var key = Environment.Is64BitOperatingSystem ? "programfiles(x86)" : "programfiles";
                var programfiles = Environment.GetEnvironmentVariable(key);
                string pathIISExpress = string.Format(@"{0}\IIS Express\iisexpress.exe", programfiles);

                // Before you can run this code, make sure that IIS Express has been installed.
                _iisServerProcess = Process.Start(new ProcessStartInfo
                {
                    FileName = pathIISExpress,
                    Arguments = arguments,
                    WorkingDirectory = sitePath
                });

                Thread.Sleep(2000);

                if (!_iisServerProcess.HasExited)
                {
                    return;
                }
            }

            throw new Exception(string.Format("IIS Express could not be started - exit code: {0}. Before running these tests, " +
                "make sure IIS Express is not already running.",
                _iisServerProcess.ExitCode));
        }

        /// <summary>
        /// Stop the site currently running in this web server object.
        /// </summary>
        public void StopSite()
        {
            if (_iisServerProcess == null)
            {
                throw new Exception("No site running in this WebServer object.");
            }

            if (!_iisServerProcess.HasExited)
            {
                _iisServerProcess.Kill();
            }

            _iisServerProcess.Dispose();
            _iisServerProcess = null;
        }
    }
}
