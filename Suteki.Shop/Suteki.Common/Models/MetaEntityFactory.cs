using System.Collections.Generic;
using Suteki.Common.Extensions;
using Suteki.Common.Services;

namespace Suteki.Common.Models
{
    public interface IMetaEntityFactory
    {
        MetaEntity CreateFrom(IEntity entity);
    }

    /// <summary>
    /// Builds a meta entity tree by reflecting over the given entity's object graph
    /// </summary>
    public class MetaEntityFactory : IMetaEntityFactory
    {
        private readonly IEntityTypeResolver entityTypeResolver;
        private readonly HashSet<string> relationshipTokens = new HashSet<string>();
        private NestingLevel level;

        public MetaEntityFactory(IEntityTypeResolver entityTypeResolver)
        {
            this.entityTypeResolver = entityTypeResolver;
        }

        public MetaEntity CreateFrom(IEntity entity)
        {
            relationshipTokens.Clear();
            level = new NestingLevel();

            var root = new RootMetaEntity(entity);
            BuildChildren(root);
            return root;
        }

        private void BuildChildren(MetaEntity parentMetaEntity)
        {
            if(!level.TryPush()) return;

            var parentEntity = parentMetaEntity.Entity;

            foreach (var property in parentEntity.GetType().GetProperties())
            {
                if(property.PropertyType.IsEntity())
                {
                    var childEntity = property.GetValue(parentEntity, null) as IEntity;
                    if(childEntity != null)
                    {
                        var childMetaEntity = new MetaEntity(childEntity, parentMetaEntity);
                        AddChild(parentMetaEntity, childMetaEntity);
                    }
                }

                if (property.PropertyType.IsEntityCollection())
                {
                    var collectionMetaEntity = new CollectionMetaEntity(property, parentMetaEntity);
                    AddChild(parentMetaEntity, collectionMetaEntity);
                }
            }

            level.Pop();
        }

        private void BuildChildren(CollectionMetaEntity parentMetaEntity)
        {
            if(!level.TryPush()) return;

            foreach (var childEntity in parentMetaEntity.Collection)
            {
                var childMetaEntity = new MetaEntity((IEntity)childEntity, parentMetaEntity);
                AddChild(parentMetaEntity, childMetaEntity);
            }

            level.Pop();
        }

        private void AddChild(MetaEntity parentMetaEntity, MetaEntity childMetaEntity)
        {
            if (!CircularReferenceDetectedOn(childMetaEntity))
            {
                parentMetaEntity.Children.Add(childMetaEntity);

                var collectionMetaEntity = childMetaEntity as CollectionMetaEntity;
                if(collectionMetaEntity == null)
                {
                    BuildChildren(childMetaEntity);
                }
                else
                {
                    BuildChildren(collectionMetaEntity);
                }
            }
        }

        /// <summary>
        /// Many entities have bi-directional associations.
        /// An Order has a collection of OrderLines: Order.OrderLines and an OrderLine has an Order
        /// OrderLine.Order. This function detects such associations and prevents the reflection of 
        /// the object graph trying routes which have already been detected.
        /// </summary>
        /// <param name="metaEntity"></param>
        /// <returns></returns>
        private bool CircularReferenceDetectedOn(MetaEntity metaEntity)
        {
            var entityType = entityTypeResolver.GetRealTypeOf(metaEntity.Relationship.EntityType);
            var parentType = entityTypeResolver.GetRealTypeOf(metaEntity.Relationship.ParentType);

            // make a simple token from the parent and child types
            var relationshipToken = "{0}:{1}".With(parentType.Name, entityType.Name);

            if (relationshipTokens.Contains(relationshipToken)) return true;

            // make a reverse token.
            var testToken = "{0}:{1}".With(entityType.Name, parentType.Name);

            // add the new reverse token to the list.
            relationshipTokens.Add(testToken);

            return false;
        }

        private class NestingLevel
        {
            private const int maxLevel = 4;
            private int level;

            public bool TryPush()
            {
                if (level == maxLevel) return false;
                level++;
                return true;
            }

            public void Pop()
            {
                level--;
            }
        }
    }
}