using Iesi.Collections.Generic;
using NUnit.Framework;
using Suteki.Common.HtmlHelpers;
using Suteki.Common.Models;

namespace Suteki.Common.Tests.HtmlHelpers
{
    [TestFixture]
    public class TreeRendererTests
    {
        [Test]
        public void ShouldRenderCorrectHtml()
        {
            var tree = CreateTree();

            var treeRenderer = new TreeRenderer<CompositeThing>(new[] {tree}, thing => thing.Name + "!!");
            var html = treeRenderer.Render();

            // the first UL element's id value is different each time so we have to replace the htmlIdToken
            // with the actual value before comparing the expected result with the actual one
            var expectedHtml = tokenisedExpectedHtml.Replace(htmlIdToken, treeRenderer.HtmlId);

            //Console.WriteLine(html);
            Assert.That(html, Is.EqualTo(expectedHtml));
        }

        private static CompositeThing CreateTree()
        {
            return new CompositeThing
            {
                Name = "Root",
                Children =
                    {
                        new CompositeThing
                        {
                            Name = "First Child",
                            Children =
                                {
                                    new CompositeThing
                                    {
                                        Name = "First Grandchild"
                                    },
                                    new CompositeThing
                                    {
                                        Name = "Second Grandchild"
                                    }
                                }
                        },
                        new CompositeThing
                        {
                            Name = "Second Child",
                            Children =
                                {
                                    new CompositeThing
                                    {
                                        Name = "Third Grandchild"
                                    },
                                    new CompositeThing
                                    {
                                        Name = "Fourth Grandchild"
                                    }
                                }
                        }
                    }
            };
        }

        private const string htmlIdToken = "%htmlId%";
        private const string tokenisedExpectedHtml =
@"
<ul id=""%htmlId%"">
	<li>Root!!
	<ul>
		<li>First Child!!
		<ul>
			<li>First Grandchild!!</li>
			<li>Second Grandchild!!</li>

		</ul>
		</li>
		<li>Second Child!!
		<ul>
			<li>Third Grandchild!!</li>
			<li>Fourth Grandchild!!</li>

		</ul>
		</li>

	</ul>
	</li>

</ul>
<script type=""text/javascript"">
    $(function() {
        $(""#%htmlId%"").treeview();
    });
</script>
";
    }

    public class CompositeThing : IComposite<CompositeThing>
    {
        public CompositeThing()
        {
            Children = new HashedSet<CompositeThing>();
        }

        public string Name { get; set; }
        public CompositeThing Parent { get; set; }
        public ISet<CompositeThing> Children { get; set; }
    }
}