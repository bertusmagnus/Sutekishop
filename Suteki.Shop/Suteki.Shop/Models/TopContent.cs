using System.Web.Mvc;
using Microsoft.Web.Mvc;
using Suteki.Shop.Controllers;

namespace Suteki.Shop
{
    public partial class TopContent : ITextContent
    {
        public override MvcHtmlString EditLink(HtmlHelper htmlHelper)
        {
            return htmlHelper.ActionLink<CmsController>(c => c.Edit(ContentId), "Edit");
        }
    }
}
