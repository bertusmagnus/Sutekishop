using System.Collections.Generic;
using Suteki.Common.Models;

namespace Suteki.Common.Tests.TestModel
{
    public class Customer : NamedEntity<Customer>
    {
        private readonly List<Order> orders = new List<Order>();

        public List<Order> Orders
        {
            get { return orders; }
        }
    }
}