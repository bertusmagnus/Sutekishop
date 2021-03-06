﻿using System;
using System.Collections.Generic;
using System.Linq;
using Rhino.Mocks;
using Suteki.Common.Repositories;
using NUnit.Framework;

namespace Suteki.Shop.Tests.Repositories
{
    public static class MockRepositoryBuilder
    {
		public static IRepository<Content> CreateContentRepository()
		{
			var repository = MockRepository.GenerateStub<IRepository<Content>>();

			var content = new[]
			{
				new Content() { Name = "Page 1"},
				new Content() { Name = "Page 2"}
			};

			repository.Expect(x => x.GetAll()).Return(content.AsQueryable());
			return repository;
		}

        public static IRepository<User> CreateUserRepository()
        {
            var userRepositoryMock = MockRepository.GenerateStub<IRepository<User>>();

            var users = new List<User>
            {
                new User { UserId = 1, Email = "Henry@suteki.co.uk", 
                    Password = "6C80B78681161C8349552872CFA0739CF823E87B", IsEnabled = true }, // henry1

                new User { UserId = 2, Email = "George@suteki.co.uk", 
                    Password = "DC25F9DC0DF2BE9E6A83E6F0B26F4B41F57ADF6D", IsEnabled = true }, // george1

                new User { UserId = 3, Email = "Sky@suteki.co.uk", 
                    Password = "980BC222DA7FDD0D37BE816D60084894124509A1", IsEnabled = true } // sky1
            };

            userRepositoryMock.Expect(ur => ur.GetAll()).Return(users.AsQueryable());

            return userRepositoryMock;
        }

        public static IRepository<Role> CreateRoleRepository()
        {
            var roleRepositoryMock = MockRepository.GenerateStub<IRepository<Role>>();

            var roles = new List<Role>
            {
                new Role { RoleId = 1, Name = "Administrator" },
                new Role { RoleId = 2, Name = "Order Processor" },
                new Role { RoleId = 3, Name = "Customer" },
                new Role { RoleId = 4, Name = "Guest" }
            };

            roleRepositoryMock.Expect(r => r.GetAll()).Return(roles.AsQueryable());

            return roleRepositoryMock;
        }

        public static IRepository<Category> CreateCategoryRepository()
        {
            var categoryRepositoryMock = MockRepository.GenerateStub<IRepository<Category>>();

            var root = new Category { CategoryId = 1, ParentId = null, Name = "root", ImageId = 5 };

            var one = new Category { CategoryId = 2, ParentId = 1, Name = "one" };
            var two = new Category { CategoryId = 3, ParentId = 1, Name = "two" };
            root.Categories.AddRange(new[] { one, two });

            var oneOne = new Category { CategoryId = 4, ParentId = 2, Name = "oneOne" };
            var oneTwo = new Category { CategoryId = 5, ParentId = 2, Name = "oneTwo" };
            one.Categories.AddRange(new[] { oneOne, oneTwo });

            var oneTwoOne = new Category { CategoryId = 6, ParentId = 5, Name = "oneTwoOne" };
            var oneTwoTwo = new Category { CategoryId = 7, ParentId = 5, Name = "oneTwoTwo" };
            oneTwo.Categories.AddRange(new[] { oneTwoOne, oneTwoTwo });

            Category[] categories = 
            {
                root,
                one,
                two,
                oneOne,
                oneTwo,
                oneTwoOne,
                oneTwoTwo
            };

            categoryRepositoryMock.Expect(c => c.GetById(1)).Return(root);
            categoryRepositoryMock.Expect(c => c.GetAll()).Return(categories.AsQueryable());

            return categoryRepositoryMock;
        }

        /// <summary>
        /// Asserts that the graph created by CreateCategoryRepository is correct
        /// </summary>
        /// <param name="root"></param>
        public static void AssertCategoryGraphIsCorrect(Category root)
        {
            Assert.IsNotNull(root, "root category is null");

            Assert.IsNotNull(root.Categories[0], "first child category is null");
            Assert.AreEqual("one", root.Categories[0].Name);

            Assert.IsNotNull(root.Categories[0].Categories[1], "second grandchild category is null");
            Assert.AreEqual("oneTwo", root.Categories[0].Categories[1].Name);

            Assert.IsNotNull(root.Categories[0].Categories[1].Categories[0], "first great grandchild category is null");
            Assert.AreEqual("oneTwoOne", root.Categories[0].Categories[1].Categories[0].Name);

            Assert.IsNotNull(root.Categories[0].Categories[1].Categories[1], "second great grandchild category is null");
            Assert.AreEqual("oneTwoTwo", root.Categories[0].Categories[1].Categories[1].Name);
        }

        public static IRepository<Product> CreateProductRepository()
        {
            var productRepositoryMock = MockRepository.GenerateStub<IRepository<Product>>();

            var products = new List<Product>
            {
                new Product { ProductId = 1, Name = "Product 1", Description = "Description 1", ProductCategories = { new ProductCategory() { CategoryId = 2 } }},
                new Product { ProductId = 2, Name = "Product 2", Description = "Description 2", ProductCategories = { new ProductCategory() { CategoryId = 2 } } },
                new Product { ProductId = 3, Name = "Product 3", Description = "Description 3", ProductCategories = { new ProductCategory() { CategoryId = 4 } }},
                new Product { ProductId = 4, Name = "Product 4", Description = "<p>\"Description 4\"</p>", ProductCategories = { new ProductCategory() { CategoryId = 4 } }},
                new Product { ProductId = 5, Name = "Product 5", Description = "Description 5", ProductCategories = { new ProductCategory() { CategoryId = 6 } } },
                new Product { ProductId = 6,  Name = "Product 6", Description = "Description 6", ProductCategories = { new ProductCategory() { CategoryId = 6 } } },
            };

            productRepositoryMock.Expect(pr => pr.GetAll()).Return(products.AsQueryable());

            return productRepositoryMock;
        }

    	public static IRepository<Review> CreateReviewRepository()
    	{
			var reviews = new List<Review> 
			{
				new Review() { Id = 1, ProductId = 1, Approved = true, Rating = 5, Text = "foo"},
				new Review() { Id = 2, ProductId = 1, Approved = true, Rating = 4, Text = "bar"},
				new Review() { Id = 2, ProductId = 1, Approved = false, Rating = 3, Text = "bar"},
				new Review() { Id = 3, ProductId = 2, Approved = true, Rating = 4, Text = "baz"},
				new Review() { Id = 4, ProductId = 3, Approved = false, Rating = 2, Text = "blah"},
			};

			var repository = MockRepository.GenerateStub<IRepository<Review>>();
			repository.Expect(x => x.GetAll()).Return(reviews.AsQueryable());
			return repository;
    	}
    }
}
