using System;
using System.Collections.Generic;
using Suteki.Shop.Services;

namespace Suteki.Shop.ViewData
{
    public class CategoryViewData : CommonViewData
    {
        public Category Category { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IImageFileService ImageFile { get; set; }
    }
}
