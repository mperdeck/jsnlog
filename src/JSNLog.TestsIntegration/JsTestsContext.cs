using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Xunit;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

namespace JSNLog.Tests.IntegrationTests
{
    public class JsTestsContext : IDisposable
    {
        public IWebDriver Driver
        {
            get; private set;
        }

        private WebServer _webServer = new WebServer();

        public JsTestsContext()
        {
            string jsnlogTestsProjectDirectory = Directory.GetCurrentDirectory();
            string jsnlogTestSiteProjectDirectory = 
                Path.GetFullPath(Path.Combine(jsnlogTestsProjectDirectory, "..\\..\\..\\", "JSNLog.TestSite"));

            _webServer.StartSite(jsnlogTestSiteProjectDirectory);

            Thread.Sleep(3000);

            // To use ChromeDriver, you must have chromedriver.exe. Download from
            // https://sites.google.com/a/chromium.org/chromedriver/downloads

            string dependenciesFolder =
                Path.GetFullPath(Path.Combine(jsnlogTestsProjectDirectory, "..\\..\\", "Dependencies"));

            Driver = new ChromeDriver(dependenciesFolder);
        }

        public void Dispose()
        {
            // Close the browser if there is no error. Otherwise leave open.
            if (!ErrorOnPage())
            {
                _webServer.StopSite();
                Driver.Quit();
            }
        }

        public void OpenPage(string relativeUrl)
        {
            string absoluteUrl = _webServer.SiteUrl + relativeUrl;
            Driver.Navigate().GoToUrl(absoluteUrl);
        }

        /// <summary>
        /// Returns true if there is an error element on the page, or if the "test running" message is still on the page
        /// (meaning the test js crashed).
        /// </summary>
        /// <returns></returns>
        public bool ErrorOnPage()
        {
            // Check for C# exception
            bool unhandledExceptionOccurred = Driver.PageSource.Contains("An unhandled exception occurred");
            bool noConnection = Driver.PageSource.Contains("ERR_CONNECTION_REFUSED");

            if (unhandledExceptionOccurred || noConnection)
            {
                return true;
            }
            
            try
            {
                // Throws NoSuchElementException if error-occurred not found
                Driver.FindElement(By.Id("loaded"));
            }
            catch (NoSuchElementException)
            {
                // page never even loaded
                return true;
            }

            try
            {
                // Throws NoSuchElementException if error-occurred not found
                Driver.FindElement(By.ClassName("error-occurred"));
            }
            catch (NoSuchElementException)
            {
                try
                {
                    // Throws NoSuchElementException if running not found
                    Driver.FindElement(By.Id("running"));
                }
                catch (NoSuchElementException)
                {
                    return false;
                }
            }

            return true;
        }
    }
}

