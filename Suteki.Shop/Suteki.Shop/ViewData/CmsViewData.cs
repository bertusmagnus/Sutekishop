using System;

namespace Suteki.Shop.ViewData
{
    public class CmsViewData : ViewDataBase
    {
        public TextContent TextContent { get; set; }
        public Menu Menu { get; set; }

        // attempt at a fluent interface

        public CmsViewData WithTextContent(TextContent textContent)
        {
            this.TextContent = textContent;
            return this;
        }

        public CmsViewData WithMenu(Menu menu)
        {
            this.Menu = menu;
            return this;
        }
    }

    public static class CmsView
    {
        public static CmsViewData Data
        {
            get
            {
                return new CmsViewData();
            }
        }
    }
}
