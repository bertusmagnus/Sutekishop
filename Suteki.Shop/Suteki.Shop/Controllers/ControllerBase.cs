using Castle.Core.Logging;
using MvcContrib.Filters;
using Suteki.Common.Extensions;
using Suteki.Shop.Services;
using MvcContrib;

namespace Suteki.Shop.Controllers
{
    [Rescue("Default")]
    public abstract class ControllerBase : ConventionController, IProvidesBaseService
    {
        private IBaseControllerService baseControllerService;

        /// <summary>
        /// Supplies services and configuration to all controllers
        /// </summary>
        public IBaseControllerService BaseControllerService
        {
            get { return baseControllerService; }
            set 
            { 
                baseControllerService = value;

                ViewData["Title"] = "{0}{1}".With(
                    baseControllerService.ShopName,
                    GetControllerName());

                ViewData["MetaDescription"] = "\"{0}\"".With(baseControllerService.MetaDescription);
            }
        }

        public ILogger Logger { get; set; }

        public virtual string GetControllerName()
        {
            return " - {0}".With(GetType().Name.Replace("Controller", ""));
        }


        public virtual void AppendTitle(string text)
        {
            ViewData["Title"] = "{0} - {1}".With(ViewData["Title"], text);
        }

        public virtual void AppendMetaDescription(string text)
        {
            ViewData["MetaDescription"] = text;
        }
    }
}
