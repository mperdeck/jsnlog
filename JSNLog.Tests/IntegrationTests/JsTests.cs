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
    public class JsTests : IntegrationTestBase
    {
        public JsTests(): base()
        {
        }

        [TestMethod]
        public void JSTests()
        {
            OpenPage("/home/JSTests");

            Assert.IsFalse(ErrorOnPage());
        }

        [TestMethod]
        public void NotEnabledTest()
        {
            OpenPage("/home/NotEnabledTest");

            Assert.IsFalse(ErrorOnPage());
        }
    }
}