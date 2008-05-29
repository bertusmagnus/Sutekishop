using System;
using NUnit.Framework;
using System.Collections.Specialized;
using Suteki.Common.Validation;
using System.Web.Mvc;

namespace Suteki.Shop.Tests.Validation
{
    [TestFixture]
    public class ValidatingBinderTests
    {
        [Test]
        public void UpdateFrom_ShouldUpdateObjectWithValues()
        {
            NameValueCollection values = new NameValueCollection();
            values.Add("userId", "4");
            values.Add("email", "mike@mike.com");
            values.Add("thedate", "24/3/2008");
            values.Add("name", "mike");

            TestThing testThing = new TestThing();

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
            NameValueCollection values = new NameValueCollection();

            TestThing testThing = new TestThing
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
        [ExpectedException(
            typeof(ValidationException), 
            ExpectedMessage = "'not a number' is not a valid value for UserId<br />")]
        public void UpdateFrom_TypeConversionErrorsShouldBeProperlyReported()
        {
            NameValueCollection values = new NameValueCollection();
            values.Add("userid", "not a number");

            TestThing testThing = new TestThing();

            ValidatingBinder.UpdateFrom(testThing, values);
        }

        [Test]
        [ExpectedException(typeof(ValidationException), ExpectedMessage = "You must enter a value for Name<br />")]
        public void UpdateFrom_ValidationErrorsShouldBeProperlyDisplayed()
        {
            NameValueCollection values = new NameValueCollection();
            values.Add("name", "");

            TestThing testThing = new TestThing();

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
