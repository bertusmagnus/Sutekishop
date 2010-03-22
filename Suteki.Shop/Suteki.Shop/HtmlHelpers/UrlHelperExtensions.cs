using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Suteki.Shop.HtmlHelpers
{
	public static class UrlHelperExtensions
	{
		public static string Action<T>(this UrlHelper helper, Expression<Action<T>> action) where T : Controller
		{
            var routeValues = Microsoft.Web.Mvc.Internal.ExpressionHelper.GetRouteValuesFromExpression(action);
			return helper.RouteUrl(routeValues);
		}
	}
}