using System;
using Suteki.Shop.Validation;

namespace Suteki.Shop
{
    public partial class Product : IOrderable
    {
        partial void OnNameChanging(string value)
        {
            value.Label("Name").IsRequired();
        }

        partial void OnDescriptionChanging(string value)
        {
            value.Label("Description").IsRequired();
        }

        public bool HasMainImage
        {
            get
            {
                return this.ProductImages.Count > 0;
            }
        }

        public Image MainImage
        {
            get
            {
                if (HasMainImage) return this.ProductImages[0].Image;
                return null;
            }
        }
    }
}
