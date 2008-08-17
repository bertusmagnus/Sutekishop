using System;
using System.Text.RegularExpressions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Suteki.Common.Validation;

namespace Suteki.Shop.Tests.Validation
{
    [TestFixture]
    public class ValidationExtensionsTests
    {
        const string validationMessage = "You must enter a value for Property 1<br />You must enter a value for Property 2<br />You must enter a value for Property 3<br />";

        [Test, ExpectedException(typeof(ValidationException), ExpectedMessage = "You must enter a value for My Property")]
        public void IsRequired_ShouldNotValidateAnEmptyString()
        {
            string property = "";
            property.Label("My Property").IsRequired();
        }

        [Test, ExpectedException(typeof(ValidationException), ExpectedMessage = validationMessage)]
        public void CollectingExceptionsSpike()
        {
            string property = "";

            Validator validator = new Validator
            {
                () => property.Label("Property 1").IsRequired(),
                () => property.Label("Property 2").IsRequired(),
                () => property.Label("Property 3").IsRequired()
            };

            validator.Validate();
        }

        [Test]
        public void IsCreditCard_ShouldSuccessfullyValidateACreditCard()
        {
            string validCardNumber = "1111111111111117";

            validCardNumber.Label("Card Number").IsCreditCard();
        }

        [Test, ExpectedException(typeof(ValidationException))]
        public void IsCreditCard_ShouldNotValidateAnInvalidCreditCard()
        {
            string validCardNumber = "1111111111211117";

            validCardNumber.Label("Card Number").IsCreditCard();
        }

        [Test]
        public void IsCreditCard_ShouldSuccessfullyValidateACreditCardWithSpaces()
        {
            string validCardNumber = "1111 1111 1111 1117";
            validCardNumber.Label("Card Number").IsCreditCard();
        }

        [Test]
        public void IsCreditCard_ShouldSuccessfullyValidateACreditCardWithDashes()
        {
            string validCardNumber = "1111-1111-1111-1117";
            validCardNumber.Label("Card Number").IsCreditCard();
        }

        [Test]
        public void RegexNumberOnlySpike()
        {
            var value = "111 222-333abc";
            var expected = "111222333";
            var result = Regex.Replace(value, "[^0-9]", "");
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
