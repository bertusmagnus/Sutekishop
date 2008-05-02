using System;

namespace Suteki.Shop
{
    public partial class BasketItem
    {
        public decimal Total
        {
            get
            {
                return Size.Product.Price * Quantity;
            }
        }
    }
}
