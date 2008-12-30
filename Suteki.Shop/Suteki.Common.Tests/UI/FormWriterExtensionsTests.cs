using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using NUnit.Framework;
using Suteki.Common.Models;
using Suteki.Common.Tests.TestHelpers;
using Suteki.Common.UI;

namespace Suteki.Common.Tests.UI
{
    [TestFixture]
    public class FormWriterExtensionsTests
    {
        private HtmlHelper html;

        [SetUp]
        public void SetUp()
        {
            html = MvcTestHelpers.CreateMockHtmlHelper();
        }

        [Test]
        public void ShouldWriteFormForType()
        {
            var criteria = new TestCriteria();

            var result = html.ForEntity(criteria).WriteForm();
            Assert.That(result, Is.EqualTo(expectedHtml1));
            //Console.WriteLine(result);
        }

        [Test]
        public void ShouldWriteFormForTypeWithValues()
        {
            var criteria = new TestCriteria
            {
                Age = 42,
                Id = 1,
                IsActive = true,
                Name = "Return to Forever"
            };

            var result = html.ForEntity(criteria).WriteForm();
            Assert.That(result, Is.EqualTo(expectedHtml2));
            //Console.WriteLine(result);
        }

        [Test]
        public void ShouldWriteFormForTypeWithChildEntities()
        {
            var criteria = new TestCriteriaWithEntity();

            var items = new[] 
                        { 
                            new TestNamedEntity { Id = 1, Name = "one" },
                            new TestNamedEntity { Id = 2, Name = "two" },
                            new TestNamedEntity { Id = 3, Name = "three" }
                        };
            var selectList = new SelectList(items, "Id", "Name", 2);
            var selectLists = new SelectListCollection {{"TestCriteria", selectList}};

            var result = html.ForEntity(criteria).WithSelectLists(selectLists).WriteForm();
            Assert.That(result, Is.EqualTo(expectedHtml3));
            //Console.WriteLine(result);
        }

        /// <summary>
        /// Note that this doesn't work with properties declared like this:
        /// public string MyProperty { get; private set; }
        /// The reflected propertyInfo CanWrite property is true.
        /// </summary>
        [Test]
        public void ReadonlyPropertiesShouldNotBeDisplayedAsEditable()
        {
            var test = new TestWithReadonlyProperty();
            var result = html.ForEntity(test).WriteForm();
            Assert.That(result, Is.EqualTo(readonlyExpectedHtml));
        }

        [Test]
        public void TextBoxSpike()
        {
            HtmlHelper htmlHelper = MvcTestHelpers.CreateMockHtmlHelper();

            var result = htmlHelper.TextBox("thing.Id");
            Assert.That(result, Is.EqualTo(@"<input id=""thing.Id"" name=""thing.Id"" type=""text"" value="""" />"));
            //Console.WriteLine(result);
        }

        [Test]
        public void SelectListSpike()
        {
            var htmlHelper = MvcTestHelpers.CreateMockHtmlHelper();

            var items = new[] {"one", "two", "three", "four"};
            var selectList = new SelectList(items);

            var result = htmlHelper.DropDownList("", "thing.Id", selectList);
            Assert.That(result, Is.EqualTo(selectListHtml));
            //Console.WriteLine(result);
        }

        public class TestNamedEntity : NamedEntity<TestNamedEntity> {}

        public class TestCriteria : Entity<TestCriteria>
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public bool IsActive { get; set; }
        }

        public class TestCriteriaWithEntity
        {
            public TestCriteria TestCriteria { get; set; }
        }

        public class TestWithReadonlyProperty
        {
            public TestWithReadonlyProperty()
            {
                name = "The value";
            }

            private readonly string name;
            public string Name
            {
                get { return name; }
            }
        }

        private const string expectedHtml1 =
@"<table>
	<tr>
		<td><label for=""Name"">Name</label></td><td><input id=""Name"" name=""Name"" type=""text"" value="""" />
		
		</td>
	</tr><tr>
		<td><label for=""Age"">Age</label></td><td><input id=""Age"" name=""Age"" type=""text"" value=""0"" />
		
		</td>
	</tr><tr>
		<td><label for=""IsActive"">Is Active</label></td><td><input id=""IsActive"" name=""IsActive"" type=""checkbox"" value=""true"" />
<input name=""IsActive"" type=""hidden"" value=""false"" />

		</td>
	</tr><tr>
		<td>&nbsp;</td><td><input id=""submit"" name=""submit"" type=""submit"" value=""OK"" />
		</td>
	</tr>
</table>";

        private const string expectedHtml2 =
@"<table>
	<tr>
		<td><label for=""Name"">Name</label></td><td><input id=""Name"" name=""Name"" type=""text"" value=""Return to Forever"" />
		
		</td>
	</tr><tr>
		<td><label for=""Age"">Age</label></td><td><input id=""Age"" name=""Age"" type=""text"" value=""42"" />
		
		</td>
	</tr><tr>
		<td><label for=""IsActive"">Is Active</label></td><td><input checked=""checked"" id=""IsActive"" name=""IsActive"" type=""checkbox"" value=""true"" />
<input name=""IsActive"" type=""hidden"" value=""false"" />

		</td>
	</tr><tr>
		<td>&nbsp;</td><td><input id=""submit"" name=""submit"" type=""submit"" value=""OK"" />
		</td>
	</tr>
</table>";

        private const string expectedHtml3 =
@"<table>
	<tr>
		<td><label for=""TestCriteria.Id"">Test Criteria</label></td><td><select id=""TestCriteria.Id"" name=""TestCriteria.Id""><option value=""1"">one</option>
<option selected=""selected"" value=""2"">two</option>
<option value=""3"">three</option>
</select>
		
		</td>
	</tr><tr>
		<td>&nbsp;</td><td><input id=""submit"" name=""submit"" type=""submit"" value=""OK"" />
		</td>
	</tr>
</table>";

        private const string readonlyExpectedHtml =
@"<table>
	<tr>
		<td>&nbsp;</td><td><input id=""submit"" name=""submit"" type=""submit"" value=""OK"" />
		</td>
	</tr>
</table>";

        private const string selectListHtml =
@"<select id=""thing.Id"" name=""thing.Id""><option>one</option>
<option>two</option>
<option>three</option>
<option>four</option>
</select>";
    }
}