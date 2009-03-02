using System;
using System.Linq;
using System.Linq.Expressions;
using MvcContrib.UI.LegacyGrid;

namespace Suteki.Common.HtmlHelpers
{
    public class ColumnInfo<T, TParameter> : IColumnInfo<T> where T : class
    {
        public Expression<Func<T, TParameter>> Expression { get; private set; }
        public Func<IRootGridColumnBuilder<T>, IExpressionColumnBuilder<T>> ForExpression { get; private set; }

        public ColumnInfo(Expression<Func<T, TParameter>> expression)
            : this(expression, col => col.For(GetObjectExpression(expression)))
        {
        }

        public ColumnInfo(
            Expression<Func<T, TParameter>> expression,
            Func<IRootGridColumnBuilder<T>, IExpressionColumnBuilder<T>> forExpression)
        {
            Expression = expression;
            ForExpression = forExpression;
        }

        private static Expression<Func<T, object>> GetObjectExpression(Expression<Func<T, TParameter>> expression)
        {
            return System.Linq.Expressions.Expression.Lambda<Func<T, object>>(
                System.Linq.Expressions.Expression.Convert(expression.Body, typeof(object)), expression.Parameters);
        }

        public IQueryable<T> SortDescending(IQueryable<T> items)
        {
            return items.OrderByDescending(Expression);
        }

        public IQueryable<T> SortAscending(IQueryable<T> items)
        {
            return items.OrderBy(Expression);
        }

        public IExpressionColumnBuilder<T> GetExpressionColumnBuilder(IRootGridColumnBuilder<T> rootGridColumnBuilder)
        {
            return ForExpression(rootGridColumnBuilder);
        }
    }
}