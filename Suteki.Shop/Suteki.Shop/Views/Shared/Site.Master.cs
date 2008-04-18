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

namespace Suteki.Shop.Views.Shared
{
    public partial class Site : System.Web.Mvc.ViewMasterPage
    {
        protected string LeftMenuContents
        {
            get
            {
                ControllerBase controller = this.ViewContext.Controller as ControllerBase;

                if (controller != null)
                {
                    IRepository<Suteki.Shop.Category> categoryRepository =
                        controller.BaseControllerService.CategoryRepository;

                    return Html.WriteCategories(categoryRepository.GetRootCategory());
                }

                return "";
            }
        }
    }
}
