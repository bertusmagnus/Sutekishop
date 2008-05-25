using System;
using Suteki.Shop.Validation;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Suteki.Shop.Controllers;

namespace Suteki.Shop
{
    public partial class Content : IOrderable, IActivatable
    {
        partial void  OnNameChanged()
        {
            UrlName = Regex.Replace(Name, @"[^A-Za-z0-9]", "_");
        }

        partial void OnNameChanging(string value)
        {
            value.Label("Name").IsRequired();
        }

        public bool IsTextContent
        {
            get
            {
                return this is ITextContent;
            }
        }

        public bool IsMenu
        {
            get
            {
                return this is Menu;
            }
        }

        public bool IsActionContent
        {
            get
            {
                return this is ActionContent;
            }
        }

        public string Type
        {
            get
            {
                if (IsMenu) return "Menu";
                if (IsTextContent) return "Page";
                if (IsActionContent) return "Action";
                throw new ApplicationException("Unknown Type");
            }
        }

        public bool HasSubMenu
        {
            get
            {
                if (ParentContentId.HasValue)
                {
                    return ParentContentId != Menu.MainMenuId;
                }
                return false;
            }
        }

        public Menu SubMenu
        {
            get
            {
                Menu thisMenu = this as Menu;
                if (thisMenu != null)
                {
                    if (thisMenu.IsMainMenu) return null;
                    if (thisMenu.Menu.IsMainMenu) return thisMenu;
                }

                return this.Menu.SubMenu;
            }
        }

        public Menu Menu
        {
            get
            {
                if (ContentId == Suteki.Shop.Menu.MainMenuId) return null;
                Menu menu = Content1 as Menu;
                if (menu == null)
                    throw new ApplicationException("Parent Content Should Always be a Menu");
                return menu;
            }
        }

        public virtual string Link(HtmlHelper htmlHelper)
        {
            return htmlHelper.ActionLink<CmsController>(c => c.Index(UrlName), Name);
        }

        public virtual string EditLink(HtmlHelper htmlHelper)
        {
            return "&nbsp;";
        }
    }
}
