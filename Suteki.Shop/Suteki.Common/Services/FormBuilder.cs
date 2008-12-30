using Castle.MicroKernel;
using Suteki.Common.Models;
using Suteki.Common.UI;

namespace Suteki.Common.Services
{
    /// <summary>
    /// Builds the lookup data needed to create a form for type T
    /// </summary>
    /// <typeparam name="T">The type to build a form for</typeparam>
    public class FormBuilder<T> : IFormBuilder<T>
    {
        // The IoC kernel
        private readonly IKernel kernel;

        public FormBuilder(IKernel kernel)
        {
            this.kernel = kernel;
        }

        /// <summary>
        /// Takes any entity, 
        /// finds any properties that are of typeNamedEntity,
        /// creates a SelectList to populate that entity by using a SelectListBuilder
        /// and adds it to the SelectListCollection
        /// </summary>
        /// <param name="entity">The entity to create select lists for</param>
        /// <returns>a collection of SelectList</returns>
        public virtual SelectListCollection GetSelectLists(T entity)
        {
            var selectLists = new SelectListCollection();

            // find any property of type NamedEntity
            var entityType = entity.GetType();
            foreach (var property in entityType.GetProperties())
            {
                if (typeof(INamedEntity).IsAssignableFrom(property.PropertyType))
                {
                    // get a SelectListBuilder for the NamedEntity type from the IoC kernel
                    var selectListBuilderType = typeof(ISelectListBuilder<>).MakeGenericType(new[] { property.PropertyType });
                    var selectListBuilder = kernel.Resolve(selectListBuilderType) as ISelectListBuilder;
                    if (selectListBuilder != null)
                    {
                        // use the SelectListBuilder to create the SelectList
                        var selectList = selectListBuilder.MakeFrom((INamedEntity)property.GetValue(entity, null));
                        selectLists.Add(property.Name, selectList);
                    }
                }
            }

            return selectLists;
        }
    }
}