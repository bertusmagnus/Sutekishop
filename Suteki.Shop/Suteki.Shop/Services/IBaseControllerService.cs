using System;
using Suteki.Shop.Repositories;

namespace Suteki.Shop.Services
{
    public interface IBaseControllerService
    {
        IRepository<Category> CategoryRepository { get; }
        IRepository<Menu> MenuRepository { get; }
        string EmailAddress { get; set; }
        string siteUrl { get; }
    }
}
