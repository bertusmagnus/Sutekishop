using System;
using System.Linq;
using System.Data.Linq;

namespace Suteki.Shop.Repositories
{
    public interface IRepository<T> where T : class
    {
        T GetById(int id);
        IQueryable<T> GetAll();
        void InsertOnSubmit(T entity);
        void DeleteOnSubmit(T entity);
        void SubmitChanges();
        ITable GetTable();
    }
}
