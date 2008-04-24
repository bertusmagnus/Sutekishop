using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Suteki.Shop.ViewData;
using Suteki.Shop.Repositories;
using Suteki.Shop.Validation;
using System.Web.Security;
using Suteki.Shop.Services;

namespace Suteki.Shop.Controllers
{
    public class OrderController : ControllerBase
    {
        IRepository<Order> orderRepository;
        IRepository<OrderItem> orderItemRepository;
        IRepository<User> userRepository;
        IUserService userService;

        public OrderController(
            IRepository<Order> orderRepository,
            IRepository<OrderItem> orderItemRepository,
            IRepository<User> userRepository,
            IUserService userService)
        {
            this.orderRepository = orderRepository;
            this.orderItemRepository = orderItemRepository;
            this.userRepository = userRepository;
            this.userService = userService;
        }

        public ActionResult Index()
        {
            User user = this.ControllerContext.HttpContext.User as User;
            if (user == null) throw new ApplicationException("HttpContext.User is not a Suteki.Shop.User");

            return RenderView("Index", View.Data.WithOrder(user.CurrentOrder));
        }

        public ActionResult Update()
        {
            User user = CurrentUser;

            // if the current user is a guest, promote them to a new customer
            if (user.RoleId == Role.GuestId)
            {
                user = userService.CreateNewCustomer();
                this.SetAuthenticationCookie(user.Email);
                this.SetContextUserTo(user);
            }

            Order order = user.CurrentOrder;

            OrderItem orderItem = new OrderItem();
            try
            {
                ValidatingBinder.UpdateFrom(orderItem, Request.Form);
                order.OrderItems.Add(orderItem);
                orderRepository.SubmitChanges();
                return RenderView("Index", View.Data.WithOrder(order));
            }
            catch(ValidationException)
            {
                // we shouldn't get a validation exception here, so this is a genuine system error
                throw;
            }
        }

        public ActionResult Remove(int id)
        {
            Order order = CurrentUser.CurrentOrder;
            OrderItem orderItem = order.OrderItems.Where(item => item.OrderItemId == id).SingleOrDefault();

            if (orderItem != null)
            {
                orderItemRepository.DeleteOnSubmit(orderItem);
                orderItemRepository.SubmitChanges();
            }

            return RenderView("Index", View.Data.WithOrder(order));
        }

        public RenderViewResult Checkout(int id)
        {
            Order order = orderRepository.GetById(id);
            return RenderView("Checkout", View.Data.WithOrder(order));
        }
    }
}
