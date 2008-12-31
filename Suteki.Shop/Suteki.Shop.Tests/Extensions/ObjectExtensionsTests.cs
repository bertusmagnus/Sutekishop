﻿using System.Linq;
using NUnit.Framework;
using Suteki.Common.Extensions;

namespace Suteki.Shop.Tests.Extensions
{
    [TestFixture]
    public class ObjectExtensionsTests
    {
        [Test]
        public void GetPropertiesShouldReturnThePropertiesOfAnAnonymousType()
        {
            var item = new { Message = "Hello World", Number = 4 };

            Assert.AreEqual("Message", item.GetProperties().First().Name);
            Assert.AreEqual("Hello World", item.GetProperties().First().Value);
            Assert.AreEqual("Number", item.GetProperties().ElementAt(1).Name);
            Assert.AreEqual(4, item.GetProperties().ElementAt(1).Value);
        }

        [Test]
        public void GetPrimaryKeyShouldReturnThePrimaryKeyOfUser()
        {
            const int userId = 10;
            var user = new User { UserId = userId };

            Assert.AreEqual("UserId", user.GetPrimaryKey().Name);
            Assert.AreEqual(10, user.GetPrimaryKey().Value);
        }
    }
}
