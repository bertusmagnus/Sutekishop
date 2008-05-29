using System;
using System.Linq;
using NUnit.Framework;
using Suteki.Common.Repositories;
using Suteki.Shop.Tests.Services;
using Suteki.Shop.Repositories;

namespace Suteki.Shop.Tests.Repositories
{
    [TestFixture]
    public class OrderableExtensionsTests
    {
        [Test]
        public void Before_ShouldReturnTheItemBefore()
        {
            var things = MoveTests.MakeSomeThings().Where(t => t.FooId == 2).AsQueryable(); // thing pos = 1 and 4
            Thing thingBeforePosition4 = things.GetItemBefore(4);
            Assert.AreEqual("one", thingBeforePosition4.Name);
        }

        [Test]
        public void After_ShouldReturnTheItemAfter()
        {
            var things = MoveTests.MakeSomeThings().Where(t => t.FooId == 2).AsQueryable(); // thing pos = 1 and 4
            Thing thingAfterPosition1 = things.GetItemAfter(1);
            Assert.AreEqual("four", thingAfterPosition1.Name);
        }
    }
}
