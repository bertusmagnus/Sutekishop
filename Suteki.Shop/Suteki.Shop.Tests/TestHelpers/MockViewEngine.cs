using System.Web.Mvc;

namespace Suteki.Shop.Tests
{
    public class MockViewEngine : IViewEngine
    {
        public ViewContext ViewContext { get; private set; }

        public void RenderView(ViewContext viewContext)
        {
            ViewContext = viewContext;
        }

        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName)
        {
            throw new System.NotImplementedException();
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName)
        {
            throw new System.NotImplementedException();
        }

        public void ReleaseView(ControllerContext controllerContext, IView view)
        {
            throw new System.NotImplementedException();
        }
    }
}
