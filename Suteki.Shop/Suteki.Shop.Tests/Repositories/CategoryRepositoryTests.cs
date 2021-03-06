﻿using System.Transactions;
using NUnit.Framework;
using Suteki.Common.Repositories;
using Suteki.Shop.Repositories;

namespace Suteki.Shop.Tests.Repositories
{
    [TestFixture]
    public class CategoryRepositoryTests
    {
        /// <summary>
        /// It should be possible to create a hierarchy of categories
        /// and have LINQ to SQL properly construct the object graph
        /// when the root is selected.
        /// 
        /// Requires Db to be available
        /// </summary>
        [Test, Explicit]
        public void CategoriesShouldBeReturnedNested()
        {
            using (new TransactionScope())
            {
                var connectionStringProvider = new ConnectionStringProvider("<my connection string>");
                var dataContextProvider = new DataContextProvider(connectionStringProvider);

                // first insert a new graph into the database
                var categoryRepository = new Repository<Category>(dataContextProvider);

                // use the object graph created by the mock category repository
                var root = MockRepositoryBuilder.CreateCategoryRepository().GetRootCategory();

                // add the root into the real database
                categoryRepository.InsertOnSubmit(root);

                categoryRepository.SubmitChanges();

                // try to get the root again (note we need the actual id because we don't want the real root,
                // which will have a different graph of categories, from the database)
                var rootFromDb = categoryRepository.GetById(root.CategoryId);

                MockRepositoryBuilder.AssertCategoryGraphIsCorrect(rootFromDb);
            }
        }
    }
}

