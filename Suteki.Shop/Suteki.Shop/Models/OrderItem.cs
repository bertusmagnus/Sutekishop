using System;

namespace Suteki.Shop
{
    public partial class OrderItem
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
