using NUnit.Framework;

namespace Suteki.Shop.Tests.Models
{
    [TestFixture]
    public class TextContentTests
    {
        [Test]
        public void UrlName_ShouldReturnAnyNonNameCharactersAsUnderscores()
        {
            string name = "That's how (he &) I like £$$$";
            string expectedName = "That_s_how__he____I_like_____";

            TextContent textContent = new TextContent
            {
                Name = name
            };

            string urlName = textContent.UrlName;

            Assert.That(urlName, Is.EqualTo(expectedName));
        }
    }
}
