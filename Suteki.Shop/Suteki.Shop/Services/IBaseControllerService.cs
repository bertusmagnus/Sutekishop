using System;
using Suteki.Shop.Repositories;

namespace Suteki.Shop.Services
{
    public interface IBaseControllerService
    {
        IRepository<Category> CategoryRepository { get; }
        string EmailAddress { get; set; }
    }
}
