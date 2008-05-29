using Suteki.Common;
using Suteki.Common.Controllers;
using Suteki.Shop.Services;

namespace Suteki.Shop.Controllers
{
    public abstract class ShopScaffoldController<T> : ScaffoldController<T>, IProvidesBaseService where T : class, IOrderable, new()
    {
        public IBaseControllerService BaseControllerService { get; set; }
    }
}
