using System;
using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using Suteki.Shop.Extensions;
using Suteki.Shop.Repositories;
using Suteki.Shop.Services;

namespace Suteki.Shop.Tests.Services
{
    [TestFixture]
    public class MoveTests
    {
        [Test]
        public void MoveUpOne_ShouldMoveAnItemUpOnePlace()
        {
            IQueryable<IOrderable> things = MakeSomeThings().Select(t => (IOrderable)t).AsQueryable();
            Assert.AreEqual("three", ((Thing)things.ElementAt(2)).Name, "things are not in expected initial order");

            Move.ItemAt(3).In(things).UpOne();
            IEnumerable<IOrderable> orderedThings = things.InOrder();

            Assert.AreEqual("three", ((Thing)orderedThings.ElementAt(1)).Name, "Things have not been reordered");
        }

        [Test]
        public void MoveDownOne_ShouldMoveAnItemDownOnePlace()
        {
            IQueryable<IOrderable> things = MakeSomeThings().Select(t => (IOrderable)t).AsQueryable();
            Assert.AreEqual("two", ((Thing)things.ElementAt(1)).Name, "things are not in expected initial order");

            Move.ItemAt(2).In(things).DownOne();
            IEnumerable<IOrderable> orderedThings = things.InOrder();

            Assert.AreEqual("two", ((Thing)orderedThings.ElementAt(2)).Name, "Things have not been reordered");
        }

        [Test]
        public void MovingTheTopItemUpOne_ShouldHaveNoEffect()
        {
            IQueryable<IOrderable> things = MakeSomeThings().Select(t => (IOrderable)t).AsQueryable();
            Assert.AreEqual("one", ((Thing)things.ElementAt(0)).Name, "things are not in expected initial order");

            Move.ItemAt(1).In(things).UpOne();
            IEnumerable<IOrderable> orderedThings = things.InOrder();

            Assert.AreEqual("one", ((Thing)orderedThings.ElementAt(0)).Name, "Things have not been reordered");
        }

        [Test]
        public void MovingTheBottomItemDownOne_ShouldHaveNoEffect()
        {
            IQueryable<IOrderable> things = MakeSomeThings().Select(t => (IOrderable)t).AsQueryable();
            Assert.AreEqual("four", ((Thing)things.ElementAt(3)).Name, "things are not in expected initial order");

            Move.ItemAt(4).In(things).DownOne();
            IEnumerable<IOrderable> orderedThings = things.InOrder();

            Assert.AreEqual("four", ((Thing)orderedThings.ElementAt(3)).Name, "Things have not been reordered");
        }

        public static IEnumerable<Thing> MakeSomeThings()
        {
            IEnumerable<Thing> things = new Thing[]
            {
                new Thing { Name = "one", Position = 1 },
                new Thing { Name = "two", Position = 2 },
                new Thing { Name = "three", Position = 3 },
                new Thing { Name = "four", Position = 4 }
            };

            return things;
        }
    }


    public class Thing : IOrderable
    {
        public string Name { get; set; }
        public int Position { get; set; }
    }
}
