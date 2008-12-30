using System;
using System.Linq;
using NUnit.Framework;
using Moq;
using Suteki.Common.Repositories;
using Suteki.Common.ViewData;
using Suteki.Shop.Controllers;
using Castle.MicroKernel;
using System.Collections.Generic;
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
            var repositoryResolver = new Mock<IRepositoryResolver>().Object;
            countryController.repositoryResolver = repositoryResolver;

            // create a list of post zones
            var postZones = new List<PostZone>
            {
                new PostZone()
            }.AsQueryable();

            // setup expectations
            var postZoneRepository = new Mock<IRepository>().Object;
            Mock.Get(repositoryResolver).Expect(k => k.GetRepository(typeof(PostZone))).Returns(postZoneRepository);
            Mock.Get(postZoneRepository).Expect(pzr => pzr.GetAll()).Returns(postZones);

            // now exercise the method
            var viewData = new ScaffoldViewData<Country>();
            countryController.AppendLookupLists(viewData);

            Assert.AreSame(postZones, viewData.GetLookupList(typeof(PostZone)));
        }
    }
}
