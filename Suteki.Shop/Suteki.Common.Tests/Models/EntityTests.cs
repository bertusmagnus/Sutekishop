using NUnit.Framework;
using Suteki.Common.Models;

namespace Suteki.Common.Tests.Models
{
    [TestFixture]
    public class EntityTests
    {
        [Test]
        public void IsNew_ShouldBeTrueWhenIdIsZero()
        {
            var thing = new Thing {Id = 0};
            Assert.That(thing.IsNew);
        }

        [Test]
        public void IsNew_ShouldBeFalseWhenIdIsNonZero()
        {
            var thing = new Thing {Id = 3};
            Assert.That(thing.IsNew, Is.False);
        }
    }

    public class Thing : Entity<Thing>
    {
        
    }
}