using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Suteki.Shop;
using Suteki.Shop.ViewData;
using Suteki.Shop.HtmlHelpers;
using Suteki.Shop.Repositories;
using Suteki.Shop.Controllers;
using Suteki.Shop.Extensions;

namespace Suteki.Shop.Views.Shared
{
    public partial class Site : System.Web.Mvc.ViewMasterPage
    {
        protected string MainMenu
        {
            get
            {
                ControllerBase controller = this.ViewContext.Controller as ControllerBase;

                if (controller != null)
                {
                    IRepository<Content> contentRepository =
                        controller.BaseControllerService.ContentRepository;

                    return Html.WriteMenu(contentRepository.GetMainMenu(), new { _class = "mainMenu" });
                }

                return "";
            }
        }

        protected string SiteUrl
        {
            get
            {
                ControllerBase controller = this.ViewContext.Controller as ControllerBase;
                return controller.BaseControllerService.SiteUrl;
            }
        }

        protected string RsdUrl
        {
            get
            {
                return "\"{0}rsd.xml\"".With(SiteUrl);
            }
        }

        protected string WlwManifestUrl
        {
            get
            {
                return "\"{0}wlwmanifest.xml\"".With(SiteUrl);
            }
        }
    }
}
