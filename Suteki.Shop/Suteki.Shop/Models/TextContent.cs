using System;
using Suteki.Shop.Controllers;
using System.Web.Mvc;

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
    }
}
