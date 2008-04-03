using System;
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
    }
}
