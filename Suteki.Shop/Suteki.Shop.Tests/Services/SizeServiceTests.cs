using System;
using System.Linq;
using NUnit.Framework;
using Suteki.Common.Repositories;
using Suteki.Shop.Services;
using System.Collections.Specialized;
using Rhino.Mocks;

namespace Suteki.Shop.Tests.Services
{
    [TestFixture]
    public class SizeServiceTests
    {
        ISizeService sizeService;
        IRepository<Size> sizeRepository;

        [SetUp]
        public void SetUp()
        {
            sizeRepository = MockRepository.GenerateStub<IRepository<Size>>();
            sizeService = new SizeService(sizeRepository);
        }

        [Test]
        public void Update_ShouldAddSizesInNameValueCollectionToProduct()
        {
            var form = new NameValueCollection
            {
                {"notASize_1", "abc"},
                {"size_1", "S"},
                {"size_2", "M"},
                {"size_3", "L"},
                {"size_4", ""},
                {"size_5", ""},
                {"someOther", "xyz"}
            };

            var product = new Product();

            sizeService.WithValues(form).Update(product);

            Assert.AreEqual(3, product.Sizes.Count, "incorrect number of sizes on product");
            Assert.AreEqual("S", product.Sizes[0].Name);
            Assert.AreEqual("M", product.Sizes[1].Name);
            Assert.AreEqual("L", product.Sizes[2].Name);
        }

        [Test]
        public void Update_ShouldNotMarkExistingSizesInactiveWhenNewOnesAreGiven()
        {
            var form = new NameValueCollection
            {
                {"size_1", "New 1"}, 
                {"size_2", "New 2"}
            };

            var product = new Product
            {
                Sizes =
                {
                    new Size { Name = "Old 1", IsActive = true },
                    new Size { Name = "Old 2", IsActive = true },
                    new Size { Name = "Old 3", IsActive = true }
                }
            };

            sizeService.WithValues(form).Update(product);

            Assert.AreEqual(5, product.Sizes.Count, "incorrect number of sizes");

            Assert.IsTrue(product.Sizes[0].IsActive);
            Assert.IsTrue(product.Sizes[1].IsActive);
            Assert.IsTrue(product.Sizes[2].IsActive);

            Assert.IsTrue(product.Sizes[3].IsActive);
            Assert.IsTrue(product.Sizes[4].IsActive);

            Assert.AreEqual("New 1", product.Sizes[3].Name);
            Assert.AreEqual("New 2", product.Sizes[4].Name);
        }

        [Test]
        public void Update_ShouldNotDeactivateExistingKeysWhenNoNewAreGiven()
        {
            var form = new NameValueCollection {{"someOtherKey", "xyz"}};

            var product = CreateProductWithSizes();

            // sizeRepository DeleteOnSubmit should not be called
            sizeRepository.Expect(sr => sr.DeleteOnSubmit(Arg<Size>.Is.Anything))
                .Throw(new Exception("Test should not call sizeRepository.DeleteOnSubmit"));

            sizeService.WithValues(form).Update(product);

            Assert.AreEqual(3, product.Sizes.Count, "incorrect number of sizes");
            Assert.IsTrue(product.Sizes[0].IsActive);
            Assert.IsTrue(product.Sizes[1].IsActive);
            Assert.IsTrue(product.Sizes[2].IsActive);
        }

        private static Product CreateProductWithSizes()
        {
            var product = new Product
            {
                Sizes =
                {
                    new Size { Name = "Old 1", IsActive = true },
                    new Size { Name = "Old 2", IsActive = true },
                    new Size { Name = "Old 3", IsActive = true }
                }
            };
            return product;
        }

        [Test]
        public void Clear_ShouldSetAllSizesToInactive()
        {
            var product = CreateProductWithSizes();

            sizeService.Clear(product);

            Assert.AreEqual(0, product.Sizes.Where(size => size.IsActive).Count());
        }
    }
}
