using System;
using System.Web.Mvc;
using System.Web.Compilation;

namespace Suteki.Shop.Components
{
    /// <summary>
    /// This class is purely to force RenderView to be virtual
    /// </summary>
    public class ComponentControllerBase : ComponentController
    {
        public ComponentControllerBase()
            : base()
        {
        }

        public ComponentControllerBase(ViewContext viewContext)
            : base(viewContext)
        {
        }

        public virtual new void RenderView(string viewName)
        {
            RenderView(viewName, null);
        }

        public virtual new void RenderView(string viewName, object viewData)
        {
            base.RenderView(viewName, viewData);
        }
    }
}
