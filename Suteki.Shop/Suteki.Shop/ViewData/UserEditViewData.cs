using System;
using System.Collections.Generic;

namespace Suteki.Shop.ViewData
{
    public class UserEditViewData : CommonViewData
    {
        public IEnumerable<Role> Roles { get; set; }
        public User User { get; set; }
    }
}
