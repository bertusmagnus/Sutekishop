using System;
using System.Web;
using Moq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Suteki.Common.Extensions;

namespace Suteki.Common.Tests.Extensions
{
    [TestFixture]
    public class UrlExtensionsHelperTests
    {
        private UrlExtensionsHelper urlExtensionsHelper;

        [SetUp]
        public void SetUp()
        {
            urlExtensionsHelper = new Mock<UrlExtensionsHelper>().Object;

            Mock.Get(urlExtensionsHelper).Expect(eh => eh.UseSsl()).Returns(true);
        }

        [Test]
        public void ToSslUrl_ShouldAddHttpsToExistingUrlWithQueryString()
        {
            const string currentUrl = "http://jtg.sutekishop.co.uk/shop/Order/UpdateCountry/66?countryId=1";
            Mock.Get(urlExtensionsHelper).Expect(eh => eh.GetRequestUri()).Returns(new Uri(currentUrl));

            var existingUrl = "/shop/Order/PlaceOrder";
            var expectedUrl = "https://jtg.sutekishop.co.uk/shop/Order/PlaceOrder";

            Assert.That(urlExtensionsHelper.ToSslUrl(existingUrl), Is.EqualTo(expectedUrl));
        }

        [Test]
        public void ToSslUrl_ShouldAddHttpsToExistingUrlWithoutQueryString()
        {
            const string currentUrl = "http://jtg.sutekishop.co.uk/shop/Order/UpdateCountry/66";
            Mock.Get(urlExtensionsHelper).Expect(eh => eh.GetRequestUri()).Returns(new Uri(currentUrl));

            var existingUrl = "/shop/Order/PlaceOrder";
            var expectedUrl = "https://jtg.sutekishop.co.uk/shop/Order/PlaceOrder";

            Assert.That(urlExtensionsHelper.ToSslUrl(existingUrl), Is.EqualTo(expectedUrl));
        }

        [Test]
        public void ToSslUrl_ShouldAddHttpsToExistingUrlWithHttps()
        {
            const string currentUrl = "https://jtg.sutekishop.co.uk/shop/Order/UpdateCountry/66";
            Mock.Get(urlExtensionsHelper).Expect(eh => eh.GetRequestUri()).Returns(new Uri(currentUrl));

            var existingUrl = "/shop/Order/PlaceOrder";
            var expectedUrl = "https://jtg.sutekishop.co.uk/shop/Order/PlaceOrder";

            Assert.That(urlExtensionsHelper.ToSslUrl(existingUrl), Is.EqualTo(expectedUrl));
        }
    }
}