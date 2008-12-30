using System.Web.Mvc;
using Suteki.Common.Models;
using Suteki.Common.UI;

namespace Suteki.Common.HtmlHelpers
{
    public static class EntityRendererHtmlHelper
    {
        public static string RenderEntity<T>(this HtmlHelper htmlHelper, T entity) where T : class, IEntity
        {
            return EntityRenderer.Render(htmlHelper, entity);
        }
    }
}