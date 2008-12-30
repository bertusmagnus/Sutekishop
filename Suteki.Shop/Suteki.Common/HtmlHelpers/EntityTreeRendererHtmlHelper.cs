using System;
using System.Web.Mvc;
using Microsoft.Practices.ServiceLocation;
using Suteki.Common.Models;
using Suteki.Common.UI;
using Suteki.Common.Extensions;

namespace Suteki.Common.HtmlHelpers
{
    public static class EntityTreeRendererHtmlHelper
    {
        public static string RenderEntityTree(this HtmlHelper htmlHelper, IEntity entity)
        {
            if (htmlHelper == null) throw new ArgumentNullException("htmlHelper");
            if(entity == null) throw new ArgumentNullException("entity");

            var metaEntityFactory = ServiceLocator.Current.GetInstance<IMetaEntityFactory>();

            var metaRoot = metaEntityFactory.CreateFrom(entity);

            return htmlHelper.RenderTree(new[] { metaRoot }, metaEntity => GetNodeName(metaEntity, htmlHelper));
        }

        private static string GetNodeName(MetaEntity metaEntity, HtmlHelper htmlHelper)
        {
            var collectionMetaEntity = metaEntity as CollectionMetaEntity;
            if (collectionMetaEntity != null)
            {
                return collectionMetaEntity.CollectionName;
            }

            return "{0}:{1}".With(
                EntityRenderer.RenderTypeName(metaEntity.Entity),
                EntityRenderer.Render(htmlHelper, metaEntity.Entity));
        }
    }
}