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
        private const string url = "http://ipv4.fiddler:49839/MetaWebLogAPI.svc";

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
            // some problem at the service end. This is not yet working :(

            var posts = client.getUsersBlogs("", "mike", "m1ke");

            Assert.That(posts.Length, Is.EqualTo(1));
            Console.WriteLine(posts[0].url);
            //Assert.That(posts[0].url, Is.EqualTo(theSiteUrl));
            
        }
    }
}