using System;
using System.Linq;

namespace Suteki.Shop.Repositories
{
    public static class MenuRepositoryExtensions
    {
        public static Menu GetTopLevelMenu(this IRepository<Menu> menuRepository)
        {
            return menuRepository.GetById(1); // top level menu has menuId = 1
        }
    }
}
