using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;
using MvcContrib.UI.LegacyGrid;

namespace Suteki.Common.HtmlHelpers
{
    public static class ColumnSortingExtensions
    {
        public const string SortBy = "sortby";
        public const string Descending = "desc";

        public static Action<IRootGridColumnBuilder<T>> CreateGridColumnBuilder<T>(
            this HtmlHelper htmlHelper,
            TableInfo<T> tableInfo)
            where T : class
        {
            var orderableGridColumnBuilder = new OrderableGridColumnBuilder<T>(htmlHelper, tableInfo);
            return orderableGridColumnBuilder.CreateGridColumnBuilder();
        }

        public static IQueryable<T> SortColumns<T>(
            this IQueryable<T> items, 
            TableInfo<T> tableInfo,
            NameValueCollection form)
            where T : class
        {
            if (form.SortColumnMissing()) return items;

            if(!tableInfo.ContainsColumn(form.SortColumnName()))
            {
                throw new ApplicationException(string.Format("Unknown column name: {0}", form.SortColumnName()));
            }

            return form.IsDescending() ?
                                           tableInfo.Columns[form.SortColumnName()].SortDescending(items) :
                                                                                                              tableInfo.Columns[form.SortColumnName()].SortAscending(items);
        }

        private static bool SortColumnMissing(this NameValueCollection form)
        {
            return string.IsNullOrEmpty(form[SortBy]);
        }

        public static string SortColumnName(this NameValueCollection form)
        {
            return System.Web.HttpUtility.UrlDecode(form[SortBy]);
        }

        private static bool IsDescending(this NameValueCollection form)
        {
            return !string.IsNullOrEmpty(form[Descending]);
        }
    }
}