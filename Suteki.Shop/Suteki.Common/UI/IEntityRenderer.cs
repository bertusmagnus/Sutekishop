using System.Web.Mvc;
using Suteki.Common.Models;

namespace Suteki.Common.UI
{
    public interface IEntityRenderer<T> : IEntityRenderer where T : class, IEntity
    {
        string Render(HtmlHelper htmlHelper, T entity);
    }

    public interface IEntityRenderer { }
}