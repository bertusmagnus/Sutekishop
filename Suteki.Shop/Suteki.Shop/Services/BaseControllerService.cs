using System;
using Suteki.Shop.Repositories;

namespace Suteki.Shop.Services
{
    public class BaseControllerService : IBaseControllerService
    {
        public IRepository<Category> CategoryRepository { get; private set; }

        public BaseControllerService(IRepository<Category> categoryRepository)
        {
            this.CategoryRepository = categoryRepository;
        }
    }
}
