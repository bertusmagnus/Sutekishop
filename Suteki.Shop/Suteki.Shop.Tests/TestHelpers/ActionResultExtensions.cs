using System;
using System.Web.Mvc;
using NUnit.Framework;
using Suteki.Shop.ViewData;

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

        public static RenderViewResult AssertNotNull<T>(this RenderViewResult result, Func<ShopViewData, T> property)
        {
            Assert.IsNotNull(property(result.GetShopViewData()));
            return result;
        }

        public static RenderViewResult AssertNull<T>(this RenderViewResult result, Func<ShopViewData, T> property)
        {
            Assert.IsNull(property(result.GetShopViewData()));
            return result;
        }

        public static RenderViewResult AssertAreSame<T>(
            this RenderViewResult result, 
            T expected, 
            Func<ShopViewData, T> property)
        {
            Assert.AreSame(expected, property(result.GetShopViewData()));
            return result;
        }

        public static RenderViewResult AssertAreEqual<T>(
            this RenderViewResult result,
            T expected,
            Func<ShopViewData, T> property)
        {
            Assert.AreEqual(expected, property(result.GetShopViewData()));
            return result;
        }

        public static ShopViewData GetShopViewData(this RenderViewResult result)
        {
            ShopViewData viewData = result.ViewData as ShopViewData;
            Assert.IsNotNull(viewData, "viewData is not ShopViewData");
            return viewData;
        }
    }
}
