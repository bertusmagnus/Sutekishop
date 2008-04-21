using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Suteki.Shop.ViewData;
using Suteki.Shop.Repositories;
using Suteki.Shop.Validation;

namespace Suteki.Shop.Controllers
{
    public class OrderController : ControllerBase
    {
        IRepository<Order> orderRepository;

        public OrderController(IRepository<Order> orderRepository)
        {
            this.orderRepository = orderRepository;
        }

        public void Index()
        {
            User user = this.ControllerContext.HttpContext.User as User;
            if (user == null) throw new ApplicationException("HttpContext.User is not a Suteki.Shop.User");

            RenderView("Index", View.Data.WithOrder(user.CurrentOrder));
        }

        public void Update()
        {
            User user = this.ControllerContext.HttpContext.User as User;
            if (user == null) throw new ApplicationException("HttpContext.User is not a Suteki.Shop.User");
            Order order = user.CurrentOrder;

            OrderItem orderItem = new OrderItem();
            try
            {
                ValidatingBinder.UpdateFrom(orderItem, Request.Form);
                order.OrderItems.Add(orderItem);
                orderRepository.SubmitChanges();
                RenderView("Index", View.Data.WithOrder(order));
            }
            catch(ValidationException)
            {
                // think about what to do here?
                throw;
            }
        }
    }
}
