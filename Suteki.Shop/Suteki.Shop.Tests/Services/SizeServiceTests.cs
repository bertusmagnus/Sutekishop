using System;
using System.Linq;
using NUnit.Framework;
using Suteki.Common.Repositories;
using Suteki.Shop.Services;
using System.Collections.Specialized;
using Suteki.Shop.Repositories;
using Moq;
using System.Collections.Generic;

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
            sizeRepository = new Mock<IRepository<Size>>().Object;
            sizeService = new SizeService(sizeRepository);
        }

        [Test]
        public void Update_ShouldAddSizesInNameValueCollectionToProduct()
        {
            NameValueCollection form = new NameValueCollection();
            form.Add("notASize_1", "abc");
            form.Add("size_1", "S");
            form.Add("size_2", "M");
            form.Add("size_3", "L");
            form.Add("size_4", "");
            form.Add("size_5", "");
            form.Add("someOther", "xyz");

            Product product = new Product();

            sizeService.WithValues(form).Update(product);

            Assert.AreEqual(3, product.Sizes.Count, "incorrect number of sizes on product");
            Assert.AreEqual("S", product.Sizes[0].Name);
            Assert.AreEqual("M", product.Sizes[1].Name);
            Assert.AreEqual("L", product.Sizes[2].Name);
        }

        [Test]
        public void Update_ShouldNotMarkExistingSizesInactiveWhenNewOnesAreGiven()
        {
            NameValueCollection form = new NameValueCollection();
            form.Add("size_1", "New 1");
            form.Add("size_2", "New 2");

            Product product = new Product
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
            NameValueCollection form = new NameValueCollection();
            form.Add("someOtherKey", "xyz");

            Product product = CreateProductWithSizes();

            // sizeRepository DeleteOnSubmit should not be called
            Mock.Get(sizeRepository).Expect(sr => sr.DeleteOnSubmit(It.IsAny<Size>()))
                .Throws(new Exception("Test should not call sizeRepository.DeleteOnSubmit"));

            sizeService.WithValues(form).Update(product);

            Assert.AreEqual(3, product.Sizes.Count, "incorrect number of sizes");
            Assert.IsTrue(product.Sizes[0].IsActive);
            Assert.IsTrue(product.Sizes[1].IsActive);
            Assert.IsTrue(product.Sizes[2].IsActive);
        }

        private static Product CreateProductWithSizes()
        {
            Product product = new Product
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
            Product product = CreateProductWithSizes();

            sizeService.Clear(product);

            Assert.AreEqual(0, product.Sizes.Where(size => size.IsActive).Count());
        }
    }
}
