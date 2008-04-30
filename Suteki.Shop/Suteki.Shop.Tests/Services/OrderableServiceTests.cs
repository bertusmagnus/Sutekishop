using System;
using System.Linq;
using System.Data.Linq;
using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using Suteki.Shop.Extensions;
using Suteki.Shop.Repositories;
using Suteki.Shop.Services;

namespace Suteki.Shop.Tests.Services
{
    [TestFixture]
    public class OrderableServiceTests
    {
        IRepository<Thing> thingRepository;
        IOrderableService<Thing> orderService;

        [SetUp]
        public void SetUp()
        {
            thingRepository = new Mock<IRepository<Thing>>().Object;
            orderService = new OrderableService<Thing>(thingRepository);
        }

        [Test]
        public void MoveUpShouldMoveElementUp()
        {
            IQueryable<Thing> things = MoveTests.MakeSomeThings().AsQueryable();

            Mock.Get(thingRepository).Expect(tr => tr.GetAll()).Returns(things).Verifiable();
            Mock.Get(thingRepository).Expect(tr => tr.SubmitChanges()).Verifiable();

            // move two up to top
            orderService.MoveItemAtPosition(2).UpOne();

            Assert.AreEqual("two", things.Single(t => t.Position == 1).Name); 
            Mock.Get(thingRepository).Verify();
        }

        [Test]
        public void MoveDownShouldMoveElementDown()
        {
            IQueryable<Thing> things = MoveTests.MakeSomeThings().AsQueryable();

            Mock.Get(thingRepository).Expect(tr => tr.GetAll()).Returns(things).Verifiable();
            Mock.Get(thingRepository).Expect(tr => tr.SubmitChanges()).Verifiable();

            // move three down to bottom
            orderService.MoveItemAtPosition(3).DownOne();

            Assert.AreEqual("three", things.Single(t => t.Position == 4).Name);
            Mock.Get(thingRepository).Verify();
        }

        [Test]
        public void MoveUpShouldNotMoveElementUpWhenConstrainedByFooId()
        {
            IQueryable<Thing> things = MoveTests.MakeSomeThings().AsQueryable();

            Mock.Get(thingRepository).Expect(tr => tr.GetAll()).Returns(things).Verifiable();
            Mock.Get(thingRepository).Expect(tr => tr.SubmitChanges()).Verifiable();

            orderService.MoveItemAtPosition(2).ConstrainedBy(thing => thing.FooId == 1).UpOne();

            Assert.AreEqual("two", things.Single(t => t.Position == 2).Name);
            Mock.Get(thingRepository).Verify();
        }

        [Test]
        public void MoveDownShouldNotMoveWhenConstrainedByFooId()
        {
            IQueryable<Thing> things = MoveTests.MakeSomeThings().AsQueryable();

            Mock.Get(thingRepository).Expect(tr => tr.GetAll()).Returns(things).Verifiable();
            Mock.Get(thingRepository).Expect(tr => tr.SubmitChanges()).Verifiable();

            orderService.MoveItemAtPosition(3).ConstrainedBy(thing => thing.FooId == 1).DownOne();

            Assert.AreEqual("three", things.Single(t => t.Position == 3).Name);
            Mock.Get(thingRepository).Verify();
        }
    }
}
