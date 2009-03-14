using System.Web.Mvc;
using Microsoft.Web.Mvc;
using Suteki.Shop.Controllers;

namespace Suteki.Shop
{
	public partial class TextContent : ITextContent
	{
		partial void OnTextChanging(string value)
		{
			//value.Label("Text").IsRequired();
		}

		public override string EditLink(HtmlHelper htmlHelper)
		{
			return htmlHelper.ActionLink<CmsController>(c => c.Edit(ContentId), "Edit");
		}

		public static TextContent DefaultTextContent(Content parentContent, int nextPosition)
		{
			return new TextContent
			{
				Content1 = parentContent,
				IsActive = true,
				ContentTypeId = ContentType.TextContentId,
				Position = nextPosition
			};
		}
	}
}