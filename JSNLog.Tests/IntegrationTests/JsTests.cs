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

        [TestMethod]
        public void MaxMessagesTest()
        {
            OpenPage("/home/MaxMessagesTest");

            Assert.IsFalse(ErrorOnPage());
        }

        [TestMethod]
        public void MaxMessagesTest0()
        {
            OpenPage("/home/MaxMessagesTest0");

            Assert.IsFalse(ErrorOnPage());
        }

        [TestMethod]
        public void MaxMessagesTestBatching()
        {
            OpenPage("/home/MaxMessagesTest0");

            Assert.IsFalse(ErrorOnPage());
        }

        [TestMethod]
        public void RequestIdTest()
        {
            OpenPage("/home/RequestIdTest");
            string requestId1 = RequestIdFieldsConsistent(false);

            OpenPage("/home/RequestIdTest");
            string requestId2 = RequestIdFieldsConsistent(false);

            Assert.AreNotEqual(requestId1, requestId2, "request ids are equal, so are not unique per request");

            OpenPage("/home/RequestIdTest/6789");
            string requestId3 = RequestIdFieldsConsistent(true);

            Assert.AreEqual(requestId3, "6789", "JL.RequestId not the same as passed in");
        }

        [TestMethod]
        public void RequireJSTests()
        {
            OpenPage("/Html/requirejstest.html");
            
            Assert.IsFalse(ErrorOnPage());
        }

        private string RequestIdFieldsConsistent(bool jlCanDifferFromOthers)
        {
            string idFromController = _driver.FindElement(By.Id("IdFromController")).Text;
            string idFromView = _driver.FindElement(By.Id("IdFromView")).Text;
            string idFromJL = _driver.FindElement(By.Id("IdFromJL")).Text;

            Assert.AreEqual(idFromView, idFromController, "request id not the same during request - 1");
            if (!jlCanDifferFromOthers)
            {
                Assert.AreEqual(idFromView, idFromJL, "request id not the same during request - 2");
            }

            return idFromJL;
        }
    }
}

