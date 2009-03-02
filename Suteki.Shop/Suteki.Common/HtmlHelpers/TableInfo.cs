using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MvcContrib.UI.LegacyGrid;

namespace Suteki.Common.HtmlHelpers
{
    public class TableInfo<T> where T : class
    {
        private readonly Dictionary<string, IColumnInfo<T>> columns =
            new Dictionary<string, IColumnInfo<T>>();

        public TableInfo<T> AddColumn<TParam>(Expression<Func<T, TParam>> columnExpression, string name)
        {
            columns.Add(name, new ColumnInfo<T, TParam>(columnExpression));
            return this;
        }

        public TableInfo<T> AddColumn<TParam>(
            Expression<Func<T, TParam>> columnExpression,
            Func<IRootGridColumnBuilder<T>, IExpressionColumnBuilder<T>> forExpression,
            string name)
        {
            columns.Add(name, new ColumnInfo<T, TParam>(columnExpression, forExpression));
            return this;
        }

        public Dictionary<string, IColumnInfo<T>> Columns
        {
            get { return columns; }
        }

        public bool ContainsColumn(string columnName)
        {
            return columns.ContainsKey(columnName);
        }
    }
}