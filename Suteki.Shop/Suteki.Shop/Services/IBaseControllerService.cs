using System;
using Suteki.Common.Repositories;
using Suteki.Shop.Repositories;

namespace Suteki.Shop.Services
{
    public interface IBaseControllerService
    {
        IRepository<Category> CategoryRepository { get; }
        IRepository<Content> ContentRepository { get; }
        string GoogleTrackingCode { get; set; }
        string ShopName { get; set; }
        string EmailAddress { get; set; }
        string SiteUrl { get; }
        string MetaDescription { get; set; }
        string Copyright { get; set; }
    }
}
