using System.Web.Mvc;
using Castle.MicroKernel;
using NUnit.Framework;
using Suteki.Common.Models;
using Suteki.Common.Services;
using Suteki.Common.UI;
using Rhino.Mocks;

namespace Suteki.Common.Tests.Services
{
    /// <summary>
    /// Summary description for FormBuilderTests
    /// </summary>
    [TestFixture]
    public class FormBuilderTests
    {
        private IFormBuilder<Address> formBuilder;
        private IKernel kernel;

        [SetUp]
        public void SetUp()
        {
            kernel = MockRepository.GenerateMock<IKernel>();
            formBuilder = new FormBuilder<Address>(kernel);
        }

        [Test]
        public void GetSelectLists_ShouldReturnTheCorrectSelectListsForCentreSearchCriteria()
        {
            SetupMockFor<Centre>(kernel);
            SetupMockFor<Country>(kernel);
            SetupMockFor<Iac>(kernel);

            // GetSelectLists takes any entity, 
            // finds any properties that are of typeNamedEntity,
            // creates a SelectList to populate that entity by using a SelectListBuilder
            // and adds it to the SelectListCollection
            var Address = new Address();
            SelectListCollection lists = formBuilder.GetSelectLists(Address);

            AssertFor<Centre>(lists);
            AssertFor<Country>(lists);
            AssertFor<Iac>(lists);
        }

        private static void SetupMockFor<T>(IKernel kernelMock) where T : NamedEntity<T>, new()
        {
            var selectBuilderMock = MockRepository.GenerateMock<ISelectListBuilder>();
            var select = new SelectList(new System.Collections.Generic.List<T>());
            kernelMock.Expect(k => k.Resolve(typeof(ISelectListBuilder<T>))).Return(selectBuilderMock);
            selectBuilderMock.Expect(sb => sb.MakeFrom(null)).Return(select).IgnoreArguments();
        }

        private static void AssertFor<T>(SelectListCollection lists)
        {
            var typeName = typeof (T).Name;
            Assert.That(lists.ContainsKey(typeName), string.Format("lists does not contain key {0}", typeName));

            var selectList = lists[typeName];
            Assert.That(selectList, Is.Not.Null, string.Format("lists[\"{0}\"] is null", typeName));
        }

        public class Address : Entity<Address>
        {
            public Centre Centre { get; set; }
            public Country Country { get; set; }
            public Iac Iac { get; set; }
        }

        public class Centre : NamedEntity<Centre>
        {
        }

        public class Country : NamedEntity<Country>
        {
        }

        public class Iac : NamedEntity<Iac>
        {
        }
    }
}