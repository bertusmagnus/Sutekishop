namespace Suteki.Common.Tests.TestModel
{
    public class CustomerBuilder
    {
        public Customer CreateCustomer()
        {
            var oldVinyl = new Supplier {Id = 1, Name = "Old Vinyl"};
            var discoHits = new Supplier {Id = 2, Name = "Disco Hits"};

            var relayer = new Product { Id = 1, Name = "Relayer", Supplier = discoHits };
            discoHits.Products.Add(relayer);
            var fragile = new Product { Id = 2, Name = "Fragile", Supplier = oldVinyl };
            var closeToTheEdge = new Product { Id = 3, Name = "Close to the Edge", Supplier = oldVinyl };
            oldVinyl.Products.Add(fragile);
            oldVinyl.Products.Add(closeToTheEdge);

            var customer = new Customer {Id = 1, Name = "Mike"};

            var order1 = new Order {Id = 1, Customer = customer, OrderNumber = 101 };
            var order2 = new Order {Id = 2, Customer = customer, OrderNumber = 102 };

            var order1Line1 = new OrderLine { Id = 1, Order = order1, Product = relayer };
            var order1Line2 = new OrderLine {Id = 2, Order = order1, Product = fragile};
            order1.OrderLines.Add(order1Line1);
            order1.OrderLines.Add(order1Line2);

            relayer.OrderLines.Add(order1Line1);
            fragile.OrderLines.Add(order1Line2);

            var order2Line1 = new OrderLine {Id = 3, Order = order2, Product = relayer};
            var order2Line2 = new OrderLine {Id = 4, Order = order2, Product = closeToTheEdge};
            var order2Line3 = new OrderLine {Id = 5, Order = order2, Product = null};

            order2.OrderLines.Add(order2Line1);
            order2.OrderLines.Add(order2Line2);
            order2.OrderLines.Add(order2Line3);

            relayer.OrderLines.Add(order2Line1);
            closeToTheEdge.OrderLines.Add(order2Line2);

            customer.Orders.Add(order1);
            customer.Orders.Add(order2);

            return customer;
        }
    }
}