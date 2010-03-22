using System.Collections.Generic;
using System.Linq;
using Suteki.Common;
using Suteki.Common.Repositories;
using Suteki.Common.Validation;
using Suteki.Shop.Repositories;

namespace Suteki.Shop
{
    public partial class Category : IActivatable, IOrderable
    {
        partial void OnNameChanging(string value)
        {
            value.Label("Name").IsRequired();
        }

        public bool HasProducts
        {
            get
            {
				return ProductCategories.Any();
            }
        }

        public bool HasActiveProducts
        {
            get
            {
                return ProductCategories.Any(pc => pc.Product.IsActive);
            }
        }

        public bool HasMainImage
        {
            get
            {
                return HasActiveProducts && Products.InOrder().Active().First().HasMainImage;
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
    }
}
