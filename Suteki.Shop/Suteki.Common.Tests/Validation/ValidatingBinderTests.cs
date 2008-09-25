using System;
using NUnit.Framework;
using System.Collections.Specialized;
using Suteki.Common.Validation;

namespace Suteki.Common.Tests.Validation
{
    [TestFixture]
    public class ValidatingBinderTests
    {
        [Test]
        public void UpdateFrom_ShouldUpdateObjectWithValues()
        {
            var values = new NameValueCollection
                             {
                                 {"userId", "4"},
                                 {"email", "mike@mike.com"},
                                 {"thedate", "24/3/2008"},
                                 {"name", "mike"}
                             };

            var testThing = new TestThing();

            ValidatingBinder.UpdateFrom(testThing, values);

            Assert.AreEqual(4, testThing.UserId);
            Assert.AreEqual("mike@mike.com", testThing.Email);
            Assert.AreEqual(new DateTime(2008, 3, 24), testThing.TheDate);
            Assert.AreEqual("mike", testThing.Name);
        }

        [Test]
        public void UpdateFrom_MissingValuesShouldNotBeUpdated()
        {
            // no values
            var values = new NameValueCollection();

            var testThing = new TestThing
                                      {
                                          UserId = 5,
                                          Email = "mike@mike.com",
                                          TheDate = new DateTime(2008, 1, 1),
                                          Name = "mike"
                                      };

            ValidatingBinder.UpdateFrom(testThing, values);

            Assert.AreEqual(5, testThing.UserId);
            Assert.AreEqual("mike@mike.com", testThing.Email);
            Assert.AreEqual(new DateTime(2008, 1, 1), testThing.TheDate);
            Assert.AreEqual("mike", testThing.Name);
        }

        [Test]
        public void UpdateFrom_ShouldNotAllowHtmlInjection()
        {
            var values = new NameValueCollection
                             {
                                 {"name", "<script></script>"}
                             };

            var testThing = new TestThing();

            ValidatingBinder.UpdateFrom(testThing, values);

            Assert.AreEqual("&lt;script&gt;&lt;/script&gt;", testThing.Name);
        }

        [Test] 
        [ExpectedException(
            typeof(ValidationException), 
            ExpectedMessage = "'not a number' is not a valid value for UserId<br />")]
        public void UpdateFrom_TypeConversionErrorsShouldBeProperlyReported()
        {
            var values = new NameValueCollection {{"userid", "not a number"}};

            var testThing = new TestThing();

            ValidatingBinder.UpdateFrom(testThing, values);
        }

        [Test]
        [ExpectedException(typeof(ValidationException), ExpectedMessage = "You must enter a value for Name<br />")]
        public void UpdateFrom_ValidationErrorsShouldBeProperlyDisplayed()
        {
            var values = new NameValueCollection {{"name", ""}};

            var testThing = new TestThing();

            ValidatingBinder.UpdateFrom(testThing, values);
        }
    }

    public class TestThing
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public DateTime TheDate { get; set; }

        string name;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                value.Label("Name").IsRequired();
                name = value;
            }
        }
    }
}