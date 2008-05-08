using System;
using NUnit.Framework;
using Moq;
using Suteki.Shop.Controllers;
using Suteki.Shop.Repositories;

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
            countryRepository = new Mock<IRepository<Country>>().Object;

            countryController = new CountryController();
            countryController.Repository = countryRepository;
        }
    }
}
