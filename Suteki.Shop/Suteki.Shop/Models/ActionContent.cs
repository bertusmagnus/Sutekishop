using System;
using System.Web.Mvc.Html;

namespace Suteki.Shop
{
    public partial class ActionContent
    {
        public override string Link(System.Web.Mvc.HtmlHelper htmlHelper)
        {
            return htmlHelper.ActionLink(Name, Action, Controller);
        }
    }
}
