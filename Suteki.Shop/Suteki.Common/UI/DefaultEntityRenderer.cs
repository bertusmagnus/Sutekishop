using System;
using System.Web.Mvc;
using Suteki.Common.Models;

namespace Suteki.Common.UI
{
    public class DefaultEntityRenderer<T> : IEntityRenderer<T> where T : class, IEntity
    {
        public string Render(HtmlHelper htmlHelper, T entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");

            var namedEntity = entity as INamedEntity;
            return (namedEntity == null) ? "" : namedEntity.Name;
        }
    }
}