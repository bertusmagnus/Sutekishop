using System;
using System.Linq;
using NUnit.Framework;
using Suteki.Common.HtmlHelpers;
using Suteki.Common.Models;
using Suteki.Common.Services;
using Suteki.Common.Tests.TestModel;

namespace Suteki.Common.Tests.Models
{
    [TestFixture]
    public class MetaEntityFactoryTests
    {
        private Customer customer;
        private MetaEntity customerEntity;
        private MetaEntity orderEntity;
        private MetaEntity supplierEntity;

        [SetUp]
        public void SetUp()
        {
            customer = new CustomerBuilder().CreateCustomer();

            var factory = new MetaEntityFactory(new DefaultEntityTypeResolver());

            customerEntity = factory.CreateFrom(customer);
            orderEntity = factory.CreateFrom(customer.Orders[0]);
            supplierEntity = factory.CreateFrom(customer.Orders[0].OrderLines[0].Product.Supplier);
        }

        [Test]
        public void ShouldHandleNullProperties()
        {
            var factory = new MetaEntityFactory(new DefaultEntityTypeResolver());
            // the second order's third orderline has a null product
            factory.CreateFrom(customer.Orders[1]);
        }

        [Test]
        public void RootShouldBeTheSameAsCustomer()
        {
            Assert.That(customerEntity.Entity, Is.SameAs(customer));
            Assert.That(((Customer)customerEntity.Entity).Name, Is.EqualTo("Mike"));
        }

        [Test]
        public void RootChildrenShouldBeOneOrderCollection()
        {
            Assert.That(customerEntity.Children.Count, Is.EqualTo(1));
            Assert.That(customerEntity.Children.First() is CollectionMetaEntity);
        }

        [Test]
        public void RootChildrenShouldBeTwoOrders()
        {
            var orderCollection = customerEntity.Children.First();

            Assert.That(orderCollection.Children.Count, Is.EqualTo(2));

            var firstChild = orderCollection.Children.First();
            var secondChild = orderCollection.Children.Last();

            Assert.That(firstChild.Entity.GetType(), Is.EqualTo(typeof(Order)));
            Assert.That(((Order)firstChild.Entity).OrderNumber, Is.EqualTo(101));

            Assert.That(secondChild.Entity.GetType(), Is.EqualTo(typeof(Order)));
            Assert.That(((Order)secondChild.Entity).OrderNumber, Is.EqualTo(102));
        }

        [Test]
        public void FirstOrderShouldHaveTwoLines()
        {
            var orderCollection = customerEntity.Children.First();
            var firstOrder = orderCollection.Children.First();
            var orderLines = firstOrder.Children.First();

            Assert.That(orderLines.Children.Count, Is.EqualTo(2));
        }

        [Test]
        public void SecondOrderFirstOrderLineShouldBeForRelayer()
        {
            var firstOrderLine = customerEntity
                .Children.First() // orders
                .Children.Last()  // last order
                .Children.First() // order lines
                .Children.First(); // first order line

            Assert.That(firstOrderLine.Entity.GetType(), Is.EqualTo(typeof(OrderLine)));
            Assert.That(firstOrderLine.Entity.Id, Is.EqualTo(3));
        }

        [Test]
        public void Order_Customer_Should_Not_Have_Orders()
        {
            var customerEntity1 = orderEntity.Children.First();
            foreach (var child in customerEntity1.Children)
            {
                Assert.That(child.Entity.GetType(), Is.Not.EqualTo(typeof(Customer)));
            }
        }

        [Test, Explicit]
        public void ShouldBeAbleToRenderGraphAsHTML()
        {
            var html = new TreeRenderer<MetaEntity>(new[] {customerEntity}, metaEntity => metaEntity.ToString()).Render();
            Console.WriteLine(html);
        }

        [Test, Explicit]
        public void ShouldBeAbleToRenderOrderGraphAsHtml()
        {
            var html = new TreeRenderer<MetaEntity>(new[] {orderEntity}, metaEntity => metaEntity.ToString()).Render();
            Console.WriteLine(html);
        }

        [Test, Explicit]
        public void ShouldBeAbleToRenderSupplierGraphAsHTML()
        {
            var html = new TreeRenderer<MetaEntity>(new[] {supplierEntity}, metaEntity => metaEntity.ToString()).Render();
            Console.WriteLine(html);
        }
    }
}