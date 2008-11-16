using System;
using System.Linq;
using System.Web;
using Castle.MicroKernel;

namespace Suteki.Shop.IoC
{
    public class UrlBasedComponentSelector : IHandlerSelector
    {
        private readonly Type[] selectableTypes;

        public UrlBasedComponentSelector(params Type[] selectableTypes)
        {
            this.selectableTypes = selectableTypes;
        }

        public bool HasOpinionAbout(string key, Type service)
        {
            foreach (var type in selectableTypes)
            {
                if(service == type) return true;
            }
            return false;
        }

        public IHandler SelectHandler(string key, Type service, IHandler[] handlers)
        {
            var id = string.Format("{0}:{1}", service.Name, GetHostname());
            var selectedHandler = handlers.Where(h => h.ComponentModel.Name == id).FirstOrDefault();
            if (selectedHandler == null)
            {
                throw new ApplicationException(string.Format(
                    "Cannot find component with id: {0}", id));
            }
            return selectedHandler;
        }

        protected string GetHostname()
        {
            return HttpContext.Current.Request.ServerVariables["SERVER_NAME"];
        }
    }
}
