using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using MvcContrib.UI.Html.Grid;

namespace Suteki.Common.HtmlHelpers
{
    public class OrderableGridColumnBuilder<T> where T : class
    {
        public const string SortBy = "sortby";
        public const string Descending = "desc";

        private readonly HtmlHelper htmlHelper;
        private readonly TableInfo<T> tableInfo;

        public OrderableGridColumnBuilder(HtmlHelper htmlHelper, TableInfo<T> tableInfo)
        {
            this.htmlHelper = htmlHelper;
            this.tableInfo = tableInfo;
        }

        public virtual Action<IRootGridColumnBuilder<T>> CreateGridColumnBuilder()
        {
            return cols =>
            {
                var form = htmlHelper.ViewContext.HttpContext.Request.QueryString;
                var routeData = htmlHelper.ViewContext.RouteData;

                foreach (var name in tableInfo.Columns.Keys)
                {
                    var localName = name;
                    var encodedName = System.Web.HttpUtility.UrlEncode(name);

                    var action = routeData.GetRequiredString("action");
                    var controller = routeData.GetRequiredString("controller");

                    var values = htmlHelper.ViewContext.HttpContext.Request.GetRequestValues("action", "controller", SortBy, Descending);

                    values.Add(SortBy, encodedName);

                    var thisColumn = form.SortColumnName() == localName;
                    var notDescending = string.IsNullOrEmpty(form[Descending]);

                    if (notDescending && thisColumn)
                    {
                        values.Add(Descending, true);
                    }

                    tableInfo.Columns[name].GetExpressionColumnBuilder(cols)
                        .Header(() => htmlHelper.ViewContext.HttpContext.Response.Write(
                            "<th>" + htmlHelper.ActionLink(
                                localName, action, controller, values, new RouteValueDictionary()
                                ) + "</th>"
                            ));
                }
            };
            
        }
    }
}