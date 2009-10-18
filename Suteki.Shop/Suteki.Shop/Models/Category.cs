using System.Collections.Generic;
using System.Linq;
using Suteki.Common.Repositories;
using Suteki.Common.Validation;
using Suteki.Shop.Repositories;
using Suteki.Shop.ViewData;

namespace Suteki.Shop
{
    public partial class Category : ICategory
    {
        partial void OnNameChanging(string value)
        {
            value.Label("Name").IsRequired();
        }

        public bool HasProducts
        {
            get
            {
				return ProductCategories.Count > 0;
            }
        }

        public bool HasMainImage
        {
            get
            {
                return HasProducts && Products.InOrder().Active().First().HasMainImage;
            }
        }

        public Image MainImage
        {
            get
            {
                return HasMainImage ? Products.InOrder().Active().First().MainImage : null;
            }
        }

    	public IEnumerable<Product> Products
    	{
			get { return ProductCategories.Select(x => x.Product); }
    	}

    	public static Category DefaultCategory(int parentId, int position)
    	{
			return new Category 
			{
				ParentId = parentId,
				Position = position
			};
    	}

        public IList<ICategory> ChildCategories
        {
            get { return Categories.Cast<ICategory>().ToList(); }
        }
    }
}
