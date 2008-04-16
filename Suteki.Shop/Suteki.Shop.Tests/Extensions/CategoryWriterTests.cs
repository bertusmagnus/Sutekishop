using System;
using NUnit.Framework;
using Suteki.Shop.HtmlHelpers;
using Suteki.Shop.Repositories;
using Suteki.Shop.Tests.Repositories;

namespace Suteki.Shop.Tests.Extensions
{
    [TestFixture]
    public class CategoryWriterTests
    {
        //[Test]
        //public void Write_ShouldWriteCorrectHtml()
        //{
        //    Category rootCategory = MockRepositoryBuilder.CreateCategoryRepository().Object.GetRootCategory();

        //    User user = new User
        //    {
        //        Role = Role.Administrator
        //    };

        //    CategoryWriter categoryWriter = new CategoryWriter(rootCategory, user);
        //    string result = categoryWriter.Write();

        //    //Assert.AreEqual(expectedOutput, result);
        //    Console.WriteLine(result);
        //}

        const string expectedOutput =
@"<ul>
	<li>one<ul>
		<li>oneOne</li><li>oneTwo<ul>
			<li>oneTwoOne</li><li>oneTwoTwo</li>
		</ul></li>
	</ul></li><li>two</li>
</ul>";
    }
}
