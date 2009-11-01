using System.Collections.Generic;
using Suteki.Common;

namespace Suteki.Shop.ViewData
{
    public class CategoryViewData : IActivatable, IOrderable
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public int Position { get; set; }
        public bool IsActive { get; set; }
        public int? ImageId { get; set; }

        private readonly IList<CategoryViewData> childCategories = new List<CategoryViewData>();

        public IList<CategoryViewData> ChildCategories
        {
            get { return childCategories; }
        }

        public void AddChild(CategoryViewData category)
        {
            childCategories.Add(category);
        }
    }
}