using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Suteki.Common.Extensions;
using System.Web.Routing;

namespace Suteki.Common.HtmlHelpers
{
    public class Pager
    {
        readonly StringBuilder htmlText = new StringBuilder();

        readonly HtmlHelper htmlHelper;
        readonly string controller;
        readonly string action;
        readonly IPagedList pagedList;
        readonly object criteria;

        public Pager(
            HtmlHelper htmlHelper,
            string controller,
            string action,
            IPagedList pagedList)
            : this(
            htmlHelper,
            controller,
            action,
            pagedList,
            null)
        {
        }

        public Pager(
            HtmlHelper htmlHelper,
            string controller,
            string action,
            IPagedList pagedList,
            object criteria)
        {
            this.htmlHelper = htmlHelper;
            this.controller = controller;
            this.action = action;
            this.pagedList = pagedList;
            this.criteria = criteria;
        }

        public string WriteHtml()
        {
            htmlText.Append("<div class=\"pager\">");

            WriteLink(0, "<<");
            WriteLink(pagedList.PageIndex - 1, "<");

            for (int i = 0; i < pagedList.NumberOfPages; i++)
            {
                WriteLink(i);
            }

            WriteLink(pagedList.PageIndex + 1, ">");
            WriteLink(pagedList.NumberOfPages - 1, ">>");

            htmlText.Append("</div>");

            return htmlText.ToString();
        }

        private void WriteLink(int pageNumber)
        {
            WriteLink(pageNumber, (pageNumber + 1).ToString());
        }

        private void WriteLink(int pageNumber, string text)
        {
            if (pageNumber == pagedList.PageIndex || pageNumber < 0 || pageNumber > pagedList.NumberOfPages-1)
            {
                htmlText.AppendFormat("{0} ", text);
            }
            else
            {
                htmlText.AppendFormat("{0} ", htmlHelper.ActionLink(text, action, controller, GetCriteria(pageNumber)));
            }
        }

        private RouteValueDictionary GetCriteria(int pageNumber)
        {
            var values = new RouteValueDictionary {{"CurrentPage", pageNumber}};

            if (criteria != null)
            {
                foreach (var property in criteria.GetType().GetProperties())
                {
                    var value = property.GetValue(criteria, null);
                    if (value == null) continue;
                    if (value.ToString() != string.Empty)
                    {
                        values.Add(property.Name, value);
                    }
                }
            }
            return values;
        }
    }
}
