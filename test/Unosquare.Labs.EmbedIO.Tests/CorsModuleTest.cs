﻿namespace Unosquare.Labs.EmbedIO.Tests
{
    using NUnit.Framework;
    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Unosquare.Labs.EmbedIO.Modules;
    using Unosquare.Labs.EmbedIO.Tests.TestObjects;

    [TestFixture]
    public class CorsModuleTest
    {
        protected WebServer WebServer;
        protected TestConsoleLog Logger = new TestConsoleLog();

        [SetUp]
        public void Init()
        {
            WebServer = new WebServer(Resources.ServerAddress, Logger)
                .EnableCors(
                    "http://client.cors-api.appspot.com,http://unosquare.github.io,http://run.plnkr.co",
                    "content-type",
                    "post,get");

            WebServer.RegisterModule(new WebApiModule());
            WebServer.Module<WebApiModule>().RegisterController<TestController>();
            WebServer.RunAsync();
        }

        [Test]
        public async Task PreFlight()
        {
            var request = (HttpWebRequest) WebRequest.Create(Resources.ServerAddress + TestController.GetPath);
            request.Headers[Constants.HeaderOrigin] = "http://unosquare.github.io";
            request.Headers[Constants.HeaderAccessControlRequestMethod] = "post";
            request.Headers[Constants.HeaderAccessControlRequestHeaders] = "content-type";
            request.Method = "OPTIONS";

            using (var response = (HttpWebResponse) await request.GetResponseAsync())
            {
                Assert.AreEqual(response.StatusCode, HttpStatusCode.OK, "Status Code OK");
            }
        }

        [TearDown]
        public void Kill()
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
            WebServer.Dispose();
        }
    }
}