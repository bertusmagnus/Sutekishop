using System;
using System.Linq;

namespace Suteki.Shop.Repositories
{
    public interface IRepository<T> where T : class
    {
        T GetById(int id);
        IQueryable<T> GetAll();
        void InsertOnSubmit(T item);
        void DeleteOnSubmit(T entity);
        void SubmitChanges();
    }
}
