using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using MvcContrib.UI.Html.Grid;
using NUnit.Framework;
using Suteki.Common.Extensions;
using Suteki.Common.HtmlHelpers;
using Suteki.Common.Models;
using Suteki.Common.Tests.TestHelpers;

namespace Suteki.Common.Tests.Spikes
{
    /// <summary>
    /// Summary description for MvcContribGridSpike
    /// </summary>
    [TestFixture]
    public class MvcContribGridSpike
    {
        private HtmlHelper html;
        private StringWriter stringWriter;

        [SetUp]
        public void MyTestInitialize()
        {
            stringWriter = new StringWriter();
            html = MvcTestHelpers.CreateMockHtmlHelper(stringWriter);
        }

        [Test]
        public void GridShouldReturnHtml()
        {
            var things = BuildSomeThings();

            html.Grid(things, columns =>
                              {
                                  columns.For(thing => thing.Id);
                                  columns.For(thing => thing.FirstName);
                                  columns.For(thing => thing.Date);
                              });

            Console.WriteLine(stringWriter.ToString());
        }

        [Test]
        public void GetForeignKeysOf_ShouldReturnTheForeignKeysOfCountry()
        {
            var keysDictionary = new Dictionary<string, object>();
            LinqForeignKeyFinder<EntityWithEntityProperties>
                .GetForeignKeysOf().ForEach(key => keysDictionary.Add(key, null));

            Assert.IsTrue(keysDictionary.ContainsKey("FirstChild"), "FirstChild not found");
            Assert.IsTrue(keysDictionary.ContainsKey("SecondChild"), "SecondChild not found");
        }

        public Thing[] BuildSomeThings()
        {
            return new[]
                   {
                       new Thing {Id = 1, FirstName = "one", Date = new DateTime(2008, 11, 30)},
                       new Thing {Id = 2, FirstName = "two", Date = new DateTime(2008, 8, 21)},
                       new Thing {Id = 3, FirstName = "three", Date = new DateTime(2008, 4, 1)}
                   };
        }
    }

    public class Thing
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public DateTime Date { get; set; }
    }

    public class EntityWithEntityProperties : Entity<EntityWithEntityProperties>    
    {
        public ChildEntity FirstChild { get; set; }
        public ChildEntity SecondChild { get; set; }
    }

    public class ChildEntity : Entity<ChildEntity>
    {
        
    }
}