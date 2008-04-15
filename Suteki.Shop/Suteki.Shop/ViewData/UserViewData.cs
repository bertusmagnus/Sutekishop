using System;
using System.Collections.Generic;

namespace Suteki.Shop.ViewData
{
    public class UserViewData : CommonViewData
    {
        public IEnumerable<User> Users { get; set; }
    }
}
