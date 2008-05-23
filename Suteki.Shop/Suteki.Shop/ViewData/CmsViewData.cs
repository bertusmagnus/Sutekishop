using System;

namespace Suteki.Shop.ViewData
{
    public class CmsViewData : ViewDataBase
    {
        public Content Content { get; set; }

        public TextContent TextContent
        {
            get
            {
                TextContent textContent = Content as TextContent;
                if (textContent == null)
                    throw new ApplicationException("ViewData Content is not of type TextContent");
                return textContent;
            }
        }

        public Menu Menu
        {
            get
            {
                Menu menu = Content as Menu;
                if (menu == null)
                    throw new ApplicationException("ViewData Content is not of type Menu");
                return menu;
            }
        }

        // attempt at a fluent interface

        public CmsViewData WithContent(Content content)
        {
            this.Content = content;
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
