using System.Collections.Generic;
using Suteki.Common.Extensions;
using Suteki.Common.Models;

namespace Suteki.Common.Tests.TestModel
{
    public class Order : Entity<Order>
    {
        private readonly List<OrderLine> orderLines = new List<OrderLine>();

        public List<OrderLine> OrderLines
        {
            get { return orderLines; }
        }

        public int OrderNumber { get; set; }
        public Customer Customer { get; set; }

        public override string ToString()
        {
            return "Order {0}".With(OrderNumber);
        }
    }
}