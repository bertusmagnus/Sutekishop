using NUnit.Framework;
using Suteki.Common.Models;
using Suteki.Common.Tests.TestModel;

namespace Suteki.Common.Tests.Models
{
    [TestFixture]
    public class MetaEntityTests
    {
        private Customer customer;
        private Order order;
        private OrderLine orderLine;
        private Product product;
        private Supplier supplier;

        private RootMetaEntity root;
        private CollectionMetaEntity ordersMetaEntity;
        private MetaEntity orderMetaEntity;
        private MetaEntity orderCustomerMetaEntity;
        private CollectionMetaEntity orderLinesMetaEntity;
        private MetaEntity orderLineMetaEntity;
        private RootMetaEntity productMetaEntity;
        private MetaEntity supplierMetaEntity;

        [SetUp]
        public void SetUp()
        {
            customer = new CustomerBuilder().CreateCustomer();

            order = customer.Orders[0];
            orderLine = order.OrderLines[0];
            product = orderLine.Product;
            supplier = product.Supplier;

            root = new RootMetaEntity(customer);
            
            var ordersProperty = customer.GetType().GetProperty("Orders");
            ordersMetaEntity = new CollectionMetaEntity(ordersProperty, root);
            orderMetaEntity = new MetaEntity(order, ordersMetaEntity);
            orderCustomerMetaEntity = new MetaEntity(customer, orderMetaEntity);

            var orderLinesProperty = order.GetType().GetProperty("OrderLines");
            orderLinesMetaEntity = new CollectionMetaEntity(orderLinesProperty, orderMetaEntity);
            orderLineMetaEntity = new MetaEntity(orderLine, orderLinesMetaEntity);

            productMetaEntity = new RootMetaEntity(product);
            supplierMetaEntity = new MetaEntity(supplier, productMetaEntity);
        }

        [Test]
        public void RootMetaEntity_Should_Take_Entity()
        {
            Assert.That(root.Entity, Is.SameAs(customer));
            Assert.That(root.Parent is NullMetaEntity);
        }

        [Test]
        public void RootMetaEntity_Relationship_Should_Be_Correct()
        {
            Assert.That(root.Relationship.EntityType, Is.EqualTo(typeof(Customer)));
            Assert.That(root.Relationship.ParentType, Is.EqualTo(typeof(NullEntity)));
        }

        [Test]
        public void Orders_CollectionMetaEntity_Should_Take_Entity_And_Parent()
        {
            Assert.That(ordersMetaEntity.Entity, Is.SameAs(customer));
            Assert.That(ordersMetaEntity.Parent, Is.SameAs(root));
        }

        [Test]
        public void Orders_CollectionMetaEntity_CollectionType_Should_Be_Order()
        {
            Assert.That(ordersMetaEntity.CollectionType, Is.EqualTo(typeof(Order)));
        }

        [Test]
        public void Orders_CollectionMetaEntity_Relationship_Should_Be_Correct()
        {
            Assert.That(ordersMetaEntity.Relationship.EntityType, Is.EqualTo(typeof(Order)));
            Assert.That(ordersMetaEntity.Relationship.ParentType, Is.EqualTo(typeof(Customer)));
        }

        [Test]
        public void Order_MetaEntity_Should_Have_Correct_Entity_And_Parent()
        {
            Assert.That(orderMetaEntity.Entity, Is.SameAs(order));
            Assert.That(orderMetaEntity.Parent, Is.SameAs(ordersMetaEntity));
        }

        [Test]
        public void Order_MetaEntity_Relationship_Should_Be_Correct()
        {
            Assert.That(orderMetaEntity.Relationship.EntityType, Is.EqualTo(typeof(Order)));
            Assert.That(orderMetaEntity.Relationship.ParentType, Is.EqualTo(typeof(Customer)));
        }

        [Test]
        public void OrderCustomer_MetaEntity_Should_Have_Correct_Entity_And_Parent()
        {
            Assert.That(orderCustomerMetaEntity.Entity, Is.SameAs(customer));
            Assert.That(orderCustomerMetaEntity.Parent, Is.SameAs(orderMetaEntity));
        }

        [Test]
        public void OrderCustomer_MetaEntity_Should_Have_Correct_Relationship()
        {
            Assert.That(orderCustomerMetaEntity.Relationship.EntityType, Is.EqualTo(typeof(Customer)));
            Assert.That(orderCustomerMetaEntity.Relationship.ParentType, Is.EqualTo(typeof(Order)));
        }

        [Test]
        public void OrderLines_CollectionMetaEntity_Should_Have_Correct_Entity_And_Parent()
        {
            Assert.That(orderLinesMetaEntity.Entity, Is.SameAs(order));
            Assert.That(orderLinesMetaEntity.Parent, Is.SameAs(orderMetaEntity));
            Assert.That(orderLinesMetaEntity.CollectionType, Is.EqualTo(typeof(OrderLine)));
        }

        [Test]
        public void OrderLines_CollectionMetaEntity_Should_Have_Correct_Relationship()
        {
            Assert.That(orderLinesMetaEntity.Relationship.EntityType, Is.EqualTo(typeof(OrderLine)));
            Assert.That(orderLinesMetaEntity.Relationship.ParentType, Is.EqualTo(typeof(Order)));
        }

        [Test]
        public void OrderLine_MetaEntity_Should_Have_Correct_Entity_And_Parent()
        {
            Assert.That(orderLineMetaEntity.Entity, Is.SameAs(orderLine));
            Assert.That(orderLineMetaEntity.Parent, Is.SameAs(orderLinesMetaEntity));
        }

        [Test]
        public void Supplier_MetaEntity_Should_Take_Entity_And_Parent()
        {
            Assert.That(supplierMetaEntity.Entity, Is.SameAs(supplier));
            Assert.That(supplierMetaEntity.Parent.Entity, Is.SameAs(product));
        }

        [Test]
        public void Supplier_MetaEntity_Relationship_Should_Be_Correct()
        {
            Assert.That(supplierMetaEntity.Relationship.EntityType, Is.EqualTo(typeof(Supplier)));
            Assert.That(supplierMetaEntity.Relationship.ParentType, Is.EqualTo(typeof(Product)));
        }
    }
}