using Microsoft.Web.Mvc;
using Suteki.Shop.Controllers;
using System.Web.Mvc;

namespace Suteki.Shop
{
    public partial class Menu
    {
        public const int MainMenuId = 1;

        public override string EditLink(HtmlHelper htmlHelper)
        {
            return htmlHelper.ActionLink<MenuController>(c => c.Edit(ContentId), "Edit");
        }

        public bool IsMainMenu
        {
            get
            {
                return ContentId == MainMenuId;
            }
        }

		public static Menu CreateDefaultMenu(int position, Content parent)
		{
			return new Menu 
			{
				ContentTypeId = ContentType.MenuId,
				Content1 = parent,
				IsActive = true,
				Position = position
			};
		}
    }
}
