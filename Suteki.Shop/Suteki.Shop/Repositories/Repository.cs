using System;
using System.Linq;
using System.Linq.Expressions;
using Suteki.Shop.Extensions;

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
    }
}
