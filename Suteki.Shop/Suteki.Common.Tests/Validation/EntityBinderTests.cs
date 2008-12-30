using System;
using System.Collections.Specialized;
using Castle.DynamicProxy;
using NUnit.Framework;
using Rhino.Mocks;
using Suteki.Common.Models;
using Suteki.Common.Repositories;
using Suteki.Common.Validation;

namespace Suteki.Common.Tests.Validation
{
    [TestFixture]
    public class EntityBinderTests
    {
        private ValidatingBinder validatingBinder;

        [SetUp]
        public void SetUp()
        {
             validatingBinder = new ValidatingBinder(
                 new SimplePropertyBinder(),
                 new BooleanPropertyBinder());
        }

        [Test]
        public void EntityBinderShouldBindEntityFromForm()
        {
            var form = new NameValueCollection
            {
                { "Name", "Ben" },
                { "Age", "43" },
                { "Birthday", "7/1/1965" },
                { "Child.Id", "10" },
                { "IsActive", "true,false" }
            };

            var thing = new Thing();

            // create an EntityBinder without a repository resolver means that dependencies will be
            // created as uninitialised objects
            var entityBinder = new EntityBinder();
            validatingBinder.PropertyBinders.Add(entityBinder);

            validatingBinder.UpdateFrom(thing, form);

            Assert.That(thing.Name, Is.EqualTo("Ben"));
            Assert.That(thing.Age, Is.EqualTo(43));
            Assert.That(thing.Birthday, Is.EqualTo(new DateTime(1965, 1, 7)));
            Assert.That(thing.Child, Is.Not.Null);
            Assert.That(thing.Child.Id, Is.EqualTo(10));
            Assert.That(thing.IsActive, Is.True);

            // child name will be null
            Assert.That(thing.Child.Name, Is.Null);
        }

        [Test]
        public void EntityBinderShouldGetDependenciesFromRepository()
        {
            // this is a form as issued by a browser POST
            var form = new NameValueCollection
            {
                { "Name", "Ben" },
                { "Age", "43" },
                { "Birthday", "7/1/1965" },
                { "Child.Id", "10" },
            };

            var thing = new Thing();

            var child = new Child
            {
                Id = 10,
                Name = "Freddie"
            };

            // set up mocks
            var childRepository = MockRepository.GenerateMock<IRepository>();
            var repositoryResolver = MockRepository.GenerateMock<IRepositoryResolver>();
            
            // set up repository resolver to return childRepository when asked for repository of type Child
            repositoryResolver.Expect(rr => rr.GetRepository(typeof (Child))).Return(childRepository);
            // set up child repository to return child when asked for id 10
            childRepository.Expect(cr => cr.GetById(10)).Return(child);

            var entityBinder = new EntityBinder(repositoryResolver);

            validatingBinder.PropertyBinders.Add(entityBinder);
            validatingBinder.UpdateFrom(thing, form);

            Assert.That(thing.Name, Is.EqualTo("Ben"));
            Assert.That(thing.Age, Is.EqualTo(43));
            Assert.That(thing.Birthday, Is.EqualTo(new DateTime(1965, 1, 7)));
            Assert.That(thing.Child, Is.Not.Null);
            Assert.That(thing.Child.Id, Is.EqualTo(10));

            // thing child should be the same as the child we created above
            Assert.That(thing.Child, Is.SameAs(child));
        }


        [Test, ExpectedException(typeof(ValidationException))]
        public void EntityBinderShouldThrowValidationExceptionForZeroDependencyId()
        {
            // this is a form as issued by a browser POST
            var form = new NameValueCollection
            {
                { "Name", "Ben" },
                { "Age", "43" },
                { "Birthday", "7/1/1965" },
                { "Child.Id", "0" }, // child id is zero
            };

            var thing = new Thing();

            // build validating binder with entity binder
            var entityBinder = new EntityBinder();
            validatingBinder.PropertyBinders.Add(entityBinder);

            // nice validation exception should be thrown here
            validatingBinder.UpdateFrom(thing, form);
        }

        [Test]
        public void EntityBinderShouldNotThrowExceptionWhenBindingNullableEntityWithZeroEntityId()
        {
            // this is a form as issued by a browser POST
            var form = new NameValueCollection
            {
                { "Child.Id", "0" }, // child id is zero
            };

            var thing = new ThingWithNullableChild();

            // build validating binder with entity binder
            var entityBinder = new EntityBinder();
            validatingBinder.PropertyBinders.Add(entityBinder);

            validatingBinder.UpdateFrom(thing, form);

            Assert.That(thing.Child, Is.Null);
        }

        [Test]
        public void EntityBinderShouldNotThrowExceptionWhenBindingProxyToZeroId()
        {
            // this is a form as issued by a browser POST
            var form = new NameValueCollection
            {
                { "Child.Id", "0" }, // child id is zero
            };

            var generator = new ProxyGenerator();
            var thing = generator.CreateClassProxy<ThingWithNullableChild>();

            // build validating binder with entity binder
            var entityBinder = new EntityBinder();
            validatingBinder.PropertyBinders.Add(entityBinder);

            validatingBinder.UpdateFrom(thing, form);

            Assert.That(thing.Child, Is.Null);
        }

        // thing shouldn't have to be an entity
        public class Thing
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public DateTime Birthday { get; set; }
            public Child Child { get; set; }
            public bool IsActive { get; set; }
        }

        public class ThingWithNullableChild
        {
            [NullableEntity]
            public virtual Child Child { get; set; }
        }

        public class Child : Entity<Child>
        {
            public string Name { get; set; }
        }
        
    }
}