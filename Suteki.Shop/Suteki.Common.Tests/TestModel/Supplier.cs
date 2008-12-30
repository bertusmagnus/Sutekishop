using System.Collections.Generic;
using Suteki.Common.Models;

namespace Suteki.Common.Tests.TestModel
{
    public class Supplier : NamedEntity<Supplier>
    {
        private readonly List<Product> products = new List<Product>();

        public List<Product> Products
        {
            get { return products; }
        }
    }
}