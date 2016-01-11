using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Xunit;
using OpenQA.Selenium;
using System.Threading;

namespace JSNLog.Tests.IntegrationTests
{
    public class JsTests : IntegrationTestBase
    {
        public JsTests(): base()
        {
        }

        [Fact]
        public void JSTests()
        {
            OpenPage("/home/JSTests");

            Assert.False(ErrorOnPage());
        }

        [Fact]
        public void NotEnabledTest()
        {
            OpenPage("/home/NotEnabledTest");

            Assert.False(ErrorOnPage());
        }

        [Fact]
        public void MaxMessagesTest()
        {
            OpenPage("/home/MaxMessagesTest");

            Assert.False(ErrorOnPage());
        }

        [Fact]
        public void MaxMessagesTest0()
        {
            OpenPage("/home/MaxMessagesTest0");

            Assert.False(ErrorOnPage());
        }

        [Fact]
        public void MaxMessagesTestBatching()
        {
            OpenPage("/home/MaxMessagesTest0");

            Assert.False(ErrorOnPage());
        }

        [Fact]
        public void RequestIdTest()
        {
            OpenPage("/home/RequestIdTest");
            string requestId1 = RequestIdFieldsConsistent(false);

            OpenPage("/home/RequestIdTest");
            string requestId2 = RequestIdFieldsConsistent(false);

            Assert.NotEqual(requestId1, requestId2); // , "request ids are equal, so are not unique per request"

            OpenPage("/home/RequestIdTest/6789");
            string requestId3 = RequestIdFieldsConsistent(true);

            Assert.Equal(requestId3, "6789"); // , "JL.RequestId not the same as passed in"
        }

        [Fact]
        public void RequireJSTests()
        {
            OpenPage("/Html/requirejstest.html");

            // Wait a bit to let the JavaScript on the page finish
            Thread.Sleep(1000);

            Assert.False(ErrorOnPage());
        }

        [Fact]
        public void ExceptionTests()
        {
            OpenPage("/Html/exceptiontests.html");

            Assert.False(ErrorOnPage());
        }

        private string RequestIdFieldsConsistent(bool jlCanDifferFromOthers)
        {
            string idFromController = _driver.FindElement(By.Id("IdFromController")).Text;
            string idFromView = _driver.FindElement(By.Id("IdFromView")).Text;
            string idFromJL = _driver.FindElement(By.Id("IdFromJL")).Text;

            Assert.Equal(idFromView, idFromController); // , "request id not the same during request - 1"
            if (!jlCanDifferFromOthers)
            {
                Assert.Equal(idFromView, idFromJL); // , "request id not the same during request - 2"
            }

            return idFromJL;
        }
    }
}

