using System.Web.Mvc;
using Suteki.Common.Models;

namespace Suteki.Common.UI
{
    public class CollectionEntityRenderer : IEntityRenderer<CollectionEntity>
    {
        public string Render(HtmlHelper htmlHelper, CollectionEntity entity)
        {
            return entity.Property.Name;
        }
    }
}