using System;
using NUnit.Framework;
using Suteki.Shop.Validation;

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
    }
}
