using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using System.IO;
using Suteki.Shop.Controllers;

namespace Suteki.Shop.HtmlHelpers
{
    public class MenuWriter
    {
        HtmlHelper htmlHelper;
        Menu mainMenu;

        public MenuWriter(HtmlHelper htmlHelper, Menu mainMenu)
        {
            this.htmlHelper = htmlHelper;
            this.mainMenu = mainMenu;
        }

        public string Write()
        {
            HtmlTextWriter writer = new HtmlTextWriter(new StringWriter());

            WriteMenu(writer, mainMenu);

            return writer.InnerWriter.ToString();
        }

        private void WriteMenu(HtmlTextWriter writer, Menu menu)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "mainMenu");
            writer.RenderBeginTag(HtmlTextWriterTag.Ul);

            foreach (Content content in menu.Contents)
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Li);
                MenuLinkFactory.ForContent(content).WithHelper(htmlHelper).WriteActionLinkWith(writer);
                writer.RenderEndTag();
            }

            writer.RenderEndTag();
        }
    }

    public class MenuLinkFactory
    {
        public static IMenuLink ForContent(Content content)
        {
            if (content is TextContent) return new TextMenuLink((TextContent)content);
            if (content is ActionContent) return new ActionMenuLink((ActionContent)content);
            throw new ArgumentException("content is not a recognised content type");
        }

        public static IMenuLink ForMenu(Menu menu)
        {
            return null;
        }
    }

    public interface IMenuLink
    {
        MenuLink WithHelper(HtmlHelper htmlHelper);
    }

    public abstract class MenuLink : IMenuLink
    {
        protected HtmlHelper htmlHelper;

        public abstract void WriteActionLinkWith(HtmlTextWriter writer);

        public MenuLink WithHelper(HtmlHelper htmlHelper)
        {
            this.htmlHelper = htmlHelper;
            return this;
        }
    }

    public class TextMenuLink : MenuLink
    {
        TextContent textContent;

        public TextMenuLink(TextContent textContent)
        {
            this.textContent = textContent;
        }

        public override void WriteActionLinkWith(HtmlTextWriter writer)
        {
            writer.Write(htmlHelper.ActionLink<CmsController>(c => c.Index(textContent.UrlName), textContent.Name));
        }
    }

    public class ActionMenuLink : MenuLink
    {
        ActionContent actionContent;

        public ActionMenuLink(ActionContent actionContent)
        {
            this.actionContent = actionContent;
        }

        public override void WriteActionLinkWith(HtmlTextWriter writer)
        {
            writer.Write(htmlHelper.ActionLink(actionContent.Name, actionContent.Action, actionContent.Controller));
        }
    }
}
