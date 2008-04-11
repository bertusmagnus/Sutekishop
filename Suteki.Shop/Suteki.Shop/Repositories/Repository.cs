using System;
using System.Linq;
using System.Linq.Expressions;
using Suteki.Shop.Extensions;
using System.Data.Linq;

namespace Suteki.Shop.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        ShopDataContext dataContext;

        public Repository(ShopDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public T GetById(int id)
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
                    new ParameterExpression[] { itemParameter }
                );

            return dataContext.GetTable<T>().Where(whereExpression).Single();
        }

        public IQueryable<T> GetAll()
        {
            return dataContext.GetTable<T>();
        }

        public void InsertOnSubmit(T entity)
        {
            this.GetTable().InsertOnSubmit(entity);
        }

        public void DeleteOnSubmit(T entity)
        {
            this.GetTable().DeleteOnSubmit(entity);
        }

        public void SubmitChanges()
        {
            dataContext.SubmitChanges();
        }

        public virtual ITable GetTable()
        {
            return dataContext.GetTable<T>();
        }
    }
}
