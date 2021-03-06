using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Web.Mvc;
using Suteki.Common.Repositories;
using Suteki.Common.Validation;
using Suteki.Shop.Services;

namespace Suteki.Shop.Binders
{
	public class OrderBinder : IModelBinder
	{
		readonly IValidatingBinder validatingBinder;
		readonly IEncryptionService encryptionService;
		readonly IRepository<Basket> basketRepository;

		public OrderBinder(IValidatingBinder validatingBinder, IEncryptionService encryptionService, IRepository<Basket> basketRepository)
		{
			this.validatingBinder = validatingBinder;
			this.basketRepository = basketRepository;
			this.encryptionService = encryptionService;
		}

		public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			var form = controllerContext.HttpContext.Request.Form;

			var order = new Order
			{
				//OrderStatusId = OrderStatus.CreatedId,
				CreatedDate = DateTime.Now,
				DispatchedDate = DateTime.Now
			};

			try
			{
				var validator = new Validator
				{
					() => UpdateOrder(order, form, bindingContext.ModelState),
					() => UpdateCardContact(order, form, bindingContext.ModelState),
					() => UpdateDeliveryContact(order, form, bindingContext.ModelState),
					() => UpdateCard(order, form, bindingContext.ModelState)
				};

				validator.Validate();
			}
			catch (ValidationException)
			{
				//Ignore validation exceptions - they will be stored in ModelState.
			}

		
			EnsureBasketCountryId(order);

			return order;
		}

		void EnsureBasketCountryId(Order order)
		{
			var basket = basketRepository.GetById(order.BasketId);

			if (order.Contact1 != null && basket.CountryId != order.Contact1.CountryId) //delivery contact
			{
				basket.CountryId = order.Contact1.CountryId;
			}
			//No Delivery contact specified - resort to CardContact. 
			else if(order.Contact != null && basket.CountryId != order.Contact.CountryId)
			{
				basket.CountryId = order.Contact.CountryId;
			}
		}

		void UpdateCardContact(Order order, NameValueCollection form, ModelStateDictionary modelState)
		{
			var cardContact = new Contact();
			order.Contact = cardContact;
			UpdateContact(cardContact, "cardcontact", form, modelState);
		}

		void UpdateDeliveryContact(Order order, NameValueCollection form, ModelStateDictionary modelState)
		{
			if (order.UseCardHolderContact) return;

			var deliveryContact = new Contact();
			order.Contact1 = deliveryContact;
			UpdateContact(deliveryContact, "deliverycontact", form, modelState);
		}

		void UpdateContact(Contact contact, string prefix, NameValueCollection form, ModelStateDictionary modelState)
		{
			//TODO: Review this FInally block. This seems bad to me as it forces the order to become attached to the datacontext
			//try
		//	{
				validatingBinder.UpdateFrom(contact, form, modelState, prefix);
		//	}
		//	finally
		//	{
		//		if (contact.CountryId != 0 && contact.Country == null)
		//		{
		//			contact.Country = countryRepository.GetById(contact.CountryId);
		//		}
		//	}
		}

		void UpdateCard(Order order, NameValueCollection form, ModelStateDictionary modelState)
		{
			if (order.PayByTelephone) return;

			var card = new Card();
			order.Card = card;
			validatingBinder.UpdateFrom(card, form, modelState, "card");
			encryptionService.EncryptCard(card);
		}

		void UpdateOrder(Order order, NameValueCollection form, ModelStateDictionary modelState)
		{
			validatingBinder.UpdateFrom(order, form, modelState, "order");
			var confirmEmail = form["emailconfirm"];
			if (order.Email != confirmEmail)
			{
				//hackery...
				modelState.AddModelError("order.email", "Email and Confirm Email do not match");
				modelState.SetModelValue("order.email", new ValueProviderResult(order.Email, order.Email, CultureInfo.CurrentCulture));
				throw new ValidationException("Email and Confirm Email do not match");
			}
		}
	}
}