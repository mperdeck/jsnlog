using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
//##############  using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using System.IO;
using System.Reflection;

namespace JSNLog.Tests.IntegrationTests
{
    [TestClass]
    public class IntegrationTestBase
    {
        protected static IWebDriver _driver = null;

        // The port 31972 is set in the properties of this project
        private const string _baseUrl = "http://localhost:31972";

        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {
            //###########            _driver = new FirefoxDriver();

            // To use ChromeDriver, you must have chromedriver.exe. Download from
            // https://sites.google.com/a/chromium.org/chromedriver/downloads

            string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string dependenciesFolder = Path.Combine(assemblyFolder, "../Dependencies");
            _driver = new ChromeDriver(dependenciesFolder);
        }

        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
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
            if (_driver.PageSource.Contains("An unhandled exception occurred"))
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

