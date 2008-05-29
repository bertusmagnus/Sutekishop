using System;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Linq;
using Suteki.Common.Extensions;
using Suteki.Common.Repositories;

namespace Suteki.Common.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        DataContext dataContext;

        public Repository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public virtual T GetById(int id)
        {
            var itemParameter = Expression.Parameter(typeof(T), "item");

            var whereExpression = Expression.Lambda<Func<T, bool>>
                (
                Expression.Equal(
                    Expression.Property(
                        itemParameter,
                        TypeExtensions.GetPrimaryKey(typeof(T)).Name
                        ),
                    Expression.Constant(id)
                    ),
                new ParameterExpression[] { itemParameter }
                );

            return GetAll().Where(whereExpression).Single();
        }

        public virtual IQueryable<T> GetAll()
        {
            return dataContext.GetTable<T>();
        }

        public virtual void InsertOnSubmit(T entity)
        {
            this.GetTable().InsertOnSubmit(entity);
        }

        public virtual void DeleteOnSubmit(T entity)
        {
            this.GetTable().DeleteOnSubmit(entity);
        }

        public virtual void SubmitChanges()
        {
            dataContext.SubmitChanges();
        }

        public virtual ITable GetTable()
        {
            return dataContext.GetTable<T>();
        }
    }
}