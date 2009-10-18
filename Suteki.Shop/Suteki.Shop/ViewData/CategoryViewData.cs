using System.Collections.Generic;
using System.Linq;
using Suteki.Common;

namespace Suteki.Shop.ViewData
{
    public interface ICategory : IActivatable, IOrderable
    {
        int CategoryId { get; set; }
        string Name { get; set; }
        int? ParentId { get; set; }
        int? ImageId { get; set; }
        IList<ICategory> ChildCategories { get; }
    }

    public class CategoryViewData : ICategory
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public int Position { get; set; }
        public bool IsActive { get; set; }
        public int? ImageId { get; set; }

        private readonly IList<CategoryViewData> childCategories = new List<CategoryViewData>();

        public IList<ICategory> ChildCategories
        {
            get { return childCategories.Cast<ICategory>().ToList(); }
        }

        public void AddChild(CategoryViewData category)
        {
            childCategories.Add(category);
        }
    }
}