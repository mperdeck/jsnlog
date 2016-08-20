using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Xunit;
using OpenQA.Selenium;
using System.Threading;

namespace JSNLog.Tests.IntegrationTests
{
    [Collection("JSNLog")]
    public class JsTests : IClassFixture<JsTestsContext>
    {
        JsTestsContext _context;

        public JsTests(JsTestsContext context)
        {
            _context = context;
        }

        [Fact]
        public void JSTests()
        {
            _context.OpenPage("/home/JSTests");

            Assert.False(_context.ErrorOnPage());
        }

        [Fact]
        public void NotEnabledTest()
        {
            _context.OpenPage("/home/NotEnabledTest");

            Assert.False(_context.ErrorOnPage());
        }

        [Fact]
        public void MaxMessagesTest()
        {
            _context.OpenPage("/home/MaxMessagesTest");

            Assert.False(_context.ErrorOnPage());
        }

        [Fact]
        public void MaxMessagesTest0()
        {
            _context.OpenPage("/home/MaxMessagesTest0");

            Assert.False(_context.ErrorOnPage());
        }

        [Fact]
        public void MaxMessagesTestBatching()
        {
            _context.OpenPage("/home/MaxMessagesTestBatching");

            Assert.False(_context.ErrorOnPage());
        }

        [Fact]
        public void RequestIdTest()
        {
            _context.OpenPage("/home/RequestIdTest");
            string requestId1 = RequestIdFieldsConsistent(false);

            _context.OpenPage("/home/RequestIdTest");
            string requestId2 = RequestIdFieldsConsistent(false);

            Assert.NotEqual(requestId1, requestId2); // , "request ids are equal, so are not unique per request"

            _context.OpenPage("/home/RequestIdTest/6789");
            string requestId3 = RequestIdFieldsConsistent(true);

            Assert.Equal(requestId3, "6789"); // , "JL.RequestId not the same as passed in"
        }

        [Fact]
        public void RequireJSTests()
        {
            _context.OpenPage("/Html/requirejstest.html");

            // Wait a bit to let the JavaScript on the page finish
            Thread.Sleep(1000);

            Assert.False(_context.ErrorOnPage());
        }

        [Fact]
        public void ExceptionTests()
        {
            _context.OpenPage("/Html/exceptiontests.html");

            Assert.False(_context.ErrorOnPage());
        }

        private string RequestIdFieldsConsistent(bool jlCanDifferFromOthers)
        {
            string idFromController = _context.Driver.FindElement(By.Id("IdFromController")).Text;
            string idFromView = _context.Driver.FindElement(By.Id("IdFromView")).Text;
            string idFromJL = _context.Driver.FindElement(By.Id("IdFromJL")).Text;

            Assert.Equal(idFromView, idFromController); // , "request id not the same during request - 1"
            if (!jlCanDifferFromOthers)
            {
                Assert.Equal(idFromView, idFromJL); // , "request id not the same during request - 2"
            }

            return idFromJL;
        }
    }
}

