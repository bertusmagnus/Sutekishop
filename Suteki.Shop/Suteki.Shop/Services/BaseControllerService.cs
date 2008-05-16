using System;
using Suteki.Shop.Repositories;

namespace Suteki.Shop.Services
{
    public class BaseControllerService : IBaseControllerService
    {
        public IRepository<Category> CategoryRepository { get; private set; }
        public IRepository<Menu> MenuRepository { get; private set; }
        private string emailAddress;

        public string EmailAddress
        {
            get 
            {
                if (string.IsNullOrEmpty(emailAddress)) return string.Empty;
                return emailAddress; 
            }
            set { emailAddress = value; }
        }

        public BaseControllerService(
            IRepository<Category> categoryRepository,
            IRepository<Menu> menuRepository)
        {
            this.CategoryRepository = categoryRepository;
            this.MenuRepository = menuRepository;
        }
    }
}
