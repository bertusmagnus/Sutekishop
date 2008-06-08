﻿using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Suteki.Shop.Tests.Models
{
    [TestFixture]
    public class PostageTests
    {
        public static IQueryable<Postage> CreatePostages()
        {
            var postages = new List<Postage>
            {
                new Postage { IsActive = true, MaxWeight = 0, Price = 0M },
                new Postage { IsActive = true, MaxWeight = 200, Price = 0.50M },
                new Postage { IsActive = true, MaxWeight = 400, Price = 1.10M }, 
            }.AsQueryable();
            return postages;
        }
    }
}
