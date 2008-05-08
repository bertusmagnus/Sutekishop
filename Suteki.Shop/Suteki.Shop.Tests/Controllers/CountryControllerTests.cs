using System;
using System.Linq;
using NUnit.Framework;
using Moq;
using Suteki.Shop.Controllers;
using Suteki.Shop.Repositories;
using Castle.MicroKernel;
using System.Collections.Generic;
using Suteki.Shop.ViewData;
using System.Threading;
using System.Security.Principal;

namespace Suteki.Shop.Tests.Controllers
{
    [TestFixture]
    public class CountryControllerTests
    {
        CountryController countryController;

        IRepository<Country> countryRepository;

        [SetUp]
        public void SetUp()
        {
            // you have to be an administrator to access the user controller
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("admin"), new string[] { "Administrator" });

            countryRepository = new Mock<IRepository<Country>>().Object;

            countryController = new CountryController();
            countryController.Repository = countryRepository;
        }

        [Test]
        public void GetLookupLists_ShouldGetPostZones()
        {
            // create a mock IoC container
            IKernel kernel = new Mock<IKernel>().Object;
            countryController.Kernel = kernel;

            // create a list of post zones
            var postZones = new List<PostZone>
            {
                new PostZone()
            }.AsQueryable();

            // setup expectations
            IRepository<PostZone> postZoneRepository = new Mock<IRepository<PostZone>>().Object;
            Mock.Get(kernel).Expect(k => k.Resolve(typeof(IRepository<PostZone>))).Returns(postZoneRepository);
            Mock.Get(postZoneRepository).Expect(pzr => pzr.GetAll()).Returns(postZones);

            // spike mock kernel
            Type repositoryType = typeof(IRepository<PostZone>);
            object repository = kernel.Resolve(repositoryType);
            Assert.IsTrue(repository is IRepository<PostZone>, "repository is not IRepository<PostZone>");

            // now exercise the method
            ScaffoldViewData<Country> viewData = new ScaffoldViewData<Country>();
            countryController.AppendLookupLists(viewData);

            Assert.AreSame(postZones, viewData.GetLookUpList<PostZone>());
        }
    }
}
