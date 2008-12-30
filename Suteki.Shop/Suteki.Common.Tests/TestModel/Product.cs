using System.Collections.Generic;
using Suteki.Common.Models;

namespace Suteki.Common.Tests.TestModel
{
    public class Product : NamedEntity<Product>
    {
        public Supplier Supplier { get; set; }

        private readonly List<OrderLine> orderLines = new List<OrderLine>();

        public List<OrderLine> OrderLines
        {
            get { return orderLines; }
        }
    }
}