using System.Linq;
using MvcContrib.UI.Html.Grid;

namespace Suteki.Common.HtmlHelpers
{
    public interface IColumnInfo<T> where T : class
    {
        IQueryable<T> SortDescending(IQueryable<T> items);
        IQueryable<T> SortAscending(IQueryable<T> items);
        IExpressionColumnBuilder<T> GetExpressionColumnBuilder(IRootGridColumnBuilder<T> rootGridColumnBuilder);
    }
}