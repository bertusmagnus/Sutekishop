using Suteki.Common.Models;

namespace Suteki.Common.Tests.TestModel
{
    public class OrderLine : Entity<OrderLine>
    {
        public Product Product { get; set; }
        public Order Order { get; set; }

        public override string ToString()
        {
            if (Product == null) return "<null>";
            return Product.Name;
        }
    }
}