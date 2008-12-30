using System;
using System.Web.Mvc;
using Microsoft.Practices.ServiceLocation;
using Suteki.Common.Models;
using Suteki.Common.Services;

namespace Suteki.Common.UI
{
    public class EntityRenderer
    {
        public static string Render(HtmlHelper htmlHelper, IEntity entity)
        {
            if (htmlHelper == null) throw new ArgumentNullException("htmlHelper");
            if (entity == null) throw new ArgumentNullException("entity");

            var entityType = GetEntityType(entity);

            var entityRendererType = typeof(IEntityRenderer<>).MakeGenericType(entityType);
            var renderer = ServiceLocator.Current.GetInstance(entityRendererType);

            var renderMethod = entityRendererType.GetMethod("Render");
            var html = (string) renderMethod.Invoke(renderer, new object[] { htmlHelper, entity });

            return html;
        }

        private static Type GetEntityType(IEntity entity)
        {
            var typeResolver = ServiceLocator.Current.GetInstance<IEntityTypeResolver>();
            return typeResolver.GetTypeOf(entity);
        }

        public static string RenderTypeName(IEntity entity)
        {
            return GetEntityType(entity).Name;
        }
    }
}