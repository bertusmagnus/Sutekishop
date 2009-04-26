using System;
using System.ServiceModel;
using Microsoft.ServiceModel.Samples.XmlRpc;
using NUnit.Framework;
using Suteki.Shop.XmlRpc;

namespace Suteki.Shop.Tests.XmlRpc
{
    [TestFixture]
    public class MetaWeblogWcfIntegrationTests
    {
        private IMetaWeblog client;

        // this must be set to your test URL before attempting to run these tests
        // and the application must be running.
        // NOTE: these are integration tests and will insert data into the application database!

        private const string siteUrl = "http://localhost:50398/";

        // private const string url = "http://ipv4.fiddler:50398/MetaWebLogAPI.svc";
        private const string url = siteUrl + "MetaWebLogAPI.svc";

        [SetUp]
        public void SetUp()
        {
            var factory = new XmlRpcChannelFactory<IMetaWeblog>(new XmlRpcHttpBinding(), new EndpointAddress(url));
            client = factory.CreateChannel();
        }

        [TearDown]
        public void TearDown()
        {
            
        }

        [Test, Explicit("The application must be running at the given url before this test can be run")]
        public void Should_be_able_to_get_blogs()
        {
            // this user must be configured as an administrator for your instance
            var blogInfos = client.getUsersBlogs("", "mike@mike.com", "m1ke");

            Assert.That(blogInfos.Length, Is.EqualTo(1));
            Console.WriteLine(blogInfos[0].url);
            Assert.That(blogInfos[0].url, Is.EqualTo(siteUrl));
            
        }
    }
}