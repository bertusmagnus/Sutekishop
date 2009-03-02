using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using MvcContrib.UI.LegacyGrid;
using NUnit.Framework;
using Suteki.Common.HtmlHelpers;
using Suteki.Common.Tests.TestHelpers;

namespace Suteki.Common.Tests.HtmlHelpers
{
    [TestFixture]
    public class ColumnOrdererTests
    {
        private GraphBuilder graphBuilder;
        private ItemsTableInfo tableInfo;

        [SetUp]
        public void SetUp()
        {
            graphBuilder = new GraphBuilder();
            tableInfo = new ItemsTableInfo();
        }

        [Test]
        public void ShouldBeAbleToOrderGraph()
        {
            var items = graphBuilder.Build().AsQueryable().OrderBy(root => root.Child.GrandChild.Name);

            AssertItemsAreOrdered(items);
        }

        private static void AssertItemsAreOrdered(IQueryable<Root> items)
        {
            Assert.That(items.ElementAt(0).Child.GrandChild.Name, Is.EqualTo("abbie"));
            Assert.That(items.ElementAt(1).Child.GrandChild.Name, Is.EqualTo("becky"));
            Assert.That(items.ElementAt(2).Child.GrandChild.Name, Is.EqualTo("debbie"));
        }

        [Test]
        public void ShouldBeAbleToOrderGraphWithExpression()
        {
            var sortExpression = GetSortExpression();
            var items = graphBuilder.Build().AsQueryable().OrderBy(sortExpression);

            AssertItemsAreOrdered(items);
        }

        private static Expression<Func<Root, object>> GetSortExpression()
        {
            return root => root.Child.GrandChild.Name;
        }

        [Test]
        public void ShouldBeAbleToBuildGridWithExpression()
        {
            var items = graphBuilder.Build().AsQueryable();
            var sortExpression = GetSortExpression();
            var writer = new StringWriter();
            var htmlHelper = MvcTestHelpers.CreateMockHtmlHelper(writer);

            var output = htmlHelper.ViewContext.HttpContext.Response.Output;

            htmlHelper.Grid(items, cols =>
                                   {
                                       cols.For(sortExpression);
                                   });

            Console.WriteLine(writer.ToString());
        }

        [Test]
        public void ShouldBeAbleToUseTableInfoToBuildGrid()
        {
            var items = graphBuilder.Build().AsQueryable();
            var writer = new StringWriter();
            var htmlHelper = MvcTestHelpers.CreateMockHtmlHelper(writer);

            htmlHelper.Grid(items, htmlHelper.CreateGridColumnBuilder(tableInfo));

            Console.WriteLine(writer.ToString());
        }

        [Test]
        public void ShouldBeAbleToSortGivenColumnName()
        {
            var items = tableInfo.Columns["Grandchild Name"].SortAscending(graphBuilder.Build().AsQueryable());

            AssertItemsAreOrdered(items);
        }

        [Test]
        public void ExpressionSpike()
        {
            Expression<Func<Root, int>> expression = root => root.Child.Age;
            Expression<Func<Root, object>> objExpression = root => root.Child.Age;

            Console.WriteLine("expression.Parameters.Count: {0}", expression.Parameters.Count);
            Console.WriteLine("expression.Parameters[0].Name {0}", expression.Parameters[0].Name);

            Assert.That(expression.Body.NodeType == ExpressionType.MemberAccess);
            Assert.That(expression.Body.Type == typeof(int));

            Assert.That(objExpression.Body.NodeType == ExpressionType.Convert);
            Assert.That(objExpression.Body.Type == typeof(object));

            var conversion = objExpression.Body as UnaryExpression;

            var memberAssignment = expression.Body as MemberExpression;

            

            Expression<Func<Root, object>> objectExpression = 
                Expression.Lambda<Func<Root,object>>(Expression.Convert(expression.Body, typeof(object)), expression.Parameters);
        }
    }

    public class ItemsTableInfo : TableInfo<Root>
    {
        public ItemsTableInfo()
        {
            AddColumn(root => root.Child.GrandChild.Name, "Grandchild Name");
            AddColumn(root => root.Child.Age, "Child Age");
        }
    }

    public class GraphBuilder
    {
        public IEnumerable<Root> Build()
        {
            return new List<Root>
            {
                new Root
                {
                    Child = new Child
                    {
                        Age = 22,
                        GrandChild = new GrandChild
                        {
                            Name = "debbie",
                        }
                    }
                },
                new Root
                {
                    Child = new Child
                    {
                        Age = 13,
                        GrandChild = new GrandChild
                        {
                            Name = "abbie"
                        }
                    }
                },
                new Root
                {
                    Child = new Child
                    {
                        Age = 41,
                        GrandChild = new GrandChild
                        {
                            Name = "becky"
                        }
                    }
                },
            };
        }
    }

    public class Root
    {
        public Child Child { get; set; }
    }

    public class Child
    {
        public GrandChild GrandChild { get; set; }
        public int Age { get; set; }
    }

    public class GrandChild
    {
        public string Name { get; set; }
    }
}