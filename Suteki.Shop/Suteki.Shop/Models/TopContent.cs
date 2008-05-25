using System;
using System.Web.Mvc;
using Suteki.Shop.Controllers;

namespace Suteki.Shop
{
    public partial class TopContent : ITextContent
    {
        public override string EditLink(HtmlHelper htmlHelper)
        {
            return htmlHelper.ActionLink<CmsController>(c => c.Edit(ContentId), "Edit");
        }
    }
}
