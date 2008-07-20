using System;
using Suteki.Common;
using Suteki.Common.Validation;
using System.Web.Mvc;
using Suteki.Shop.Controllers;
using Suteki.Shop.Models.ModelHelpers;

namespace Suteki.Shop
{
    public partial class Content : IOrderable, IActivatable, IUrlNamed
    {
        partial void  OnNameChanged()
        {
            UrlName = Name.ToUrlFriendly();
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
                var menu = Content1 as Menu;
                if (menu == null)
                    throw new ApplicationException("Parent Content Should Always be a Menu");
                return menu;
            }
        }

        public virtual string Link(HtmlHelper htmlHelper)
        {
            if (ContentId == 0) return string.Empty;
            return htmlHelper.ActionLink<CmsController>(c => c.Index(UrlName), Name);
        }

        public virtual string EditLink(HtmlHelper htmlHelper)
        {
            return "&nbsp;";
        }
    }
}
