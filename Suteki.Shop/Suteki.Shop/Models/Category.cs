using System;
using Suteki.Shop.Validation;

namespace Suteki.Shop
{
    public partial class Category : IOrderable
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
    }
}
