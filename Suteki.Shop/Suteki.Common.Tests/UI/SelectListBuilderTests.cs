using System.Linq;
using Rhino.Mocks;
using NUnit.Framework;
using Suteki.Common.Models;
using Suteki.Common.Repositories;
using Suteki.Common.UI;

namespace Suteki.Common.Tests.UI
{
    [TestFixture]
    public class SelectListBuilderTests
    {
        private IRepository<NamedThing> repository;
        private ISelectListBuilder<NamedThing> selectListBuilder;

        [SetUp]
        public void SetUp()
        {
            repository = MockRepository.GenerateMock<IRepository<NamedThing>>();
            selectListBuilder = new SelectListBuilder<NamedThing>(repository);
        }

        [Test]
        public void ShouldBeAbleToCreate()
        {
            Assert.That(selectListBuilder, Is.Not.Null);
        }

        [Test]
        public void ShouldBeAbleToCreateASelectList()
        {
            var id = 4;
            var name = "The Thing";
            var thing = new NamedThing {Id = id, Name = name};

            // create a list of namedThings
            var things = new System.Collections.Generic.List<NamedThing>{ thing }.AsQueryable();

            // set up expectation that repository will return them
            repository.Expect(r => r.GetAll()).Return(things);

            // create the select list
            var selectList = selectListBuilder.MakeFrom(thing);

            var enumerator = selectList.Items.GetEnumerator();
            Assert.That(enumerator.MoveNext(), Is.True, "no items in selectList.Items");
            var item1 = enumerator.Current as NamedThing;
            Assert.That(enumerator.MoveNext(), Is.True, "only one item in selectList.Items");
            var item2 = enumerator.Current as NamedThing;

            Assert.That(item1.Name, Is.EqualTo("<select>"));
            Assert.That(item2, Is.SameAs(thing));
            Assert.That(selectList.DataValueField, Is.EqualTo("Id"));
            Assert.That(selectList.DataTextField, Is.EqualTo("Name"));
            Assert.That(selectList.SelectedValue, Is.EqualTo(id));
        }

        [Test]
        public void ShouldBeAbleToCreateASelectListWithNullSelectedItem()
        {
            var id = 4;
            var name = "The Thing";
            var thing = new NamedThing { Id = id, Name = name };

            // create a list of namedThings
            var things = new System.Collections.Generic.List<NamedThing> { thing }.AsQueryable();

            // set up expectation that repository will return them
            repository.Expect(r => r.GetAll()).Return(things);

            // create the select list
            var selectList = selectListBuilder.MakeFrom(null);

            var enumerator = selectList.Items.GetEnumerator();
            Assert.That(enumerator.MoveNext(), Is.True, "no items in selectList.Items");
            var item1 = enumerator.Current as NamedThing;
            Assert.That(enumerator.MoveNext(), Is.True, "only one item in selectList.Items");
            var item2 = enumerator.Current as NamedThing;

            Assert.That(item1.Name, Is.EqualTo("<select>"));
            Assert.That(item2, Is.SameAs(thing));
            Assert.That(selectList.DataValueField, Is.EqualTo("Id"));
            Assert.That(selectList.DataTextField, Is.EqualTo("Name"));
            Assert.That(selectList.SelectedValue, Is.EqualTo(0));
        }

        public class NamedThing : NamedEntity<NamedThing>
        {
            
        }
    }
}