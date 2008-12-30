using System;
using System.Reflection;
using Suteki.Common.Extensions;
using Suteki.Common.Models;
using Suteki.Common.Repositories;

namespace Suteki.Common.Validation
{
    /// <summary>
    /// Binds an entity incluing creating all it's dependent entities
    /// </summary>
    public class EntityBinder : IBindProperties
    {
        private readonly IRepositoryResolver repositoryResolver;

        public EntityBinder(IRepositoryResolver repositoryResolver)
        {
            this.repositoryResolver = repositoryResolver;
        }

        public EntityBinder()
        {
        }

        public void Bind(PropertyInfo property, BindingContext bindingContext)
        {
            if (typeof(IEntity).IsAssignableFrom(property.PropertyType))
            {
                string value = bindingContext.GetValue(property.Name + ".Id");
                if (value == null) return;

                int id;
                if (int.TryParse(value, out id))
                {
                    if (id == 0)
                    {
                        if(property.AllowNull())
                        {
                            property.SetValue(bindingContext.Target, null, null);
                            return;                            
                        }
                        throw new ValidationException("Please select one item");
                    }

                    IEntity entity = GetEntity(property.PropertyType, id);
                    property.SetValue(bindingContext.Target, entity, null);
                }
                else
                {
                    throw new ApplicationException(string.Format(
                        "'{0}' is not a valid value for {1}",
                        value, property.Name + ".Id"));
                }
            }
        }

        /// <summary>
        /// Get an entity of the given type and id.
        /// If a repositoryResolver is present it gets the entity from the repository
        /// otherwise it just creates a new instance using Activator
        /// </summary>
        /// <param name="type">The entity type to create</param>
        /// <param name="id">the id</param>
        /// <returns>The created entity</returns>
        private IEntity GetEntity(Type type, int id)
        {
            IEntity entity = null;

            if(repositoryResolver == null)
            {
                entity = Activator.CreateInstance(type) as IEntity;

                if (entity == null)
                {
                    throw new ApplicationException(string.Format(
                        "Failed to create new instance of IEntity type {0}, check that this type implements IEntity",
                        type.Name));
                }

                entity.Id = id;
                return entity;
            }

            var repository = repositoryResolver.GetRepository(type);
            entity = repository.GetById(id) as IEntity;

            if (entity == null)
            {
                throw new ApplicationException(string.Format(
                    "Failed to get entity type {0}, id {1} from repository",
                    type.Name, id));
            }

            return entity;
        }
    }
}