using System.Web.Mvc;
using Suteki.Common.Extensions;

namespace Suteki.Common.HtmlHelpers
{
    public static class HtmlHelperExtensions
    {
        public static string Pager(
            this HtmlHelper htmlHelper,
            string controller,
            string action,
            IPagedList pagedList)
        {
            Pager pageListBuilder = new Pager(htmlHelper, controller, action, pagedList);
            return pageListBuilder.WriteHtml();
        }
    }
}
