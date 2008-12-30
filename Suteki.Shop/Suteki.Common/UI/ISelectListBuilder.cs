using System.Web.Mvc;
using Suteki.Common.Models;

namespace Suteki.Common.UI
{
    public interface ISelectListBuilder<T> where T : NamedEntity<T>, new()
    {
        SelectList MakeFrom(T entity);
    }

    public interface ISelectListBuilder
    {
        SelectList MakeFrom(INamedEntity entity);
    }
}