using System;
using Microsoft.Web.Mvc;
using Suteki.Common;
using Suteki.Common.Validation;
using System.Web.Mvc;
using Suteki.Shop.Controllers;
using Suteki.Shop.Models.Exceptions;
using Suteki.Shop.Models.ModelHelpers;
using Suteki.Common.HtmlHelpers;
using Suteki.Shop.HtmlHelpers;
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
                    if (thisMenu.Menu != null && this.Menu.IsMainMenu) return thisMenu;
                }

				return this.Menu == null ? null : Menu.SubMenu;
            }
        }

        public Menu Menu
        {
            get
            {
                if (ContentId == Suteki.Shop.Menu.MainMenuId) return null;
               /* var menu = Content1 as Menu;
                if (menu == null)
                    throw new NoMenuException("Parent Content Should Always be a Menu");*/
				return Content1 as Menu;
            }
        }

        public virtual string Link(HtmlHelper htmlHelper)
        {
            if (ContentId == 0) return string.Empty;
            return htmlHelper.ActionLink<CmsController>(c => c.Index(UrlName), Name);
        }

		public virtual string Url(UrlHelper urlHelper)
		{
			return urlHelper.Action<CmsController>(c => c.Index(UrlName));
		}

        public virtual string EditLink(HtmlHelper htmlHelper)
        {
            return "&nbsp;";
        }

        public bool CanEdit(User user)
        {
            if (ContentId == 0) return false;
            return user.IsAdministrator;
        }
    }
}
