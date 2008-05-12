using System;
using System.Text;
using System.Web.Mvc;
using Suteki.Common.Extensions;

namespace Suteki.Common.HtmlHelpers
{
    public class Pager
    {
        StringBuilder htmlText = new StringBuilder();

        HtmlHelper htmlHelper;
        string controller;
        string action;
        IPagedList pagedList;

        public Pager(
            HtmlHelper htmlHelper,
            string controller,
            string action,
            IPagedList pagedList)
        {
            this.htmlHelper = htmlHelper;
            this.controller = controller;
            this.action = action;
            this.pagedList = pagedList;
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
                htmlText.AppendFormat("{0} ", htmlHelper.ActionLink(text, action, controller, new
                {
                    CurrentPage = pageNumber
                }));
            }
        }
    }
}
