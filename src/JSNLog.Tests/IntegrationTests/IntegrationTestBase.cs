using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Xunit;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.IO;
using System.Reflection;

namespace JSNLog.Tests.IntegrationTests
{
    public class IntegrationTestBase : IDisposable
    {
        protected static IWebDriver _driver = null;

        // The port 31972 is set in the properties of the TestSite project
        private const string _baseUrl = "http://localhost:31972";

        // Runs before each test runs
        public IntegrationTestBase()
        {
            // To use ChromeDriver, you must have chromedriver.exe. Download from
            // https://sites.google.com/a/chromium.org/chromedriver/downloads

            string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string dependenciesFolder = Path.Combine(assemblyFolder, "Dependencies");
            _driver = new ChromeDriver(dependenciesFolder);
        }

        // Runs after each test has run
        public void Dispose()
        {
            // Close the browser if there is no error. Otherwise leave open.
            if (!ErrorOnPage())
            {
                _driver.Quit();
            }
        }

        public void OpenPage(string relativeUrl)
        {
            string absoluteUrl = _baseUrl + relativeUrl;
            _driver.Navigate().GoToUrl(absoluteUrl);
        }

        /// <summary>
        /// Returns true if there is an error element on the page, or if the "test running" message is still on the page
        /// (meaning the test js crashed).
        /// </summary>
        /// <returns></returns>
        public bool ErrorOnPage()
        {
            // Check for C# exception
            bool unhandledExceptionOccurred = _driver.PageSource.Contains("An unhandled exception occurred");
            bool noConnection = _driver.PageSource.Contains("ERR_CONNECTION_REFUSED");

            if (unhandledExceptionOccurred || noConnection)
            {
                return true;
            }

            try
            {
                // Throws NoSuchElementException if error-occurred not found
                _driver.FindElement(By.ClassName("error-occurred"));
            }
            catch(NoSuchElementException)
            {
                try
                {
                    // Throws NoSuchElementException if running not found
                    _driver.FindElement(By.Id("running"));
                }
                catch(NoSuchElementException)
                {
                    return false;
                }
            }

            return true;
        }
    }
}

