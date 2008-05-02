using System;
using System.Linq;

namespace Suteki.Shop
{
    public partial class Basket
    {
        public decimal Total
        {
            get
            {
                return BasketItems.Sum(item => item.Total);
            }
        }
    }
}
