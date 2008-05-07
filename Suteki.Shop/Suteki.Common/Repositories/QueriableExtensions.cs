using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Suteki.Common.Repositories
{
    public static class QueriableExtensions
    {
        const string expression_not_property = "propertyExpression must be a property accessor. e.g: 'x => x.MyProperty'";

        public static IQueryable<T> WithSelectItem<T>(this IQueryable<T> items, Expression<Func<T, string>> propertyExpression) 
            where T : new()
        {
            if (propertyExpression.Body.NodeType != ExpressionType.MemberAccess)
            {
                throw new ArgumentException(expression_not_property);
            }
            MemberExpression memberExpression = propertyExpression.Body as MemberExpression;
            PropertyInfo property = memberExpression.Member as PropertyInfo;
            if (property == null)
            {
                throw new ArgumentException(expression_not_property);
            }

            T selectItem = new T();
            property.SetValue(selectItem, "<select>", null);

            var selectItems = new List<T> { selectItem }.AsQueryable();
            return selectItems.Union(items);
        }
    }
}
