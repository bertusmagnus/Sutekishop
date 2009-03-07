using Suteki.Shop.Filters;

namespace Suteki.Shop.Controllers
{
	[AdministratorsOnly]
    public class PostZoneController : ShopScaffoldController<PostZone>
    {
    }
}
