using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

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
            _driver = new FirefoxDriver();
        }

        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            // Close the browser
            _driver.Quit();
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

