using NUnit.Framework;
using Suteki.Common.Tests.TestModel;
using Suteki.Common.Extensions;
using System.Collections.Generic;

namespace Suteki.Common.Tests.Extensions
{
    [TestFixture]
    public class TypeExtensionsTests
    {
        [Test]
        public void IsEntityCollection_Should_Return_True_If_Type_Is_Entity_Collection()
        {
            Assert.That(typeof(IEnumerable<Customer>).IsEntityCollection());
        }

        [Test]
        public void IsEntityCollection_Should_Return_False_If_Type_Is_Entity()
        {
            Assert.That(typeof(Customer).IsEntityCollection(), Is.False);
        }

        [Test]
        public void IsEntityCollection_Should_Return_False_If_Type_Is_Non_Entity_Collection()
        {
            Assert.That(typeof(IEnumerable<string>).IsEntityCollection(), Is.False);
        }
    }
}