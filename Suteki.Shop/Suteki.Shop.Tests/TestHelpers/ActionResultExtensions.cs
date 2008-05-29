using System;
using System.Web.Mvc;
using NUnit.Framework;
using Suteki.Common.Extensions;
using Suteki.Shop.ViewData;

namespace Suteki.Shop.Tests
{
    public static class ActionResultExtensions
    {
        public static ViewResult ReturnsViewResult(this ActionResult result)
        {
            ViewResult viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult, "result is not a ViewResult");
            return viewResult;
        }

        public static ViewResult ForView(this ViewResult result, string viewName)
        {
            Assert.AreEqual(viewName, result.ViewName);
            return result;
        }

        // View Data

        public static ViewResult AssertNotNull<TViewData, T>(
            this ViewResult result,
            Func<TViewData, T> property) where TViewData : class
        {
            Assert.IsNotNull(property(result.GetViewData<TViewData>()));
            return result;
        }

        public static ViewResult AssertNull<TViewData, T>(
            this ViewResult result,
            Func<TViewData, T> property) where TViewData : class
        {
            Assert.IsNull(property(result.GetViewData<TViewData>()));
            return result;
        }

        public static ViewResult AssertAreSame<TViewData, T>(
            this ViewResult result, 
            T expected,
            Func<TViewData, T> property) where TViewData : class
        {
            Assert.AreSame(expected, property(result.GetViewData<TViewData>()));
            return result;
        }

        public static ViewResult AssertAreEqual<TViewData, T>(
            this ViewResult result,
            T expected,
            Func<TViewData, T> property) where TViewData : class
        {
            Assert.AreEqual(expected, property(result.GetViewData<TViewData>()));
            return result;
        }

        public static TViewData GetViewData<TViewData>(this ViewResult result) where TViewData : class
        {
            TViewData viewData = result.ViewData.Model as TViewData;
            Assert.IsNotNull(viewData, "viewData is not {0}".With(typeof(TViewData).Name));
            return viewData;
        }
    }
}
