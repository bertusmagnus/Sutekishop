using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using Microsoft.Web.Mvc.Internal;

namespace Suteki.Shop.HtmlHelpers
{
	public static class UrlHelperExtensions
	{
		public static string Action<T>(this UrlHelper helper, Expression<Action<T>> action) where T : Controller
		{
			var routeValues = ExpressionHelper.GetRouteValuesFromExpression(action);
			return helper.RouteUrl(routeValues);
		}
	}
}