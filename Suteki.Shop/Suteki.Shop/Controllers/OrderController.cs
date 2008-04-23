using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Suteki.Shop.ViewData;
using Suteki.Shop.Repositories;
using Suteki.Shop.Validation;
using System.Web.Security;

namespace Suteki.Shop.Controllers
{
    public class OrderController : ControllerBase
    {
        IRepository<Order> orderRepository;
        IRepository<OrderItem> orderItemRepository;
        IRepository<User> userRepository;

        public OrderController(
            IRepository<Order> orderRepository,
            IRepository<OrderItem> orderItemRepository,
            IRepository<User> userRepository)
        {
            this.orderRepository = orderRepository;
            this.orderItemRepository = orderItemRepository;
            this.userRepository = userRepository;
        }

        public ActionResult Index()
        {
            User user = this.ControllerContext.HttpContext.User as User;
            if (user == null) throw new ApplicationException("HttpContext.User is not a Suteki.Shop.User");

            return RenderView("Index", View.Data.WithOrder(user.CurrentOrder));
        }

        public ActionResult Update()
        {
            User user = GetCurrentUser();
            if (user.RoleId == Role.GuestId) user = PromoteGuestToNewCustomer();

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
                // think about what to do here?
                throw;
            }
        }

        private User GetCurrentUser()
        {
            User user = this.ControllerContext.HttpContext.User as User;
            if (user == null) throw new ApplicationException("HttpContext.User is not a Suteki.Shop.User");
            return user;
        }

        public ActionResult Remove(int id)
        {
            User user = GetCurrentUser();
            Order order = user.CurrentOrder;
            OrderItem orderItem = order.OrderItems.Where(item => item.OrderItemId == id).SingleOrDefault();

            if (orderItem != null)
            {
                orderItemRepository.DeleteOnSubmit(orderItem);
                orderItemRepository.SubmitChanges();
            }

            return RenderView("Index", View.Data.WithOrder(order));
        }

        [NonAction]
        public virtual User PromoteGuestToNewCustomer()
        {
            User user = new User
            {
                Email = Guid.NewGuid().ToString(),
                Password = "",
                RoleId = Role.CustomerId
            };

            userRepository.InsertOnSubmit(user);
            userRepository.SubmitChanges();

            FormsAuthentication.SetAuthCookie(user.Email, true);
            System.Threading.Thread.CurrentPrincipal = this.ControllerContext.HttpContext.User = user;

            return user;
        }
    }
}
