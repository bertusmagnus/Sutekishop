using System;
using System.Collections;
using System.Reflection;
using Iesi.Collections.Generic;
using Suteki.Common.Extensions;

namespace Suteki.Common.Models
{
    /// <summary>
    /// Represents an Entity graph as IComposite.
    /// This is so that we can process an entity graph as if it was any other composite
    /// </summary>
    public class MetaEntity : IComposite<MetaEntity>
    {
        protected readonly IEntity entity;
        public virtual IEntity Entity
        {
            get { return entity; }
        }

        public MetaEntity Parent { get; private set; }

        private readonly ISet<MetaEntity> children = new HashedSet<MetaEntity>();

        public ISet<MetaEntity> Children
        {
            get { return children; }
        }

        public MetaEntity(IEntity entity, MetaEntity parent)
        {
            this.entity = entity;
            Parent = parent;
        }

        public virtual EntityRelationship Relationship
        {
            get
            {
                return new EntityRelationship(Entity.GetType(), Parent.Entity.GetType());
            }
        }

        public override string ToString()
        {
            return "{0}, {1}".With(Entity.GetType().Name, Entity.ToString());
        }
    }

    public class RootMetaEntity : MetaEntity
    {
        public RootMetaEntity(IEntity entity) : base(entity, new NullMetaEntity())
        {
        }

        public override EntityRelationship Relationship
        {
            get
            {
                return new EntityRelationship(Entity.GetType(), typeof(NullEntity));
            }
        }
    }

    /// <summary>
    /// Specialised MetaEntity that represents collections
    /// </summary>
    public class CollectionMetaEntity : MetaEntity
    {
        public Type CollectionType { get; private set; }
        public string CollectionName { get; private set; }
        public IEnumerable Collection { get; private set; }

        public CollectionMetaEntity(PropertyInfo property, MetaEntity parent) : base(new CollectionEntity(property, parent.Entity), parent)
        {
            var propertyType = property.PropertyType;

            if (!propertyType.IsEnumerable()) throw new ArgumentException("property is not of type IEnumerable");
            if (!propertyType.IsGenericType) throw new ArgumentException("property is not a generic type");

            var genericTypeArguments = propertyType.GetGenericArguments();
            if (genericTypeArguments.Length != 1)
                throw new ArgumentException("Expected 1 type argument for collection");

            CollectionType = genericTypeArguments[0];

            if (!typeof(IEntity).IsAssignableFrom(genericTypeArguments[0])) 
                throw new ArgumentException("collection type is not IEnumerable");

            Collection = property.GetValue(parent.Entity, null) as IEnumerable;
            CollectionName = property.Name;
        }

        public override IEntity Entity
        {
            get
            {
                return ((CollectionEntity) entity).Parent;
            }
        }

        public override EntityRelationship Relationship
        {
            get
            {
                return new EntityRelationship(CollectionType, Parent.Entity.GetType());
            }
        }

        public override string ToString()
        {
            return "Collection of {0}".With(CollectionType.Name);
        }
    }

    public class NullMetaEntity : MetaEntity
    {
        public NullMetaEntity() : base(null, null)
        {
        }
    }

    /// <summary>
    /// Special entity type for representing collections
    /// </summary>
    public class CollectionEntity : Entity<CollectionEntity>
    {
        public PropertyInfo Property { get; private set; }
        public IEntity Parent { get; private set; }

        public CollectionEntity(PropertyInfo property, IEntity parent)
        {
            Property = property;
            Parent = parent;
        }
    }

    public class NullEntity : Entity<NullEntity> {}

    public class EntityRelationship
    {
        public Type EntityType { get; private set; }
        public Type ParentType { get; private set; }

        public EntityRelationship(Type entityType, Type parentType)
        {
            EntityType = entityType;
            ParentType = parentType;
        }
    }
}