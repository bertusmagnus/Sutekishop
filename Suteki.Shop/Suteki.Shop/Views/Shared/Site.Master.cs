using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Suteki.Common.Extensions;
using Suteki.Common.Repositories;
using Suteki.Shop;
using Suteki.Shop.ViewData;
using Suteki.Shop.HtmlHelpers;
using Suteki.Shop.Repositories;
using Suteki.Shop.Controllers;

namespace Suteki.Shop.Views.Shared
{
    public partial class Site : System.Web.Mvc.ViewMasterPage
    {
        protected string MainMenu
        {
            get
            {
                var controller = this.ViewContext.Controller as IProvidesBaseService;

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
                return GetController().BaseControllerService.SiteUrl;
            }
        }

        private IProvidesBaseService GetController()
        {
            if (ViewContext != null) return ViewContext.Controller as IProvidesBaseService;
            throw new ApplicationException("Controller does not implement IProvidesBaseService");
        }

        protected string Title
        {
            get
            {
                return GetController().BaseControllerService.ShopName;
            }
        }

        protected string Email
        {
            get
            {
                return GetController().BaseControllerService.EmailAddress;
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

        protected string GoogleTrackingCode
        {
            get
            {
                return "\"{0}\"".With(GetController().BaseControllerService.GoogleTrackingCode);
            }
        }
    }
}
