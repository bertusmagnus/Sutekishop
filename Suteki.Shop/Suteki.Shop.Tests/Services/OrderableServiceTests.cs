using System.Linq;
using NUnit.Framework;
using Suteki.Common.Repositories;
using Suteki.Common.Services;
using Rhino.Mocks;

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
            thingRepository = MockRepository.GenerateStub<IRepository<Thing>>();
            orderService = new OrderableService<Thing>(thingRepository);
        }

        [Test]
        public void MoveUpShouldMoveElementUp()
        {
            IQueryable<Thing> things = MoveTests.MakeSomeThings().AsQueryable();

            thingRepository.Expect(tr => tr.GetAll()).Return(things);
            thingRepository.Expect(tr => tr.SubmitChanges());

            // move two up to top
            orderService.MoveItemAtPosition(2).UpOne();

            Assert.AreEqual("two", things.Single(t => t.Position == 1).Name); 
            thingRepository.VerifyAllExpectations();
        }

        [Test]
        public void MoveDownShouldMoveElementDown()
        {
            IQueryable<Thing> things = MoveTests.MakeSomeThings().AsQueryable();

            thingRepository.Expect(tr => tr.GetAll()).Return(things);
            thingRepository.Expect(tr => tr.SubmitChanges());

            // move three down to bottom
            orderService.MoveItemAtPosition(3).DownOne();

            Assert.AreEqual("three", things.Single(t => t.Position == 4).Name);
            thingRepository.VerifyAllExpectations();
        }

        [Test]
        public void MoveUpShouldNotMoveElementUpWhenConstrainedByFooId()
        {
            IQueryable<Thing> things = MoveTests.MakeSomeThings().AsQueryable();

            thingRepository.Expect(tr => tr.GetAll()).Return(things);
            thingRepository.Expect(tr => tr.SubmitChanges());

            orderService.MoveItemAtPosition(2).ConstrainedBy(thing => thing.FooId == 1).UpOne();

            Assert.AreEqual("two", things.Single(t => t.Position == 2).Name);
            thingRepository.VerifyAllExpectations();
        }

        [Test]
        public void MoveDownShouldNotMoveWhenConstrainedByFooId()
        {
            IQueryable<Thing> things = MoveTests.MakeSomeThings().AsQueryable();

            thingRepository.Expect(tr => tr.GetAll()).Return(things);
            thingRepository.Expect(tr => tr.SubmitChanges());

            orderService.MoveItemAtPosition(3).ConstrainedBy(thing => thing.FooId == 1).DownOne();

            Assert.AreEqual("three", things.Single(t => t.Position == 3).Name);
            thingRepository.VerifyAllExpectations();
        }
    }
}
