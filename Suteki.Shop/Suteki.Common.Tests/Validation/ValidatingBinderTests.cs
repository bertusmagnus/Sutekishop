using System;
using System.Globalization;
using System.Threading;
using System.Web.Mvc;
using NUnit.Framework;
using Suteki.Common.Validation;

namespace Suteki.Common.Tests.Validation
{
    [TestFixture]
    public class ValidatingBinderTests
    {
        private ValidatingBinder validatingBinder;

        [SetUp]
        public void SetUp()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-GB");

            validatingBinder = new ValidatingBinder(
                new SimplePropertyBinder(),
                new BooleanPropertyBinder());
        }

        [Test]
        public void ShouldBindCorrectValues()
        {
            var thing = new Thing
            {
                Id = 4,
                Name = "Henry",
                Date = new DateTime(2008, 09, 04),
                Price = 10.42M,
                IsValid = false, 
                Age = 1
            };

            var form = new FormCollection
            {
                { "Id", "4" },
                { "Name", "Joe" },
                { "Date", "2007-01-01" },
                { "Price", "2.45" },
                { "IsValid", "True,False" },
                { "Age", "34" }
            };

            validatingBinder.UpdateFrom(thing, form);

            Assert.That(thing.Id, Is.EqualTo(4));
            Assert.That(thing.Name, Is.EqualTo("Joe"));
            Assert.That(thing.Date, Is.EqualTo(new DateTime(2007, 01, 01)));
            Assert.That(thing.Price, Is.EqualTo(2.45M));
            Assert.That(thing.IsValid, Is.True);
            Assert.That(thing.Age, Is.EqualTo(34));
        }

        [Test, ExpectedException(typeof(ValidationException))]
        public void ShouldThrowValidationExceptions()
        {
            var thing = new Thing();

            var form = new FormCollection
            {
                { "Age", "0" }
            };

            validatingBinder.UpdateFrom(thing, form);
        }

        [Test, ExpectedException(typeof(ValidationException))]
        public void ShouldThrowTypeConversionExceptions()
        {
            var thing = new Thing();

            var form = new FormCollection
            {
                { "Age", "not a number" }
            };

            validatingBinder.UpdateFrom(thing, form);
        }

        [Test]
        public void ShouldSetNonPresentBoolValueToFalse()
        {
            var thing = new Thing {IsValid = true};
            var form = new FormCollection();

            validatingBinder.UpdateFrom(thing, form);

            Assert.That(thing.IsValid, Is.False);
        }

    	[Test]
    	public void Should_add_to_modelstate_without_prefix()
    	{
			var thing = new Thing();
			var form = new FormCollection 
			{
				{ "Age", "not a number" }
			};

			var modelState = new ModelStateDictionary();
			try 
			{
				validatingBinder.UpdateFrom(thing, form, modelState);
			}
			catch {}
			finally 
			{
				modelState.ContainsKey("Age").ShouldBeTrue();
			}
    	}

    	[Test]
    	public void Should_add_to_modelstate_with_prefix()
    	{
			var thing = new Thing();
			var form = new FormCollection 
			{
				{ "thing.Age", "not a number" }
			};

			var modelState = new ModelStateDictionary();
			try
			{
				validatingBinder.UpdateFrom(thing, form, modelState, "thing");				
			}
			catch {}
			finally
			{
				modelState.ContainsKey("thing.Age").ShouldBeTrue();				
			}
    	}

        public class Thing
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime Date { get; set; }
            public decimal Price { get; set; }
            public bool IsValid { get; set; }

            private int age;
            public int Age
            {
                get { return age; }
                set
                {
                    age = value.Label("Age").IsNonZero().Value;
                }
            }
        }
    }
}