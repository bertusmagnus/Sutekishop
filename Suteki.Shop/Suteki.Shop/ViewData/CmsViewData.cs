using System;
using System.Collections.Generic;
using Suteki.Common.ViewData;

namespace Suteki.Shop.ViewData
{
    public class CmsViewData : ViewDataBase
    {
        public Content Content { get; set; }
        public IEnumerable<Menu> Menus { get; set; }

        public ITextContent TextContent
        {
            get
            {
                ITextContent textContent = Content as ITextContent;
                if (textContent == null)
                    throw new ApplicationException("ViewData Content is not of type ITextContent");
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

        public CmsViewData WithMenus(IEnumerable<Menu> menus)
        {
            this.Menus = menus;
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
