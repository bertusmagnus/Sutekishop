using System;
using System.Web.Mvc;
using NUnit.Framework;
using Suteki.Shop.ViewData;
using Suteki.Shop.Extensions;

namespace Suteki.Shop.Tests
{
    public static class ActionResultExtensions
    {
        public static RenderViewResult ReturnsRenderViewResult(this ActionResult result)
        {
            RenderViewResult renderViewResult = result as RenderViewResult;
            Assert.IsNotNull(renderViewResult, "result is not a RenderViewResult");
            return renderViewResult;
        }

        public static RenderViewResult ForView(this RenderViewResult result, string viewName)
        {
            Assert.AreEqual(viewName, result.ViewName);
            return result;
        }

        // View Data

        public static RenderViewResult AssertNotNull<TViewData, T>(
            this RenderViewResult result, 
            Func<TViewData, T> property) where TViewData : ViewDataBase
        {
            Assert.IsNotNull(property(result.GetViewData<TViewData>()));
            return result;
        }

        public static RenderViewResult AssertNull<TViewData, T>(
            this RenderViewResult result,
            Func<TViewData, T> property) where TViewData : ViewDataBase
        {
            Assert.IsNull(property(result.GetViewData<TViewData>()));
            return result;
        }

        public static RenderViewResult AssertAreSame<TViewData, T>(
            this RenderViewResult result, 
            T expected,
            Func<TViewData, T> property) where TViewData : ViewDataBase
        {
            Assert.AreSame(expected, property(result.GetViewData<TViewData>()));
            return result;
        }

        public static RenderViewResult AssertAreEqual<TViewData, T>(
            this RenderViewResult result,
            T expected,
            Func<TViewData, T> property) where TViewData : ViewDataBase
        {
            Assert.AreEqual(expected, property(result.GetViewData<TViewData>()));
            return result;
        }

        public static TViewData GetViewData<TViewData>(this RenderViewResult result) where TViewData : ViewDataBase
        {
            TViewData viewData = result.ViewData as TViewData;
            Assert.IsNotNull(viewData, "viewData is not {0}".With(typeof(TViewData).Name));
            return viewData;
        }
    }
}
