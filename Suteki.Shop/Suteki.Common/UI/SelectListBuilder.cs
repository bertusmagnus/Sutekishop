using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NHibernate;
using Suteki.Common.Extensions;
using Suteki.Common.Models;
using Suteki.Common.Repositories;

namespace Suteki.Common.UI
{
    /// <summary>
    /// Create a select list for the given NamedEntity
    /// </summary>
    /// <typeparam name="T">The NamedEntity to create the select list for</typeparam>
    public class SelectListBuilder<T> : ISelectListBuilder<T>, ISelectListBuilder where T : NamedEntity<T>, new()
    {
        private readonly IRepository<T> repository;

        public SelectListBuilder(IRepository<T> repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// Make a select list for the given NamedEntity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public SelectList MakeFrom(T entity)
        {
            return ((ISelectListBuilder)this).MakeFrom(entity);
        }

        SelectList ISelectListBuilder.MakeFrom(INamedEntity entity)
        {
            IEnumerable<T> items = null;
            try
            {
                items = repository.GetAll().OrderBy(e => e.Name).WithSelectItem(e => e.Name);
            }
            catch (PropertyAccessException e)
            {
                throw new ApplicationException("Load of type: {0}, failed".With(typeof (T).Name, entity.Id), e);
            }

            return (entity == null)
                       ? new SelectList(items, "Id", "Name", 0)
                       : new SelectList(items, "Id", "Name", entity.Id);
        }
    }
}