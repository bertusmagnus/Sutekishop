using System;
using System.Web.Mvc;
using System.Web.UI;
using System.IO;
using Suteki.Common.Extensions;
using Suteki.Common.HtmlHelpers;
using Suteki.Common.ViewData;
using Suteki.Shop.ViewData;
using Suteki.Shop.Controllers;
using System.Security.Principal;
using System.Linq.Expressions;
using IPagedList=Suteki.Common.Extensions.IPagedList;

namespace Suteki.Shop.HtmlHelpers
{
    public static class HtmlHelperExtensions
    {
        public static string LoginStatus(this HtmlHelper htmlHelper)
        {
            return htmlHelper.CurrentUser().PublicIdentity;
        }

        public static string LoginLink(this HtmlHelper htmlHelper)
        {
            if (htmlHelper.CurrentUser().CanLogin)
            {
                return htmlHelper.ActionLink<LoginController>(c => c.Logout(), "Logout");
            }
            return htmlHelper.ActionLink<LoginController>(c => c.Index(), "Login").ToSslLink();
        }

        public static User CurrentUser(this HtmlHelper htmlHelper)
        {
            User user = htmlHelper.ViewContext.HttpContext.User as User;
            if (user == null) throw new ApplicationException("Current context user cannot be cast to Suteki.Shop.User");
            return user;
        }

        public static string WriteCategories(this HtmlHelper htmlHelper, Category rootCategory, CategoryDisplay display)
        {
            CategoryWriter categoryWriter = new CategoryWriter(rootCategory, htmlHelper, display);
            return categoryWriter.Write();
        }

        public static string WriteStock(this HtmlHelper htmlHelper, Category rootCategory)
        {
            StockWriter stockWriter = new StockWriter(htmlHelper, rootCategory);
            return stockWriter.Write();
        }

        public static string WriteMenu(this HtmlHelper htmlHelper, Menu menu)
        {
            MenuWriter menuWriter = new MenuWriter(htmlHelper, menu);
            return menuWriter.Write();
        }

        public static string WriteMenu(this HtmlHelper htmlHelper, Menu menu, object attributes)
        {
            MenuWriter menuWriter = new MenuWriter(htmlHelper, menu, attributes);
            return menuWriter.Write();
        }

        public static string WriteMenu(this HtmlHelper htmlHelper, Menu menu, bool nest)
        {
            MenuWriter menuWriter = new MenuWriter(htmlHelper, menu, nest, false);
            return menuWriter.Write();
        }

        public static string WriteMenu(this HtmlHelper htmlHelper, Menu menu, bool nest, object attributes)
        {
            MenuWriter menuWriter = new MenuWriter(htmlHelper, menu, nest, attributes);
            return menuWriter.Write();
        }
    }
}
