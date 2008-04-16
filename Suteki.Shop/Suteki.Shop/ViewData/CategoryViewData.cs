using System;
using System.Collections.Generic;

namespace Suteki.Shop.ViewData
{
    public class CategoryViewData : CommonViewData
    {
        public Category Category { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }
}
