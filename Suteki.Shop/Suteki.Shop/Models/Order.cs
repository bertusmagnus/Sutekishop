using System;
using System.Linq;

namespace Suteki.Shop
{
    public partial class Order
    {
        public decimal Total
        {
            get
            {
                return OrderItems.Sum(item => item.Total);
            }
        }
    }
}
