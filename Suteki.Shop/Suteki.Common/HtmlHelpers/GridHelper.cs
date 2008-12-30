using System;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MvcContrib.UI.Html.Grid;
using Suteki.Common.Extensions;

namespace Suteki.Common.HtmlHelpers
{
    public static class GridHelper
    {
        public static Action<IRootGridColumnBuilder<T>> LinqAutoColumn<T>(this HtmlHelper htmlHelper) where T : class
        {
            return columnBuilder =>
            {
                var keys = new LinqForeignKeyFinder<T>();
                PropertyInfo primaryKeyProperty = null;

                foreach (var propertyInfo in typeof (T).GetProperties())
                {
                    // don't include enumerables (except string)
                    if (propertyInfo.PropertyType.IsEnumerable() &&
                        propertyInfo.PropertyType != typeof (string)) continue;

                    // don't include foreign keys
                    if (keys.IsForeignKey(propertyInfo)) continue;

                    var thisPropertyInfo = propertyInfo;

                    if (thisPropertyInfo.IsPrimaryKey())
                    {
                        primaryKeyProperty = thisPropertyInfo;

                        columnBuilder.For(
                            item =>
                            htmlHelper.ActionLink(thisPropertyInfo.GetValue(item, null).ToString(), "Edit",
                                new {id = thisPropertyInfo.GetValue(item, null)}),
                                thisPropertyInfo.Name.Pretty()).DoNotEncode();
                    }
                    else
                    {
                        columnBuilder.For(item => thisPropertyInfo.GetValue(item, null),
                            thisPropertyInfo.Name.Pretty());
                    }

                }

                if (primaryKeyProperty != null)
                {
                    columnBuilder.For(item => htmlHelper.ActionLink("X", "Delete",
                        new { id = primaryKeyProperty.GetValue(item, null) }), "Delete").DoNotEncode();
                }
            };
        }
    }
}