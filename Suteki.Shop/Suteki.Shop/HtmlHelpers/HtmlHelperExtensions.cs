using System;
using System.Web.Mvc;
using System.Web.UI;
using System.IO;
using Suteki.Shop.ViewData;
using Suteki.Shop.Controllers;
using System.Security.Principal;

namespace Suteki.Shop.HtmlHelpers
{
    public static class HtmlHelperExtensions
    {
        public static string LoginStatus(this HtmlHelper htmlHelper)
        {
            HtmlTextWriter writer = new HtmlTextWriter(new StringWriter());

            writer.AddAttribute("id", "loginStatus");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            IPrincipal user = htmlHelper.ViewContext.HttpContext.User;
            if (user.Identity.IsAuthenticated)
            {
                writer.Write(user.Identity.Name);
            }
            else
            {
                writer.Write("Guest");
            }
            
            writer.RenderEndTag();
            
            return writer.InnerWriter.ToString();
        }

        public static string LoginLink(this HtmlHelper htmlHelper)
        {
            if (htmlHelper.ViewContext.HttpContext.User.Identity.IsAuthenticated)
            {
                return htmlHelper.ActionLink<LoginController>(c => c.Logout(), "Logout");
            }
            else
            {
                return htmlHelper.ActionLink<LoginController>(c => c.Index(), "Login");
            }
        }

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
            writer.Write("&nbsp;");
            writer.RenderEndTag();
            return writer.InnerWriter.ToString();
        }
    }
}
