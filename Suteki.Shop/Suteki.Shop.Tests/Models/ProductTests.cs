using NUnit.Framework;

namespace Suteki.Shop.Tests.Models
{
    [TestFixture]
    public class ProductTests 
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void PlainTextDescription_should_strip_HTML_and_quotes()
        {
            var product = new Product
            {
                Description = description
            };
            product.PlainTextDescription.ShouldEqual(expectedPlainTextDescription);
        }

        const string description = 
@"<p>This is an exact copy by ""North Sea Clothing"" of the Royal Navy sweater worn during the Atlantic convoys of WW2.</p>
<p>Made in the Uk from 100% English wool. Adapted by a lot of Rockers in the 50's to go under their leathers and we think well worthy of a place on our site and in our shop.</p>
<p>Reasonably fitted with a deep ribbed waistband.</p>

<p>Also available in White. Not all in stock at the moment but we will let you know how long once order placed. We hope within a week.</p>";

        const string expectedPlainTextDescription =
@"This is an exact copy by North Sea Clothing of the Royal Navy sweater worn during the Atlantic convoys of WW2.Made in the Uk from 100% English wool. Adapted by a lot of Rockers in the 50's to go under their leathers and we think well worthy of a place on our site and in our shop.Reasonably fitted with a deep ribbed waistband.Also available in White. Not all in stock at the moment but we will let you know how long once order placed. We hope within a week.";
    }
}