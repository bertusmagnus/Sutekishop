﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;
using MvcContrib.Filters;
using Suteki.Common.Extensions;
using Suteki.Common.Filters;
using Suteki.Common.Repositories;
using Suteki.Common.Services;
using Suteki.Common.Validation;
using Suteki.Shop.Filters;
using Suteki.Shop.ViewData;
using Suteki.Shop.Repositories;
using Suteki.Shop.Services;
using System.Security.Permissions;
using Suteki.Common.Binders;
using MvcContrib;
namespace Suteki.Shop.Controllers
{
    public class OrderController : ControllerBase
    {
        readonly IRepository<Order> orderRepository;
    	readonly IRepository<Country> countryRepository;
        readonly IRepository<CardType> cardTypeRepository;
        readonly IEncryptionService encryptionService;
    	readonly IPostageService postageService;
    	readonly IUserService userService;
		readonly IOrderSearchService searchService;
		readonly IRepository<OrderStatus> statusRepository;

        public OrderController(
			IRepository<Order> orderRepository, 
			IRepository<Country> countryRepository, 
			IRepository<CardType> cardTypeRepository, 
			IEncryptionService encryptionService, 
			IPostageService postageService, 
			IUserService userService, IOrderSearchService searchService, IRepository<OrderStatus> statusRepository)
        {
            this.orderRepository = orderRepository;
        	this.statusRepository = statusRepository;
        	this.searchService = searchService;
        	this.userService = userService;
        	this.countryRepository = countryRepository;
            this.cardTypeRepository = cardTypeRepository;
            this.encryptionService = encryptionService;
        	this.postageService = postageService;
        }

		[AdministratorsOnly]
        public ActionResult Index(OrderSearchCriteria orderSearchCriteria)
        {
			orderSearchCriteria = orderSearchCriteria ?? new OrderSearchCriteria();
			var orders = searchService.PerformSearch(orderSearchCriteria);

            return View("Index", ShopView.Data
                .WithOrders(orders)
				.WithOrderStatuses(OrderStatuses())
                .WithOrderSearchCriteria(orderSearchCriteria));
        }

    	IEnumerable<OrderStatus> OrderStatuses()
    	{
			var list = statusRepository.GetAll().Where(x => x.OrderStatusId > 0).ToList();
			list.Insert(0, new OrderStatus() { Name = "Any", OrderStatusId = 0 });
			return list;
    	}

    	public ActionResult Item(int id)
        {
            return ItemView(id);
        }

        private ViewResult ItemView(int id)
        {
            var order = orderRepository.GetById(id);

            if (userService.CurrentUser.IsAdministrator)
            {
                var cookie = Request.Cookies["privateKey"];
                if (cookie != null)
                {
                    var privateKey = cookie.Value.Replace("%3D", "=");

                    if (!order.PayByTelephone)
                    {
                        var card = order.Card.Copy();
                        try
                        {
                            encryptionService.PrivateKey = privateKey;
                            encryptionService.DecryptCard(card);
                            return View("Item", CheckoutViewData(order).WithCard(card));
                        }
                        catch (ValidationException exception)
                        {
                            return View("Item", CheckoutViewData(order).WithErrorMessage(exception.Message));
                        }
                    }
                }
            }

			userService.CurrentUser.EnsureCanViewOrder(order);
            return View("Item", CheckoutViewData(order));
        }

        public ActionResult Print(int id)
        {
            var viewResult = ItemView(id);
            viewResult.MasterName = "Print";
            ((ShopViewData) viewResult.ViewData.Model).IsPrint = true;
            return viewResult;
        }

		[AdministratorsOnly]
        public ActionResult ShowCard(int orderId, string privateKey)
        {
            var order = orderRepository.GetById(orderId);

            var card = order.Card.Copy();

            try
            {
                encryptionService.PrivateKey = privateKey;
                encryptionService.DecryptCard(card);
                return View("Item", CheckoutViewData(order).WithCard(card));
            }
            catch (ValidationException exception)
            {
                return View("Item", CheckoutViewData(order).WithErrorMessage(exception.Message));
            }
        }

		[AcceptVerbs(HttpVerbs.Post), AdministratorsOnly, UnitOfWork, ModelStateToTempData]
		public ActionResult UpdateNote([DataBind] Order order)
		{
			if(ModelState.IsValid)
			{
				Message = "Note successfully updated.";
			}
			return this.RedirectToAction(c => c.Item(order.OrderId));
		}

        private ShopViewData CheckoutViewData(Order order)
        {
			userService.CurrentUser.EnsureCanViewOrder(order);
            postageService.CalculatePostageFor(order);

            return ShopView.Data
                .WithCountries(countryRepository.GetAll().Active().InOrder())
                .WithCardTypes(cardTypeRepository.GetAll())
                .WithOrder(order);
        }
    }
}
