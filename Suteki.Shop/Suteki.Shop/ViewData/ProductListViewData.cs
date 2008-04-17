using System;
using System.Collections.Generic;

namespace Suteki.Shop.ViewData
{
    public class ProductListViewData : CategoryViewData
    {
        public IEnumerable<Product> Products { get; set; }
    }
}
