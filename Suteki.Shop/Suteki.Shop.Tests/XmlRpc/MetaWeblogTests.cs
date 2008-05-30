using CookComputing.XmlRpc;
using NUnit.Framework;
using Suteki.Shop.XmlRpc;
using System;

namespace Suteki.Shop.Tests.XmlRpc
{
    [TestFixture]
    public class MetaWeblogTests
    {
        /// <summary>
        /// Explicit test for the metaWeblog service. 
        /// If you want to run this, make sure you change the URL, username and password to values that you use.
        /// </summary>
        [Test, Explicit]
        public void GetRecentPosts_ShouldReturnAListOfRecentPosts()
        {
            var metaWeblog = XmlRpcProxyGen.Create<IMetaWeblog>();
            ((IXmlRpcProxy)metaWeblog).Url = "http://localhost:63638/metablogapi.aspx";
            var posts = metaWeblog.getRecentPosts("1", "admin@sutekishop.co.uk", "admin", 10);

            foreach(var post in posts)
            {
                Console.WriteLine(post.title);
            }
        }
    }
}
