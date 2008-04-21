using System;
using NUnit.Framework;
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

            sizeService.WithVaues(form).Update(product);

            Assert.AreEqual(3, product.Sizes.Count, "incorrect number of sizes on product");
            Assert.AreEqual("S", product.Sizes[0].Name);
            Assert.AreEqual("M", product.Sizes[1].Name);
            Assert.AreEqual("L", product.Sizes[2].Name);
        }

        [Test]
        public void Update_ShouldDeleteExistingSizesWhenNewOnesAreGiven()
        {
            NameValueCollection form = new NameValueCollection();
            form.Add("size_1", "New 1");
            form.Add("size_2", "New 2");

            Product product = new Product
            {
                Sizes =
                {
                    new Size { Name = "Old 1" },
                    new Size { Name = "Old 2" },
                    new Size { Name = "Old 3" }
                }
            };

            // mock the behaviour of the sizeRepository
            List<Size> sizesToDelete = new List<Size>();
            Mock.Get(sizeRepository).Expect(sr => sr.DeleteOnSubmit(It.IsAny<Size>()))
                .Callback<Size>(s => sizesToDelete.Add(s));

            sizeService.WithVaues(form).Update(product);

            // bit of a test hack, but you get the idea :)
            sizesToDelete.ForEach(s => product.Sizes.Remove(s));

            Assert.AreEqual(2, product.Sizes.Count, "incorrect number of sizes");
            Assert.AreEqual("New 1", product.Sizes[0].Name);
            Assert.AreEqual("New 2", product.Sizes[1].Name);
        }

        [Test]
        public void Update_ShouldNotDeleteExistingKeysWhenNoNewAreGiven()
        {
            NameValueCollection form = new NameValueCollection();
            form.Add("someOtherKey", "xyz");

            Product product = new Product
            {
                Sizes =
                {
                    new Size { Name = "Old 1" },
                    new Size { Name = "Old 2" },
                    new Size { Name = "Old 3" }
                }
            };

            // sizeRepository DeleteOnSubmit should not be called
            Mock.Get(sizeRepository).Expect(sr => sr.DeleteOnSubmit(It.IsAny<Size>()))
                .Throws(new Exception("Test should not call sizeRepository.DeleteOnSubmit"));

            sizeService.WithVaues(form).Update(product);

            Assert.AreEqual(3, product.Sizes.Count, "incorrect number of sizes");
            Assert.AreEqual("Old 1", product.Sizes[0].Name);
            Assert.AreEqual("Old 2", product.Sizes[1].Name);
            Assert.AreEqual("Old 3", product.Sizes[2].Name);
        }
    }
}
