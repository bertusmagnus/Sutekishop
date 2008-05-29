using System;
using System.IO;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.UI;
using Suteki.Common.Extensions;
using Suteki.Common.ViewData;

namespace Suteki.Common.HtmlHelpers
{
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Render an error box 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string ErrorBox(this HtmlHelper htmlHelper, IErrorViewData errorViewData)
        {
            if (errorViewData.ErrorMessage == null) return string.Empty;

            HtmlTextWriter writer = new HtmlTextWriter(new StringWriter());

            writer.AddAttribute("class", "error");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.Write(errorViewData.ErrorMessage);
            writer.RenderEndTag();
            return writer.InnerWriter.ToString();
        }

        public static string MessageBox(this HtmlHelper htmlHelper, IMessageViewData messageViewData)
        {
            if (messageViewData.Message == null) return string.Empty;

            HtmlTextWriter writer = new HtmlTextWriter(new StringWriter());

            writer.AddAttribute("class", "message");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.Write(messageViewData.Message);
            writer.RenderEndTag();
            return writer.InnerWriter.ToString();
        }

        public static string Tick(this HtmlHelper htmlHelper, bool ticked)
        {
            HtmlTextWriter writer = new HtmlTextWriter(new StringWriter());

            if (ticked)
            {
                writer.AddAttribute("class", "tick");
            }
            else
            {
                writer.AddAttribute("class", "cross");
            }
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.Write("&nbsp;&nbsp;&nbsp;&nbsp;");
            writer.RenderEndTag();
            return writer.InnerWriter.ToString();
        }

        public static string UpArrowLink<T>(this HtmlHelper htmlHelper, Expression<Action<T>> action) where T : Controller
        {
            return StringExtensions.With("<a href=\"{0}\" class=\"arrowlink\">{1}</a>", htmlHelper.BuildUrlFromExpression<T>(action),
                htmlHelper.Image("~/Content/Images/Up.png"));
        }

        public static string DownArrowLink<T>(this HtmlHelper htmlHelper, Expression<Action<T>> action) where T : Controller
        {
            return "<a href=\"{0}\" class=\"arrowlink\">{1}</a>".With(
                htmlHelper.BuildUrlFromExpression<T>(action),
                htmlHelper.Image("~/Content/Images/Down.png"));
        }

        public static string CrossLink<T>(this HtmlHelper htmlHelper, Expression<Action<T>> action) where T : Controller
        {
            return "<a href=\"{0}\">{1}</a>".With(
                htmlHelper.BuildUrlFromExpression<T>(action),
                htmlHelper.Image("~/Content/Images/Cross.png"));
        }

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
