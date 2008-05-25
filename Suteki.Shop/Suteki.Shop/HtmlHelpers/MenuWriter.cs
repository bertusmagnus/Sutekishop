using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using System.IO;
using Suteki.Shop.Controllers;
using Suteki.Shop.Repositories;
using Suteki.Shop.Extensions;

namespace Suteki.Shop.HtmlHelpers
{
    public class MenuWriter
    {
        HtmlHelper htmlHelper;
        Menu menu;
        object attributes;
        bool nest;

        public MenuWriter(HtmlHelper htmlHelper, Menu menu)
            : this(htmlHelper, menu, false, null)
        { }

        public MenuWriter(HtmlHelper htmlHelper, Menu menu, object attributes)
            : this(htmlHelper, menu, false, attributes)
        { }

        public MenuWriter(HtmlHelper htmlHelper, Menu menu, bool nest, object attributes)
        {
            this.htmlHelper = htmlHelper;
            this.menu = menu;
            this.nest = nest;
            this.attributes = attributes;
        }

        public string Write()
        {
            HtmlTextWriter writer = new HtmlTextWriter(new StringWriter());

            WriteMenu(writer, menu);

            return writer.InnerWriter.ToString();
        }

        private void WriteMenu(HtmlTextWriter writer, Menu menu)
        {
            if (menu == null) return;

            WriteAttributes(writer);
            writer.RenderBeginTag(HtmlTextWriterTag.Ul);

            foreach (Content content in menu.Contents.InOrder().ActiveFor(htmlHelper.CurrentUser()))
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Li);
                writer.Write(content.Link(htmlHelper));

                if (nest && content is Menu)
                {
                    WriteMenu(writer, (Menu)content);
                }

                writer.RenderEndTag();
            }

            WriteEditLink(writer, menu);

            writer.RenderEndTag();
        }

        private void WriteEditLink(HtmlTextWriter writer, Menu menu)
        {
            if (htmlHelper.CurrentUser().IsAdministrator)
            {
                writer.AddAttribute("class", "editMenuLink");
                writer.RenderBeginTag(HtmlTextWriterTag.Li);
                writer.Write(htmlHelper.ActionLink<CmsController>(c => c.List(menu.ContentId), "Edit this menu"));
                writer.RenderEndTag();
            }
        }

        private void WriteAttributes(HtmlTextWriter writer)
        {
            if (attributes == null) return;
            foreach (var property in attributes.GetProperties())
            {
                writer.AddAttribute(property.Name, property.Value.ToString());
            }
        }
    }
}
