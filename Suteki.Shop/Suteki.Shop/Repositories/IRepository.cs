using System;
using System.Linq;

namespace Suteki.Shop.Repositories
{
    public interface IRepository<T> where T : class
    {
        T GetById(int id);
    }
}
