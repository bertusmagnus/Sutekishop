using System;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Linq;
using Suteki.Common.Extensions;

namespace Suteki.Common.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        readonly DataContext dataContext;

        public Repository(IDataContextProvider dataContextProvider)
        {
            this.dataContext = dataContextProvider.DataContext;
        }

        public virtual T GetById(int id)
        {
            var itemParameter = Expression.Parameter(typeof(T), "item");

            var whereExpression = Expression.Lambda<Func<T, bool>>
                (
                Expression.Equal(
                    Expression.Property(
                        itemParameter,
                        typeof(T).GetPrimaryKey().Name
                        ),
                    Expression.Constant(id)
                    ),
                new[] { itemParameter }
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