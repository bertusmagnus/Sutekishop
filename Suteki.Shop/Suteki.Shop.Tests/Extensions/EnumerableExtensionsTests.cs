using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Suteki.Common.Extensions;

namespace Suteki.Shop.Tests.Extensions
{
    [TestFixture]
    public class EnumerableExtensionsTests
    {
        [Test]
        public void GetCsv_ShouldRenderCorrectCsv()
        {
            IEnumerable<Thing> things = new List<Thing>()
                {
                    new Thing
                        {
                            Id = 12,
                            Name = "Thing one",
                            Date = new DateTime(2008, 4, 20),
                            Child = new Child
                                        {
                                            Name = "Max"
                                        }
                        },
                    new Thing
                        {
                            Id = 13,
                            Name = "Thing two",
                            Date = new DateTime(2008, 5, 20),
                            Child = new Child
                                        {
                                            Name = "Robbie"
                                        }
                        }
                };

            string csv = things.Select(t => new { Id = t.Id, Name = t.Name, Date = t.Date, Child = t.Child.Name }).AsCsv();

            Assert.That(csv, Is.EqualTo(expectedCsv));
        }

        const string expectedCsv = 
@"12,""Thing one"",""20/04/2008 00:00:00"",""Max""
13,""Thing two"",""20/05/2008 00:00:00"",""Robbie""
";

        public class Thing
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime Date { get; set; }
            public Child Child { get; set; }
        }

        public class Child
        {
            public string Name { get; set; }
        }
    }
}
