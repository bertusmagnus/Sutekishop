using System;
using Suteki.Shop.Repositories;

namespace Suteki.Shop.Services
{
    public interface IBaseControllerService
    {
        IRepository<Category> CategoryRepository { get; }
        IRepository<Content> ContentRepository { get; }
        string EmailAddress { get; set; }
        string SiteUrl { get; }
    }
}
