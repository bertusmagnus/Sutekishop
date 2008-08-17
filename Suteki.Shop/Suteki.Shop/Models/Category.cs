using System.Linq;
using Suteki.Common;
using Suteki.Common.Repositories;
using Suteki.Common.Validation;
using Suteki.Shop.Repositories;

namespace Suteki.Shop
{
    public partial class Category : IOrderable, IActivatable
    {
        partial void OnNameChanging(string value)
        {
            value.Label("Name").IsRequired();
        }

        public bool HasProducts
        {
            get
            {
                return Products.Count > 0;
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
                if (HasMainImage) return Products.InOrder().Active().First().MainImage;
                return null;
            }
        }
    }
}
